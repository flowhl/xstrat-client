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

namespace xstrat.Overlay
{
    /// <summary>
    /// Interaction logic for OverlayStep.xaml
    /// </summary>
    public partial class OverlayStep : UserControl
    {
        public int duration { get; set; }
        private int current_duration;
        public int count { get; set; }
        private int current_count;
        public string title { get; set; }
        public bool running { get; set; } = false;
        public bool isFinished = false;
        private bool timeRelated = false;
        private bool _isSelected;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                if (_isSelected)
                {
                    StartButton.IsEnabled = true;
                    StoptButton.IsEnabled = true;
                    SkipButton.IsEnabled = true;
                    Background.Background = Brushes.DarkOrange;
                }
                else
                {
                    if (!isFinished)
                    {
                        Background.Background = Brushes.DarkRed;
                    }
                    else
                    {
                        Background.Background = Brushes.DarkGreen;
                    }
                    StartButton.IsEnabled = false;
                    StoptButton.IsEnabled = false;
                    SkipButton.IsEnabled = false;
                }
            }
        }

        public OverlayStep(int _duration, int _count, string _title, bool _running, bool _isSelected)
        {
            InitializeComponent();
            duration = _duration;
            count = _count -1;
            title = _title;
            running = _running;
            IsSelected = _isSelected;
            current_count = 0;
            current_duration = 0;
            TextBlock.Text = title;
            infoLabel.Content = current_count + "x " + DurationConverter(current_duration);
            if (duration > 0)
            {
                timeRelated = true;
            }
            else
            {
                StartButton.Content = "Continue";
                StoptButton.Visibility = Visibility.Hidden;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Start();
        }

        private void StoptButton_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            Skip();
        }

        private void Skip()
        {
            running = false;
            isFinished = true;
        }
        private void Stop()
        {
            running = false;
        }
        private void Start()
        {
            running = true;
            if (!timeRelated)
            {
                if(current_count < count)
                {
                    current_count++;
                }
                else
                {
                    isFinished = true;
                }
            }
        }

        /// <summary>
        /// update loop called every second
        /// </summary>
        public void Update()
        {
            if (!isFinished && IsSelected && running) // nicht finished
            {
                infoLabel.Content = current_count + "x " + DurationConverter(current_duration);
                if (timeRelated)
                {
                    if(current_duration < duration)
                    {
                        current_duration++;
                    }
                    else if(current_duration >= duration)
                    {
                        if(current_count < count)
                        {
                            current_count++;
                            current_duration = 0;
                        }
                        else
                        {
                            Background.Background = Brushes.DarkGreen;
                            isFinished = true;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }
        private string DurationConverter(int value)
        {
            int minutes = (int)(value / 60);
            int seconds = value % 60;
            string padseconds = seconds.ToString().PadLeft(3 - seconds.ToString().Length, '0');
            string sduration = minutes + ":" + padseconds;
            return sduration;
        }

    }
}
