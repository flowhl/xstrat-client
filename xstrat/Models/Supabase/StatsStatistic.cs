using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XStratApi.Models.Supabase
{
    public class StatsStatistic : INotifyPropertyChanged
    {
        private string matchID;
        private int? roundNr;
        private string username;
        private int kills;
        private int deaths;
        private int assists;
        private int headshots;
        private double headshotpercentage;
        private double kd;
        private int entrykills;
        private double kost;
        private double kpr;
        private int plant;
        private int defuse;
        private int survivals;
        private double survivalRate;

        public string MatchID
        {
            get { return matchID; }
            set { SetProperty(ref matchID, value); }
        }

        public int? RoundNr
        {
            get { return roundNr; }
            set { SetProperty(ref roundNr, value); }
        }

        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }

        public int Kills
        {
            get { return kills; }
            set { SetProperty(ref kills, value); }
        }

        public int Deaths
        {
            get { return deaths; }
            set { SetProperty(ref deaths, value); }
        }

        public int Assists
        {
            get { return assists; }
            set { SetProperty(ref assists, value); }
        }

        public int Headshots
        {
            get { return headshots; }
            set { SetProperty(ref headshots, value); }
        }

        public double Headshotpercentage
        {
            get { return headshotpercentage; }
            set { SetProperty(ref headshotpercentage, value); }
        }

        public double KD
        {
            get { return kd; }
            set { SetProperty(ref kd, value); }
        }

        public int Entrykills
        {
            get { return entrykills; }
            set { SetProperty(ref entrykills, value); }
        }

        public double KOST
        {
            get { return kost; }
            set { SetProperty(ref kost, value); }
        }

        public double KPR
        {
            get { return kpr; }
            set { SetProperty(ref kpr, value); }
        }

        public int Plant
        {
            get { return plant; }
            set { SetProperty(ref plant, value); }
        }

        public int Defuse
        {
            get { return defuse; }
            set { SetProperty(ref defuse, value); }
        }

        public int Survivals
        {
            get { return survivals; }
            set { SetProperty(ref survivals, value); }
        }

        public double SurvivalRate
        {
            get { return survivalRate; }
            set { SetProperty(ref survivalRate, value); }
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
