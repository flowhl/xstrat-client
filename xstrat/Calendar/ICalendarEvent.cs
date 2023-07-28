using System;
using System.Collections.Generic;
using xstrat.Json;
using xstrat.Models.Supabase;

namespace xstrat.Calendar
{
    public interface ICalendarEvent
    {
        DateTime? DateFrom { get; set; }
        DateTime? DateTo { get; set; }
        string Label { get; set; }

        /// <summary>
        /// 0 = scrim / blue
        /// 1 = offday / red
        /// 2 = ??? / purple
        /// </summary>
        int Typ { get; set; }

        List<Object> Args { get; set; }
        bool Visible { get; set; }
        UserData User { get; set; }
        Scrim Scrim { get; set; }
    }
}
