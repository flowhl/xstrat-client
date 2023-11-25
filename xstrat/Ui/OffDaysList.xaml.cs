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
    public partial class OffDaysList : StateUserControl
    {
        public List<Models.Supabase.CalendarBlock> offDays = new List<Models.Supabase.CalendarBlock>();
        public OffDaysList()
        {
            InitializeComponent();
            Loaded += OffDaysList_Loaded;
        }

        private void OffDaysList_Loaded(object sender, RoutedEventArgs e)
        {
            RetrieveOffDays();
            if (DataCache.CurrentTeam.UseOnDays == 0)
            {
                TxtTitle.Content = "Your off days:";
            }
            else
            {
                TxtTitle.Content = "Your available days:";
            }
            HasChanges = false;
        }

        private async void NewOffDay_Click(object sender, RoutedEventArgs e)
        {
            Save(true);
            DateTime dt = DateTime.Now;
            bool result = await ApiHandler.NewOffDay(0, "Title", dt.ToString("yyyy'/'MM'/'dd' 'HH:mm:ss"), dt.AddHours(1).ToString("yyyy'/'MM'/'dd' 'HH:mm:ss"));
            if (result)
            {
                RetrieveOffDays();
            }
            else
            {
                Notify.sendError("Could not save new entry");
            }
            HasChanges = false;
        }

        private async void SaveOffDays_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        public override void Save(bool silent = false)
        {
            bool success = true;
            foreach (UIElement offday in ODList.Children)
            {
                var odc = offday as OffdayControl;
                var od = odc.GetOffDay();
                if (od?.Id != null)
                {
                    var result = ApiHandler.SaveOffDay(od.Id, od.Typ, od.Title, od.Start.GetValueOrDefault(), od.End.GetValueOrDefault()).Result;
                    if (result == false)
                    {
                        success = false;
                        Notify.sendError("Could not save off days");
                    }
                }
                else
                {
                    success = false;
                }

            }
            if (success)
            {
                if (!silent)
                    Notify.sendSuccess("Saved successfully");
                base.Save(silent);
            }
        }

        public async void DeleteOffDay(string id)
        {
            if (id.IsNullOrEmpty())
                return;

            Save(true);

            (bool, string) result = await ApiHandler.DeleteOffDay(id);
            if (result.Item1)
            {
                RetrieveOffDays();
                //Notify.sendSuccess("Deleted successfully");
            }
            else
            {
                Notify.sendError("Could not delete off day: " + result.Item2);
            }

            HasChanges = false;
        }

        private void LoadOffDaysFromList()
        {
            ODList.Children.Clear();
            foreach (var offday in offDays)
            {
                var od = new OffdayControl();
                od.ValuesChanged += Offday_ValuesChanged;
                //od.Width = 700;
                od.LoadOffDay(offday);
                od.Margin = new Thickness(0, 10, 0, 0);
                ODList.Children.Add(od);
            }
            HasChanges = false;
        }

        private void Offday_ValuesChanged(object sender, EventArgs e)
        {
            HasChanges = true;
        }

        private void RetrieveOffDays()
        {
            var result = ApiHandler.GetUserOffDays();

            offDays = result;

            LoadOffDaysFromList();
            HasChanges = false;
        }

    }
}
