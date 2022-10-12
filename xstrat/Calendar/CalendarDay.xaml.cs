using System;
using System.ComponentModel;
using System.Linq.Expressions;
using xstrat.Core;

namespace xstrat.Calendar
{
    public partial class CalendarDay
    {
        public DateTime Date { get; set; }

        public CalendarDay()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void calendarDay_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Globals.CallCalendarEventCreated(Date);
        }
    }
}