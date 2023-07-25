using System;

namespace xstrat.Models.Supabase
{
    public class Strat
    {
        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string TeamId { get; set; }

        public string GameId { get; set; }

        public string MapId { get; set; }

        public string PositionId { get; set; }

        public int Version { get; set; }

        public string Content { get; set; }

        public string CreatedUser { get; set; }

        public DateTime? LastEdit { get; set; }

        public int Disabled { get; set; }
    }
}
