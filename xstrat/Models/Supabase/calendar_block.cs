using System;

namespace xstrat.Models.Supabase
{
    public class CalendarBlock
    {
        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string UserId { get; set; }


        /// <summary>
        /// types:
        /// 0 exactly
        /// 1 entire day
        /// 2 weekly
        /// 3 every second week
        /// 4 monthly
        /// </summary>
        public int Typ { get; set; }

        public string Title { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? End { get; set; }
    }
}
