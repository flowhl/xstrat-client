using DotNetProjects.SVGImage.SVG.Shapes.Filter;
using Microsoft.Win32;
using Newtonsoft.Json;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Configuration;
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

        public ObservableCollection<MatchReplayFolder> ReplayFolders { get; set; }


        public ReplayView()
        {
            InitializeComponent();
            Loaded += ReplayView_Loaded;
        }

        private void ReplayView_Loaded(object sender, RoutedEventArgs e)
        {
            LoadReplays();
            Globals.wnd.KeyUp += Wnd_KeyDown;
        }

        private void Wnd_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Key == Key.S)
                {
                    SaveTitleDict();
                }
                if (e.Key == Key.R)
                {
                    LoadReplays();
                }
            }
        }

        public void LoadReplays()
        {
            //Refresh List

            GenerateFolderList();

            //Update UI
            ReplayDG.ItemsSource = ReplayFolders;
            ReplayDG.DataContext = ReplayFolders;
        }

        private System.Timers.Timer statusTimer;

        public void SetStatus(string message)
        {
            if (message.IsNullOrEmpty()) return;
            this.Dispatcher.Invoke(() =>
            {
                StatusText.Content = message;
                // Reset the timer when the status is set again
                if (statusTimer != null) statusTimer.Stop();
                statusTimer = new System.Timers.Timer(10000);
                statusTimer.Elapsed += (sender, e) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        StatusText.Content = "";
                    });
                };
                statusTimer.Start();
            });
        }

        #region FileHandling

        public void GenerateFolderList()
        {
            SetStatus("Loading Folders");
            var list = new ObservableCollection<MatchReplayFolder>();

            string xstratpath = SettingsHandler.XStratReplayPath;
            string gamepath = SettingsHandler.Settings.GameReplayPath;

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

                //rep.Di = File.Exists(Path.Combine(xstratpath, foldername + ".json"));

                list.Add(rep);
                SetStatus($"Loaded: {xreplay}");
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

                //rep.JsonCreated = File.Exists(Path.Combine(xstratpath, foldername + ".json"));

                if (needsAdd) list.Add(rep);
                SetStatus($"Loaded: {greplay}");
            }

            //Get Titles from XML
            var titles = GetTitleDict();
            if (titles != null)
            {
                foreach (var title in titles)
                {
                    var listItems = list.Where(x => x.FileHash == title.FileHash);
                    foreach (var item in listItems)
                    {
                        item.Title = title.Title;
                        item.DissectReplay = title.DissectReplay;
                    }
                }
            }

            ReplayFolders = list;
            ReplayDG.ItemsSource = null;
            ReplayDG.ItemsSource = ReplayFolders;
        }

        public void ImportAll()
        {
            LoadReplays();
            if (ReplayFolders == null) return;

            var toImport = ReplayFolders.Where(x => !x.IsXStratFolder && x.IsInGameFolder).AsEnumerable();

            foreach (var item in toImport)
            {
                Import(item.FolderName);
            }
            LoadReplays();
        }

        public void Import(string folderName)
        {
            SetStatus($"Importing: {folderName}");
            string xstratpath = SettingsHandler.XStratReplayPath;
            string gamepath = SettingsHandler.Settings.GameReplayPath;

            string sourceDirectory = Path.Combine(SettingsHandler.Settings.GameReplayPath, folderName);
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

            if (!Directory.Exists(sourceDirectory)) return;
            if (Directory.Exists(targetDirectory)) return;

            Globals.CopyFolder(sourceDirectory, targetDirectory);
        }

        //public async void CreateJson(string folderName, bool refeshAfter = false)
        //{
        //    SetStatus($"Analyzing: {folderName}");
        //    if (folderName.IsNullOrEmpty()) return;

        //    string exe = Path.Combine(Globals.XStratInstallPath, "External/r6-dissect.exe");
        //    ProcessStartInfo startInfo = new ProcessStartInfo
        //    {
        //        FileName = exe,
        //        Arguments = $"{folderName} -x {folderName}.json",
        //        RedirectStandardOutput = true,
        //        UseShellExecute = false,
        //        WorkingDirectory = SettingsHandler.XStratReplayPath,
        //        CreateNoWindow = true
        //    };
        //    Process process = new Process();
        //    process.StartInfo = startInfo;
        //    process.OutputDataReceived += Process_OutputDataReceived;
        //    process.Start();
        //    process.BeginOutputReadLine();
        //    process.WaitForExit();
        //    process.OutputDataReceived -= Process_OutputDataReceived;
        //    SetStatus($"Analyzed Successfully: {folderName}");
        //    if (refeshAfter) ReplayFolders.Where(x => x.FolderName == folderName).FirstOrDefault().JsonCreated = true;
        //}

        public void AnalyzeFile(MatchReplayFolder folder)
        {
            if (folder == null) return;
            SetStatus("Analyzing");
            folder.DissectReplay = DissectHelper.GetReplay(Path.Combine(SettingsHandler.XStratReplayPath, folder.FolderName));
            SetStatus("");
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                this.Dispatcher.Invoke(() =>
                {
                    SetStatus(e.Data);
                });
            }
        }

        private async Task AnalyzeAll()
        {
            var toAnalyze = ReplayFolders.Where(x => x.IsXStratFolder && x.DissectReplay == null).AsEnumerable();
            SetStatus("Analyzing all replays");
            ApiHandler.Waiting();
            var tasks = new List<Task>();
            foreach (var item in toAnalyze)
            {
                tasks.Add(Task.Run(() => AnalyzeFile(item)));
            }
            await tasks.WhenAll();
            SetStatus("Done");
            ApiHandler.EndWaiting();
            SaveTitleDict();
            ReplayDG.ItemsSource = null;
            ReplayDG.DataContext = null;
            ReplayDG.ItemsSource = ReplayFolders;
            ReplayDG.DataContext = ReplayFolders;
        }

        private void Delete(string folderName)
        {
            SetStatus($"Deleting: {folderName}");
            if (folderName.IsNullOrEmpty()) return;
            string dirXStrat = Path.Combine(SettingsHandler.XStratReplayPath, folderName);
            string dirGame = Path.Combine(SettingsHandler.Settings.GameReplayPath, folderName);
            string jsonFile = Path.Combine(SettingsHandler.Settings.GameReplayPath, $"{folderName}.json");
            if (Directory.Exists(dirXStrat)) Directory.Delete(dirXStrat, true);
            if (Directory.Exists(dirGame)) Directory.Delete(dirGame, true);
            if (File.Exists(jsonFile)) File.Delete(jsonFile);
            LoadReplays();
        }

        private void CopyToGameFolder(string folderName)
        {
            SetStatus($"Copying to Game: {folderName}");
            if (folderName.IsNullOrEmpty()) return;
            string dirXStrat = Path.Combine(SettingsHandler.XStratReplayPath, folderName);
            string dirGame = Path.Combine(SettingsHandler.Settings.GameReplayPath, folderName);

            if (ReplayFolders.Where(x => x.IsInGameFolder).Count() >= 12)
            {
                MessageBox.Show("Cannot have more than 12 replays in folder as they wont be loaded in the game");
                return;
            }

            if (Directory.Exists(dirXStrat) && !Directory.Exists(dirGame))
            {
                Globals.CopyFolder(dirXStrat, dirGame);
            }
            else
            {
                SetStatus("Could not copy to game folder:");
            }
            LoadReplays();
        }

        private void RemoveFromGameFolder(string folderName)
        {
            if (folderName.IsNullOrEmpty()) return;
            string dirGame = Path.Combine(SettingsHandler.Settings.GameReplayPath, folderName);
            if (Directory.Exists(dirGame)) Directory.Delete(dirGame, true);
            LoadReplays();
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

        private void ShowTimeLine(string folderName)
        {
            Dissect.MatchReplay replay = ReplayFolders.Where(x => x.FolderName == folderName).FirstOrDefault().DissectReplay;

            StatsDG.ItemsSource = null;
            StatsDG.ItemsSource = replay.Stats.ToList();
            StatsDG.DataContext = replay.Stats;
        }

        private void ClearGameBtn_Click(object sender, RoutedEventArgs e)
        {
            var replaysToDelete = ReplayFolders.Where(x => x.IsInGameFolder);
            foreach (var replay in replaysToDelete)
            {
                RemoveFromGameFolder(replay.FolderName);
            }
        }


        #endregion

        #region FileHelpers

        public bool hasRounds(string path)
        {
            if (!Directory.Exists(path)) return false;
            var files = Directory.GetFiles(path, "*.rec", SearchOption.AllDirectories);

            return files.Length > 0;
        }

        public List<MatchReplayTitle> GetTitleDict()
        {
            string xmlFile = Path.Combine(SettingsHandler.XStratReplayPath, "ReplayTitles.xml");

            if (xmlFile.IsNullOrEmpty() || !File.Exists(xmlFile))
            {
                Logger.Log("Could not find existing replay file in: " + xmlFile);
                return null;
            }

            string xmlContent = File.ReadAllText(xmlFile);

            XmlSerializer deserializer = new XmlSerializer(typeof(MatchReplayTitle[]));
            using (StringReader stringReader = new StringReader(xmlContent))
            {
                return ((MatchReplayTitle[])deserializer.Deserialize(stringReader)).ToList();
            }
        }

        public void SaveTitleDict()
        {
            List<MatchReplayTitle> dict = new List<MatchReplayTitle>();

            if (ReplayFolders == null || ReplayFolders.Count == 0) return;

            foreach (var folder in ReplayFolders.Where(x => x.Title.IsNotNullOrEmpty() || x.DissectReplay != null))
            {
                dict.Add(new MatchReplayTitle { FileHash = folder.FileHash, Title = folder.Title, DissectReplay = folder.DissectReplay });
            }

            SerializeTitleDict(dict.ToArray());
            Notify.sendSuccess("Saved sucessfully");
        }

        public void SerializeTitleDict(MatchReplayTitle[] dict)
        {
            string xmlFile = Path.Combine(SettingsHandler.XStratReplayPath, "ReplayTitles.xml");

            if (xmlFile.IsNullOrEmpty())
            {
                Logger.Log("XML Path is empty - could not save dictionary: " + xmlFile);
                return;
            }
            string xml = dict.SerializeToString();

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

        private async void CreateAllJsonBtn_Click(object sender, RoutedEventArgs e)
        {
            await AnalyzeAll();
        }

        private async void AnalyzeButtonColumn_Click(object sender, RoutedEventArgs e)
        {
            var item = (ReplayDG.SelectedItem as MatchReplayFolder);
            if (item == null) return;

            if (item.DissectReplay == null)
            {
                AnalyzeFile(item);
                SaveTitleDict();
                ReplayDG.ItemsSource = null;
                ReplayDG.DataContext = null;
                ReplayDG.ItemsSource = ReplayFolders;
                ReplayDG.DataContext = ReplayFolders;
            }
            else
            {
                ShowTimeLine(item.FolderName);
            }
        }

        private void DeleteButtonColumn_Click(object sender, RoutedEventArgs e)
        {
            Delete((ReplayDG.SelectedItem as MatchReplayFolder).FolderName);
        }

        private void CopyToGameButtonColumn_Click(object sender, RoutedEventArgs e)
        {
            string folderName = (ReplayDG.SelectedItem as MatchReplayFolder).FolderName;
            if (folderName.IsNullOrEmpty()) return;

            if (ReplayFolders.Where(x => x.FolderName == folderName).FirstOrDefault()?.IsInGameFolder ?? false)
            {
                RemoveFromGameFolder(folderName);
            }
            else
            {
                CopyToGameFolder(folderName);
            }
        }

        private void ShowInExplorerColumn_Click(object sender, RoutedEventArgs e)
        {

            ShowInExplorer((ReplayDG.SelectedItem as MatchReplayFolder).FolderName);
        }

        private void ImportColumn_Click(object sender, RoutedEventArgs e)
        {
            Import((ReplayDG.SelectedItem as MatchReplayFolder).FolderName);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveTitleDict();
        }

        private void ExportExcelButtonColumn_Click(object sender, RoutedEventArgs e)
        {
            var item = (ReplayDG.SelectedItem as MatchReplayFolder);
            if (item == null) return;
            if (!item.IsXStratFolder)
            {
                Notify.sendWarn("Please backup the replay first");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog.FileName = item.FolderName;

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                DissectHelper.ExportReplay(Path.Combine(SettingsHandler.XStratReplayPath, item.FolderName), filePath);    
            }
        }
        #endregion
    }
    public class MatchReplayFolder : INotifyPropertyChanged
    {
        private string folderName;
        public string FolderName
        {
            get { return folderName; }
            set
            {
                if (folderName != value)
                {
                    folderName = value;
                    OnPropertyChanged(nameof(FolderName));
                }
            }
        }

        private string fileHash;
        public string FileHash
        {
            get
            {
                if (fileHash.IsNullOrEmpty())
                {
                    fileHash = Globals.CreateDirectoryMd5(Path.Combine(SettingsHandler.XStratReplayPath, FolderName));
                }
                return fileHash;
            }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        private bool isInGameFolder;
        public bool IsInGameFolder
        {
            get { return isInGameFolder; }
            set
            {
                if (isInGameFolder != value)
                {
                    isInGameFolder = value;
                    OnPropertyChanged(nameof(IsInGameFolder));
                }
            }
        }

        private bool isXStratFolder;
        public bool IsXStratFolder
        {
            get { return isXStratFolder; }
            set
            {
                if (isXStratFolder != value)
                {
                    isXStratFolder = value;
                    OnPropertyChanged(nameof(IsXStratFolder));
                }
            }
        }

        private Dissect.MatchReplay dissectReplay;
        public Dissect.MatchReplay DissectReplay
        {
            get { return dissectReplay; }
            set
            {
                if (dissectReplay != value)
                {
                    dissectReplay = value;
                    OnPropertyChanged(nameof(DissectReplay));
                }
            }
        }

        public bool Analyzed
        {
            get
            {
                return dissectReplay != null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class MatchReplayTitle
    {
        public string FileHash { get; set; }
        public string Title { get; set; }
        public Dissect.MatchReplay DissectReplay { get; set; }

        public MatchReplayTitle()
        {
        }
    }
}
