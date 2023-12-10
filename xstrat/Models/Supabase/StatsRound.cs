using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XStratApi.Models.Supabase
{
    public class StatsRound : INotifyPropertyChanged
    {
        private string id;
        private DateTime? createdAt;
        private string matchId;
        private DateTime? start;
        private string positionId;
        private int? nr;
        private int? winnerTeamNr;
        private string winCondition;
        private ObservableCollection<StatsPlayer> players;
        private ObservableCollection<StatsActivity> activityFeed;

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

        public string MatchId
        {
            get { return matchId; }
            set { SetProperty(ref matchId, value); }
        }

        public DateTime? Start
        {
            get { return start; }
            set { SetProperty(ref start, value); }
        }

        public string PositionId
        {
            get { return positionId; }
            set { SetProperty(ref positionId, value); }
        }

        public int? Nr
        {
            get { return nr; }
            set { SetProperty(ref nr, value); }
        }

        public int? WinnerTeamNr
        {
            get { return winnerTeamNr; }
            set { SetProperty(ref winnerTeamNr, value); }
        }

        public string WinCondition
        {
            get { return winCondition; }
            set { SetProperty(ref winCondition, value); }
        }

        public ObservableCollection<StatsPlayer> Players
        {
            get { return players; }
            set { SetProperty(ref players, value); }
        }

        public ObservableCollection<StatsActivity> ActivityFeed
        {
            get { return activityFeed; }
            set { SetProperty(ref activityFeed, value); }
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
    }
}
