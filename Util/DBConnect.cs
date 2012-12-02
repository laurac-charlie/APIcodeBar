using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.Odbc;
using MySql.Data.MySqlClient;

namespace APIcodeBar
{
    public class DBConnect
    {
        private static SqlConnection sqlconnect = null;
        private static OdbcConnection odbcconnect = null;
        private static MySqlConnection mysqlconnect = null;

        //Renvoi une connection ouverte à la BDD en vérifiant si elle n'est pas déjà ouverte
        public static SqlConnection getSqlConnection()
        {
            if (sqlconnect == null)
            {
                sqlconnect = new SqlConnection(SqlStringBuilder());
                sqlconnect.Open();
            }
            else
            {
                if (sqlconnect.State == ConnectionState.Closed)
                    sqlconnect.Open();
            }

            return sqlconnect;
        }

        public static OdbcConnection getOdbcConnection()
        {
            if (odbcconnect == null)
            {
                odbcconnect = new OdbcConnection(OdbcStringBuilder());
                odbcconnect.Open();
            }
            else
            {
                if (odbcconnect.State == ConnectionState.Closed)
                    odbcconnect.Open();
            }

            return odbcconnect;
        }

        public static MySqlConnection getMysqlConnection()
        {
            if (mysqlconnect == null)
            {
                mysqlconnect = new MySqlConnection(MySqlStringBuilder());
                mysqlconnect.Open();
            }
            else
            {
                if (mysqlconnect.State == ConnectionState.Closed)
                    mysqlconnect.Open();
            }

            return mysqlconnect;
        }

        //Tente une connection à la base de données
        public static bool trySqlConnection(string id, string pass, string serveur, string database)
        {
            bool sucess = true;
            SqlConnection con = null;
            string chaine = "user id=" + id + ";password=" + pass + ";server=" + serveur + ";database=" + database + ";Trusted_Connection=no;connection timeout=30";
            try
            {
                con = new SqlConnection(chaine);
                con.Open();
            }
            catch (Exception e)
            {
                sucess = false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return sucess;
        }

        public static bool tryMySqlConnection(string id, string pass, string serveur, string database)
        {
            bool sucess = true;
            MySqlConnection con = null;
            string chaine = "uid=" + id + ";password=" + pass + ";server=" + serveur + ";database=" + database + ";connection timeout=30";
            try
            {
                con = new MySqlConnection(chaine);
                con.Open();
            }
            catch (Exception e)
            {
                sucess = false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return sucess;
        }

        public static bool tryOdbcConnection(string id, string pass, string serveur, string database)
        {
            bool sucess = true;
            OdbcConnection con = null;
            string chaine = "Driver={Microsoft ODBC for Oracle};Server=" + serveur + ";UID=" + id + ";PWD=" + pass + ";";
            try
            {
                con = new OdbcConnection(chaine);
                con.Open();
            }
            catch (Exception e)
            {
                sucess = false;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            return sucess;
        }

        //Construit la chaîne de connection à partir du fichier ini
        private static string SqlStringBuilder()
        {
            IniFile ini = new IniFile();
            string chaine = String.Empty;

            if (ini.IniReadValue("DB", "id") == String.Empty || ini.IniReadValue("DB", "pass") == String.Empty || ini.IniReadValue("DB", "serveur") == String.Empty || ini.IniReadValue("DB", "database") == String.Empty)
                throw new ArgumentException("La chaine de connexion à la base de données n'est pas complète.");
            else
            {
                chaine = "user id=" + ini.IniReadValue("DB", "id") + ";";
                chaine += "password=" + ini.IniReadValue("DB", "pass") + ";";
                chaine += "server=" + ini.IniReadValue("DB", "serveur") + ";";
                chaine += "database=" + ini.IniReadValue("DB", "database") + ";";
                chaine += "Trusted_Connection=no;";
                chaine += "connection timeout=30";
            }

            return chaine;
        }

        private static string MySqlStringBuilder()
        {
            IniFile ini = new IniFile();
            string chaine = String.Empty;

            if (ini.IniReadValue("DB", "id") == String.Empty || ini.IniReadValue("DB", "pass") == String.Empty || ini.IniReadValue("DB", "serveur") == String.Empty || ini.IniReadValue("DB", "database") == String.Empty)
                throw new ArgumentException("La chaine de connexion à la base de données n'est pas complète.");
            else
            {
                chaine = "user id=" + ini.IniReadValue("DB", "id") + ";";
                chaine += "password=" + ini.IniReadValue("DB", "pass") + ";";
                chaine += "server=" + ini.IniReadValue("DB", "serveur") + ";";
                chaine += "database=" + ini.IniReadValue("DB", "database") + ";";
                chaine += "connection timeout=30";
            }

            return chaine;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string OdbcStringBuilder()
        {
            IniFile ini = new IniFile();
            string chaine = String.Empty;

            if (ini.IniReadValue("DB", "id") == String.Empty || ini.IniReadValue("DB", "pass") == String.Empty || ini.IniReadValue("DB", "serveur") == String.Empty || ini.IniReadValue("DB", "database") == String.Empty)
                throw new ArgumentException("La chaine de connexion à la base de données n'est pas complète.");
            else
            {
                chaine = "Driver={Microsoft ODBC for Oracle};";
                chaine += "Server=" + ini.IniReadValue("DB", "serveur") + ";";
                chaine += "UID=" + ini.IniReadValue("DB", "id") + ";";
                chaine += "PWD=" + ini.IniReadValue("DB", "pass") + ";";
                //chaine += "DATABASE=" + ini.IniReadValue("DB", "database") + ";";
                //chaine += "Integrated Security=no;";
            }

            return chaine;
        }
    }
}
