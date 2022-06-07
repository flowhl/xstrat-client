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
    /// Interaction logic for NumberBox.xaml
    /// </summary>
    public partial class NumberBox : UserControl
    {
        private int _value = 0;
        public int Value { 
            get { return _value; } 
            set {
                if (!allowNegative && value < 0)
                {
                    _value = 0;
                    return;
                }
                if (value > limit)
                {
                    _value = limit;
                    return;
                }
                _value = value;

                Number.Text = _value.ToString();
            } }
        public bool allowNegative { get; set; } 
        public int limit { get; set; }
        public NumberBox()
        {
            InitializeComponent();
            Loaded += NumberBox_Loaded;
        }

        private void NumberBox_Loaded(object sender, RoutedEventArgs e)
        {
            Number.Text = _value.ToString();
        }

        private void UpBtn_Click(object sender, RoutedEventArgs e)
        {
            Value++;
        }

        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {
            Value--;
        }

        private void Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Value = Convert.ToInt32(Number.Text);
            }
            catch (Exception ex)
            {
                Value = 0;
            }
        }
    }
}
