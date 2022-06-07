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
    /// Interaction logic for ToggleSwitch.xaml
    /// </summary>
    public partial class ToggleSwitch : UserControl
    {
        Thickness LeftSide = new Thickness(-39, 0, 0, 0);
        Thickness RightSide = new Thickness(0, 0, -39, 0);
        SolidColorBrush Off = new SolidColorBrush(Color.FromRgb(160, 160, 160));
        SolidColorBrush On = (SolidColorBrush)new BrushConverter().ConvertFrom("#336cb5");
        private bool Toggled = false;
        public ToggleSwitch()
        {
            InitializeComponent();
            Back.Fill = Off;
            Toggled = false;
            Dot.Margin = LeftSide;
        }
        public bool Toggled1 { get => Toggled; set => Toggled = value; }

        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Toggled)
            {
                Back.Fill = On;
                Toggled = true;
                Dot.Margin = RightSide;

            }
            else
            {

                Back.Fill = Off;
                Toggled = false;
                Dot.Margin = LeftSide;

            }
        }
        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Toggled)
            {
                Back.Fill = On;
                Toggled = true;
                Dot.Margin = RightSide;

            }
            else
            {

                Back.Fill = Off;
                Toggled = false;
                Dot.Margin = LeftSide;

            }
        }
        public void setStatus(Boolean status)
        {
            if (status)
            {
                Back.Fill = On;
                Toggled = true;
                Dot.Margin = RightSide;

            }
            else
            {
                Back.Fill = Off;
                Toggled = false;
                Dot.Margin = LeftSide;
            }

        }
        public Boolean getStatus()
        {
            return Toggled;
        }
    }
}
