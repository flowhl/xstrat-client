using System;
using System.ComponentModel;
using System.Linq.Expressions;

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
    }
}