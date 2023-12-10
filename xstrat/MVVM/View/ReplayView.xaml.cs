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
using System.Windows.Threading;
using System.Xml.Serialization;
using xstrat.Core;
using xstrat.Dissect;
using static System.Net.Mime.MediaTypeNames;
using Path = System.IO.Path;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for ReplayView.xaml
    /// </summary>
    public partial class ReplayView : StateUserControl
    {
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
                    DataCache.SaveTitleDict();
                }
                if (e.Key == Key.R)
                {
                    LoadReplays();
                }
            }
        }

        public void LoadReplays()
        {
            ShowWaitingCursor();
            ReplayDG.ItemsSource = null;
            ReplayDG.DataContext = null;

            //Refresh List
            Task.Run(() => GenerateFolderList());

            //Update UI
            UpdateUI();
            ShowNormalCursor();
        }

        public void UpdateUI()
        {
            ReplayDG.ItemsSource = null;
            ReplayDG.DataContext = null;
            ReplayDG.ItemsSource = DataCache.ReplayFolders;
            ReplayDG.DataContext = DataCache.ReplayFolders;
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

        public async Task GenerateFolderList()
        {
            ShowWaitingCursor();
            SetStatus("Loading Replays...");

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.SetCursor(Cursors.Wait);
                ReplayDG.IsReadOnly = true;
            });

            DataCache.RetrieveReplayFolders();

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ReplayDG.ItemsSource = null;
                ReplayDG.ItemsSource = DataCache.ReplayFolders;
            });

            ShowNormalCursor();
        }

        public void ImportAll()
        {
            ShowWaitingCursor();
            if (DataCache.ReplayFolders == null) return;

            var toImport = DataCache.ReplayFolders.Where(x => x.Selected && !x.IsXStratFolder && x.IsInGameFolder).AsEnumerable();

            foreach (var item in toImport)
            {
                Import(item.FolderName);
            }
            ShowNormalCursor();
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

            DataCache.ReplayFolders.FirstOrDefault(x => x.FolderName == folderName).IsXStratFolder = true;
        }

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
            SetStatus("Analyzing selected replays");
            ImportAll();
            ShowWaitingCursor();
            var toAnalyze = DataCache.ReplayFolders.Where(x => x.Selected && x.IsXStratFolder && x.DissectReplay == null).AsEnumerable();
            var tasks = new List<Task>();
            foreach (var item in toAnalyze)
            {
                tasks.Add(Task.Run(() => AnalyzeFile(item)));
            }
            await tasks.WhenAll();
            SetStatus("Done");
            DataCache.SaveTitleDict(true);
            ShowNormalCursor();
            UpdateUI();
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
            DataCache.ReplayFolders.Remove(DataCache.ReplayFolders.FirstOrDefault(x => x.FolderName == folderName));
            SetStatus($"Deleted: {folderName}");
            UpdateUI();
        }

        private void CopyToGameFolder(string folderName)
        {
            SetStatus($"Copying to Game: {folderName}");
            if (folderName.IsNullOrEmpty()) return;
            string dirXStrat = Path.Combine(SettingsHandler.XStratReplayPath, folderName);
            string dirGame = Path.Combine(SettingsHandler.Settings.GameReplayPath, folderName);

            if (DataCache.ReplayFolders.Where(x => x.IsInGameFolder).Count() >= 12)
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
            var replay = DataCache.ReplayFolders.Where(x => x.FolderName == folderName).FirstOrDefault();
            if (replay == null || !replay.IsXStratFolder) return;
            string xstratpath = SettingsHandler.XStratReplayPath;
            string path = Path.Combine(xstratpath, folderName);
            Process.Start(path);
        }

        private void ShowStats(string folderName)
        {
            Dissect.MatchReplay replay = DataCache.ReplayFolders.Where(x => x.FolderName == folderName).FirstOrDefault()?.DissectReplay;
            if (replay == null) return;
            StatsDG.ItemsSource = null;
            StatsDG.ItemsSource = replay.Stats.ToList();
            StatsDG.DataContext = replay.Stats;
        }

        private void ClearGameBtn_Click(object sender, RoutedEventArgs e)
        {
            var replaysToDelete = DataCache.ReplayFolders.Where(x => x.IsInGameFolder && x.Selected);
            foreach (var replay in replaysToDelete)
            {
                RemoveFromGameFolder(replay.FolderName);
            }
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

        private void DeleteButtonColumn_Click(object sender, RoutedEventArgs e)
        {
            Delete((ReplayDG.SelectedItem as MatchReplayFolder).FolderName);
        }

        private void ShowInExplorerColumn_Click(object sender, RoutedEventArgs e)
        {

            ShowInExplorer((ReplayDG.SelectedItem as MatchReplayFolder).FolderName);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            DataCache.SaveTitleDict();
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

        public void ShowWaitingCursor()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        public void ShowNormalCursor()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = null;
            });
            System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }

        private void ShowStatsBtn_Click(object sender, RoutedEventArgs e)
        {
            var item = (ReplayDG.SelectedItem as MatchReplayFolder);
            if (item == null) return;
            ShowStats(item.FolderName);
        }

        private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            DataCache.ReplayFolders.ToList().ForEach(x => x.Selected = true);
        }
    }
    public class MatchReplayFolder : INotifyPropertyChanged
    {
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    OnPropertyChanged(nameof(Selected));
                }
            }
        }

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
            get
            {
                if (title.IsNullOrEmpty())
                {
                    return folderName;
                }
                return title;
            }
            set
            {
                if (value.IsNullOrEmpty())
                {
                    title = FolderName;
                    OnPropertyChanged(nameof(Title));
                    return;
                }
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
                    OnPropertyChanged(nameof(Map));
                    OnPropertyChanged(nameof(GameMode));
                    OnPropertyChanged(nameof(Result));
                    OnPropertyChanged(nameof(TeamA));
                    OnPropertyChanged(nameof(TeamB));
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

        //replay summary
        public string Map
        {
            get
            {
                if (DissectReplay == null || DissectReplay.Rounds == null || DissectReplay.Rounds.Count < 1) return "";
                return DissectReplay?.Rounds?.FirstOrDefault()?.Map.Name;
            }
        }

        public string GameMode
        {
            get
            {
                if (DissectReplay == null || DissectReplay.Rounds == null || DissectReplay.Rounds.Count < 1) return "";
                return DissectReplay?.Rounds?.FirstOrDefault()?.Gamemode.Name;
            }
        }

        public string Result
        {
            get
            {
                if (DissectReplay == null || DissectReplay.Rounds == null || DissectReplay.Rounds.Count < 1) return "";
                int teamA = DissectReplay?.Rounds?.Last().Teams[0].Score ?? 0;
                int teamB = DissectReplay?.Rounds?.Last().Teams[1].Score ?? 0;
                return teamA + ":" + teamB;
            }
        }

        public string TeamA
        {
            get
            {
                if (DissectReplay == null || DissectReplay.Rounds == null || DissectReplay.Rounds.Count < 1) return "";
                string teamA = DissectReplay?.Rounds?.FirstOrDefault().Teams[0].Name;
                return teamA;
            }
        }

        public string TeamB
        {
            get
            {
                if (DissectReplay == null || DissectReplay.Rounds == null || DissectReplay.Rounds.Count < 1) return "";
                string teamB = DissectReplay?.Rounds?.FirstOrDefault().Teams[1].Name;
                return teamB;
            }
        }

    }
    public class MatchReplayTitle
    {
        public string FileHash { get; set; }
        public string Title { get; set; }

        public string DissectJson { get; set; }

        [XmlIgnore]
        public Dissect.MatchReplay DissectReplay { get; set; }

        public MatchReplayTitle()
        {
        }

        public void SerializeToJson()
        {
            DissectJson = Newtonsoft.Json.JsonConvert.SerializeObject(DissectReplay);
        }
        public void DeserializeJson()
        {
            if (DissectJson == null) { return; }
            DissectReplay = Newtonsoft.Json.JsonConvert.DeserializeObject<MatchReplay>(DissectJson);

            DissectReplay.Rounds.ForEach(x => x.Root = DissectReplay);
            DissectReplay.Stats.ForEach(x => x.Root = DissectReplay);
            DissectReplay.Rounds.ForEach(x => x.Stats.ForEach(y => y.Root = DissectReplay));
            DissectReplay.Rounds.ForEach(x => x.Stats.ForEach(y => y.Round = x));
        }
    }
}
