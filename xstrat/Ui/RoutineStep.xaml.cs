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
using xstrat.Core;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for Routine.xaml
    /// </summary>
    public partial class RoutineStep : UserControl
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public int Count { get; set; }
        public int Duration { get; set; }
        public EventHandler<RoutineStepButtonClicked> MoveButtonEvent;


        public RoutineStep(string header, string body, int count, int duration)
        {
            InitializeComponent();
            Header = header;
            Body = body;
            Duration = duration;
            Count = count;
            UpdateUi();
        }

        private void UpdateUi()
        {
            Count_Value.Content = Count.ToString();
            Duration_Value.Content = DurationConverter(Duration);
            Body_Textbox.Text = Body;
            Header_Textbox.Text = Header;
        }

        private string DurationConverter(int value)
        {
            int minutes = (int)(value / 60);
            int seconds = value % 60;
            string padseconds = seconds.ToString().PadLeft(3 - seconds.ToString().Length, '0');
            string sduration = minutes + ":" + padseconds;
            return sduration;
        }

        private void Header_Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Header = Header_Textbox.Text;
        }

        private void Body_Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Body = Body_Textbox.Text;
        }

        private void Duration_Minus_Click(object sender, RoutedEventArgs e)
        {
            if (Duration > 0)
            {
                Duration -= 10;
            }
            else if (Count < 0)
            {
                Duration = 0;
            }
            UpdateUi();
        }

        private void Duration_Plus_Click(object sender, RoutedEventArgs e)
        {
            if (Duration >= 0 && Duration <= 590)
            {
                Duration += 10;
            }
            else if (Count < 0)
            {
                Duration = 0;
            }
            UpdateUi();
        }

        private void Count_Plus_Click(object sender, RoutedEventArgs e)
        {
            if (Count >= 0)
            {
                Count++;
            }
            else if (Count < 0)
            {
                Count = 0;
            }
            UpdateUi();
        }

        private void Count_Minus_Click(object sender, RoutedEventArgs e)
        {
            if (Count > 0)
            {
                Count--;
            }
            else if (Count < 0)
            {
                Count = 0;
            }
            UpdateUi();
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            if(MoveButtonEvent != null)
            {
                MoveButtonEvent(sender, new RoutineStepButtonClicked(1, this));
            }
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (MoveButtonEvent != null)
            {
                MoveButtonEvent(sender, new RoutineStepButtonClicked(-1, this));
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MoveButtonEvent != null)
            {
                MoveButtonEvent(sender, new RoutineStepButtonClicked(2, this));
            }
        }

        private void AddBelowButton_Click(object sender, RoutedEventArgs e)
        {
            if (MoveButtonEvent != null)
            {
                MoveButtonEvent(sender, new RoutineStepButtonClicked(3, this));
            }
        }
    }
}
