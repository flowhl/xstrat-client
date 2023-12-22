using LiveChartsCore.Geo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Serialization;
using xstrat.Models.Supabase;
using xstrat.MVVM.View;

namespace xstrat.Core
{
    public static class DataCache
    {
        #region RetrieveAll

        public static void RetrieveAllCaches()
        {
            try
            {
                RetrieveUser();
                RetrieveTeam();
                RetrieveTeamMates();
                RetrieveGames();
                RetrieveMaps();
                RetrieveOperators();
                RetrievePositions();
                RetrieveRoutines();
                RetrieveStrats();
                RetrieveCalendarEvents();
                RetrieveCalendarBlocks();
                RetrieveCalendarEventResponses();
            }
            catch (Exception ex)
            {
                Notify.sendError("Error when retrieving datacaches: " + ex.Message);
            }
        }

        #endregion

        #region Team
        public static Team _currentTeam;
        public static Team CurrentTeam
        {
            get
            {
                if (_currentTeam == null)
                {
                    RetrieveTeam();
                }
                return _currentTeam;
            }
            set
            {
                _currentTeam = value;
            }
        }

        public static void RetrieveTeam()
        {
            bool teamNull = _currentTeam == null;
            var task = ApiHandler.GetTeamInfoAsync();
            task.Wait();
            CurrentTeam = task.Result;

            if ((teamNull && task.Result != null) || (!teamNull && task.Result == null))
            {
                Globals.wnd.UpdateMenuButtons();
            }

        }
        #endregion

        #region User
        public static UserData _currentUser;
        public static UserData CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    RetrieveUser();
                }
                return _currentUser;
            }
            set
            {
                _currentUser = value;
            }
        }

        public static void RetrieveUser()
        {
            var task = ApiHandler.GetUserDataAsync();
            task.Wait();
            CurrentUser = task.Result;
        }
        #endregion

        #region Teammates
        public static List<UserData> _currentTeamMates;
        public static List<UserData> CurrentTeamMates
        {
            get
            {
                if (_currentTeamMates == null)
                {
                    RetrieveTeamMates();
                }
                return _currentTeamMates;
            }
            set
            {
                _currentTeamMates = value;
            }
        }

        public static void RetrieveTeamMates()
        {
            var task = ApiHandler.GetTeamMembersAsync();
            task.Wait();
            CurrentTeamMates = task.Result.EmptyIfNull();
        }
        #endregion

        #region Maps
        public static List<Map> _currentMaps;
        public static List<Map> CurrentMaps
        {
            get
            {
                if (_currentMaps == null)
                {
                    RetrieveMaps();
                }
                return _currentMaps;
            }
            set
            {
                _currentMaps = value;
            }
        }

        public static void RetrieveMaps()
        {
            var task = ApiHandler.GetMapsAsync();
            task.Wait();
            CurrentMaps = task.Result.EmptyIfNull();
        }
        #endregion

        #region Operators
        public static List<Operator> _currentOperators;
        public static List<Operator> CurrentOperators
        {
            get
            {
                if (_currentOperators == null)
                {
                    RetrieveOperators();
                }
                return _currentOperators;
            }
            set
            {
                _currentOperators = value;
            }
        }

        public static void RetrieveOperators()
        {
            var task = ApiHandler.GetOperatorsAsync();
            task.Wait();
            CurrentOperators = task.Result.EmptyIfNull();
        }
        #endregion

        #region Games
        public static List<Game> _currentGames;
        public static List<Game> CurrentGames
        {
            get
            {
                if (_currentGames == null)
                {
                    RetrieveGames();
                }
                return _currentGames;
            }
            set
            {
                _currentGames = value;
            }
        }

        public static void RetrieveGames()
        {
            var task = ApiHandler.GetGamesAsync();
            task.Wait();
            CurrentGames = task.Result.EmptyIfNull();
        }
        #endregion

        #region Positions
        public static List<Position> _currentPositions;
        public static List<Position> CurrentPositions
        {
            get
            {
                if (_currentPositions == null)
                {
                    RetrievePositions();
                }
                return _currentPositions;
            }
            set
            {
                _currentPositions = value;
            }
        }

        public static void RetrievePositions()
        {
            var task = ApiHandler.GetPositionsAsync();
            task.Wait();
            CurrentPositions = task.Result.EmptyIfNull();
        }
        #endregion

        #region Routines
        public static List<Routine> _currentRoutines;
        public static List<Routine> CurrentRoutines
        {
            get
            {
                if (_currentRoutines == null)
                {
                    RetrieveRoutines();
                }
                return _currentRoutines;
            }
            set
            {
                _currentRoutines = value;
            }
        }

        public static void RetrieveRoutines()
        {
            var task = ApiHandler.GetRoutinesAsync();
            task.Wait();
            CurrentRoutines = task.Result.EmptyIfNull();
        }
        #endregion

        #region CalendarBlocks
        public static List<CalendarBlock> _currentCalendarBlocks;
        public static List<CalendarBlock> CurrentCalendarBlocks
        {
            get
            {
                if (_currentCalendarBlocks == null)
                {
                    RetrieveCalendarBlocks();
                }
                return _currentCalendarBlocks;
            }
            set
            {
                _currentCalendarBlocks = value;
            }
        }

        public static void RetrieveCalendarBlocks()
        {
            var task = ApiHandler.GetTeamOffDays();
            task.Wait();
            CurrentCalendarBlocks = task.Result.EmptyIfNull();
        }
        #endregion

        #region CalendarEvents
        public static List<CalendarEvent> _currentCalendarEvents;
        public static List<CalendarEvent> CurrentCalendarEvents
        {
            get
            {
                if (_currentCalendarEvents == null)
                {
                    RetrieveCalendarEvents();
                }
                return _currentCalendarEvents;
            }
            set
            {
                _currentCalendarEvents = value;
            }
        }

        public static void RetrieveCalendarEvents()
        {
            var task = ApiHandler.GetTeamScrims();
            task.Wait();
            CurrentCalendarEvents = task.Result.EmptyIfNull();
        }
        #endregion

        #region Strats
        public static List<Strat> _currentStrats;
        public static List<Strat> CurrentStrats
        {
            get
            {
                if (_currentStrats == null)
                {
                    RetrieveStrats();
                }
                return _currentStrats;
            }
            set
            {
                _currentStrats = value;
            }
        }

        public static void RetrieveStrats()
        {
            var task = ApiHandler.GetStrats();
            task.Wait();
            CurrentStrats = task.Result.EmptyIfNull();
        }
        #endregion

        #region Replays


        private static ObservableCollection<MatchReplayFolder> _replayFolders;
        public static ObservableCollection<MatchReplayFolder> ReplayFolders
        {
            get
            {
                if (_replayFolders == null)
                {
                    RetrieveReplayFolders();
                }
                return _replayFolders;
            }
            set
            {
                _replayFolders = value;
            }
        }

        public static async Task RetrieveReplaysAsync()
        {
            await Task.Run(() => RetrieveReplayFolders());
        }

        public static void RetrieveReplayFolders()
        {
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
                //SetStatus($"Loaded: {xreplay}");
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
                //SetStatus($"Loaded: {greplay}");
                //SetStatus("Loaded Replays");
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

            list.ToList().ForEach(x => x.Selected = false);

            DataCache.ReplayFolders = list;
        }

        #region FileHelpers

        public static bool hasRounds(string path)
        {
            if (!Directory.Exists(path)) return false;
            var files = Directory.GetFiles(path, "*.rec", SearchOption.AllDirectories);

            return files.Length > 0;
        }

        public static List<MatchReplayTitle> GetTitleDict()
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
                var res = ((MatchReplayTitle[])deserializer.Deserialize(stringReader)).ToList();
                res.ForEach(x => x.DeserializeJson());
                return res;
            }
        }

        public static void SaveTitleDict(bool silent = false)
        {
            List<MatchReplayTitle> dict = new List<MatchReplayTitle>();

            if (DataCache.ReplayFolders == null || DataCache.ReplayFolders.Count == 0) return;

            foreach (var folder in DataCache.ReplayFolders.Where(x => x.Title.IsNotNullOrEmpty() || x.DissectReplay != null))
            {
                dict.Add(new MatchReplayTitle { FileHash = folder.FileHash, Title = folder.Title, DissectReplay = folder.DissectReplay });
            }

            dict.ForEach(x => x.SerializeToJson());

            SerializeTitleDict(dict.ToArray());
            if (!silent)
                Notify.sendSuccess("Saved sucessfully");
        }

        public static void SerializeTitleDict(MatchReplayTitle[] dict)
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

        #endregion

        #region CalendarEventResponses
        public static List<CalendarEventResponse> _currentCalendarEventResponses;
        public static List<CalendarEventResponse> CurrentCalendarEventResponses
        {
            get
            {
                if (_currentCalendarEventResponses == null)
                {
                    RetrieveCalendarEventResponses();
                }
                return _currentCalendarEventResponses;
            }
            set
            {
                _currentCalendarEventResponses = value;
            }
        }

        public static void RetrieveCalendarEventResponses()
        {
            var task = ApiHandler.GetCalendarEventResponsesAsync();
            task.Wait();
            CurrentCalendarEventResponses = task.Result.EmptyIfNull();
        }
        #endregion
    }
}
