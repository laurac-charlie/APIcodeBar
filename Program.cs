﻿using System;
using System.Collections.Generic;
using System.Linq;
using APIcodeBar.Entity;
using APIcodeBar.DAO;
using System.Threading;

namespace APIcodeBar
{
    class Program
    {
        /// <summary>
        /// Fonction de présentation de l'APIcodeBar
        /// </summary>
        /// <param name="args">arguments de la ligne de commande</param>
        static void Main(string[] args)
        {
            string[] info =  DAOFactory.getTransfertDAO().info_user("176054");
            Console.WriteLine(info[0] + " a réalisé le reassort le " + info[1] + " à " + info[2]);
            
            /*Insertion des reassorts avec leurs lignes
            Reassort rea = new Reassort("G0");
            List<LigneReassort> lignes_rea = new List<LigneReassort>();
            LigneReassort lig = new LigneReassort();

            for (int count = 0; count < 10; count++ )
            {
                lig = new LigneReassort();
                lig.BarCode = "code" + count;
                lig.Couleur = "couleur" + count;
                lig.Designation = "designation" + count ;
                lig.GenCode = "100000000" + count;
                lig.Taille = "taille" + count;

                lignes_rea.Add(lig);
                //rea.lignes.Add(lig);
            }
            rea.lignes = lignes_rea;
            DAOFactory.getReassortDAO().create(rea);
             * */
            
            //Variables
            string code_transfert = string.Empty;
            string code_rea = string.Empty;
            string code_mag = string.Empty;
            string origine = string.Empty;

            //Au chargement de l'application ou de la fenêtre
            IniFile ini = new IniFile();
            if (!String.IsNullOrWhiteSpace(ini.IniReadValue("REASSORT", "codeMag")))
                code_mag = ini.IniReadValue("REASSORT", "codeMag");
            else
                throw new Exception("Le paramètre ini du code magasin n'a pas été initialisé.");
            
            //L'utilisateur tape le code réassort et le code de transfert
            code_transfert = "144161";
            code_rea = "TESTREA1";

            int nb_lignes_transfert = DAOFactory.getTransfertDAO().find(code_transfert, code_mag, DAOFactory.getReassortDAO().find(code_rea).codeMag_sortie).Count;
            //nb_lignes_reassort = DAOFactory.getReassortDAO().find(code_rea).lignes.Count;
            int nb_lignes_reassort = DAOFactory.getLigneReassortDAO().findAllLignes(DAOFactory.getReassortDAO().find(code_rea), code_mag).Count;

            //Vérifier que le code reassort et que le code de transfert existe
            bool rea_valid,transfert_valid = false;
            rea_valid = DAOFactory.getReassortDAO().is_valid(code_rea, code_mag);
            transfert_valid = DAOFactory.getTransfertDAO().is_valid(code_transfert, code_rea, code_mag);
            if (rea_valid) Console.WriteLine("reassort exist");
            if (transfert_valid) Console.WriteLine("transfert exist");

            //On compare le fichier de la douchette au transfert
            Fichier_Transfert fichier = new Fichier_Transfert("data.txt");
            if (!fichier.fichier_existe())
                Console.WriteLine("Le fichier n'existe pas");
            else
            {
                List<LigneReassort> lignes2 = fichier.compare_fichier_trasfert(code_rea, code_transfert, code_mag);
                fichier.archive_fichier();

                if (lignes2.Count > 0)
                {
                    foreach (LigneReassort l in lignes2)
                    {
                        switch (code_mag)
                        {
                            case "G0": Console.WriteLine("il manque " + l.Entree_G0  + " produits du modèle :" + l.Designation);
                                break;
                            case "RESERVEG0": Console.WriteLine("il manque " + l.Entree_RESERVEG0 + " produits du modèle :" + l.Designation);
                                break;
                            case "ROBERT": Console.WriteLine("il manque " + l.Entree_ROBERT+ " produits du modèle :" + l.Designation);
                                break;
                            case "M0": Console.WriteLine("il manque " + l.Entree_M0 + " produits du modèle :" + l.Designation);
                                break;
                            case "RESERVEM0": Console.WriteLine("il manque " + l.Entree_RESERVEG0 + " produits du modèle :" + l.Designation);
                                break;
                            default: throw new Exception("Le code magasin du fichier ini n'est pas référencé.");
                        }
                    }
                }
                else
                    Console.WriteLine("pas de ligne en erreur");

            }

            //Compare le transfert au reassort en erreur
            List<LigneReassort> lignes = Transfert_Reassort.Compare(code_transfert, code_rea, code_mag);
            if (lignes.Count > 0)
            {
                //Récupéré les lignes en erreur et les mettre dans le corps du mail
                Console.WriteLine("Envoi de mail");
            }

            //On remet le contexte à null pour éviter les problèmes de mise à jour
            DAOFactory.CONTEXT = null;

            int i = 0;
        }
    }
}
