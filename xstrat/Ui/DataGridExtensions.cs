using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;

namespace xstrat.Ui
{
    public class DataGridButtonColumn : DataGridBoundColumn
    {
        public event RoutedEventHandler Click;

        public string Title { get; set; }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            // Create a button control
            var button = new Button()
            {
                Content = Title,
                Tag = dataItem,
                Style = (Style)Application.Current.Resources["DataGridSmall"]
            };

            // Attach the Click event handler
            button.Click += OnClick;

            // Return the button control as the element for the cell
            return button;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            // Editing is not supported for this column
            return null;
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            // Raise the Click event
            Click?.Invoke(sender, e);
        }
    }
    public class DataGridIndicatorColumn : DataGridBoundColumn
    {
        public string BindingPath { get; set; }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            // Create an Ellipse control
            var ellipse = new Ellipse()
            {
                Width = 10,
                Height = 10,
                Fill = new SolidColorBrush()
            };

            // Bind the Fill property to the bound value using markup extension
            ellipse.SetBinding(Shape.FillProperty, new Binding(BindingPath)
            {
                Source = dataItem,
                Converter = new IndicatorValueConverter()
            });

            // Return the Ellipse control as the element for the cell
            return ellipse;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            // Editing is not supported for this column
            return null;
        }
    }

    // Value converter to convert the bound value to a Brush for the Ellipse Fill property
    public class IndicatorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the value is true, return a green Brush, else return a red Brush
            if (value is bool indicatorValue && indicatorValue)
            {
                return new SolidColorBrush(Colors.Green);
            }
            else
            {
                return new SolidColorBrush(Colors.Red);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
