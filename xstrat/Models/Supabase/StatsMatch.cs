
using SVGImage.SVG;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using xstrat.Core;
using xstrat.Dissect;
using xstrat.Models.Supabase;
using YoutubeExplode.Channels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace XStratApi.Models.Supabase
{
    public class StatsMatch : INotifyPropertyChanged
    {
        private string id;
        private DateTime? createdAt;
        private string teamID;
        private string title;
        private string calendarEventID;
        private string mapID;
        private int? gamemode;
        private int? gametype;
        private DateTime? start;
        private string creator;
        private ObservableCollection<StatsRound> rounds;
        private ObservableCollection<StatsTeam> teams;
        private ObservableCollection<StatsBan> bans;
        private ObservableCollection<StatsStatistic> stats;

        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        public DateTime? CreatedAt
        {
            get { return createdAt; }
            set { SetProperty(ref createdAt, value); }
        }

        public string TeamID
        {
            get { return teamID; }
            set { SetProperty(ref teamID, value); }
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public string CalendarEventID
        {
            get { return calendarEventID; }
            set { SetProperty(ref calendarEventID, value); }
        }

        public string MapID
        {
            get { return mapID; }
            set { SetProperty(ref mapID, value); }
        }

        public int? Gamemode
        {
            get { return gamemode; }
            set { SetProperty(ref gamemode, value); }
        }

        public int? Gametype
        {
            get { return gametype; }
            set { SetProperty(ref gametype, value); }
        }

        public DateTime? Start
        {
            get { return start; }
            set { SetProperty(ref start, value); }
        }

        public string Creator
        {
            get { return creator; }
            set { SetProperty(ref creator, value); }
        }

        public ObservableCollection<StatsRound> Rounds
        {
            get { return rounds; }
            set { SetProperty(ref rounds, value); }
        }

        public ObservableCollection<StatsTeam> Teams
        {
            get { return teams; }
            set { SetProperty(ref teams, value); }
        }

        public ObservableCollection<StatsBan> Bans
        {
            get { return bans; }
            set { SetProperty(ref bans, value); }
        }

        public ObservableCollection<StatsStatistic> Stats
        {
            get { return stats; }
            set { SetProperty(ref stats, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        //Generate Stats
        public void GenerateStats()
        {
            if (Stats == null) Stats = new ObservableCollection<StatsStatistic>();
            Stats.Clear();
            //Generate Match Stats
            var playerUsernames = Rounds.SelectMany(x => x.Players.Select(y => y.Username)).Distinct().ToList();
            foreach (var playerName in playerUsernames)
            {
                //Generate Round Stats for Player
                foreach (var round in Rounds)
                {
                    var newRoundStatsItem = new StatsStatistic();
                    newRoundStatsItem.Username = playerName;
                    newRoundStatsItem.MatchID = Id;
                    newRoundStatsItem.RoundNr = round.Nr;

                    var playerRow = round.Players.FirstOrDefault(x => x.Username == playerName);
                    var teamNr = playerRow?.TeamNr;
                    var teamKillFeed = round.ActivityFeed.Where(x => x.ActivityType == ActivityType.Kill && round.Players.Where(x => x.TeamNr == teamNr).Select(x => x.Username).Contains(x.Username));
                    var playerKillFeed = round.ActivityFeed.Where(x => x.ActivityType == ActivityType.Kill && x.Username == playerName);


                    newRoundStatsItem.Kills = round.ActivityFeed.Where(x => x.ActivityType == ActivityType.Kill && x.Username == playerName).Count();
                    newRoundStatsItem.Headshots = round.ActivityFeed.Where(x => x.ActivityType == ActivityType.Kill && x.Username == playerName && x.Headshot == 1).Count();
                    newRoundStatsItem.Deaths = round.ActivityFeed.Where(x => x.ActivityType == ActivityType.Kill && x.TargetUsername == playerName).Count();
                    newRoundStatsItem.Survivals = round.ActivityFeed.Any(x => x.ActivityType == ActivityType.Kill && x.TargetUsername == playerName) ? 0 : 1;
                    newRoundStatsItem.KPR = newRoundStatsItem.Kills;
                    newRoundStatsItem.Plant = round.ActivityFeed.Where(x => x.ActivityType == ActivityType.DefuserPlantComplete && x.Username == playerName).Count();
                    newRoundStatsItem.Defuse = round.ActivityFeed.Where(x => x.ActivityType == ActivityType.DefuserDisableComplete && x.Username == playerName).Count();

                    //Set Entrykill
                    if (teamKillFeed.Count() == 0)
                    {
                        newRoundStatsItem.Entrykills = 0;
                    }
                    else if (teamKillFeed.FirstOrDefault().Username == playerName)
                    {
                        newRoundStatsItem.Entrykills = 1;
                    }
                    else
                    {
                        newRoundStatsItem.Entrykills = 0;
                    }

                    newRoundStatsItem.Headshotpercentage = newRoundStatsItem.Kills == 0 ? 0 : (double)newRoundStatsItem.Headshots / (double)newRoundStatsItem.Kills;
                    newRoundStatsItem.SurvivalRate = newRoundStatsItem.Survivals;
                    newRoundStatsItem.KD = newRoundStatsItem.Deaths == 0 ? newRoundStatsItem.Kills : (double)newRoundStatsItem.Kills / (double)newRoundStatsItem.Deaths;

                    //Set KOST
                    //Kill
                    bool isKill = playerKillFeed.Any();
                    //Objective
                    bool isObjective = round.ActivityFeed.Any(x => (x.ActivityType == ActivityType.DefuserPlantComplete || x.ActivityType == ActivityType.DefuserDisableComplete) && x.Username == playerName);
                    //Survive
                    bool isSurvive = newRoundStatsItem.Survivals == 1;
                    //Trade
                    bool isTrade = false;
                    var killRow = round.ActivityFeed.FirstOrDefault(x => x.ActivityType == ActivityType.Kill && x.TargetUsername == playerName);
                    if (killRow != null)
                    {
                        string killer = killRow.Username;
                        isTrade = teamKillFeed.Any(x => x.ActivityType == ActivityType.Kill && x.TargetUsername == killer && ((x.Time.Value - killRow.Time.Value).TotalSeconds <= 10));
                    }

                    if (isKill || isObjective || isSurvive || isTrade)
                    {
                        newRoundStatsItem.KOST = 1;
                    }
                    else
                    {
                        newRoundStatsItem.KOST = 0;
                    }
                    Stats.Add(newRoundStatsItem);
                }

                var playerStats = Stats.Where(x => x.Username == playerName && x.RoundNr != null);
                var newStatsItem = new StatsStatistic();
                newStatsItem.Username = playerName;
                newStatsItem.MatchID = Id;
                newStatsItem.Kills = playerStats.Sum(x => x.Kills);
                newStatsItem.Headshots = playerStats.Sum(x => x.Headshots);
                newStatsItem.Deaths = playerStats.Sum(x => x.Deaths);
                newStatsItem.Survivals = playerStats.Sum(x => x.Survivals);
                newStatsItem.KPR = playerStats.Count() == 0 ? 0 : (double)playerStats.Sum(x => x.Kills) / (double)playerStats.Count();
                newStatsItem.Plant = playerStats.Sum(x => x.Plant);
                newStatsItem.Defuse = playerStats.Sum(x => x.Defuse);
                newStatsItem.Entrykills = playerStats.Sum(x => x.Entrykills);
                newStatsItem.Headshotpercentage = newStatsItem.Kills == 0 ? 0 : (double)newStatsItem.Headshots / (double)newStatsItem.Kills;
                newStatsItem.SurvivalRate = playerStats.Count() == 0 ? 0 : (double)playerStats.Sum(x => x.Survivals) / (double)playerStats.Count();
                newStatsItem.KD = newStatsItem.Deaths == 0 ? newStatsItem.Kills : (double)newStatsItem.Kills / (double)newStatsItem.Deaths;
                newStatsItem.KOST = playerStats.Count() == 0 ? 0 : (double)playerStats.Sum(x => x.KOST) / (double)playerStats.Count();
                newStatsItem.Assists = playerStats.Sum(x => x.Assists);

                Stats.Add(newStatsItem);
            }
        }

        #region Imports

        public void ImportDissect(MatchReplay matchReplay)
        {
            if (matchReplay == null)
            {
                Notify.sendError("No Match Replay found");
                return;
            }

            #region General Data
            this.MapID = DataCache.CurrentMaps.Where(x => x.Name == matchReplay.Rounds.FirstOrDefault()?.Map.Name).FirstOrDefault()?.Id;
            string gameMode = matchReplay.Rounds.FirstOrDefault()?.Gamemode.Name;
            switch (gameMode)
            {
                case "Bomb":
                    this.Gamemode = (int)MatchGameMode.Bomb;
                    break;
                case "Hostage":
                    this.Gamemode = (int)MatchGameMode.Hostage;
                    break;
                case "Secure Area":
                    this.Gamemode = (int)MatchGameMode.SecureArea;
                    break;
                default:
                    this.Gamemode = (int)MatchGameMode.Unknown;
                    break;
            }

            string gameType = matchReplay.Rounds.FirstOrDefault()?.MatchType.Name;
            switch (gameType)
            {
                case "Ranked":
                    this.Gametype = (int)MatchGametype.Ranked;
                    break;
                case "Unranked":
                    this.Gametype = (int)MatchGametype.Unranked;
                    break;
                case "Casual":
                    this.Gametype = (int)MatchGametype.Causal;
                    break;
                case "Arcade":
                    this.Gametype = (int)MatchGametype.Arcade;
                    break;
                case "Custom":
                    this.Gametype = (int)MatchGametype.Custom;
                    break;
                default:
                    this.Gametype = (int)MatchGametype.Unknown;
                    break;
            }
            this.Start = matchReplay.Rounds.FirstOrDefault()?.Timestamp;
            this.TeamID = DataCache.CurrentUser.TeamId;

            this.Teams = new ObservableCollection<StatsTeam>();

            #endregion

            #region Teams
            var teamRows = matchReplay.Rounds.FirstOrDefault()?.Teams;
            this.Bans = new ObservableCollection<StatsBan>();
            foreach (var t in teamRows)
            {
                var newTeam = new StatsTeam();
                newTeam.MatchId = Id;
                newTeam.Name = t.Name;
                newTeam.Nr = teamRows.IndexOf(t);
                newTeam.Score = t.Score;

                //TODO: Set Winner Team
                //newTeam.Won = t.Won ? 1 : 0;

                newTeam.CreatedAt = DateTime.Now;

                //Todo: Set TeamID if Teammate or Player is in Team

                #region Bans
                //Create Ban rows
                this.Bans.Add(new StatsBan() { MatchId = Id, TeamNr = newTeam.Nr, CreatedAt = DateTime.Now });
                this.Bans.Add(new StatsBan() { MatchId = Id, TeamNr = newTeam.Nr, CreatedAt = DateTime.Now });

                #endregion

                Teams.Add(newTeam);
            }
            #endregion

            //ToDo: Add proper Bans

            #region Rounds
            this.Rounds = new ObservableCollection<StatsRound>();
            foreach (var round in matchReplay.Rounds)
            {
                var newRound = new StatsRound();
                newRound.MatchId = Id;
                newRound.Nr = round.RoundNumber;
                newRound.WinnerTeamNr = round.Teams.IndexOf(round.Teams.FirstOrDefault(x => x.Won));
                newRound.CreatedAt = DateTime.Now;
                newRound.Start = round.Timestamp;

                //Todo: Set PositionID
                #region ActivityFeed
                newRound.ActivityFeed = new ObservableCollection<StatsActivity>();
                foreach (var item in round.MatchFeedback)
                {
                    var newActivity = new StatsActivity();
                    newActivity.MatchId = Id;
                    newActivity.RoundNr = round.RoundNumber;
                    newActivity.CreatedAt = DateTime.Now;
                    newActivity.Time = TimeSpan.FromSeconds(item.TimeInSeconds);
                    newActivity.Headshot = item.Headshot ?? false ? 1 : 0;
                    newActivity.Username = item.Username;
                    newActivity.TargetUsername = item.Target;
                    newActivity.Message = item.Message;
                    if (item.Operator != null)
                        newActivity.OperatorId = DataCache.CurrentOperators.Where(x => x.Name == item.Operator.Name).FirstOrDefault()?.Id;
                    newActivity.RoundId = newRound.Id;

                    //Activity Type
                    int id = item.Type.Id;
                    string name = item.Type.Name;
                    //Kill,
                    newActivity.Type = (int)ActivityType.Unknown;
                    if (name == "Kill")
                        newActivity.Type = (int)ActivityType.Kill;
                    //FriendlyFireOff,
                    else if (name == "Other" && (item.Message?.Contains("Friendly Fire is now active") ?? false))
                        newActivity.Type = (int)ActivityType.FriendlyFireOn;
                    //FriendlyFireOn,
                    else if (name == "Other" &&(item.Message?.Contains("Friendly Fire turned off") ?? false))
                        newActivity.Type = (int)ActivityType.FriendlyFireOff;
                    //LocateObjective,
                    else if (name == "LocateObjective")
                        newActivity.Type = (int)ActivityType.LocateObjective;
                    //OperatorSwap,
                    else if (name == "OperatorSwap")
                        newActivity.Type = (int)ActivityType.OperatorSwap;
                    //ReverseFriendlyFireOff,
                    if (item.Message?.Contains("Reverse Friendly Fire has been disabled") ?? false) //Todo : Check if this is correct
                        newActivity.Type = (int)ActivityType.ReverseFriendlyFireOff;
                    //ReverseFriendlyFireOn,
                    if (item.Message?.Contains("Reverse Friendly Fire has been activated") ?? false)
                        newActivity.Type = (int)ActivityType.ReverseFriendlyFireOn;
                    //SurrenderAccepted,
                    else if (item.Message == "Surrender was accepted") //Todo: Check if this is correct
                        newActivity.Type = (int)ActivityType.SurrenderAccepted;
                    //SurrenderDenied,
                    else if (item.Message == "Surrender was denied")
                        newActivity.Type = (int)ActivityType.SurrenderDenied;
                    //DefuserPlantStart,
                    else if (name == "DefuserPlantStart")
                        newActivity.Type = (int)ActivityType.DefuserPlantStart;
                    //DefuserPlantComplete,
                    else if (name == "DefuserPlantComplete")
                        newActivity.Type = (int)ActivityType.DefuserPlantComplete;
                    //DefuserDisableStart,
                    else if (name == "DefuserDisableStart")
                        newActivity.Type = (int)ActivityType.DefuserDisableStart;
                    //DefuserDisableComplete,
                    else if (name == "DefuserDisableComplete")
                        newActivity.Type = (int)ActivityType.DefuserDisableComplete;
                    newRound.ActivityFeed.Add(newActivity);
                }
                #endregion

                #region Players
                newRound.Players = new ObservableCollection<StatsPlayer>();
                foreach (var player in round.Players)
                {
                    var newPlayer = new StatsPlayer();
                    newPlayer.MatchId = Id;
                    newPlayer.RoundNr = round.RoundNumber;
                    newPlayer.CreatedAt = DateTime.Now;
                    newPlayer.Username = player.Username;
                    newPlayer.TeamNr = player.TeamIndex;
                    newPlayer.OperatorId = DataCache.CurrentOperators.Where(x => x.Name == player.Operator.Name).FirstOrDefault()?.Id;
                    newPlayer.RoundId = newRound.Id;
                    newPlayer.ProfileId = player.ProfileID;
                    newPlayer.Spawnpoint = player.Spawn;

                    //Todo check which player is the current user

                    newRound.Players.Add(newPlayer);
                }
                #endregion

                Rounds.Add(newRound);
            }
            #endregion

            GenerateStats();
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        #endregion
    }
}