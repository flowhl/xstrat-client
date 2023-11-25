using AngleSharp.Html;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
using xstrat.Json;
using xstrat.Models.Supabase;
using xstrat.MVVM.View;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for OffdayControl.xaml
    /// </summary>
    public partial class OffdayControl : UserControl
    {
        // Declare the event using the delegate
        public event ValuesChangedEventHandler ValuesChanged;
        // Define a delegate for the event handler
        public delegate void ValuesChangedEventHandler(object sender, EventArgs e);

        public string id { get; set; }
        public string user_id { get; set; }
        public OffdayControl()
        {
            //fix datetime
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            Thread.CurrentThread.CurrentCulture = ci;

            InitializeComponent();
        }

        public void LoadOffDay(CalendarBlock offDay)
        {
            id = offDay.Id;
            user_id = offDay.UserId;
            //YYYY-MM-DD hh:mm:ss
            TitleText.Text = offDay.Title;

            CreationDate.Text = "Created on: " + offDay.CreatedAt.ToString().Split('T').First() ;
            
            FromDatePicker.Text = offDay.Start.ToString().Split(' ').First();
            ToDatePicker.Text = offDay.CreatedAt.ToString().Split(' ').First();

            FromTimeSelector.SetTime(offDay.Start.GetValueOrDefault().Hour, offDay.Start.GetValueOrDefault().Minute);
            ToTimeSelector.SetTime(offDay.End.GetValueOrDefault().Hour, offDay.End.GetValueOrDefault().Minute);
            
            TypeSelector.SelectIndexWhenLoaded(offDay.Typ);
            if(offDay.Typ == 1)
            {
                FromTimeSelector.Visibility = Visibility.Hidden;
                ToTimeSelector.Visibility = Visibility.Hidden;
            }
            ValuesChanged?.Invoke(this, EventArgs.Empty);
        }
        public CalendarBlock GetOffDay()
        {
            if (FromDatePicker.SelectedDate == null)
            {
                Notify.sendError("No date selected or wrong format");
                return null;
            }
            if (ToDatePicker.SelectedDate == null)
            {
                Notify.sendError("No date selected or wrong format");
                return null;
            }

            DateTime start = FromDatePicker.SelectedDate.GetValueOrDefault().SetTime(FromTimeSelector.GetHour(), FromTimeSelector.GetMinute(), 0);
            DateTime end = ToDatePicker.SelectedDate.GetValueOrDefault().SetTime(ToTimeSelector.GetHour(), ToTimeSelector.GetMinute(), 0);

            if(end.Hour == 0 && end.Minute == 0) { end.SetTime(23, 59, 59); }

            CalendarBlock offDay = new CalendarBlock() { Id = id, UserId = user_id, Typ = TypeSelector.selectedOffDayType.id, Title = TitleText.Text, Start =  start, End = end};
            return offDay;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject ucParent = this.Parent;
            if(ucParent != null)
            {
                while (!(ucParent is UserControl))
                {
                    ucParent = LogicalTreeHelper.GetParent(ucParent);
                }
                var tv = ucParent as OffDaysList;
                var index = tv.ODList.Children.IndexOf(this);
                tv.DeleteOffDay(tv.offDays[index].Id);
            }
        }

        private void TitleText_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValuesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void FromDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ValuesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ToDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ValuesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void TypeSelector_SelectionChanged(object sender, EventArgs e)
        {
            if (TypeSelector.selectedOffDayType.id == 1)
            {
                FromTimeSelector.Visibility = Visibility.Hidden;
                ToTimeSelector.Visibility = Visibility.Hidden;
            }
            else
            {
                FromTimeSelector.Visibility = Visibility.Visible;
                ToTimeSelector.Visibility = Visibility.Visible;
            }
            ValuesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void FromTimeSelector_TimeChanged(object sender, EventArgs e)
        {
            ValuesChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ToTimeSelector_TimeChanged(object sender, EventArgs e)
        {
            ValuesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
