using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for TimeSelector.xaml
    /// </summary>
    public partial class TimeSelector : UserControl
    {

        // Declare the event using the delegate
        public event TimeChangedEventHandler TimeChanged;
        // Define a delegate for the event handler
        public delegate void TimeChangedEventHandler(object sender, EventArgs e);

        public TimeSelector()
        {
            InitializeComponent();
            Loaded += TimeSelector_Loaded;
        }

        private void TimeSelector_Loaded(object sender, RoutedEventArgs e)
        {
            MinuteBox.ValueChanged += MinuteBox_ValueChanged;
            MinuteBox.ValueDecreasedToLimit += MinuteBox_ValueDecreasedToLimit;
            MinuteBox.ValueIncreasedToLimit += MinuteBox_ValueIncreasedToLimit;

            HourBox.ValueChanged += HourBox_ValueChanged;
            HourBox.ValueDecreasedToLimit += HourBox_ValueDecreasedToLimit;
            HourBox.ValueIncreasedToLimit += HourBox_ValueIncreasedToLimit;
        }

        private void HourBox_ValueIncreasedToLimit(object sender, EventArgs e)
        {
            TimeChanged.Invoke(this, EventArgs.Empty);
        }

        private void HourBox_ValueDecreasedToLimit(object sender, EventArgs e)
        {
            TimeChanged.Invoke(this, EventArgs.Empty);
        }

        private void HourBox_ValueChanged(object sender, EventArgs e)
        {
            TimeChanged.Invoke(this, EventArgs.Empty);
        }

        private void MinuteBox_ValueIncreasedToLimit(object sender, EventArgs e)
        {
            HourBox.Value++;
            TimeChanged.Invoke(this, EventArgs.Empty);
        }

        private void MinuteBox_ValueDecreasedToLimit(object sender, EventArgs e)
        {
            HourBox.Value--;
            TimeChanged.Invoke(this, EventArgs.Empty);
        }

        private void MinuteBox_ValueChanged(object sender, EventArgs e)
        {
            TimeChanged.Invoke(this, EventArgs.Empty);
        }

        public string GetTimeString()
        {
            if (HourBox == null || MinuteBox == null) return null;
            return HourBox.Value.ToString().PadLeft(2, '0') + ":" + MinuteBox.Value.ToString().PadLeft(2, '0') + ":00";
        }

        public void SetTime(int hour = 0, int minute = 0)
        {
            if (hour < 0) hour = 0;
            if (minute < 0) minute = 0;
            HourBox.Value = hour;
            MinuteBox.Value = minute;
            TimeChanged.Invoke(this, EventArgs.Empty);
        }

        public int GetHour() {  return HourBox.Value; }
        public int GetMinute() {  return MinuteBox.Value; }
    }
}
