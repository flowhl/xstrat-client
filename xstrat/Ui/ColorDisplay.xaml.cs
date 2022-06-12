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
        public ColorDisplay()
        {
            InitializeComponent();
            Loaded += ColorDisplay_Loaded;
        }

        private void ColorDisplay_Loaded(object sender, RoutedEventArgs e)
        {
            ColorCircle.Fill = Color;
            LabelText.Content = NameInput;
        }

        public ColorDisplay(Brush color, string name)
        {
            InitializeComponent();
            Color = color;
            NameInput = name;

            ColorCircle.Fill = Color;
            LabelText.Content = NameInput;
        }
    }
}
