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
    /// Interaction logic for Teammate.xaml
    /// </summary>
    public partial class Teammate : UserControl
    {
        public Teammate(string name, int ID, string color)
        {
            InitializeComponent();
            UserName.Text = name;
            UserID.Content = ID;
            UserColorRectangle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }
    }
}
