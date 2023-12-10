using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XStratApi.Models.Supabase
{
    public class StatsPlayer : INotifyPropertyChanged
    {
        private string id;
        private string matchId;
        private DateTime? createdAt;
        private string username;
        private string profileId;
        private long? teamNr;
        private string operatorId;
        private string spawnpoint;
        private string userId;
        private string roundId;
        private int roundNr;

        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        public string MatchId
        {
            get { return matchId; }
            set { SetProperty(ref matchId, value); }
        }

        public DateTime? CreatedAt
        {
            get { return createdAt; }
            set { SetProperty(ref createdAt, value); }
        }

        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }

        public string ProfileId
        {
            get { return profileId; }
            set { SetProperty(ref profileId, value); }
        }

        public long? TeamNr
        {
            get { return teamNr; }
            set { SetProperty(ref teamNr, value); }
        }

        public string OperatorId
        {
            get { return operatorId; }
            set { SetProperty(ref operatorId, value); }
        }

        public string Spawnpoint
        {
            get { return spawnpoint; }
            set { SetProperty(ref spawnpoint, value); }
        }

        public string UserId
        {
            get { return userId; }
            set { SetProperty(ref userId, value); }
        }

        public string RoundId
        {
            get { return roundId; }
            set { SetProperty(ref roundId, value); }
        }

        public int RoundNr
        {
            get { return roundNr; }
            set { SetProperty(ref roundNr, value); }
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
