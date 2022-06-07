using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Window = xstrat.Core.Window;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for ScrimView.xaml
    /// </summary>
    public partial class ScrimView : UserControl, INotifyPropertyChanged
    {
        public TimeSpan ScrimDuration { get; set; }

        public int ScrimStartMin { get; set; }
        public int ScrimStartHour { get; set; }
        public int ScrimEndMin { get; set; }
        public int ScrimEndHour { get; set; }



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

        public ScrimView()
        {
            InitializeComponent();
            // set date of first example event to +- middle of month
            DateTime startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 15);

            // add example events
            Events = new List<ICalendarEvent>();
            //Events.Add(new CalendarEntry() { DateFrom = DateTime.Now.AddDays(6), DateTo = DateTime.Now.AddDays(6).AddHours(1), Label = "Scrim", typ = 0 });
            RetrieveOffDays();
            RetrieveScrims();
            // draw days with events calendar
            CalendarMonthUI.DrawDays();

            // subscribe to double cliked event
            CalendarMonthUI.CalendarEventDoubleClickedEvent += Calendar_CalendarEventDoubleClickedEvent;
        }


        private void Calendar_CalendarEventDoubleClickedEvent(object sender, CalendarEventView e)
        {
            if (e.DataContext is ICalendarEvent calendarEvent)
            {
                if (calendarEvent.typ == 0) //scrim
                {
                    var responseWindow = new ScrimWindow(calendarEvent.scrim);
                    responseWindow.Show();
                    responseWindow.Closing += ResponseWindow_Closing;
                }

                if (calendarEvent.typ == 1) //offday
                {
                    var responseWindow = new CalendarEventInfo(calendarEvent as CalendarEntry);
                    responseWindow.Show();
                    responseWindow.Closing += ResponseWindow_Closing;
                }

                if (calendarEvent.typ == 2) //recommendation
                {
                    var responseWindow = new ScrimWindow(calendarEvent.args.First() as Core.Window);
                    responseWindow.Show();
                    responseWindow.Closing += ResponseWindow_Closing;
                }
            }
        }

        public void UpdateScrimList()
        {
            if(scrims != null && scrims.Count > 0)
            {
                ScrimListPanel.Children.Clear();
                foreach (var scrim in scrims)
                {
                    DateTime enddate = DateTime.ParseExact(scrim.time_end, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    if (enddate >= DateTime.Now)
                    {
                        ScrimListPanel.Children.Add(new ScrimControl(scrim));
                    }
                }
            }
        }

        private void ResponseWindow_Closing(object sender, CancelEventArgs e)
        {
            RetrieveScrims();
            CalendarMonthUI.DrawDays();
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
                        Notify.sendError("Events could not be created");
                        throw new Exception("Events could not be created");
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

        public async void RetrieveScrims()
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
                        Events = Events.Where(x => x.typ != 0).ToList();
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
                Notify.sendError( ex.Message);
            }

            foreach (var sc in scrims)
            {
                MakeCalendarEntry(sc);
            }
            CalendarMonthUI.DrawDays();
            UpdateScrimList();
        }

        #region Helper Methods
        private void MakeCalendarEntry(OffDay od)
        {
            try
            {
                DateTime? from = null;
                DateTime? to = null;
                if (od.typ == 0) // exact
                {
                    from = DateTime.ParseExact(od.start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    to = DateTime.ParseExact(od.end, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    if (from != null && to != null)
                    {
                        Events.Add(new CalendarEntry() { DateFrom = from, DateTo = to, Label = GetLabel(od), typ = 1, user = Globals.getUserFromId(od.user_id.GetValueOrDefault()) });
                    }
                }
                else if (od.typ == 1) //entire day
                {
                    string sfrom = od.start.Split(' ').First() + " 00:00:00";
                    string sto = od.end.Split(' ').First() + " 23:59:59";
                    from = DateTime.ParseExact(od.start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    to = DateTime.ParseExact(od.end, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    if (from != null && to != null)
                    {
                        Events.Add(new CalendarEntry() { DateFrom = from, DateTo = to, Label = GetLabel(od), typ = 1, user = Globals.getUserFromId(od.user_id.GetValueOrDefault()) });
                    }
                }
                else if (od.typ == 2) //weekly
                {
                    for (int i = 0; i < 24; i++)
                    {
                        from = DateTime.ParseExact(od.start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).AddDays(7 * i);
                        to = DateTime.ParseExact(od.end, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).AddDays(7 * i);
                        if (from != null && to != null)
                        {
                            Events.Add(new CalendarEntry() { DateFrom = from, DateTo = to, Label = GetLabel(od), typ = 1, user = Globals.getUserFromId(od.user_id.GetValueOrDefault()) });
                        }
                    }
                }
                else if (od.typ == 3) // every second week
                {
                    for (int i = 0; i < 12; i++)
                    {
                        from = DateTime.ParseExact(od.start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).AddDays(14 * i);
                        to = DateTime.ParseExact(od.end, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).AddDays(14 * i);
                        if (from != null && to != null)
                        {
                            Events.Add(new CalendarEntry() { DateFrom = from, DateTo = to, Label = GetLabel(od), typ = 1, user = Globals.getUserFromId(od.user_id.GetValueOrDefault()) });
                        }
                    }
                }
                else if (od.typ == 4) // monthly
                {
                    for (int i = 0; i < 6; i++)
                    {
                        from = DateTime.ParseExact(od.start, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).AddMonths(i);
                        to = DateTime.ParseExact(od.end, "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).AddMonths(i);
                        if (from != null && to != null)
                        {
                            Events.Add(new CalendarEntry() { DateFrom = from, DateTo = to, Label = GetLabel(od), typ = 1, user = Globals.getUserFromId(od.user_id.GetValueOrDefault()) });
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
                    Events.Add(new CalendarEntry() { DateFrom = from, DateTo = to, Label = GetLabel(sc), typ = 0, scrim = sc });
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
            if (od.title.Length > 30)
            {
                stitle = od.title.Substring(0, 30) + "...";
            }
            else
            {
                stitle = od.title;
            }

            string sstart = "";
            string send = "";
            try
            {
                sstart = od.start.Split(' ')[1].Replace(":00", "");
                send = od.end.Split(' ')[1].Replace(":00", "");
            }
            catch (Exception ex)
            {
                Notify.sendError(ex.Message);
            }
            //return Globals.UserIdToName(od.user_id.GetValueOrDefault()) + " | " + stitle + ": " + sstart + "-" + send;
            return Globals.UserIdToName(od.user_id.GetValueOrDefault()) + " | " + sstart + "-" + send;

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
            return "Scrim: " + sc.opponent_name + " | " + sstart + "-" + send;

        }

        #endregion

        public void SearchButtonClicked()
        {
            StartSearch();
        }

        #region TimeFinder


        private void StartSearch()
        {
            CalendarFilterType calendarFilterType = SFControl.FilterType;
            int playeramount = SFControl.PlayerCount;
            List<User> users = SFControl.Users;
            List<int> SelectedPlayerNumbers = new List<int>();
            foreach (var player in users)
            {
                SelectedPlayerNumbers.Add(player.id);
            }

            ScrimDuration = new TimeSpan(SFControl.DurHour.Value, SFControl.DurMinute.Value, 0);

            ScrimStartHour = SFControl.FromHour.Value;
            ScrimStartMin = SFControl.FromMinute.Value;
            ScrimEndHour = SFControl.ToHour.Value;
            ScrimEndMin = SFControl.ToMinute.Value;


            Events = Events.Where(x => x.typ != 2).ToList();
            List<DateTime> dates = new List<DateTime>();

            var now = DateTime.Now;

            for (int i = 0; i < 31; i++)
            {
                dates.Add(now.AddDays(i));
            }

            //string results = "";
            var players = new List<Player>();

            foreach (var date in dates)
            {
                foreach (var user in Globals.teammates)
                {
                    var newPlayer = new Player();
                    newPlayer.Responses = GetTimespans(date, user.name, SelectedPlayerNumbers, calendarFilterType.id);
                    newPlayer.ID = user.id;
                    players.Add(newPlayer);
                }
            }

            var windows = GetMeetingWindows(players, TimeSpan.FromMinutes(60));
            foreach (var window in windows)
            {
                if(calendarFilterType.id == 0) //min
                {
                    if (window.AvailablePlayers.Count() >= playeramount)
                    {
                        //results += String.Format("Start: {0:yyyy-MM-dd HH:mm}, End: {1:yyyy-MM-dd HH:mm}, Player count: {2}", window.StartDateTime, window.EndDateTime, window.AvailableAttendees.Count()) + "\n";
                        List<Object> newargs = new List<Object>();
                        newargs.Add(window);
                        Events.Add(new CalendarEntry() { DateFrom = window.StartDateTime, DateTo = window.EndDateTime, Label = window.StartDateTime.ToString("HH:mm") + "-" + window.EndDateTime.ToString("HH:mm") + " | " + window.AvailablePlayers.Count(), typ = 2, args = newargs });
                    }
                }

                if (calendarFilterType.id == 1) //specific
                {
                    List<int> AvailablePlayerNumbers = new List<int>();
                    foreach (var player in window.AvailablePlayers)
                    {
                        AvailablePlayerNumbers.Add(player.ID);
                    }

                    if (SelectedPlayerNumbers.All(i => AvailablePlayerNumbers.Contains(i)))
                    {
                        window.AvailablePlayers = window.AvailablePlayers.Where(x => SelectedPlayerNumbers.Contains(x.ID));
                        List<Object> newargs = new List<Object>();
                        newargs.Add(window);
                        Events.Add(new CalendarEntry() { DateFrom = window.StartDateTime, DateTo = window.EndDateTime, Label = window.StartDateTime.ToString("HH:mm") + "-" + window.EndDateTime.ToString("HH:mm") + " | " + window.AvailablePlayers.Count(), typ = 2, args = newargs });
                    }
                }

                if (calendarFilterType.id == 2) //specific min
                {
                    List<int> AvailablePlayerNumbers = new List<int>();
                    foreach (var player in window.AvailablePlayers)
                    {
                        AvailablePlayerNumbers.Add(player.ID);
                    }

                    bool hasMinPlayers = true;

                    if(SelectedPlayerNumbers.Where(x => !AvailablePlayerNumbers.Contains(x)).Any())
                    {
                        hasMinPlayers = false;
                    };

                    if (hasMinPlayers)
                    {
                        List<Object> newargs = new List<Object>();
                        newargs.Add(window);
                        Events.Add(new CalendarEntry() { DateFrom = window.StartDateTime, DateTo = window.EndDateTime, Label = window.StartDateTime.ToString("HH:mm") + "-" + window.EndDateTime.ToString("HH:mm") + " | " + window.AvailablePlayers.Count(), typ = 2, args = newargs });
                    }
                }

                if (calendarFilterType.id == 3) //all
                {
                    if (window.AvailablePlayers.Count() >= Globals.teammates.Count)
                    {
                        List<Object> newargs = new List<Object>();
                        newargs.Add(window);
                        Events.Add(new CalendarEntry() { DateFrom = window.StartDateTime, DateTo = window.EndDateTime, Label = window.StartDateTime.ToString("HH:mm") + "-" + window.EndDateTime.ToString("HH:mm") + " | " + window.AvailablePlayers.Count(), typ = 2, args = newargs });
                    }
                }

                //if (window.AvailablePlayers.Count() >= 5)
                //{
                //    List<Object> newargs = new List<Object>();
                //    newargs.Add(window);
                //    Events.Add(new CalendarEntry() { DateFrom = window.StartDateTime, DateTo = window.EndDateTime, Label = window.StartDateTime.ToString("HH:mm") + "-" + window.EndDateTime.ToString("HH:mm") + " | " + window.AvailablePlayers.Count(), typ = 2 , args = newargs });
                //}
            }
            CalendarMonthUI.DrawDays();
            //MessageBox.Show(results.ToString());
        }

        private List<Response> GetTimespans(DateTime date, string user_name, List<int> SelectedPlayerNumbers, int filtertype)
        {
            List<Response> timespans = new List<Response>();

            int day = (int)date.DayOfWeek;

            if ( filtertype == 1 &&  !SelectedPlayerNumbers.Contains(Globals.getUserIdFromName(user_name)))
            {
                return timespans;
            }

            if(day == 0)
            {
                if (!SFControl.subtn)
                {
                    return timespans;
                }
            }
            if(day == 1)
            {
                if (!SFControl.mobtn)
                {
                    return timespans;
                }
            }
            if (day == 2)
            {
                if (!SFControl.tubtn)
                {
                    return timespans;
                }
            }
            if (day == 3)
            {
                if (!SFControl.webtn)
                {
                    return timespans;
                }
            }
            if (day == 4)
            {
                if (!SFControl.thbtn)
                {
                    return timespans;
                }
            }
            if (day == 5)
            {
                if (!SFControl.frbtn)
                {
                    return timespans;
                }
            }
            if (day == 6)
            {
                if (!SFControl.sabtn)
                {
                    return timespans;
                }
            }


            var events = _events.Where(x => x.typ == 1 && x.Label.StartsWith(user_name)).Where(x => x.DateFrom.GetValueOrDefault().ToString("yyyy/MM/dd") == date.ToString("yyyy/MM/dd") && x.DateTo.GetValueOrDefault().ToString("yyyy/MM/dd") == date.ToString("yyyy/MM/dd")); //all events of given date

            List<DateTime> times = new List<DateTime>();

            var sDate = date.ToString("yyyy/MM/dd");

            var ScrimStartTime = DateTime.ParseExact((sDate + " " + ScrimStartHour.ToString().PadLeft(2, '0') + ":" + ScrimStartMin.ToString().PadLeft(2, '0') + ":00"), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            var ScrimEndTime = DateTime.ParseExact((sDate + " " + ScrimEndHour.ToString().PadLeft(2, '0') + ":" + ScrimEndMin.ToString().PadLeft(2, '0') + ":00"), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            times.Add(DateTime.ParseExact( (sDate + " 00:00:00"), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
            times.Add(DateTime.ParseExact((sDate + " 23:59:59"), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));

            foreach (var ev in events)
            {
                times.Add(ev.DateFrom.GetValueOrDefault());
                times.Add(ev.DateTo.GetValueOrDefault());
            }

            times.Sort();

            bool free = true;

            for (int i = 0; i < times.Count - 1; i++)
            {
                if (free)
                {
                    var t1 = times[i];
                    var t2 = times[i + 1];

                    if(t1 < ScrimStartTime)
                    {
                        t1 = ScrimStartTime;
                    }
                    if(t2 > ScrimEndTime)
                    {
                        t2 = ScrimEndTime;
                    }

                    if (ScrimDuration <= t2 - t1) //mehr als scrim zeit
                    {
                        timespans.Add(new Response { StartDateTime = t1, EndDateTime = t2 });
                    }
                }
                free = !free;
                
            }

            return timespans;
        }

       

        public IEnumerable<Window> GetMeetingWindows(IEnumerable<Player> players, TimeSpan meetingDuration)
        {
            var windows = new List<Window>();
            var responses = players.SelectMany(x => x.Responses).Where(x => x.EndDateTime - x.StartDateTime >= meetingDuration);

            foreach (var time in (responses.Select(x => x.StartDateTime)).Distinct())
            {
                var matches = players.Select(x => new {
                    Attendee = x,
                    MatchingAvailabilities = x.Responses.Where(y => y.StartDateTime <= time && y.EndDateTime >= time.Add(meetingDuration))
                });

                windows.Add(new Window
                {
                    StartDateTime = time,
                    EndDateTime = matches.SelectMany(x => x.MatchingAvailabilities).Min(x => x.EndDateTime),
                    AvailablePlayers = matches.Where(y => y.MatchingAvailabilities.Any()).Select(x => x.Attendee)
                });
            }

            foreach (var time in (responses.Select(x => x.EndDateTime)).Distinct())
            {
                var matches = players.Select(x => new {
                    Attendee = x,
                    MatchingAvailabilities = x.Responses.Where(y => y.EndDateTime >= time && y.StartDateTime <= time.Add(-meetingDuration))
                });

                windows.Add(new Window
                {
                    EndDateTime = time,
                    StartDateTime = matches.SelectMany(x => x.MatchingAvailabilities).Max(x => x.StartDateTime),
                    AvailablePlayers = matches.Where(y => y.MatchingAvailabilities.Any()).Select(x => x.Attendee)
                });
            }

            return windows.GroupBy(x => new { x.StartDateTime, x.EndDateTime }).Select(x => x.First()).OrderBy(x => x.StartDateTime).ThenBy(x => x.EndDateTime);
        }



        #endregion

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.Window window = new Core.Window
            {
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddHours(3),
                AvailablePlayers = new List<Player>()
            };
            var response = new ScrimWindow(window);
            response.Show();
            response.Closed += Response_Closed;
        }

        private void Response_Closed(object sender, EventArgs e)
        {
            RetrieveScrims();
            UpdateScrimList();
        }

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            RetrieveScrims();
            UpdateScrimList();
        }
    }
}
