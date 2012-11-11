using System;
using System.Collections.Generic;
using APIcodeBar.Entity;
using DbLinq.MySql;
using MySql.Data.MySqlClient;

namespace APIcodeBar.DAO
{
    public class TransfertDAO
    {
        private MySqlConnection _connexion = null;

        public TransfertDAO(MySqlDataContext context) 
        {
            this._connexion = context.Connection as MySqlConnection;
        }

        /// <summary>
        /// Reccherche les données d'un tranfert
        /// </summary>
        /// <param name="code_transfert">code du transfert</param>
        /// <param name="code_mag">code du magasin de destination</param>
        /// <param name="origine">code du magasin d'origine</param>
        /// <returns>renvoi un dictionnaire contenant les genCode et quantité de produit pour le tranfert</returns>
        public Dictionary<string, int> find(string code_transfert, string code_mag, string origine)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            string command = String.Format("select distinct p.GenCod, Qte from produits p, stock s where s.barcode = p.barcode and s.taille = p.taille and s.couleur = p.couleur and origine = '-> {0} {1}' and codeMag = '{2}'",origine,code_transfert,code_mag);
            MySqlDataReader reader = null;
            try
            {
                reader = new MySqlCommand(command, this._connexion).ExecuteReader();
                while (reader.Read())
                {
                    //Correction du bug de doublons dans les gencodes du transfert
                    if (!result.ContainsKey(reader.GetString("GenCod")))
                        result.Add(reader.GetString("GenCod"), reader.GetInt16("Qte"));
                    else
                        result[reader.GetString("GenCod")] = result[reader.GetString("GenCod")] + reader.GetInt16("Qte");
                }
                return result;
            }
            catch (Exception e)
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
                return null;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        /// <summary>
        /// Transforme le dictionnaire de gencode et quantité (transfert) en liste de ligne reassort avec la quantité manquante dans le champ entré
        /// </summary>
        /// <param name="code_transfert">code du transfert</param>
        /// <param name="code_mag">code du magasin</param>
        /// <param name="origine">code du magasin d'origine</param>
        /// <param name="dico">dictionnaire de gencode et quantité correspondant aux produits manquantes</param>
        /// <returns>renvoi la liste des produits du reassort manquant dans le transfert</returns>
        public List<LigneReassort> dico_to_lignes(string code_transfert, string code_mag, string origine,Dictionary<string, int> dico)
        {
            List<LigneReassort> result = new List<LigneReassort>() ;
            LigneReassort ligne = null;
            string command = String.Format("select distinct p.GenCod, s.barcode, p.taille, p.couleur, s.designation, Qte from produits p, stock s where s.barcode = p.barcode and s.taille = p.taille and s.couleur = p.couleur and origine = '-> {0} {1}' and codeMag = '{2}'", origine, code_transfert, code_mag);
            MySqlDataReader reader = null;
            try
            {
                reader = new MySqlCommand(command, this._connexion).ExecuteReader();
                while (reader.Read())
                {
                    ligne = new LigneReassort(reader.GetString("GenCod"), reader.GetString("barcode"),reader.GetString("designation"),
                        reader.GetString("couleur"),reader.GetString("taille"));

                    switch (code_mag)
                    {
                        case "G0": ligne.Entree_G0 = dico[reader.GetString("GenCod")];
                            break;
                        case "RESERVEG0": ligne.Entree_RESERVEG0 = dico[reader.GetString("GenCod")];
                            break;
                        case "ROBERT": ligne.Entree_ROBERT = dico[reader.GetString("GenCod")];
                            break;
                        case "M0": ligne.Entree_M0 = dico[reader.GetString("GenCod")];
                            break;
                        case "RESERVEM0": ligne.Entree_RESERVEM0 = dico[reader.GetString("GenCod")];
                            break;
                        case "DEPOTM0": ligne.Entree_DEPOTM0 = dico[reader.GetString("GenCod")];
                            break;
                        default: throw new Exception("Le code magasin du fichier ini n'est pas référencé.");
                    }
                    result.Add(ligne);
                }
                return result;
            }
            catch (Exception e)
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
                return null;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        /// <summary>
        /// La fonction va vérifier si le transfert existe
        /// </summary>
        /// <param name="code_transfert">code du transfert</param>
        /// <param name="code_rea">code du reassort</param>
        /// <param name="code_mag">code du magasin où l'on se trouve</param>
        /// <returns>retourne vrai si le transfert existe</returns>
        public bool is_valid(string code_transfert, string code_rea, string code_mag)
        {
            bool valid = false;
            string command = string.Empty;
            MySqlDataReader reader = null;
            try
            {
                Reassort reassort = DAOFactory.getReassortDAO().find(code_rea);
                command = String.Format("select distinct p.GenCod, s.barcode, p.taille, p.couleur, s.designation, Qte from produits p, stock s where s.barcode = p.barcode and s.taille = p.taille and s.couleur = p.couleur and origine = '-> {0} {1}' and codeMag = '{2}'", reassort.codeMag_sortie, code_transfert, code_mag);
                if (reassort == null)   return false;

                reader = new MySqlCommand(command, this._connexion).ExecuteReader();
                valid = (reader.HasRows) ? true : false;
            }
            catch (Exception e)
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
                return false;
            }
            finally
            {
                if(reader != null) reader.Close();
            }

            return valid;
        }
    }
}
