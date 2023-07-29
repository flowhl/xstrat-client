
using System;
using xstrat.Core;

namespace xstrat.Models.Supabase
{
    public class Routine
    {
        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string Title { get; set; }

        public string UserId { get; set; }

        public string Content { get; set; }

        public EventHandler<RoutineButtonClicked> MoveButtonEvent;
    }
}
