
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
                    bool isObjective = round.ActivityFeed.Any(x => (x.ActivityType == ActivityType.DefuserPlantComplete|| x.ActivityType == ActivityType.DefuserDisableComplete) && x.Username == playerName);
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

        }

        #endregion
    }
}