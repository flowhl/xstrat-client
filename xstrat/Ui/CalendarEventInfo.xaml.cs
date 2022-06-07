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
using System.Windows.Shapes;
using xstrat.Core;
using xstrat.Json;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for CalendarEventInfo.xaml
    /// </summary>
    public partial class CalendarEventInfo : System.Windows.Window
    {
        public CalendarEntry calendarEntry{ get; set; }
        public CalendarEventInfo(CalendarEntry calendarEntry)
        {
            InitializeComponent();
            this.calendarEntry = calendarEntry;
            Loaded += CalendarEventInfo_Loaded;
        }

        private void CalendarEventInfo_Loaded(object sender, RoutedEventArgs e)
        {
            if(calendarEntry.typ == 0)
            {
                TypeLabel.Content = "Scrim";
            }
            if (calendarEntry.typ == 1)
            {
                TypeLabel.Content = "Off day";
            }
            if (calendarEntry.typ == 2)
            {
                TypeLabel.Content = "Recommended scrim time";
                PlayerlistSP.Visibility = Visibility.Visible;
                if(calendarEntry.args != null && calendarEntry.args.FirstOrDefault() != null && calendarEntry.args.FirstOrDefault() is xstrat.Core.Window)
                {
                    var list = (calendarEntry.args.FirstOrDefault() as xstrat.Core.Window).AvailablePlayers;

                    if (list != null && list.Count() > 0) 
                    {
                        foreach (var item in list)
                        {
                            Label newlabel = new Label();
                            newlabel.Foreground = Brushes.White;
                            newlabel.Content = Globals.getUserFromId(item.ID).name;
                            newlabel.FontSize = 14;
                            PlayerList.Children.Add(newlabel);
                        }                    
                    }
                }
            }
            try
            {
                FromLabel.Content = calendarEntry.DateFrom.GetValueOrDefault().ToString("yyyy/MM/dd HH:mm:ss");
                ToLabel.Content = calendarEntry.DateTo.GetValueOrDefault().ToString("yyyy/MM/dd HH:mm:ss");
                if(calendarEntry.user != null)
                {
                    UserLabel.Content = calendarEntry.user.name ?? "";
                }
                else
                {
                    User.Visibility = Visibility.Collapsed;
                }
                TitleLabel.Content = calendarEntry.Label;
            }
            catch (Exception ex)
            {

            }
        }
    }
}
