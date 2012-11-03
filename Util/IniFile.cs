using System;
using System.Runtime.InteropServices;
using System.Text;

namespace APIcodeBar
{
    // Create a New INI file to store or load data

    public class IniFile
    {
        //private static string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + @"\file.ini";
        private static readonly string path = System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + @"\config.ini";

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section,
            string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section,
                 string key, string def, StringBuilder retVal,
            int size, string filePath);

        public IniFile()
        {
            if (!System.IO.File.Exists(IniFile.path))
            {
                System.IO.FileStream fs = System.IO.File.Create(IniFile.path);
                fs.Close();
            }
            //else
            //    System.IO.File.Decrypt(IniFile.path);
        }

        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, IniFile.path);
        }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp,
                                            255, IniFile.path);
            return temp.ToString();

        }

        ~IniFile()
        {
            //System.IO.File.Encrypt(IniFile.path);
        }
    }
}
