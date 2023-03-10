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
using MaterialDesignThemes.Wpf;
using System.Runtime.CompilerServices;
using xstrat.Core;

namespace xstrat
{
    #region Converter
    [ValueConversion(typeof(bool), typeof(bool))]
    public class NotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
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
    #endregion

    public class DataGridButtonColumn : DataGridBoundColumn
    {
        public event RoutedEventHandler Click;

        public string Title { get; set; }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(DataGridButtonColumn), new PropertyMetadata(true));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            // Create a button control
            var button = new Button()
            {
                Content = Title,
                Tag = dataItem,
                Style = (Style)Application.Current.Resources["DataGridSmall"]
            };

            // Bind the IsEnabled property to the dependency property
            button.SetBinding(Button.IsEnabledProperty, new Binding(nameof(IsEnabled))
            {
                Source = this
            });

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
    public class DataGridIconColumn : DataGridBoundColumn
    {
        public event RoutedEventHandler Click;

        public PackIconKind Kind { get; set; }
        public string ButtonToolTip { get; set; }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(DataGridIconColumn), new PropertyMetadata(true));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var icon = new PackIcon();
            icon.Kind = Kind;
            icon.Height = 15;
            icon.Width = 15;

            // Create a button control
            var button = new Button()
            {
                Content = icon,
                Tag = dataItem,
                Style = (Style)Application.Current.Resources["DataGridSmall"],
                ToolTip = ButtonToolTip
            };

            // Bind the IsEnabled property to the dependency property
            button.SetBinding(Button.IsEnabledProperty, new Binding(nameof(IsEnabled))
            {
                Source = this
            });

            // Set the IsEnabled property of the button
            button.IsEnabled = IsEnabled;
            
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

            // Subscribe to the DataContextChanged event to update the Fill property
            var row = cell.FindVisualParent<DataGridRow>();
            if (row != null)
            {
                row.DataContextChanged += (sender, e) =>
                {
                    // Get the current value of the bound property
                    var propInfo = e.NewValue.GetType().GetProperty(BindingPath);
                    var value = propInfo.GetValue(e.NewValue);

                    // Convert the value to a Brush using the value converter
                    var converter = new IndicatorValueConverter();
                    var brush = (Brush)converter.Convert(value, typeof(Brush), null, CultureInfo.CurrentCulture);

                    // Update the Fill property of the Ellipse control
                    ellipse.Fill = brush;
                };
            }

            // Return the Ellipse control as the element for the cell
            return ellipse;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            // Editing is not supported for this column
            return null;
        }
    }
    



}
