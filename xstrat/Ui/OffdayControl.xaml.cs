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
using xstrat.MVVM.View;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for OffdayControl.xaml
    /// </summary>
    public partial class OffdayControl : UserControl
    {
        public int? id { get; set; }
        public int? user_id { get; set; }
        public OffdayControl()
        {
            //fix datetime
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            Thread.CurrentThread.CurrentCulture = ci;

            InitializeComponent();
            TypeSelector.CBox.SelectionChanged += CBox_SelectionChanged;
        }

        private void CBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        }

        public void LoadOffDay(OffDay offDay)
        {
            id = offDay.Id;
            user_id = offDay.user_id;
            //YYYY-MM-DD hh:mm:ss
            TitleText.Text = offDay.title;

            CreationDate.Text = "Created on: " + offDay.creation_date.Split('T').First() ;
            
            FromDatePicker.Text = offDay.start.Split(' ').First();
            
            int fromHour = int.Parse(offDay.start.Split(' ')[1].Split(':').First());
            int fromMinute = int.Parse(offDay.start.Split(' ')[1].Split(':')[1]);
            
            int toHour = int.Parse(offDay.end.Split(' ')[1].Split(':').First());
            int toMinute = int.Parse(offDay.end.Split(' ')[1].Split(':')[1]);

            FromTimeSelector.SetTime(fromHour, fromMinute);
            ToTimeSelector.SetTime(toHour, toMinute);
            
            TypeSelector.SelectIndexWhenLoaded(offDay.typ);
            if(offDay.typ == 1)
            {
                FromTimeSelector.Visibility = Visibility.Hidden;
                ToTimeSelector.Visibility = Visibility.Hidden;
            }

        }
        public OffDay GetOffDay()
        {
            DateTime tempdate = FromDatePicker.SelectedDate.GetValueOrDefault();
            if (tempdate == null)
            {
                Notify.sendError("No date selected or wrong format");
                return null;
            }

            string datestring = tempdate.ToString("yyyy/MM/dd HH:mm:ss").Replace(".", "/").Replace("-", "/");

            string startDate = datestring.Split(' ')[0];
            string endDate = datestring.Split(' ')[0];
            string startTime = FromTimeSelector.GetTimeString();
            string endTime = ToTimeSelector.GetTimeString();
            endTime = endTime.Replace("00:00:00", "23:59:59");

            string start = startDate + " " + startTime;
            string end = endDate + " " + endTime;
            OffDay offDay = new OffDay(id.GetValueOrDefault(), user_id.GetValueOrDefault(), null, TypeSelector.selectedOffDayType.id, TitleText.Text, start, end);
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
    }
}
