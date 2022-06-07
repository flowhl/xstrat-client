using System;
using System.Collections.Generic;
using xstrat.Json;

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
        int typ { get; set; }

        List<Object> args { get; set; }
        bool visible { get; set; }
        User user { get; set; }
        Scrim scrim { get; set; }
    }
}
