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
            InitializeComponent();
            TypeSelector.CBox.SelectionChanged += CBox_SelectionChanged;
        }

        private void CBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeSelector.selectedOffDayType.id == 1)
            {
                TimeControl.Visibility = Visibility.Hidden;
            }
            else
            {
                TimeControl.Visibility = Visibility.Visible;
            }
        }

        public void LoadOffDay(OffDay offDay)
        {
            id = offDay.Id;
            user_id = offDay.user_id;
            //YYYY-MM-DD hh:mm:ss
            TitleText.Text = offDay.title;

            CreationDate.Content = offDay.creation_date.Split('T').First() ;
            
            FromDatePicker.Text = offDay.start.Split(' ').First();
            
            FromHour.Value = int.Parse(offDay.start.Split(' ')[1].Split(':').First());
            FromMinute.Value = int.Parse(offDay.start.Split(' ')[1].Split(':')[1]);
            
            ToHour.Value = int.Parse(offDay.end.Split(' ')[1].Split(':').First());
            ToMinute.Value = int.Parse(offDay.end.Split(' ')[1].Split(':')[1]);
            
            TypeSelector.SelectIndex(offDay.typ);
            if(offDay.typ == 1)
            {
                TimeControl.Visibility = Visibility.Hidden;
            }

        }
        public OffDay GetOffDay()
        {
            string start = FromDatePicker.Text + " " + FromHour.Value.ToString().PadLeft(2, '0') + ":" + FromMinute.Value.ToString().PadLeft(2, '0') + ":00";
            string end = FromDatePicker.Text + " " + ToHour.Value.ToString().PadLeft(2, '0') + ":" + ToMinute.Value.ToString().PadLeft(2, '0') + ":00";
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
                tv.DeleteOffDay(tv.offDays[index - 1].Id);
            }
        }
    }
}
