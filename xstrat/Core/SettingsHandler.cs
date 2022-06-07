using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xstrat.Core
{
    public static class SettingsHandler
    {
        //path: documents/xstrat/settings

        /*FILE:
         * Stay logged in
         * SkinswitcherUbiFolder
         * API domain
         * Last Login Mail
         */
        

        static readonly string SettingsFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/xstrat/settings/settings.txt";
        static readonly string SettingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/xstrat/settings";

        //settings propeties:
        public static bool StayLoggedin { get; set; }
        public static string token { get; set; }
        public static string SkinSwitcherPath { get; set; }
        public static bool SkinSwitcherStatus { get; set; }
        public static string APIURL { get; set; }
        public static string LastLoginMail { get; set; }

        public static void Initialize()
        {
            if (!Directory.Exists(SettingsFolder))
            {
                Directory.CreateDirectory(SettingsFolder);
            }
            if (!File.Exists(SettingsFile))
            {
                File.Create(SettingsFile);
                Create();
            }
            Load();
        }

        public static void Load()
        {
            string[] lines = File.ReadAllLines(Path.Combine(SettingsFile));
            try
            {
                StayLoggedin = Convert.ToBoolean(lines[0]);
                token = lines[1];
                SkinSwitcherPath = lines[2];
                SkinSwitcherStatus = Convert.ToBoolean(lines[3]);
                APIURL = lines[4];
                LastLoginMail = lines[5];
            }
            catch (Exception ex)
            {
                Logger.Log("Error when loading settings:" + ex.Message);
            }

        }

        public static void Save()
        {
            string[] newlines = {
            StayLoggedin.ToString(),
            token,
            SkinSwitcherPath,
            SkinSwitcherStatus.ToString(),
            APIURL,
            LastLoginMail
            };

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(SettingsFile)))
            {
                foreach (string line in newlines)
                    outputFile.WriteLine(line);
            }
        }
        private static void Create()
        {
            string[] newlines = {
            false.ToString(),
            "",
            "",
            false.ToString(),
            "",
            ""
            };

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(SettingsFile)))
            {
                foreach (string line in newlines)
                    outputFile.WriteLine(line);
            }
        }
    }
}
