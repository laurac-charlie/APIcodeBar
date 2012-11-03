using System.Data.Linq.Mapping;
using DbLinq.Data.Linq;

namespace APIcodeBar.Entity
{
    [Table(Name = "REASSORT_LIGNE")]
    public class LigneReassort
    {
        #region variable_propriété
        private int _id;
        [Column(IsPrimaryKey = true, Name = "id_ligne", IsDbGenerated = true)]
        public int id
        {
            get { return this._id; }
            set { this._id = value; }
        }
        private int _id_reassort;
        [Column(Name = "id_reassort")]
        public int Id_reassort
        {
            get { return _id_reassort; }
            set { _id_reassort = value; }
        }
        private EntityRef<Reassort> _reassort = new EntityRef<Reassort>();
        [Association(Storage = "_reassort", ThisKey = "Id_reassort")]
        public Reassort Reassort
        {
            get { return this._reassort.Entity; }
            set { this._reassort.Entity = value; }
        }
        /*
        private string _codeMag_sortie;
        [Column(Name = "codeMag_sortie")]
        public string CodeMag_sortie
        {
            get { return _codeMag_sortie; }
            set { _codeMag_sortie = value; }
        }
         * */
        private string _genCode;
        [Column(Name = "GenCode")]
        public string GenCode
        {
            get { return _genCode; }
            set { _genCode = value; }
        }
        private string _barCode;
        [Column(Name = "barCode")]
        public string BarCode
        {
            get { return _barCode; }
            set { _barCode = value; }
        }
        private string _designation;
        [Column(Name = "designation")]
        public string Designation
        {
            get { return _designation; }
            set { _designation = value; }
        }
        private string _couleur;
        [Column(Name = "couleur")]
        public string Couleur
        {
            get { return _couleur; }
            set { _couleur = value; }
        }
        private string _taille;
        [Column(Name = "taille")]
        public string Taille
        {
            get { return _taille; }
            set { _taille = value; }
        }
        private int? _stock_mag_sortie;
        [Column(Name = "stock_mag_sortie", CanBeNull = true)]
        public int? Stock_mag_sortie
        {
            get { return _stock_mag_sortie; }
            set { _stock_mag_sortie = value; }
        }
        private int? _sortie_mag;
        [Column(Name = "sortie_mag", CanBeNull = true)]
        public int? Sortie_mag
        {
            get { return _sortie_mag; }
            set { _sortie_mag = value; }
        }
        private int? _entree_M0;
        [Column(Name = "entree_M0", CanBeNull = true)]
        public int? Entree_M0
        {
            get { return _entree_M0; }
            set { _entree_M0 = value; }
        }
        private int? _entree_M0_reel;
        [Column(Name = "entree_M0_reel", CanBeNull = true)]
        public int? Entree_M0_reel
        {
            get { return _entree_M0_reel; }
            set { _entree_M0_reel = value; }
        }
        private int? _entree_G0;
        [Column(Name = "entree_G0", CanBeNull = true)]
        public int? Entree_G0
        {
            get { return _entree_G0; }
            set { _entree_G0 = value; }
        }
        private int? _entree_G0_reel;
        [Column(Name = "entree_G0_reel", CanBeNull = true)]
        public int? Entree_G0_reel
        {
            get { return _entree_G0_reel; }
            set { _entree_G0_reel = value; }
        }
        private int? _entree_ROBERT;
        [Column(Name = "entree_ROBERT",CanBeNull=true)]
        public int? Entree_ROBERT
        {
            get { return _entree_ROBERT; }
            set { _entree_ROBERT = value; }
        }
        private int? _entree_ROBERT_reel;
        [Column(Name = "entree_ROBERT_reel", CanBeNull = true)]
        public int? Entree_ROBERT_reel
        {
            get { return _entree_ROBERT_reel; }
            set { _entree_ROBERT_reel = value; }
        }
        private int? _entree_RESERVEM0;
        [Column(Name = "entree_RESERVEM0")]
        public int? Entree_RESERVEM0
        {
            get { return _entree_RESERVEM0; }
            set { _entree_RESERVEM0 = value; }
        }
        private int? _entree_RESERVEM0_reel;
        [Column(Name = "entree_RESERVEM0_reel")]
        public int? Entree_RESERVEM0_reel
        {
            get { return _entree_RESERVEM0_reel; }
            set { _entree_RESERVEM0_reel = value; }
        }
        private int? _entree_DEPOTM0;
        [Column(Name = "entree_DEPOTM0")]
        public int? Entree_DEPOTM0
        {
            get { return _entree_DEPOTM0; }
            set { _entree_DEPOTM0 = value; }
        }
        private int? _entree_DEPOTM0_reel;
        [Column(Name = "entree_DEPOTM0_reel")]
        public int? Entree_DEPOTM0_reel
        {
            get { return _entree_DEPOTM0_reel; }
            set { _entree_DEPOTM0_reel = value; }
        }
        private int? _entree_RESERVEG0;
        [Column(Name = "entree_RESERVEG0")]
        public int? Entree_RESERVEG0
        {
            get { return _entree_RESERVEG0; }
            set { _entree_RESERVEG0 = value; }
        }
        private int? _entree_RESERVEG0_reel;
        [Column(Name = "entree_RESERVEG0_reel")]
        public int? Entree_RESERVEG0_reel
        {
            get { return _entree_RESERVEG0_reel; }
            set { _entree_RESERVEG0_reel = value; }
        }
        /*
        private bool _valide;
        [Column(Name = "valide")]
        public bool Valide
        {
            get { return _valide; }
            set { _valide = value; }
        }*/
        #endregion

        public LigneReassort() { }

        public LigneReassort(Reassort reassort, /*string code_mag_sortie,*/ string gencode,
            string barcode)
        {
            this._reassort.Entity = reassort;
            this._id_reassort = reassort.id;
            //this._codeMag_sortie = code_mag_sortie;
            this._genCode = gencode;
            this._barCode = barcode;
        }

        public LigneReassort(Reassort reassort, /*string code_mag_sortie,*/ string gencode, 
            string barcode, string designation, string couleur, string taille, int stock_mag_sortie, 
            int sortie_mag, int entree_m0, int entree_g0, int entree_robert, int entree_reservem0) 
        {
            this._reassort.Entity = reassort;
            //this._codeMag_sortie = code_mag_sortie;
            this._genCode = gencode;
            this._barCode = barcode;
            this._designation = designation;
            this._couleur = couleur;
            this._taille = taille;
            this._stock_mag_sortie = stock_mag_sortie;
            this._sortie_mag = sortie_mag;
            this._entree_M0 = entree_m0;
            this._entree_G0 = entree_g0;
            this._entree_ROBERT = entree_robert;
            this._entree_RESERVEM0 = entree_reservem0;
        }

        public LigneReassort(string gencode, string barcode, string designation, string couleur, string taille/*,
            int entree_m0, int entree_g0, int entree_robert, int entree_reservem0,int entree_reserveG0,int entree_reserveM0*/)
        {
            this._genCode = gencode;
            this._barCode = barcode;
            this._designation = designation;
            this._couleur = couleur;
            this._taille = taille;
            /*this._entree_M0 = entree_m0;
            this._entree_G0 = entree_g0;
            this._entree_ROBERT = entree_robert;
            this._entree_RESERVEM0 = entree_reserveM0;
            this.Entree_RESERVEG0 = entree_reserveG0;*/
        }
    }
}
