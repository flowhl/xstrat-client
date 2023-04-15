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
        // ValueChanged event
        public event EventHandler ValueChanged;

        // Invoke ValueChanged event
        protected virtual void OnValueChanged(EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        // ValueIncreasedToLimit event
        public event EventHandler ValueIncreasedToLimit;

        // Invoke ValueIncreasedToLimit event
        protected virtual void OnValueIncreasedToLimit(EventArgs e)
        {
            ValueIncreasedToLimit?.Invoke(this, e);
        }

        // ValueIncreasedToLimit event
        public event EventHandler ValueDecreasedToLimit;

        // Invoke ValueDecreasedToLimit event
        protected virtual void OnValueDecreasedToLimit(EventArgs e)
        {
            ValueDecreasedToLimit?.Invoke(this, e);
        }

        private int _value = 0;
        public int Value { 
            get { return _value; } 
            set {
                if(!AllowNegative && value < 0 && _value == 0)
                {
                    _value = Limit;
                    Number.Text = _value.ToString();
                    return;
                }
                if (!AllowNegative && value < 0)
                {
                    _value = 0;
                    Number.Text = _value.ToString();
                    OnValueDecreasedToLimit(new EventArgs());
                    return;
                }
                if(_value == Limit && value > Limit)
                {
                    _value = 0;
                    Number.Text = _value.ToString();
                    OnValueIncreasedToLimit(new EventArgs());
                    return;
                }
                if (value > Limit)
                {
                    _value = Limit;
                    Number.Text = _value.ToString();
                    return;
                }
                _value = value;

                Number.Text = _value.ToString();
            } }
        public bool AllowNegative { get; set; } 
        public int Limit { get; set; }
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
            // Raise ValueChanged event
            OnValueChanged(new EventArgs());
        }

        private void DownBtn_Click(object sender, RoutedEventArgs e)
        {
            Value--;
            // Raise ValueChanged event
            OnValueChanged(new EventArgs());
        }

        private void Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Value = Convert.ToInt32(Number.Text);
                // Raise ValueChanged event
                OnValueChanged(new EventArgs());
            }
            catch (Exception ex)
            {
                Value = 0;
                Number.SelectAll();
                // Raise ValueChanged event
                OnValueChanged(new EventArgs());
            }
        }
    }
}
