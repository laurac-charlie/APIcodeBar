using System;
using System.Collections.Generic;
using System.Linq;
using DbLinq.Data.Linq;
using DbLinq.MySql;
using APIcodeBar.Entity;

namespace APIcodeBar.DAO
{
    public class LigneReassortDAO
    {
        private MySqlDataContext _context = null;
        private Table<Reassort> _table_reassort = null;
        private Table<LigneReassort> _table_lignes_reassort = null;

        public LigneReassortDAO(MySqlDataContext context)
        {
            this._context = context;
            this._table_reassort = context.GetTable<Reassort>();
            this._table_lignes_reassort = context.GetTable<LigneReassort>();
        }

        /// <summary>
        /// Crée un nouveau reassort à partir d'un objet ligne Reassort
        /// </summary>
        /// <param name="lignes_reassort">une ligne reassort à recréer</param>
        /// <returns>retourne vrai si l'opération a réussi et faux en cas d'echec</returns>
        public bool create(LigneReassort lignes_reassort) 
        {
            try
            {
                this._table_lignes_reassort.InsertOnSubmit(lignes_reassort);
                this._context.SubmitChanges();
                return true;
            }
            catch (Exception e )
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
                return false;
            }
        }

        /// <summary>
        /// Trouve un une ligne d'un reassort à partit de son id
        /// </summary>
        /// <param name="id">id de la ligne (entier)</param>
        /// <returns>renvoi la ligne ou null en cas d'échec</returns>
        public LigneReassort find(int id)
        {
            try
            {
                LigneReassort ligne = (from l in this._table_lignes_reassort
                                       where l.id == id
                                       select l).FirstOrDefault<LigneReassort>();
                ligne.Reassort = (from r in this._table_reassort
                                  where r.id == ligne.Id_reassort
                                  select r).FirstOrDefault<Reassort>();
                return ligne;
            }
            catch (Exception e)
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
                return null;
            }
        }

        /// <summary>
        /// Renvoi toutes les lignes d'un reassort à partir d'un objet Reassort existant
        /// </summary>
        /// <param name="reassort">un objet reassort</param>
        /// <returns>retourne un collection contenant les lignes du reassort ou null en cas d'echec</returns>
        public ICollection<LigneReassort> findAllLignes(Reassort reassort, string codeMag)
        {
            ICollection<LigneReassort> lignes_reassort = null;

            try
            {
                
                lignes_reassort = (from l in this._table_lignes_reassort where l.Id_reassort == reassort.id select l).ToList<LigneReassort>();

                switch (codeMag)
                {
                    case "G0": lignes_reassort = (from l in this._table_lignes_reassort where l.Id_reassort == reassort.id && l.Entree_G0 != null  && l.Entree_G0 > 0 select l).ToList<LigneReassort>();
                        break;
                    case "RESERVEG0": lignes_reassort = (from l in this._table_lignes_reassort where l.Id_reassort == reassort.id && l.Entree_RESERVEG0 != null && l.Entree_RESERVEG0 > 0 select l).ToList<LigneReassort>();
                        break;
                    case "ROBERT": lignes_reassort = (from l in this._table_lignes_reassort where l.Id_reassort == reassort.id && l.Entree_ROBERT != null && l.Entree_ROBERT > 0 select l).ToList<LigneReassort>();
                        break;
                    case "M0": lignes_reassort = (from l in this._table_lignes_reassort where l.Id_reassort == reassort.id && l.Entree_M0 != null && l.Entree_M0 > 0 select l).ToList<LigneReassort>();
                        break;
                    case "RESERVEM0": lignes_reassort = (from l in this._table_lignes_reassort where l.Id_reassort == reassort.id && l.Entree_RESERVEM0 != null && l.Entree_RESERVEM0 > 0 select l).ToList<LigneReassort>();
                        break;
                    case "DEPOTM0": lignes_reassort = (from l in this._table_lignes_reassort where l.Id_reassort == reassort.id && l.Entree_DEPOTM0 != null && l.Entree_DEPOTM0 > 0 select l).ToList<LigneReassort>();
                        break;
                    default: throw new Exception("Le code magasin du fichier ini n'est pas référencé.");
                }

                return lignes_reassort;
            }
            catch (Exception e)
            {
                SystemLog.ErrorLog(e.GetType().ToString(), e.Message);
                return null;
            }
        }
    }
}
