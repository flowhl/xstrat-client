using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XStratApi.Models.Supabase
{
    public class StatsBan : INotifyPropertyChanged
    {
        private string id;
        private DateTime? createdAt;
        private string matchId;
        private int teamNr;
        private string teamId;
        private string operatorId;

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

        public int TeamNr
        {
            get { return teamNr; }
            set { SetProperty(ref teamNr, value); }
        }

        public string TeamId
        {
            get { return teamId; }
            set { SetProperty(ref teamId, value); }
        }

        public string OperatorId
        {
            get { return operatorId; }
            set { SetProperty(ref operatorId, value); }
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
