using System;
using System.Text;
using System.IO;

namespace APIcodeBar
{
    public class SystemLog
    {
        private static readonly string BASE_PATH =  Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + @"\log";
        
        public static void ErrorLog(string errorType, string sErrMsg)
        {
            defaultLog(errorType, sErrMsg,@"\erreur.log");
        }

        public static void InfoLog(string sInfMsg)
        {
            defaultLog("Info",sInfMsg,@"\info.log");
        }

        private static void defaultLog(string type, string Msg,string logfile)
        {
            if (!Directory.Exists(BASE_PATH))
                Directory.CreateDirectory(BASE_PATH);

            string sPathName = SystemLog.BASE_PATH + logfile;
            StreamWriter sw = null;
            
            try
            {
                if (File.Exists(sPathName))
                {
                    FileInfo file = new FileInfo(sPathName);
                    //Limite de taille du fichier pour archivage en octects
                    if (file.Length > 5000000)
                        File.Move(sPathName, BASE_PATH + @"\info-" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".log");
                }
                
                string sLogFormat = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + " ==> ";
                
                sw = new StreamWriter(sPathName, true);
                sw.WriteLine(sLogFormat + type + " : " + Msg);
                //Si l'application est en mode graphique, on affiche une message box en cas d'erreur
                /*if (!App.auto)
                {
                    if (!type.Equals("Info"))
                        System.Windows.Forms.MessageBox.Show(type + " : " + Msg, "GestPv");
                }*/
            }
            finally
            {
                sw.Flush();
                sw.Close();
            }
        }

    }
}
