using System;

namespace xstrat.Models.Supabase
{
    public class CalendarEventResponse
    {
        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string UserId { get; set; }

        public string CalendarEventID { get; set; }

        public int ResponseTyp { get; set; }
    }
}
