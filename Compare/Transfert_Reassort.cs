using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using APIcodeBar.DAO;
using APIcodeBar.Entity;

namespace APIcodeBar
{
    public class Transfert_Reassort
    {
        private Transfert_Reassort() { }

        /// <summary>
        /// Compare le tranfert au reassort et renvoi les lignes reassort qui n'ont pas été correctement traitées
        /// </summary>
        /// <param name="code_transfert">code du transfert a traité</param>
        /// <param name="code_reassort">code du reassort correspondant</param>
        /// <param name="codeMag">code du magasin ou l'on se trouve</param>
        /// <returns>renvoi la liste des lignes de reassort qui n'ont pas été correctement traité et null en cas d'erreur</returns>
        public static List<LigneReassort> Compare(string code_transfert, string code_reassort, string codeMag)
        {
            Dictionary<string, int> dico_gencod = null;
            List<LigneReassort> result = new List<LigneReassort>();

            try
            {
                //On récupère le reassort pour connaître le magasin d'origine
                Reassort reassort = DAOFactory.getReassortDAO().find(code_reassort);
                dico_gencod = DAOFactory.getTransfertDAO().find(code_transfert, codeMag, reassort.codeMag_sortie);

                reassort.lignes.GetEnumerator();
                foreach (LigneReassort ligne in reassort.lignes)
                {
                    result.Add(ligne);
                    if (dico_gencod.ContainsKey(ligne.GenCode))
                    {
                        //Selon le magasin où l'on est, on va vérifié la colonne qui nous intéresse
                        //On met à jour le nomdre d'entrées réelles
                        switch (codeMag)
                        {
                            case "G0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_G0_reel = dico_gencod[ligne.GenCode];
                                break;
                            case "RESERVEG0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_RESERVEG0_reel = dico_gencod[ligne.GenCode];
                                break;
                            case "ROBERT": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_ROBERT_reel = dico_gencod[ligne.GenCode];
                                break;
                            case "M0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_M0_reel = dico_gencod[ligne.GenCode];
                                break;
                            case "RESERVEM0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_RESERVEM0_reel = dico_gencod[ligne.GenCode];
                                break;
                            case "DEPOTM0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_DEPOTM0_reel = dico_gencod[ligne.GenCode];
                                break;
                            default: throw new Exception("Le code magasin du fichier ini n'est pas référencé.");
                        }
                    }
                    else
                    {
                        //Si on ne trouve pas l'entrée, on va mettre l'entrée réel à 0
                        switch (codeMag)
                        {
                            case "G0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_G0_reel = 0;
                                break;
                            case "RESERVEG0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_RESERVEG0_reel = 0;
                                break;
                            case "ROBERT": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_ROBERT_reel = 0;
                                break;
                            case "M0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_M0_reel = 0;
                                break;
                            case "RESERVEM0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_RESERVEM0_reel = 0;
                                break;
                            case "DEPOTM0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_DEPOTM0_reel = 0;
                                break;
                            default: throw new Exception("Le code magasin du fichier ini n'est pas référencé.");
                        }
                    }
                }
                
                //Après le traitement, on met à jour le reassort en base
                if (!DAOFactory.getReassortDAO().update(reassort))
                    throw new Exception("La mise à jour de la table des reassorts a échoué.");
                if (!DAOFactory.getReassortDAO().update_traitement_reassort(reassort, codeMag))
                    throw new Exception("La mise à jour du traitement du reassort ");


                //On renvoi la liste des lignes réassort qui ne sont pas conforme.
                switch (codeMag)
                {
                    case "G0": result = result.Where(l => l.Entree_G0 != l.Entree_G0_reel).ToList<LigneReassort>();
                        break;
                    case "RESERVEG0": result = result.Where(l => l.Entree_RESERVEG0 != l.Entree_RESERVEG0_reel).ToList<LigneReassort>();
                        break;
                    case "ROBERT": result = result.Where(l => l.Entree_ROBERT != l.Entree_ROBERT_reel).ToList<LigneReassort>();
                        break;
                    case "M0": result = result.Where(l => l.Entree_M0 != l.Entree_M0_reel).ToList<LigneReassort>();
                        break;
                    case "RESERVEM0": result = result.Where(l => l.Entree_RESERVEM0 != l.Entree_RESERVEM0_reel).ToList<LigneReassort>();
                        break;
                    case "DEPOTM0": result = result.Where(l => l.Entree_DEPOTM0 != l.Entree_DEPOTM0_reel).ToList<LigneReassort>();
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
    }
}
