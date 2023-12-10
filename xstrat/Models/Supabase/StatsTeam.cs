using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XStratApi.Models.Supabase
{
    public class StatsTeam : INotifyPropertyChanged
    {
        private string id;
        private DateTime? createdAt;
        private string matchId;
        private string name;
        private int score;
        private int won;
        private string teamId;
        private int nr;

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

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public int Score
        {
            get { return score; }
            set { SetProperty(ref score, value); }
        }

        public int Won
        {
            get { return won; }
            set { SetProperty(ref won, value); }
        }

        public string TeamId
        {
            get { return teamId; }
            set { SetProperty(ref teamId, value); }
        }

        public int Nr
        {
            get { return nr; }
            set { SetProperty(ref nr, value); }
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
