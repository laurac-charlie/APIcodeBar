using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using APIcodeBar.DAO;
using APIcodeBar.Entity;

namespace APIcodeBar
{
    public class Fichier_Transfert
    {
        #region variables
        public static readonly string BASE_PATH = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        
        private string _path_rea = String.Empty;
        public string Path_rea { get { return _path_rea; } set { _path_rea = value; } }
        #endregion

        #region constructeur
        private Fichier_Transfert() { }

        ///<summary>Créer un objet à partir du fichier</summary>
        ///<param name="nom_fichier">nom du fichier dans le répertoire C:/PMS/reassort/import</param>
        ///<returns></returns>
        public Fichier_Transfert(string nom_fichier)
        {
            try
            {
                //On récupère le dossier par défaut depuis le fichier ini puis on construit le path complet du fichier
                IniFile ini = new IniFile();

                if (!String.IsNullOrWhiteSpace(ini.IniReadValue("REASSORT", "path")))
                     this._path_rea = ini.IniReadValue("REASSORT", "path") + @"\"+ nom_fichier;
                else
                    throw new Exception("Le paramètre ini du dossier d'import n'a pas été initialisé.");
            }
            catch (Exception e)
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
            }
        }
        #endregion

        public bool fichier_existe()
        {
            if (File.Exists(this._path_rea))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Charge les données du fichier data.txt et les comparent au transfert dont le code est donné en paramètre
        /// </summary>
        /// <param name="code_reassort">code du reassort valide, donné par l'utilisateur</param>
        /// <param name="code_transfert">code du transfert</param>
        /// <param name="codeMag">code du magasin où l'on se trouve</param>
        /// <returns>renvoi la liste des lignes reassort pour lesquelles il manque de produit, null en cas d'erreur</returns>
        public List<LigneReassort> compare_fichier_trasfert(string code_reassort, string code_transfert,string codeMag)
        {
            string ligne_douchette = String.Empty;
            Reassort reassort = null;
            Dictionary<string, int> dico = new Dictionary<string, int>();
            Dictionary<string, int> dico_gencod = new Dictionary<string, int>();
            Dictionary<string, int> dico_temp = new Dictionary<string, int>();
            List<LigneReassort> result = new List<LigneReassort>();

            try
            {
                //On ouvre le fichier en lecture
                StreamReader sr = new StreamReader(File.OpenRead(this._path_rea));
                while (!sr.EndOfStream)
                {
                    //On lit chaque ligne du fichier représentant une entrée de la douchette
                    ligne_douchette = sr.ReadLine();

                    //On range ensuite chaque entrée dans un dictionnaire avec comme clé le GenCode et comme valeur, le nombre de fois où il a été douché
                    //Si on retrouve la clé (le gencode) une plusieur fois, on met à jour le nombre
                    try
                    { 
                        dico.Add(ligne_douchette, 1);
                    }
                    catch (ArgumentException ae)
                    {
                        dico[ligne_douchette] = dico[ligne_douchette] + 1;
                    }
                }
                sr.Close();

                //On récupère le réassort qui nous intéresse à partir du code
                //La validité du réassort est contrôlé préalablement
                reassort = DAOFactory.getReassortDAO().find(code_reassort);
                dico_gencod = DAOFactory.getTransfertDAO().find(code_transfert, codeMag, reassort.codeMag_sortie);
                //reassort.lignes.GetEnumerator();

                //On parcours le dictionnaire du transfert et on enlève les éléments du dictionnaire du fichier
                //dico_temp = dico_gencod;
                ICollection<string> keys = dico_gencod.Keys.ToList<string>();
                foreach (string s in keys)
                {
                    if (dico.ContainsKey(s))
                        dico_gencod[s] -= dico[s];
                }

                result = DAOFactory.getTransfertDAO().dico_to_lignes(code_transfert, codeMag, reassort.codeMag_sortie, dico_gencod);

                //On renvoi la liste des lignes réassort qui ne sont pas conformes.
                switch (codeMag)
                {
                    case "G0": result = result.Where(l => l.Entree_G0 != 0).ToList<LigneReassort>();
                        break;
                    case "RESERVEG0": result = result.Where(l => l.Entree_RESERVEG0 != 0).ToList<LigneReassort>();
                        break;
                    case "ROBERT": result = result.Where(l => l.Entree_ROBERT != 0).ToList<LigneReassort>();
                        break;
                    case "M0": result = result.Where(l => l.Entree_M0 != 0).ToList<LigneReassort>();
                        break;
                    case "RESERVEM0": result = result.Where(l => l.Entree_RESERVEM0 != 0).ToList<LigneReassort>();
                        break;
                    case "DEPOTM0": result = result.Where(l => l.Entree_DEPOTM0 != 0).ToList<LigneReassort>();
                        break;
                    default: throw new Exception("Le code magasin du fichier ini n'est pas référencé.");
                }

            }
            catch (Exception e)
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
                return null;
            }
            return result;
        }

        /// <summary>
        /// Déplace le fichier vers le dossier d'archive en lui donnant la date du jour
        /// </summary>
        /// <returns>renvoi vrai si l'opération a réussi, faux sinon</returns>
        public bool archive_fichier()
        {
            //On récupère le dossier par défaut depuis le fichier ini puis on construit le path complet du fichier
            IniFile ini = new IniFile();
            string path = string.Empty;

            try
            {
                path = ini.IniReadValue("REASSORT", "path") + @"\archives";

                //Si le dossier d'archives n'exite pas, on va essayer de le créer
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                File.Move(this._path_rea, path + @"\data_" + DateTime.Now.ToString("yyyyMMdd-hhmmss") + ".txt");
            }
            catch (Exception e)
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
                return false;
            }
            
            return true;
        }
    }
}
