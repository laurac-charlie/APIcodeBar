using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DbLinq.Data.Linq;
using DbLinq.MySql;

namespace APIcodeBar.DAO
{
    public class DAOFactory
    {
        public static MySqlDataContext CONTEXT = null;

        private DAOFactory()
        {      }

        /// <summary>
        /// Initialise un contexte mysql en s'assurant qu'il ne soit pas recré à chaque appel
        /// </summary>
        /// <returns>renvoi un contexte mysql</returns>
        protected static MySqlDataContext getContext()
        {
            if (DAOFactory.CONTEXT == null)
            {
                DAOFactory.CONTEXT = new MySqlDataContext(DBConnect.getMysqlConnection());
                return DAOFactory.CONTEXT;
            }
            else
                return DAOFactory.CONTEXT;
        }

        /// <summary>
        /// Créer un objet ReassortDAO à partir du context qui vous permettra de manipuler les reassorts en base
        /// </summary>
        /// <returns>retourne un objet ReassortDAO</returns>
        public static ReassortDAO getReassortDAO()
        {
            //DataLoadOptions loadOptions = new DataLoadOptions();
            //loadOptions.AssociateWith<Reassort>(l => l.lignes);
            //context.LoadOptions = loadOptions;
            return new ReassortDAO(DAOFactory.getContext());
        }

        /// <summary>
        /// Créer un objet LigneReassortDAO à partir du context qui vous permettra de manipuler les lignes en base
        /// </summary>
        /// <returns>retourne un objet LigneReassortDAO</returns>
        public static LigneReassortDAO getLigneReassortDAO()
        {
            return new LigneReassortDAO(DAOFactory.getContext());
        }

        /// <summary>
        /// Créer un objet TransfertDAO à partir du context qui vous permettra de manipuler les transferts
        /// </summary>
        /// <returns>retourne un objet TransfertDAO</returns>
        public static TransfertDAO getTransfertDAO()
        {
            return new TransfertDAO(DAOFactory.getContext());
        }
    }
}
