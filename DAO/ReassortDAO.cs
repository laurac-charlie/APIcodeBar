using System;
using System.Linq;
using System.Transactions;
using DbLinq.Data.Linq;
using DbLinq.MySql;
using APIcodeBar.Entity;

namespace APIcodeBar.DAO
{
    public class ReassortDAO
    {
        private MySqlDataContext _context = null;
        private Table<Reassort> _table_reassort = null;
        private Table<LigneReassort> _table_lignes_reassort = null;

        public ReassortDAO(MySqlDataContext context) 
        {
            this._context = context;
            this._table_reassort = context.GetTable<Reassort>();
            this._table_lignes_reassort = context.GetTable<LigneReassort>();
        }

        /// <summary>
        /// Crée un nouveau reassort à partir d'un objet Reassort
        /// </summary>
        /// <param name="reassort">un reassort a créé</param>
        /// <returns>retourne vrai si l'opération a réussi et faux en cas d'echec</returns>
        public bool create(Reassort reassort) 
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    this._table_reassort.InsertOnSubmit(reassort);
                    this._context.SubmitChanges();
                    this._table_reassort.Single(r => r.id == reassort.id).code = reassort.genCodeReassort();
                    this._context.SubmitChanges();

                    //Pour chaque ligne on met à jour le reassort auquel elle est liée puis on ajoute la ligne
                    foreach (LigneReassort l in reassort.lignes)
                    {
                        l.Reassort = reassort;
                        l.Id_reassort = reassort.id;
                        this._table_lignes_reassort.InsertOnSubmit(l);
                    }

                    this._context.SubmitChanges();
                    ts.Complete();
                }
                return true;
            }
            catch (Exception e )
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
                return false;
            }
        }

        /// <summary>
        /// Trouve un reassort dans la base à partir de son code
        /// </summary>
        /// <param name="code_rea">Chaine de caractère correspondant au code reassort</param>
        /// <returns>Renvoi le reassort concerné ou null si aucun reassort n'a été trouvé</returns>
        public Reassort find(string code_rea)
        {
            try
            {
                Reassort reassort = (from r in this._table_reassort 
                                     where r.code == code_rea 
                                     select r).First<Reassort>();
                int id_rea = reassort.id;
                reassort.lignes = (from l in this._table_lignes_reassort 
                                   where l.Id_reassort == id_rea
                                   select l).ToList<LigneReassort>();
                return reassort;
            }
            catch (Exception e)
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
                return null;
            }
        }

        /// <summary>
        /// Vérifie si un code reassort a déjà été traité pour le magasin concerné.
        /// </summary>
        /// <param name="code_rea">code du reassort a vérifié</param>
        /// <param name="codeMag">code du magasin dans lequel on rçoit le reassort</param>
        /// <returns>retour vrai si le reassort existe et qu'il n'a pas encore été traité et faux dans le cas contraire</returns>
        public bool is_valid(string code_rea, string codeMag)
        {
            bool valid = false;
            try
            {
                Reassort reassort = (from r in this._table_reassort
                                     where r.code == code_rea
                                     select r).First<Reassort>();
                if (reassort == null)
                    return false;

                switch (codeMag)
                {
                    /*
                    case "G0": valid = (reassort.valide_G0) ? true : false;
                        break;
                    case "RESERVEG0": valid = (reassort.valide_RESERVEG0) ? true : false;
                        break;
                    case "ROBERT": valid = (reassort.valide_ROBERT) ? true : false;
                        break;
                    case "M0": valid = (reassort.valide_M0) ? true : false;
                        break;
                    case "RESERVEM0": valid = (reassort.valide_RESERVEM0) ? true : false;
                        break;
                    case "DEPOTM0": valid = (reassort.valide_DEPOTM0) ? true : false;
                        break;
                    default: throw new Exception("Le code magasin du fichier ini n'est pas référencé.");
                     */
                    case "G0": valid = (reassort.valide_G0) ? false : true;
                        break;
                    case "RESERVEG0": valid = (reassort.valide_RESERVEG0) ? false : true;
                        break;
                    case "ROBERT": valid = (reassort.valide_ROBERT) ? false : true;
                        break;
                    case "M0": valid = (reassort.valide_M0) ? false : true;
                        break;
                    case "RESERVEM0": valid = (reassort.valide_RESERVEM0) ? false : true;
                        break;
                    case "DEPOTM0": valid = (reassort.valide_DEPOTM0) ? false : true;
                        break;
                    default: throw new Exception("Le code magasin du fichier ini n'est pas référencé.");
                }
            }
            catch (Exception e)
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
                return false;
            }
            
            return valid;
        }

        /// <summary>
        /// Met à jour le reassort(ligne et Code) à partir de celui donné en paramètre
        /// </summary>
        /// <param name="reassort">reassort mis à jour</param>
        /// <returns>vrai si l'opération a réussi, faux dans le cas contraire</returns>
        public bool update(Reassort reassort)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    this._table_reassort.Single(r => r.id == reassort.id).lignes = reassort.lignes;
                    this._table_reassort.Single(r => r.id == reassort.id).code = reassort.code;
                    this._context.SubmitChanges();
                    ts.Complete();
                }
                return true;
            }
            catch (Exception e)
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
                return false;
            }
        }

        /// <summary>
        /// Met à jour le champ de traitement du reassort pour le magasin entré en paramètre
        /// </summary>
        /// <param name="reassort">le reassort que l'on est en train de traiter (on le récupère avec find)</param>
        /// <param name="codeMag">le code du magasin ou l'on se trouve récupéré depuis le fichier ini</param>
        /// <returns>retourne vrai si l'opération a réussi, faux dans le cas contraire</returns>
        public bool update_traitement_reassort(Reassort reassort,string codeMag)
        {
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    switch (codeMag)
                    {
                        case "G0": this._table_reassort.Single(r => r.id == reassort.id).valide_G0 = true;
                            break;
                        case "RESERVEG0": this._table_reassort.Single(r => r.id == reassort.id).valide_RESERVEG0 = true;
                            break;
                        case "ROBERT": this._table_reassort.Single(r => r.id == reassort.id).valide_ROBERT = true;
                            break;
                        case "M0": this._table_reassort.Single(r => r.id == reassort.id).valide_M0 = true;
                            break;
                        case "RESERVEM0": this._table_reassort.Single(r => r.id == reassort.id).valide_RESERVEM0 = true;
                            break;
                        case "DEPOTM0": this._table_reassort.Single(r => r.id == reassort.id).valide_DEPOTM0 = true;
                            break;
                        default: throw new Exception("Le code magasin du fichier ini n'est pas référencé.");
                    }
                    this._context.SubmitChanges();
                    ts.Complete();
                }
                return true;
            }
            catch (Exception e)
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
                return false;
            }
        }
    }
}
