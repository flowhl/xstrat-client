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
    /// Interaction logic for ColorDisplay.xaml
    /// </summary>
    public partial class ColorDisplay : UserControl
    {
        public Brush Color { get; set; }
        public string NameInput { get; set; }
        public bool Status { get; set; }
        public bool HasCheckbox { get; set; }

        public event EventHandler<EventArgs> ColorDisplayCheckstatusChanged;
        public delegate void ColorDisplayEventHandler(object sender, EventArgs e);

        public ColorDisplay()
        {
            InitializeComponent();
            CBox.IsChecked = Status;
            Loaded += ColorDisplay_Loaded;
        }

        public void CBox_Checked(object sender, RoutedEventArgs e)
        {
            Status = CBox.IsChecked.GetValueOrDefault();
            ColorDisplayCheckstatusChanged.Invoke(this, EventArgs.Empty);
        }

        public void ColorDisplay_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ColorDisplay_Loaded;
            ColorCircle.Fill = Color;
            LabelText.Content = NameInput;
            CBox.Checked += CBox_Checked;
            CBox.Unchecked += CBox_Checked;
        }

        public void SetStatus(bool status = false)
        {
            Status=status;
            CBox.IsChecked=Status;
        }

        public ColorDisplay(Brush color, string name, bool hasCheckBox = false)
        {
            InitializeComponent();
            Loaded += ColorDisplay_Loaded;
            Color = color;
            NameInput = name;
            HasCheckbox = hasCheckBox;
            if (HasCheckbox)
            {
                CBox.Visibility = Visibility.Visible;
            }
            else
            {
                CBox.Visibility = Visibility.Collapsed;
            }

            ColorCircle.Fill = Color;
            LabelText.Content = NameInput;
        }
    }
}
