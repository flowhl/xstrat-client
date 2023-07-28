using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
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
using xstrat.Calendar;
using xstrat.Core;
using xstrat.Json;
using xstrat.Ui;
using static xstrat.Ui.ScrimWindow;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for CalendarView.xaml
    /// </summary>
    public partial class CalendarView : UserControl, INotifyPropertyChanged
    {
        private List<OffDay> offDays = new List<OffDay>();
        private List<Scrim> scrims = new List<Scrim>();
        public List<ICalendarEvent> _events;
        public List<ICalendarEvent> Events
        {
            get { return _events; }
            set
            {
                if (_events != value)
                {
                    _events = value;
                    OnPropertyChanged(() => Events);

                    // redraw days with events when Events property changes
                    if (CalendarMonthUI != null)
                    {
                        CalendarMonthUI.DrawDays();
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CalendarView()
        {
            InitializeComponent();
            // set date of first example event to +- middle of month
            DateTime startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 15);

            // add example events
            Events = new List<ICalendarEvent>();
            //Events.Add(new CalendarEntry() { DateFrom = DateTime.Now.AddDays(6), DateTo = DateTime.Now.AddDays(6).AddHours(1), Label = "Scrim", typ = 0 });

            Loaded += CalendarView_Loaded;
        }

        private void CalendarView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataCache.CurrentTeam.UseOnDays == 0)
            {
                OffDayChecks.Content = "Your off days";
            }
            else
            {
                OffDayChecks.Content = "Available days";
            }

            RetrieveScrims();
            RetrieveOffDays();
            // draw days with events calendar
            CreateUserCheckbox();
            CalendarMonthUI.DrawDays();

            // subscribe to double cliked event
            CalendarMonthUI.CalendarEventDoubleClickedEvent += Calendar_CalendarEventDoubleClickedEvent;
            Globals.CalendarEventCreated += Globals_CalendarEventCreated;
        }

        private void Globals_CalendarEventCreated(object sender, Globals.CalendarEventCreatedArgs e)
        {
            if (this.IsLoaded)
            {
                DateTime start = new DateTime(e.Date.Year, e.Date.Month, e.Date.Day, 0, 0, 0);
                DateTime end = new DateTime(e.Date.Year, e.Date.Month, e.Date.Day, 23, 59, 59);
                var responseWindow = new ScrimWindow(new Core.Window { StartDateTime = start, EndDateTime = end, AvailablePlayers = new List<Player>() });
                responseWindow.Show();
                responseWindow.Closing += ResponseWindow_Closing;
            }
        }

        private void Calendar_CalendarEventDoubleClickedEvent(object sender, CalendarEventView e)
        {
            if (e.DataContext is ICalendarEvent calendarEvent)
            {
                if (calendarEvent.Typ == 0) //scrim
                {
                    var responseWindow = new ScrimWindow(calendarEvent.Scrim);
                    responseWindow.Show();
                    responseWindow.Closing += ResponseWindow_Closing;
                }

                if (calendarEvent.Typ == 1) //offday
                {
                    var responseWindow = new CalendarEventInfo(calendarEvent as CalendarEntry);
                    responseWindow.Show();
                    responseWindow.Closing += ResponseWindow_Closing;
                }

                if (calendarEvent.Typ == 2) //recommendation
                {
                    var responseWindow = new ScrimWindow(calendarEvent.Args.First() as Core.Window);
                    responseWindow.Show();
                    responseWindow.Closing += ResponseWindow_Closing;
                }
            }
        }

        private void ResponseWindow_Closing(object sender, CancelEventArgs e)
        {
            OnClose();
        }

        private async void OnClose()
        {
            Events.Clear();
            RetrieveScrims();
            RetrieveOffDays();
        }


        public void OnPropertyChanged<T>(Expression<Func<T>> exp)
        {
            var memberExpression = (MemberExpression)exp.Body;
            var propertyName = memberExpression.Member.Name;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private async void RetrieveOffDays()
        {
            try
            {
                (bool, string) result = await ApiHandler.GetTeamOffDays();
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
                        Notify.sendError("Event could not be created");
                        throw new Exception("Event could not be created");
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Notify.sendError(ex.Message);
            }

            foreach (var od in offDays)
            {
                MakeCalendarEntry(od);
            }

            await Task.Delay(100);

            CalendarMonthUI.DrawDays();
        }

        private async void RetrieveScrims()
        {
            try
            {
                (bool, string) result = await ApiHandler.GetTeamScrims();
                if (result.Item1)
                {
                    string response = result.Item2;
                    //convert to json instance
                    JObject json = JObject.Parse(response);
                    var data = json.SelectToken("data").ToString();
                    if (data != null && data != "")
                    {
                        List<xstrat.Json.Scrim> scList = JsonConvert.DeserializeObject<List<Json.Scrim>>(data);
                        scrims.Clear();
                        foreach (var sc in scList)
                        {
                            scrims.Add(sc);
                        }
                    }
                    else
                    {
                        Notify.sendError("Scrim could not be created");
                        throw new Exception("Scrim could not be created");
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Notify.sendError(ex.Message);
            }

            foreach (var sc in scrims)
            {
                MakeCalendarEntry(sc);
            }
            CalendarMonthUI.DrawDays();
        }

        #region Helper Methods
        private void UpdateHiddenEntrys()
        {
            List<UserCheckbox> checkboxes = new List<UserCheckbox>();
            foreach (var item in Row2.Children)
            {
                checkboxes.Add(item as UserCheckbox);
            }
            foreach (var item in Row3.Children)
            {
                checkboxes.Add(item as UserCheckbox);
            }
            foreach (var item in Row4.Children)
            {
                checkboxes.Add(item as UserCheckbox);
            }
            foreach (var item in Row5.Children)
            {
                checkboxes.Add(item as UserCheckbox);
            }
            foreach (var userCheckbox in checkboxes)
            {
                var events = Events.Where(x => x.User != null && x.User.Id == userCheckbox.User.Id);

                foreach (var _event in events)
                {
                    _event.Visible = userCheckbox.UserCheckboxItem.IsChecked.GetValueOrDefault(true);
                }
            }
            CalendarMonthUI.DrawDays();
        }

        public void CreateUserCheckbox()
        {
            int counter = 0;
            foreach (var user in DataCache.CurrentTeamMates)
            {
                if (user != null)
                {
                    StackPanel rowToAdd = null;
                    if (counter < 3)
                    {
                        rowToAdd = Row2;
                    }
                    else if (counter < 6)
                    {
                        rowToAdd = Row3;
                    }
                    else if (counter < 9)
                    {
                        rowToAdd = Row4;
                    }
                    else if (counter < 12)
                    {
                        rowToAdd = Row5;
                    }
                    if (rowToAdd != null)
                    {
                        UserCheckbox userCheckbox = new UserCheckbox(user);
                        userCheckbox.UserCheckboxItem.Checked += UserCheckboxItem_Checked;
                        userCheckbox.UserCheckboxItem.Unchecked += UserCheckboxItem_Unchecked;
                        rowToAdd.Children.Add(userCheckbox);
                    }
                    counter++;
                }
            }
        }

        private void UserCheckboxItem_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateHiddenEntrys();
        }

        private void UserCheckboxItem_Checked(object sender, RoutedEventArgs e)
        {
            UpdateHiddenEntrys();
        }

        private void MakeCalendarEntry(OffDay od)
        {
            try
            {
                DateTime? from = null;
                DateTime? to = null;
                if (od.Typ == 0) // exact
                {
                    from = DateTime.ParseExact(od.Start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    to = DateTime.ParseExact(od.End, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    if (from != null && to != null)
                    {
                        Events.Add(new CalendarEntry() { DateFrom = from, DateTo = to, Label = GetLabel(od), Typ = 1, User = DataCache.CurrentTeamMates.Where(x => x.Id == od.UserId).FirstOrDefault() });
                    }
                }
                else if (od.Typ == 1) //entire day
                {
                    string sfrom = od.Start.Split(' ').First() + " 00:00:00";
                    string sto = od.End.Split(' ').First() + " 23:59:59";
                    from = DateTime.ParseExact(od.Start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    to = DateTime.ParseExact(od.End, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    if (from != null && to != null)
                    {
                        Events.Add(new CalendarEntry() { DateFrom = from, DateTo = to, Label = GetLabel(od), Typ = 1, User = DataCache.CurrentTeamMates.Where(x => x.Id == od.UserId).FirstOrDefault() });
                    }
                }
                else if (od.Typ == 2) //weekly
                {
                    int offset = (int)(DateTime.ParseExact(od.Start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) - DateTime.Now.Date).TotalDays / 7;
                    if (offset < 0) offset = 0;

                    for (int i = 0; i < offset + 24; i++)
                    {
                        from = DateTime.ParseExact(od.Start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).AddDays(7 * i);
                        to = DateTime.ParseExact(od.End, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).AddDays(7 * i);
                        if (from != null && to != null)
                        {
                            Events.Add(new CalendarEntry() { DateFrom = from, DateTo = to, Label = GetLabel(od), Typ = 1, User = DataCache.CurrentTeamMates.Where(x => x.Id == od.UserId).FirstOrDefault() });
                        }
                    }
                }
                else if (od.Typ == 3) // every second week
                {
                    int offset = (int)(DateTime.ParseExact(od.Start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) - DateTime.Now.Date).TotalDays / 14;
                    if (offset < 0) offset = 0;

                    for (int i = 0; i < offset + 12; i++)
                    {
                        from = DateTime.ParseExact(od.Start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).AddDays(14 * i);
                        to = DateTime.ParseExact(od.End, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).AddDays(14 * i);
                        if (from != null && to != null)
                        {
                            Events.Add(new CalendarEntry() { DateFrom = from, DateTo = to, Label = GetLabel(od), Typ = 1, User = DataCache.CurrentTeamMates.Where(x => x.Id == od.UserId).FirstOrDefault() });
                        }
                    }
                }
                else if (od.Typ == 4) // monthly
                {
                    int offset = (int)(DateTime.ParseExact(od.Start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) - DateTime.Now.Date).TotalDays / 30;
                    if (offset < 0) offset = 0;

                    for (int i = 0; i < offset + 6; i++)
                    {
                        from = DateTime.ParseExact(od.Start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).AddMonths(i);
                        to = DateTime.ParseExact(od.End, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).AddMonths(i);
                        if (from != null && to != null)
                        {
                            Events.Add(new CalendarEntry() { DateFrom = from, DateTo = to, Label = GetLabel(od), Typ = 1, User = DataCache.CurrentTeamMates.Where(x => x.Id == od.UserId).FirstOrDefault() });
                        }
                    }
                }
                else if (od.Typ == 5) // daily
                {
                    int offset = (int)(DateTime.ParseExact(od.Start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture) - DateTime.Now.Date).TotalDays;
                    if (offset < 0) offset = 0;

                    for (int i = 0; i < offset + 365; i++)
                    {
                        from = DateTime.ParseExact(od.Start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).AddDays(i);
                        to = DateTime.ParseExact(od.End, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).AddDays(i);
                        if (from != null && to != null)
                        {
                            Events.Add(new CalendarEntry() { DateFrom = from, DateTo = to, Label = GetLabel(od), Typ = 1, User = DataCache.CurrentTeamMates.Where(x => x.Id == od.UserId).FirstOrDefault() });
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Notify.sendError(ex.Message);
            }
        }

        private void MakeCalendarEntry(Scrim sc)
        {
            try
            {
                DateTime? from = null;
                DateTime? to = null;

                from = DateTime.ParseExact(sc.time_start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                to = DateTime.ParseExact(sc.time_end, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                if (from != null && to != null)
                {
                    Events.Add(new CalendarEntry() { DateFrom = from, DateTo = to, Label = GetLabel(sc), Typ = 0, Scrim = sc });
                }
            }
            catch (Exception ex)
            {
                Notify.sendError(ex.Message);
            }
        }

        private string GetLabel(OffDay od)
        {
            string stitle = "";
            if (od.Title.Length > 30)
            {
                stitle = od.Title.Substring(0, 30) + "...";
            }
            else
            {
                stitle = od.Title;
            }

            if (od.Typ == 1)
            {
                return Globals.UserIdToName(od.UserId) + " | entire day";
            }

            string stringStartTime = "";
            string stringEndTime = "";
            try
            {
                stringStartTime = od.Start.Split(' ')[1].Replace(":00", "");
                stringEndTime = od.End.Split(' ')[1].Replace(":00", "");
            }
            catch (Exception ex)
            {
                Notify.sendError(ex.Message);
            }

            string stringStartDay = "";
            string stringEndDay = "";
            if (od.Start.Split(' ')[0].Trim().ToLower() != od.End.Split(' ')[0].Trim().ToLower())
            {
                DateTime from = DateTime.ParseExact(od.Start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                DateTime to = DateTime.ParseExact(od.End, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                stringStartDay = from.ToString("ddd") + " ";
                stringEndDay = to.ToString("ddd") + " ";
            }


            //return Globals.UserIdToName(od.user_id.GetValueOrDefault()) + " | " + stitle + ": " + sstart + "-" + send;
            return Globals.UserIdToName(od.UserId) + " | " + stringStartDay + stringStartTime + " - " + stringEndDay + stringEndTime;

        }

        private string GetLabel(Scrim sc)
        {
            string stitle = "";
            if (sc.title.Length > 30)
            {
                stitle = sc.title.Substring(0, 30) + "...";
            }
            else
            {
                stitle = sc.title;
            }

            string sstart = "";
            string send = "";
            try
            {
                sstart = sc.time_start.Split(' ')[1].Replace(":00", "");
                send = sc.time_end.Split(' ')[1].Replace(":00", "");
            }
            catch (Exception ex)
            {
                Notify.sendError(ex.Message);
            }
            //return "Scrim: " + sc.opponent_name + " | " + stitle + ": " + sstart + "-" + send;
            return Globals.EventTypes[sc.event_type].name + ": " + sc.opponent_name + " | " + sstart + "-" + send;

        }
        #endregion

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Events != null)
            {
                var sevents = Events.Where(x => x.Typ == 0);

                foreach (var _event in sevents)
                {
                    _event.Visible = ScrimChecks.IsChecked.GetValueOrDefault(true);
                }

                var odevents = Events.Where(x => x.Typ == 1);

                foreach (var _event in odevents)
                {
                    _event.Visible = OffDayChecks.IsChecked.GetValueOrDefault(true);
                }

                CalendarMonthUI.DrawDays();
            }

        }

    }
}
