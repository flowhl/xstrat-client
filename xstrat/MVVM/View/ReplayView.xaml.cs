using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using xstrat.Core;
using static System.Net.Mime.MediaTypeNames;
using Path = System.IO.Path;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for ReplayView.xaml
    /// </summary>
    public partial class ReplayView : UserControl
    {

        public List<MatchReplayFolder> ReplayFolders { get; set; }

        public ReplayView()
        {
            InitializeComponent();
            Loaded += ReplayView_Loaded;
        }

        private void ReplayView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReplays();
            myDataGrid.ItemsSource = ReplayFolders;
        }

        public void LoadReplays()
        {
            //Refresh List

            GenerateFolderList();

            //Update UI
        }

        #region FileHandling

        public void GenerateFolderList()
         {
            var list = new List<MatchReplayFolder>();

            string xstratpath = SettingsHandler.XStratReplayPath;
            string gamepath = SettingsHandler.GameReplayPath;

            if (gamepath.IsNullOrEmpty() || !Directory.Exists(gamepath))
            {
                Notify.sendWarn("Please set Game Replay Path in Settings");
                return;
            }
            if (xstratpath.IsNullOrEmpty())
            {
                Notify.sendWarn("Could not find XStrat Replay Folder: " + xstratpath);
                return;
            }
            Directory.CreateDirectory(xstratpath);

            //Get Saved Replays

            var xstratreplays = Directory.GetDirectories(xstratpath, "*", SearchOption.AllDirectories);
            foreach (var xreplay in xstratreplays)
            {
                //has rounds in it
                if (!hasRounds(xreplay)) continue;

                var rep = new MatchReplayFolder();

                string foldername = Path.GetFileName(xreplay);

                rep.FolderName = foldername;
                rep.IsXStratFolder = true;

                rep.JsonCreated = File.Exists(Path.Combine(xstratpath, foldername + ".json"));

                list.Add(rep);
            }

            //Get Game Folder Replays
            var gamereplays = Directory.GetDirectories(gamepath, "*", SearchOption.AllDirectories);
            foreach (var greplay in gamereplays)
            {
                //has rounds in it
                if (!hasRounds(greplay)) continue;

                string foldername = Path.GetFileName(greplay);
                
                MatchReplayFolder rep;

                rep = list.Where(x => x.FolderName == foldername).FirstOrDefault();

                bool needsAdd = false;

                if (rep == null)
                {
                    rep = new MatchReplayFolder();

                    rep.FolderName = foldername;
                    needsAdd = true;
                }

                rep.IsInGameFolder = true;

                rep.JsonCreated = File.Exists(Path.Combine(xstratpath, foldername + ".json"));

                if (needsAdd) list.Add(rep);
            }

            //Get Titles from XML
            var titles = GetTitleDict();
            if(titles != null)
            {

            foreach (var title in titles)
            {
                var listItems = list.Where(x => x.Title == title.Key);
                foreach (var item in listItems)
                {
                    item.Title = title.Value;
                }            
            }
            }

            ReplayFolders = list;
        }

        public void ImportAll()
        {
            LoadReplays();
            if(ReplayFolders == null) return;

            var toImport = ReplayFolders.Where(x => !x.IsXStratFolder && x.IsInGameFolder).AsEnumerable();

            foreach (var item in toImport)
            {
                Import(item.FolderName);
            }
            LoadReplays();
        }

        public void Import(string folderName)
        {
            string xstratpath = SettingsHandler.XStratReplayPath;
            string gamepath = SettingsHandler.GameReplayPath;

            string sourceDirectory = Path.Combine(SettingsHandler.GameReplayPath, folderName);
            string targetDirectory = Path.Combine(SettingsHandler.XStratReplayPath, folderName);

            if (gamepath.IsNullOrEmpty() || !Directory.Exists(gamepath))
            {
                Notify.sendWarn("Please set Game Replay Path in Settings");
                return;
            }
            if (xstratpath.IsNullOrEmpty() || !Directory.Exists(xstratpath))
            {
                Notify.sendWarn("Could not find XStrat Replay Folder: " + xstratpath);
                return;
            }
            
            if(!Directory.Exists(sourceDirectory)) return;
            if (Directory.Exists(targetDirectory)) return;

            Globals.CopyFolder(sourceDirectory, targetDirectory);
        }

        public void CreateJson(string folderName)
        {
            if(folderName.IsNullOrEmpty()) return;
        }

        public void DeleteMatch(string folderName)
        {
            if (folderName.IsNullOrEmpty()) return;
        }

        private void AnalyzeAll()
        {
            throw new NotImplementedException();
        }

        private void Delete(string folderName)
        {
            if (folderName.IsNullOrEmpty()) return;
            throw new NotImplementedException();
        }

        private void Export(string folderName)
        {
            if (folderName.IsNullOrEmpty()) return;
            throw new NotImplementedException();
        }

        private void RemoveFromGameFolder(string folderName)
        {
            if (folderName.IsNullOrEmpty()) return;
            throw new NotImplementedException();
        }

        private void ShowInExplorer(string folderName)
        {
            if (folderName.IsNullOrEmpty()) return;
            var replay = ReplayFolders.Where(x => x.FolderName == folderName).FirstOrDefault();
            if (replay == null || !replay.IsXStratFolder) return;
            string xstratpath = SettingsHandler.XStratReplayPath;
            string path = Path.Combine(xstratpath, folderName);
            Process.Start(path);
        }

        #endregion

        #region FileHelpers
        public bool hasRounds(string path)
        {
            if (!Directory.Exists(path)) return false;
            var files = Directory.GetFiles(path, "*.rec", SearchOption.AllDirectories);

            return files.Length > 0;
        }

        public Dictionary<string, string> GetTitleDict()
        {
            string xmlFile = Path.Combine(SettingsHandler.XStratReplayPath, "ReplayTitles.xml");

            if (xmlFile.IsNullOrEmpty() || !File.Exists(xmlFile))
            {
                Logger.Log("Could not find existing replay file in: " + xmlFile);
                return null;
            }
            XmlSerializer deserializer = new XmlSerializer(typeof(Dictionary<string, string>));
            using (StringReader stringReader = new StringReader(xmlFile))
            {
                return (Dictionary<string, string>)deserializer.Deserialize(stringReader);                
            }
        }

        public void SaveTitleDict()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            if (ReplayFolders == null || ReplayFolders.Count == 0) return;

            foreach (var folder in ReplayFolders)
            {
                dict.Add(folder.FolderName, folder.Title);
            }

            SerializeTitleDict(dict);
        }

        public void SerializeTitleDict(Dictionary<string, string> dict)
        {
            string xmlFile = Path.Combine(SettingsHandler.XStratReplayPath, "ReplayTitles.xml");

            if (xmlFile.IsNullOrEmpty())
            {
                Logger.Log("XML Path is empty - could not save dictionary: " + xmlFile);
                return;
            }
            string xml = dict.SerializeObject();

            File.WriteAllText(xmlFile, xml);
        }
        #endregion

        #region Click Events

        private void ImportAllBtn_Click(object sender, RoutedEventArgs e)
        {
            ImportAll();
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadReplays();
        }

        private void CreateAllJsonBtn_Click(object sender, RoutedEventArgs e)
        {
            AnalyzeAll();
        }

        private void AnalyzeButtonColumn_Click(object sender, RoutedEventArgs e)
        {
            CreateJson((myDataGrid.SelectedItem as MatchReplayFolder).FolderName);
        }

        private void DeleteButtonColumn_Click(object sender, RoutedEventArgs e)
        {
            Delete((myDataGrid.SelectedItem as MatchReplayFolder).FolderName);
        }

        private void CopyToGameButtonColumn_Click(object sender, RoutedEventArgs e)
        {
            Export((myDataGrid.SelectedItem as MatchReplayFolder).FolderName);
        }

        private void RemoveFromGameFolderButtonColumn_Click(object sender, RoutedEventArgs e)
        {
            RemoveFromGameFolder((myDataGrid.SelectedItem as MatchReplayFolder).FolderName);
        }

        private void ShowInExplorerColumn_Click(object sender, RoutedEventArgs e)
        {

            ShowInExplorer((myDataGrid.SelectedItem as MatchReplayFolder).FolderName);
        }

        private void ImportColumn_Click(object sender, RoutedEventArgs e)
        {
            Import((myDataGrid.SelectedItem as MatchReplayFolder).FolderName);
        }

        #endregion
        
    }
    public class MatchReplayFolder
    {
        public string FolderName { get; set; }
        public string Title { get; set; }
        public bool IsInGameFolder{ get; set; }
        public bool IsXStratFolder { get; set; }
        public bool JsonCreated { get; set; }
    }
}
