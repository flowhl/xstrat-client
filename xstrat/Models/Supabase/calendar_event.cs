using System;

namespace xstrat.Models.Supabase
{
    public class CalendarEvent
    {
        public string Id { get; set; }

        public int EventType { get; set; }

        public string Title { get; set; }

        public string Comment { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }

        public string OpponentName { get; set; }

        public string TeamId { get; set; }

        public string Map1Id { get; set; }

        public string Map2Id { get; set; }

        public string Map3Id { get; set; }

        public int Typ { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string CreatedUser { get; set; }

        public int NotifySent { get; set; }
    }
}
