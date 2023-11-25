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
        private readonly Thickness LeftSide = new Thickness(-39, 0, 0, 0);
        private readonly Thickness RightSide = new Thickness(0, 0, -39, 0);
        private readonly SolidColorBrush Off = new SolidColorBrush(Color.FromRgb(160, 160, 160));
        private readonly SolidColorBrush On = (SolidColorBrush)new BrushConverter().ConvertFrom("#336cb5");

        private bool _toggled;

        // Declare the Toggled event
        public event EventHandler Toggled;

        public ToggleSwitch()
        {
            InitializeComponent();
            SetToggleState(false);
        }

        public bool IsToggled
        {
            get => _toggled;
            set
            {
                if (_toggled != value)
                {
                    SetToggleState(value);
                    OnToggled(EventArgs.Empty); // Raise the event
                }
            }
        }

        protected virtual void OnToggled(EventArgs e)
        {
            Toggled?.Invoke(this, e);
        }

        private void SetToggleState(bool toggled)
        {
            _toggled = toggled;
            Back.Fill = _toggled ? On : Off;
            Dot.Margin = _toggled ? RightSide : LeftSide;
        }

        private void Dot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Toggle();
        }

        private void Back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Toggle();
        }

        private void Toggle()
        {
            IsToggled = !IsToggled;
        }
    }

}
