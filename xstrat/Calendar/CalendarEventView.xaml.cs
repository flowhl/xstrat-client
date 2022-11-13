using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using xstrat.Core;

namespace xstrat.Calendar
{
    /// <summary>
    ///    Interaction logic for CalendarEvent.xaml
    /// </summary>
    public partial class CalendarEventView
    {
        private CalendarMonth _calendar;

        public SolidColorBrush BackgroundColor
        {
            get { return (SolidColorBrush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(SolidColorBrush), typeof(CalendarEventView));

        public SolidColorBrush DefaultBackfoundColor;

        public CalendarEventView()
        {
            InitializeComponent();
        }

        public CalendarEventView(SolidColorBrush color, CalendarMonth calendar) : this()
        {
            _calendar = calendar;
            DefaultBackfoundColor = BackgroundColor = color;
        }

        private void EventMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if ((DateTime.Now - xstrat.Core.Globals.lastEventClicked).TotalMilliseconds < 1000) return;
                _calendar.CalendarEventDoubleClicked(this);
                e.Handled = true;
                xstrat.Core.Globals.lastEventClicked = DateTime.Now;
            }
            else if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1)
            {
                _calendar.CalendarEventClicked(this);
                e.Handled = true;
            }
            else if(e.ChangedButton == MouseButton.Middle && e.ClickCount == 1)
            {
                if ((DateTime.Now - xstrat.Core.Globals.lastEventClicked).TotalMilliseconds < 1000) return;
                _calendar.CalendarEventMiddleMouseClicked(this);
                e.Handled = true;
                xstrat.Core.Globals.lastEventClicked = DateTime.Now;
            }
        }
    }
}
