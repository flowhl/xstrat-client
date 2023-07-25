using System;

namespace xstrat.Models.Supabase
{
    public class CalendarBlock
    {
        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string UserId { get; set; }

        public int Typ { get; set; }

        public string Title { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }
    }
}
