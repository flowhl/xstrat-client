using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using xstrat.Core;
using xstrat.Json;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for OffDaysList.xaml
    /// </summary>
    public partial class OffDaysList : UserControl
    {
        public List<OffDay> offDays = new List<OffDay>();
        public OffDaysList()
        {
            InitializeComponent();
            RetrieveOffDays();
        }


        private async void NewOffDay_Click(object sender, RoutedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            (bool, string) result = await ApiHandler.NewOffDay(0, "Title", dt.ToString("yyyy'/'MM'/'dd' 'HH:mm:ss"), dt.AddHours(1).ToString("yyyy'/'MM'/'dd' 'HH:mm:ss"));
            if (result.Item1)
            {
                RetrieveOffDays();
            }
            else
            {
                Notify.sendError("Could not save new off day: " + result.Item2);
            }
        }

        private async void SaveOffDays_Click(object sender, RoutedEventArgs e)
        {
            bool success = true;
            foreach (UIElement offday in ODList.Children)
            {
                if (ODList.Children.IndexOf(offday) != 0)
                {
                    var odc = offday as OffdayControl;
                    var od = odc.GetOffDay();
                    if (od.Id != null)
                    {
                        (bool, string) result = await ApiHandler.SaveOffDay(od.Id.GetValueOrDefault(), od.typ, od.title, od.start, od.end);
                        if (result.Item1 == false)
                        {
                            success = false;
                            Notify.sendError("Could not save off days: " + result.Item2);
                        }
                    }
                }

            }
            if (success)
            {
                Notify.sendSuccess("Saved successfully");
            }
        }

        public async void DeleteOffDay(int? id)
        {
            if (id != null)
            {

                (bool, string) result = await ApiHandler.DeleteOffDay(id.GetValueOrDefault());
                if (result.Item1)
                {
                    RetrieveOffDays();
                    Notify.sendSuccess("Deleted successfully");
                }
                else
                {
                    Notify.sendError("Could not delete off day: " + result.Item2);
                }
            }
        }

        private void LoadOffDaysFromList()
        {
            var buttons = ODList.Children[0];
            ODList.Children.Clear();
            ODList.Children.Add(buttons);
            foreach (var offday in offDays)
            {
                var od = new OffdayControl();
                od.Width = 700;
                od.LoadOffDay(offday);
                od.Margin = new Thickness(0, 10, 0, 0);
                ODList.Children.Add(od);
            }
        }
        private async void RetrieveOffDays()
        {
            (bool, string) result = await ApiHandler.GetUserOffDays();
            if (result.Item1)
            {
                string response = result.Item2;
                //convert to json instance
                JObject json = JObject.Parse(response);
                var data = json.SelectToken("data").ToString();
                if (data != null && data != "")
                {
                    List<xstrat.Json.OffDay> odList = JsonConvert.DeserializeObject<List<Json.OffDay>>(data);
                    offDays.Clear();
                    foreach (var od in odList)
                    {
                        offDays.Add(od);
                    }
                }
                else
                {
                    Notify.sendError("Offdays could not be created");
                    throw new Exception("Offdays could not be created");
                }
                LoadOffDaysFromList();
            }
            else
            {
                return;
            }
        }

    }
}
