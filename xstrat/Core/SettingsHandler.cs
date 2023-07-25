using NuGet;
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
        public static readonly string OldSettingsFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/xstrat/settings/settings.txt";
        public static readonly string SettingsFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/xstrat/settings/settings.xml";
        public static readonly string SettingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/xstrat/settings";
        public static readonly string MapsFolder = Globals.XStratInstallPath + @"/Images/Maps/";
        public static readonly string XStratReplayPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/xstrat/replays";

        public static SettingsModel Settings { get; private set; }

       
        public static void Initialize()
        {
            if (!Directory.Exists(SettingsFolder))
            {
                Directory.CreateDirectory(SettingsFolder);
            }
            if (!Directory.Exists(MapsFolder))
            {
                Directory.CreateDirectory(MapsFolder);
            }
            if (File.Exists(OldSettingsFile) && !File.Exists(SettingsFile))
            {
                Migrate(OldSettingsFile);
            }
            if (!File.Exists(SettingsFile))
            {
                Create();
            }
            Load();
        }

        public static void Load()
        {
            Settings = Globals.DeserializeFromFile<SettingsModel>(SettingsFile);
        }

        public static void Save()
        {
            Globals.SerializeToFile<SettingsModel>(Settings, SettingsFile);
        }
        private static void Create()
        {
            var newSettings = new SettingsModel();
            Globals.SerializeToFile<SettingsModel>(newSettings, SettingsFile);
        }

        public static void Migrate(string txtPath)
        {
            Notify.sendInfo("Migrating settings...");
            var newSettings = new SettingsModel();
            string[] lines = File.ReadAllLines(Path.Combine(txtPath));
            try
            {
                newSettings.StayLoggedin = Convert.ToBoolean(lines[0]);
                newSettings.Token = lines[1];
                newSettings.SkinSwitcherPath = lines[2];
                newSettings.SkinSwitcherStatus = Convert.ToBoolean(lines[3]);
                newSettings.APIURL = lines[4];
                newSettings.LastLoginMail = lines[5];
                newSettings.CurrentUserId = lines[6];
                newSettings.GameReplayPath = lines[7];
            }
            catch (Exception ex)
            {
                Logger.Log("Error when migrating settings:" + ex.Message);
            }
            Settings = newSettings;
            Save();
            Notify.sendSuccess("Migrated successfully");
        }
    }

    public class SettingsModel
    {
        //settings propeties:
        public bool StayLoggedin { get; set; }
        public string Token { get; set; }
        public string SkinSwitcherPath { get; set; }
        public bool SkinSwitcherStatus { get; set; }
        public string APIURL { get; set; }
        public string LastLoginMail { get; set; }
        public string CurrentUserId { get; set; }
        public string GameReplayPath { get; set; }
    }

}
