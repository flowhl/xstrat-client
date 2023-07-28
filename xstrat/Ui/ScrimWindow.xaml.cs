using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Windows.Shapes;
using xstrat.Core;
using xstrat.Json;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for ScrimWindow.xaml
    /// </summary>
    public partial class ScrimWindow : System.Windows.Window
    {
        /// <summary>
        /// type
        /// 0 - new scrim
        /// 1 - edit scrim
        /// </summary>
        /// 

        private bool opened = false;
        public int type { get; set; }
        public Scrim scrim { get; set; }
        public Core.Window window { get; set; }


        public delegate void CalendarEventUpdateEventHandler(object source, EventArgs e);
        public event CalendarEventUpdateEventHandler CalendarEventUpdated;

        public ScrimWindow(xstrat.Core.Window window)
        {
            //fix datetime
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            Thread.CurrentThread.CurrentCulture = ci;

            this.window = window;
            if (window.StartDateTime.Date != window.EndDateTime.Date)
            {
                window.EndDateTime = new DateTime(window.StartDateTime.Year, window.StartDateTime.Month, window.StartDateTime.Day, 23, 59, 00);
            }
            type = 0;
            InitializeComponent();
            Loaded += ScrimWindow_Loaded;
            GotFocus += ScrimWindow_GotFocus;
        }

        private void ScrimWindow_GotFocus(object sender, RoutedEventArgs e)
        {
            opened = true;
        }

        private void ScrimWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }

        public ScrimWindow(Scrim scrim)
        {
            this.scrim = scrim;
            type = 1;
            InitializeComponent();
            Loaded += ScrimWindow_Loaded;
            GotFocus += ScrimWindow_GotFocus;
        }

        public void UpdateUI()
        {
            if(type == 0)
            {
                TypeLabel.Content = "Create Scrim";
                Title = "Create Scrim";
                TitleBox.Text = "Title";
                CommentBox.Text = "Description";
                FromHour.Value = window.StartDateTime.Hour;
                FromMinute.Value = window.StartDateTime.Minute;
                ToHour.Value = window.EndDateTime.Hour;
                ToMinute.Value = window.EndDateTime.Minute;
                CreatorLabel.Content = "";
                CreationDateLabel.Content = "";
                CalendarDatePicker.SelectedDate = window.StartDateTime.Date;
                PlayerStack.Children.Clear();
                foreach (var user in window.AvailablePlayers)
                {
                    Label label = new Label();
                    label.Foreground = Brushes.White;
                    label.Background = Brushes.Transparent;
                    label.Margin = new Thickness(5, 0, 5, 0);
                    label.Content = Globals.GetUserFromId(user.Id).name;
                    PlayerStack.Children.Add(label);
                }
                DeleteBtn.Visibility = Visibility.Collapsed;
                EventTypeSelector.SelectIndexWhenLoaded(0);
            }
            else if(type == 1)
            {
                DateTime start = DateTime.ParseExact(scrim.time_start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                DateTime end = DateTime.ParseExact(scrim.time_end, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                TypeLabel.Content = "Edit Scrim";
                Title = "Edit Scrim";
                OpponentNameBox.Text = scrim.opponent_name;
                TitleBox.Text = scrim.title;
                CommentBox.Text = scrim.comment;
                CalendarDatePicker.SelectedDate = start.Date;
                FromHour.Value = start.Hour;
                FromMinute.Value = start.Minute;
                ToHour.Value = end.Hour;
                ToMinute.Value = end.Minute;
                MapSelector1.SelectIndexWhenLoaded(DataCache.CurrentMaps.IndexOf(DataCache.CurrentMaps.Where(x => x.Id == scrim.map_1_id).FirstOrDefault()));
                MapSelector2.SelectIndexWhenLoaded(DataCache.CurrentMaps.IndexOf(DataCache.CurrentMaps.Where(x => x.Id == scrim.map_2_id).FirstOrDefault()));
                MapSelector3.SelectIndexWhenLoaded(DataCache.CurrentMaps.IndexOf(DataCache.CurrentMaps.Where(x => x.Id == scrim.map_3_id).FirstOrDefault()));
                CreatorLabel.Content = Globals.getUserFromId(scrim.creator_id).name;
                CreationDateLabel.Content = scrim.creation_date.Replace("T", " ").Replace("Z", "");
                if (!Globals.AdminUser)
                {
                    DeleteBtn.Visibility = Visibility.Collapsed;
                }
                EventTypeSelector.Loaded += EventTypeSelector_Loaded;
            }
            if(PlayerStack.Children.Count < 1)
            {
                UserViewer.Visibility = Visibility.Collapsed;
            }
        }

        private void EventTypeSelector_Loaded(object sender, RoutedEventArgs e)
        {
            EventTypeSelector.Loaded -= EventTypeSelector_Loaded;
            EventTypeSelector.SelectIndexWhenLoaded(scrim.event_type);
        }

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if(type == 1)
            {
                DateTime tempdate = CalendarDatePicker.SelectedDate.GetValueOrDefault();
                if(tempdate == null)
                {
                    Notify.sendError("No date selected or wrong format");
                    return;
                }

                string start = tempdate.ToString("yyyy/MM/dd HH:mm:ss").Split(' ')[0] + " " + FromHour.Value.ToString().PadLeft(2, '0') + ":" + FromMinute.Value.ToString().PadLeft(2, '0') + ":00";
                string end = tempdate.ToString("yyyy/MM/dd HH:mm:ss").Split(' ')[0] + " " + ToHour.Value.ToString().PadLeft(2, '0') + ":" + ToMinute.Value.ToString().PadLeft(2, '0') + ":00";
                start = start.Replace(".", "/").Replace("-", "/");
                end = end.Replace(".", "/").Replace("-", "/");

                string map1;
                if (MapSelector1.SelectedMap != null)
                {
                    map1 = MapSelector1.SelectedMap.Id;
                }
                string map2;
                if (MapSelector2.SelectedMap != null)
                {
                    map2 = MapSelector2.SelectedMap.Id;
                }
                string map3;
                if (MapSelector3.SelectedMap != null)
                {
                    map3 = MapSelector3.SelectedMap.Id;
                }
                int event_type = EventTypeSelector.selectedEventType.id;
                var result = await ApiHandler.SaveScrim(scrim.id, TitleBox.Text, CommentBox.Text, start, end, OpponentNameBox.Text, map1, map2, map3, ScrimModeSelector.selectedScrimMode.id, event_type);
                if (result.Item1)
                {
                    Notify.sendSuccess("Saved successfully");
                    opened = false;
                    Close();
                }
                else
                {
                    Notify.sendError(result.Item2);
                }
            }
            else if(type == 0)
            {
                DateTime tempdate = CalendarDatePicker.SelectedDate.GetValueOrDefault();
                if (tempdate == null)
                {
                    Notify.sendError("No date selected or wrong format");
                    return;
                }

                string start = tempdate.ToString("yyyy/MM/dd HH:mm:ss").Split(' ')[0] + " " + FromHour.Value.ToString().PadLeft(2, '0') + ":" + FromMinute.Value.ToString().PadLeft(2, '0') + ":00";
                string end = tempdate.ToString("yyyy/MM/dd HH:mm:ss").Split(' ')[0] + " " + ToHour.Value.ToString().PadLeft(2, '0') + ":" + ToMinute.Value.ToString().PadLeft(2, '0') + ":00";
                start = start.Replace(".", "/").Replace("-", "/");
                end = end.Replace(".", "/").Replace("-", "/");


                int? scrim_id = null;
                var result = await ApiHandler.NewScrim(ScrimModeSelector.selectedScrimMode.id, TitleBox.Text, OpponentNameBox.Text, start, end, EventTypeSelector.selectedEventType.id);
                if (result.Item1)
                {
                    string response = result.Item2;
                    //convert to json instance
                    JObject json = JObject.Parse(response);
                    var data = json.SelectToken("data").ToString();
                    if (data != null && data != "")
                    {
                        xstrat.Json.Data dresult = JsonConvert.DeserializeObject<Json.Data>(data);
                        scrim_id = dresult.insertId;
                    }
                    else
                    {
                        Notify.sendError("insertId could not be loaded");
                        throw new Exception("insertId could not be loaded");
                    }
                    if(scrim_id != null)
                    {
                        string map1 = null;
                        if(MapSelector1.SelectedMap != null)
                        {
                            map1 = MapSelector1.SelectedMap.Id;
                        }
                        string map2 = null;
                        if (MapSelector2.SelectedMap != null)
                        {
                            map2 = MapSelector2.SelectedMap.Id;
                        }
                        string map3 = null;
                        if (MapSelector3.SelectedMap != null)
                        {
                            map3 = MapSelector3.SelectedMap.Id;
                        }
                        int event_type = EventTypeSelector.selectedEventType.id;
                        var result2 = await ApiHandler.SaveScrim(scrim_id.GetValueOrDefault(), TitleBox.Text, CommentBox.Text, start, end, OpponentNameBox.Text, map1, map2, map3, ScrimModeSelector.selectedScrimMode.id, event_type);
                        if (result2.Item1)
                        {
                            Notify.sendSuccess("Saved successfully");
                            opened = false;
                            Close();
                        }
                        else
                        {
                            Notify.sendError("Error when modifying scrim data " + result2.Item2);
                        }
                    }
                    else
                    {
                        Notify.sendError("insertId could not be loaded");
                        throw new Exception("insertId could not be loaded");
                    }
                   
                }
                else
                {
                    Notify.sendError( result.Item2);
                }
                CalendarEventUpdated?.Invoke(this, e);
            }
            
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteScrimAsync();
        }

        public async Task DeleteScrimAsync()
        {
            if(type == 1)
            {
                var result = await ApiHandler.DeleteScrim(scrim.id);
                if (result.Item1)
                {
                    Notify.sendSuccess("Deleted successfully");
                    opened = false;
                    Close();
                }
                else
                {
                    Notify.sendError("Scrim could not be delted: " + result.Item2);
                    throw new Exception("Scrim could not be delted: " + result.Item2);

                }
            }
        }

        //private void Window_Deactivated(object sender, EventArgs e)
        //{
        //    if (IsLoaded && opened)
        //    {
        //        this.Close();
        //    }
        //}
    }
}
