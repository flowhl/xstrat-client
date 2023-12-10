using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using xstrat.Models.Supabase;

namespace XStratApi.Models.Supabase
{
    public class StatsActivity : INotifyPropertyChanged
    {
        private string id;
        private DateTime? createdAt;
        private string matchId;
        private long? type;
        private DateTime? time;
        private string message;
        private string username;
        private string operatorId;
        private string targetUsername;
        private long? headshot;
        private double? posX;
        private double? posY;
        private string roundId;
        private int roundNr;

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

        public long? Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }

        public ActivityType ActivityType
        {
            get { return (ActivityType)Type; }
        }

        public DateTime? Time
        {
            get { return time; }
            set { SetProperty(ref time, value); }
        }

        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }

        public string OperatorId
        {
            get { return operatorId; }
            set { SetProperty(ref operatorId, value); }
        }

        public string TargetUsername
        {
            get { return targetUsername; }
            set { SetProperty(ref targetUsername, value); }
        }

        public long? Headshot
        {
            get { return headshot; }
            set { SetProperty(ref headshot, value); }
        }

        public double? PosX
        {
            get { return posX; }
            set { SetProperty(ref posX, value); }
        }

        public double? PosY
        {
            get { return posY; }
            set { SetProperty(ref posY, value); }
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
            if(propertyName == "Type")
            {
                OnPropertyChanged("ActivityType");
            }
            return true;
        }
    }
}
