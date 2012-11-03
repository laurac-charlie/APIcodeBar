using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using APIcodeBar.DAO;
using APIcodeBar.Entity;

namespace APIcodeBar
{
    public class Fichier_reassort
    {
        #region variables
        public static readonly string BASE_PATH = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        
        private string _path_rea = String.Empty;
        public string Path_rea { get { return _path_rea; } set { _path_rea = value; } }

        private string _codeMag = String.Empty;
        public string CodeMag { get { return _codeMag; } set { _codeMag = value; } }
        #endregion

        #region constructeur
        private Fichier_reassort() { }

        ///<summary>Créer un objet à partir du fichier</summary>
        ///<param name="nom_fichier">nom du fichier dans le répertoire C:/PMS/reassort/import</param>
        ///<returns></returns>
        public Fichier_reassort(string nom_fichier)
        {
            try
            {
                //On récupère le dossier par défaut depuis le fichier ini puis on construit le path complet du fichier
                IniFile ini = new IniFile();
                string path = string.Empty;

                if (!String.IsNullOrWhiteSpace(ini.IniReadValue("REASSORT", "codeMag")))
                    this._codeMag = ini.IniReadValue("REASSORT", "codeMag");
                else
                    throw new Exception("Le paramètre ini du code magasin n'a pas été initialisé.");

                if (!String.IsNullOrWhiteSpace(ini.IniReadValue("REASSORT", "path")))
                     path = ini.IniReadValue("REASSORT", "path") + @"\"+ nom_fichier;
                else
                    throw new Exception("Le paramètre ini du dossier d'import n'a pas été initialisé.");

                if (!File.Exists(path))
                    throw new Exception("Le fichier n'existe pas et n'a donc pas pu être chargé.");
                else
                    this._path_rea = path;
            }
            catch (Exception e)
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
            }
        }
        #endregion

        ///<summary>Charge les données du fichier data.txt et les comparent au reassort dont le code est donné en paramètre</summary>
        ///<param name="code_reassort">code du reassort valide, donné par l'utilisateur</param>
        ///<returns>renvoi la liste des lignes reassort pour lesquelles il manque de produit, null en cas d'erreur</returns>
        public List<LigneReassort> compare_fichier_reassort(string code_reassort)
        {
            string ligne_douchette = String.Empty;
            Reassort reassort = null;
            Dictionary<string, int> dico = new Dictionary<string, int>();
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
                        //dico.Add(tab_text[0], int.Parse(tab_text[1])); 
                        dico.Add(ligne_douchette, 1);
                    }
                    catch (ArgumentException ae)
                    {
                        //dico[tab_text[0]] = int.Parse(tab_text[1]) + dico[tab_text[0]];
                        dico[ligne_douchette] = dico[ligne_douchette] + 1;
                    }
                }
                sr.Close();

                //On récupère le réassort qui nous intéresse à partir du code
                //La validité du réassort est contrôlé préalablement
                reassort = DAOFactory.getReassortDAO().find(code_reassort);
                reassort.lignes.GetEnumerator();
                foreach (LigneReassort ligne in reassort.lignes)
                {
                    if (dico.ContainsKey(ligne.GenCode))
                    {
                        //Selon le magasin où l'on est, on va vérifié la colonne qui nous intéresse
                        //On met à jour le nomdre d'entrées réelles
                        switch (this._codeMag)
                        {
                            case "G0" : reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_G0_reel = dico[ligne.GenCode];
                                break;
                            case "RESERVEG0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_RESERVEG0_reel = dico[ligne.GenCode];
                                break;
                            case "ROBERT": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_ROBERT_reel = dico[ligne.GenCode];
                                break;
                            case "M0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_M0_reel = dico[ligne.GenCode];
                                break;
                            case "RESERVEM0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_RESERVEM0_reel = dico[ligne.GenCode];
                                break;
                            default: throw new Exception("Le code magasin du fichier ini n'est pas référencé.");
                        }
                    }
                    else
                    {
                        //Si on ne trouve pas l'entrée, on va mettre l'entrée réel à 0
                        switch (this._codeMag)
                        {
                            case "G0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_G0_reel = 0;// reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_G0;
                                break;
                            case "RESERVEG0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_RESERVEG0_reel = 0;// reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_RESERVEG0;
                                break;
                            case "ROBERT": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_ROBERT_reel = 0;// reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_ROBERT;
                                break;
                            case "M0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_M0_reel = 0;// reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_M0;
                                break;
                            case "RESERVEM0": reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_RESERVEM0_reel = 0;// reassort.lignes.Where(l => l.id == ligne.id).FirstOrDefault<LigneReassort>().Entree_RESERVEM0;
                                break;
                            default: throw new Exception("Le code magasin du fichier ini n'est pas référencé.");
                        }
                    }
                }

                //Après le traitement, on met à jour le reassort en base
                DAOFactory.getReassortDAO().update(reassort);

                switch (this._codeMag)
                {
                    case "G0": result = reassort.lignes.Where(l => l.Entree_G0 > l.Entree_G0_reel).ToList<LigneReassort>();
                        break;
                    case "RESERVEG0": result = reassort.lignes.Where(l => l.Entree_RESERVEG0 > l.Entree_RESERVEG0_reel).ToList<LigneReassort>();
                        break;
                    case "ROBERT": result = reassort.lignes.Where(l => l.Entree_ROBERT > l.Entree_ROBERT_reel).ToList<LigneReassort>();
                        break;
                    case "M0": result = reassort.lignes.Where(l => l.Entree_M0 > l.Entree_M0_reel).ToList<LigneReassort>();
                        break;
                    case "RESERVEM0": result = reassort.lignes.Where(l => l.Entree_RESERVEM0 > l.Entree_RESERVEM0_reel).ToList<LigneReassort>();
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

        
        public bool archive_fichier()
        {
            //On récupère le dossier par défaut depuis le fichier ini puis on construit le path complet du fichier
            IniFile ini = new IniFile();
            string path = string.Empty;

            try
            {
                path = this._path_rea + @"\archives";

                //Si le dossier d'archives n'exite pas, on va essayer de le créer
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                File.Move(this._path_rea, path + @"\data" + DateTime.Now.ToString("yyyyMMdd-hhmmss") + ".txt");
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
