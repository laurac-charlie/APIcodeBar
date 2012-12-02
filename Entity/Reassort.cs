using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using DbLinq.Data.Linq;

namespace APIcodeBar.Entity
{
    [Table(Name = "REASSORT")]
    public class Reassort
    {
        #region variable_propriété
        private int _id;
        private string _code= String.Empty;
        private DateTime _date = DateTime.Now;
        private string _codeMag_sortie;
        private bool _valide_M0 = false;
        private bool _valide_RESERVEM0 = false;
        private bool _valide_DEPOTM0 = false;
        private bool _valide_ROBERT = false;
        private bool _valide_G0 = false;
        private bool _valide_RESERVEG0 = false;
        private EntitySet<LigneReassort> _lignes = new EntitySet<LigneReassort>();
        [Column(IsPrimaryKey = true, Name = "id_reassort", IsDbGenerated = true)]
        public int id { get { return _id; } set { _id = value;} }
        [Column(Name = "code_rea")]
        public string code { get { return _code; } set { _code = value; } }
        [Column(Name = "date_rea")]
        public DateTime date { get { return _date; } set { _date = value; } }
        [Column(Name = "codeMag_sortie")]
        public string codeMag_sortie { get { return _codeMag_sortie; } set { _codeMag_sortie = value; } }
        [Column(Name = "valide_M0")]
        public bool valide_M0 { get { return _valide_M0; } set { _valide_M0 = value; } }
        [Column(Name = "valide_RESERVEM0")]
        public bool valide_RESERVEM0 { get { return _valide_RESERVEM0; } set { _valide_RESERVEM0 = value; } }
        [Column(Name = "valide_DEPOTM0")]
        public bool valide_DEPOTM0 { get { return _valide_DEPOTM0; } set { _valide_DEPOTM0 = value; } }
        [Column(Name = "valide_ROBERT")]
        public bool valide_ROBERT { get { return _valide_ROBERT; } set { _valide_ROBERT = value; } }
        [Column(Name = "valide_G0")]
        public bool valide_G0 { get { return _valide_G0; } set { _valide_G0 = value; } }
        [Column(Name = "valide_RESERVEG0")]
        public bool valide_RESERVEG0 { get { return _valide_RESERVEG0; } set { _valide_RESERVEG0 = value; } }
        [Association(Storage = "_lignes", OtherKey = "id")]
        public ICollection<LigneReassort> lignes { get { return this._lignes; } set { this._lignes.Assign(value); } }
        #endregion

        public Reassort() 
        {
            IniFile ini = new IniFile();
            this._codeMag_sortie = ini.IniReadValue("Reassort", "codeMag");
        }

        public Reassort(string codeMag_sortie)
        {
            this._codeMag_sortie = codeMag_sortie;
            this._date = DateTime.Now;
        }

        /*public Reassort(string code, DateTime date) 
        {
            
            this._code = code;
            this._date = date;
        }*/

        /// <summary>
        /// Génrère un code aléatoire identifiant le reassort
        /// </summary>
        /// <returns>le code aléatoirement généré du reassort</returns>
        public string genCodeReassort()
        {
            //On enlève le seed pour avoir des chiffres différents
            Random d = new Random();
            string code = this._id + this._codeMag_sortie + d.Next(1000, 9999);
            return code;
        }

    }
}
