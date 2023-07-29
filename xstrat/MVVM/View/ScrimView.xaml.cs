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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using xstrat.Calendar;
using xstrat.Core;
using xstrat.Json;
using xstrat.Models.Supabase;
using xstrat.Ui;
using UserControl = System.Windows.Controls.UserControl;
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



        private List<CalendarBlock> CalendarBlocks = new List<CalendarBlock>();
        private List<CalendarEvent> scrims = new List<CalendarEvent>();
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
            Loaded += ScrimView_Loaded;
        }

        private void ScrimView_Loaded(object sender, RoutedEventArgs e)
        {
            // set date of first example event to +- middle of month
            DateTime startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 15);

            // add example events
            Events = new List<ICalendarEvent>();
            //Events.Add(new CalendarEntry() { DateFrom = DateTime.Now.AddDays(6), DateTo = DateTime.Now.AddDays(6).AddHours(1), Label = "Scrim", typ = 0 });
            RetrieveCalendarBlocks();
            RetrieveScrims();
            // draw days with events calendar
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

                if (calendarEvent.Typ == 1) //CalendarBlock
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

        public void UpdateScrimList()
        {
            if (scrims != null && scrims.Count > 0)
            {
                ScrimListPanel.Children.Clear();
                foreach (var scrim in scrims)
                {
                    if (scrim.End > DateTime.Now)
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
        private async void RetrieveCalendarBlocks()
        {
            CalendarBlocks = DataCache.CurrentCalendarBlocks;

            foreach (var od in CalendarBlocks)
            {
                MakeCalendarEntry(od);
            }

            await Task.Delay(100);

            CalendarMonthUI.DrawDays();

        }

        public async void RetrieveScrims()
        {
            scrims = DataCache.CurrentCalendarEvents;

            foreach (var sc in scrims)
            {
                MakeCalendarEntry(sc);
            }
            CalendarMonthUI.DrawDays();
            UpdateScrimList();
        }

        #region Helper Methods
        private void MakeCalendarEntry(CalendarBlock od)
        {
            try
            {
                DateTime? from = null;
                DateTime? to = null;
                if (od.Typ == 0) // exact
                {
                    if (from != null && to != null)
                    {
                        Events.Add(new CalendarEntry() { DateFrom = od.Start, DateTo = od.End, Label = GetLabel(od), Typ = 1, User = DataCache.CurrentTeamMates.Where(x => x.Id == od.UserId).FirstOrDefault() });
                    }
                }
                else if (od.Typ == 1) //entire day
                {
                    if (from != null && to != null)
                    {
                        Events.Add(new CalendarEntry() { DateFrom = od.Start.GetValueOrDefault().SetTime(0, 0, 0), DateTo = od.End.GetValueOrDefault().SetTime(23, 59, 59), Label = GetLabel(od), Typ = 1, User = DataCache.CurrentTeamMates.Where(x => x.Id == od.UserId).FirstOrDefault() });
                    }
                }
                else if (od.Typ == 2) //weekly
                {
                    int offset = (int)(od.Start.GetValueOrDefault() - DateTime.Now.Date).TotalDays / 7;
                    if (offset < 0) offset = 0;

                    for (int i = 0; i < offset + 24; i++)
                    {
                        Events.Add(new CalendarEntry() { DateFrom = od.Start.GetValueOrDefault().AddDays(7 * i), DateTo = od.End.GetValueOrDefault().AddDays(7 * i), Label = GetLabel(od), Typ = 1, User = DataCache.CurrentTeamMates.Where(x => x.Id == od.UserId).FirstOrDefault() });
                    }
                }
                else if (od.Typ == 3) // every second week
                {
                    int offset = (int)(od.Start.GetValueOrDefault() - DateTime.Now.Date).TotalDays / 14;
                    if (offset < 0) offset = 0;

                    for (int i = 0; i < offset + 12; i++)
                    {
                        Events.Add(new CalendarEntry() { DateFrom = od.Start.GetValueOrDefault().AddDays(14 * i), DateTo = od.End.GetValueOrDefault().AddDays(14 * i), Label = GetLabel(od), Typ = 1, User = DataCache.CurrentTeamMates.Where(x => x.Id == od.UserId).FirstOrDefault() });
                    }
                }
                else if (od.Typ == 4) // monthly
                {
                    int offset = (int)(od.Start.GetValueOrDefault() - DateTime.Now.Date).TotalDays / 30;
                    if (offset < 0) offset = 0;

                    for (int i = 0; i < offset + 6; i++)
                    {
                        Events.Add(new CalendarEntry() { DateFrom = od.Start.GetValueOrDefault().AddMonths(i), DateTo = od.End.GetValueOrDefault().AddMonths(i), Label = GetLabel(od), Typ = 1, User = DataCache.CurrentTeamMates.Where(x => x.Id == od.UserId).FirstOrDefault() });
                    }
                }
                else if (od.Typ == 5) // daily
                {
                    int offset = (int)(od.Start.GetValueOrDefault() - DateTime.Now.Date).TotalDays;
                    if (offset < 0) offset = 0;

                    for (int i = 0; i < offset + 365; i++)
                    {
                        Events.Add(new CalendarEntry() { DateFrom = od.Start.GetValueOrDefault().AddDays(i), DateTo = od.End.GetValueOrDefault().AddDays(i), Label = GetLabel(od), Typ = 1, User = DataCache.CurrentTeamMates.Where(x => x.Id == od.UserId).FirstOrDefault() });
                    }
                }

            }
            catch (Exception ex)
            {
                Notify.sendError(ex.Message);
            }
        }

        private void MakeCalendarEntry(CalendarEvent sc)
        {
            try
            {
                if (sc.Start != null && sc.End != null)
                {
                    Events.Add(new CalendarEntry() { DateFrom = sc.Start, DateTo = sc.End, Label = GetLabel(sc), Typ = 0, Scrim = sc });
                }
            }
            catch (Exception ex)
            {
                Notify.sendError(ex.Message);
            }
        }

        private string GetLabel(CalendarBlock od)
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
                stringStartTime = od.Start.ToString().Split(' ')[1].Replace(":00", "");
                stringEndTime = od.End.ToString().Split(' ')[1].Replace(":00", "");
            }
            catch (Exception ex)
            {
                Notify.sendError(ex.Message);
            }

            string stringStartDay = "";
            string stringEndDay = "";
            if (od.Start.ToString().Split(' ')[0].Trim().ToLower() != od.End.ToString().Split(' ')[0].Trim().ToLower())
            {
                DateTime from = DateTime.ParseExact(od.Start.ToString(), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                DateTime to = DateTime.ParseExact(od.End.ToString(), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                stringStartDay = from.ToString("ddd") + " ";
                stringEndDay = to.ToString("ddd") + " ";
            }


            //return Globals.UserIdToName(od.user_id.GetValueOrDefault()) + " | " + stitle + ": " + sstart + "-" + send;
            return Globals.UserIdToName(od.UserId) + " | " + stringStartDay + stringStartTime + " - " + stringEndDay + stringEndTime;

        }

        private string GetLabel(CalendarEvent sc)
        {
            string stitle = "";
            if (sc.Title.Length > 30)
            {
                stitle = sc.Title.Substring(0, 30) + "...";
            }
            else
            {
                stitle = sc.Title;
            }

            string sstart = "";
            string send = "";
            try
            {
                sstart = sc.Start.ToString().Split(' ')[1].Replace(":00", "");
                send = sc.End.ToString().Split(' ')[1].Replace(":00", "");
            }
            catch (Exception ex)
            {
                Notify.sendError(ex.Message);
            }
            //return "Scrim: " + sc.opponent_name + " | " + stitle + ": " + sstart + "-" + send;
            return Globals.EventTypes[sc.Typ].name + ": " + sc.OpponentName + " | " + sstart + "-" + send;

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
            List<UserData> users = SFControl.Users;
            List<string> SelectedPlayerIDs = new List<string>();
            foreach (var player in users)
            {
                SelectedPlayerIDs.Add(player.Id);
            }

            ScrimDuration = new TimeSpan(SFControl.DurHour.Value, SFControl.DurMinute.Value, 0);

            ScrimStartHour = SFControl.FromHour.Value;
            ScrimStartMin = SFControl.FromMinute.Value;
            ScrimEndHour = SFControl.ToHour.Value;
            ScrimEndMin = SFControl.ToMinute.Value;


            Events = Events.Where(x => x.Typ != 2).ToList();
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
                foreach (var user in DataCache.CurrentTeamMates)
                {
                    var newPlayer = new Player();
                    newPlayer.Responses = GetTimespans(date, user.Name, SelectedPlayerIDs, calendarFilterType.id, ScrimDuration);
                    newPlayer.Id = user.Id;
                    players.Add(newPlayer);
                }
            }

            var windows = GetMeetingWindows(players, TimeSpan.FromMinutes(60));
            foreach (var window in windows)
            {
                if (calendarFilterType.id == 0) //min
                {
                    if (window.AvailablePlayers.Count() >= playeramount)
                    {
                        //results += String.Format("Start: {0:yyyy-MM-dd HH:mm}, End: {1:yyyy-MM-dd HH:mm}, Player count: {2}", window.StartDateTime, window.EndDateTime, window.AvailableAttendees.Count()) + "\n";
                        List<Object> newargs = new List<Object>();
                        newargs.Add(window);
                        Events.Add(new CalendarEntry() { DateFrom = window.StartDateTime, DateTo = window.EndDateTime, Label = window.StartDateTime.ToString("HH:mm") + "-" + window.EndDateTime.ToString("HH:mm") + " | " + window.AvailablePlayers.Count(), Typ = 2, Args = newargs });
                    }
                }

                if (calendarFilterType.id == 1) //specific
                {
                    List<string> AvailablePlayerIDs = new List<string>();
                    foreach (var player in window.AvailablePlayers)
                    {
                        AvailablePlayerIDs.Add(player.Id);
                    }

                    if (SelectedPlayerIDs.All(i => AvailablePlayerIDs.Contains(i)))
                    {
                        window.AvailablePlayers = window.AvailablePlayers.Where(x => SelectedPlayerIDs.Contains(x.Id));
                        List<Object> newargs = new List<Object>();
                        newargs.Add(window);
                        Events.Add(new CalendarEntry() { DateFrom = window.StartDateTime, DateTo = window.EndDateTime, Label = window.StartDateTime.ToString("HH:mm") + "-" + window.EndDateTime.ToString("HH:mm") + " | " + window.AvailablePlayers.Count(), Typ = 2, Args = newargs });
                    }
                }

                if (calendarFilterType.id == 2) //specific min
                {
                    List<string> AvailablePlayerIDs = new List<string>();
                    foreach (var player in window.AvailablePlayers)
                    {
                        AvailablePlayerIDs.Add(player.Id);
                    }

                    bool hasMinPlayers = true;

                    if (SelectedPlayerIDs.Where(x => !AvailablePlayerIDs.Contains(x)).Any())
                    {
                        hasMinPlayers = false;
                    };

                    if (hasMinPlayers)
                    {
                        List<Object> newargs = new List<Object>();
                        newargs.Add(window);
                        Events.Add(new CalendarEntry() { DateFrom = window.StartDateTime, DateTo = window.EndDateTime, Label = window.StartDateTime.ToString("HH:mm") + "-" + window.EndDateTime.ToString("HH:mm") + " | " + window.AvailablePlayers.Count(), Typ = 2, Args = newargs });
                    }
                }

                if (calendarFilterType.id == 3) //all
                {
                    if (window.AvailablePlayers.Count() >= DataCache.CurrentTeamMates.Count)
                    {
                        List<Object> newargs = new List<Object>();
                        newargs.Add(window);
                        Events.Add(new CalendarEntry() { DateFrom = window.StartDateTime, DateTo = window.EndDateTime, Label = window.StartDateTime.ToString("HH:mm") + "-" + window.EndDateTime.ToString("HH:mm") + " | " + window.AvailablePlayers.Count(), Typ = 2, Args = newargs });
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

        //Gets Free Timespans for day and player
        private List<Response> GetTimespans(DateTime date, string user_name, List<string> SelectedPlayerNumbers, int filtertype, TimeSpan duration)
        {
            List<Response> timespans = new List<Response>();

            int day = (int)date.DayOfWeek;

            if (filtertype == 1 && !SelectedPlayerNumbers.Contains(Globals.GetUserIdFromName(user_name)))
            {
                return timespans;
            }

            if (day == 0)
            {
                if (!SFControl.subtn)
                {
                    return timespans;
                }
            }
            if (day == 1)
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


            var events = _events.Where(x => x.Typ == 1 && x.Label.StartsWith(user_name)).Where(x => x.DateFrom.GetValueOrDefault().ToString("yyyy/MM/dd") == date.ToString("yyyy/MM/dd") && x.DateTo.GetValueOrDefault().ToString("yyyy/MM/dd") == date.ToString("yyyy/MM/dd")); //all events of given date

            List<DateTime> times = new List<DateTime>();

            var sDate = date.ToString("yyyy/MM/dd");
            sDate = sDate.Replace(".", "/").Replace("-", "/");

            var ScrimStartTime = DateTime.ParseExact((sDate + " " + ScrimStartHour.ToString().PadLeft(2, '0') + ":" + ScrimStartMin.ToString().PadLeft(2, '0') + ":00"), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            var ScrimEndTime = DateTime.ParseExact((sDate + " " + ScrimEndHour.ToString().PadLeft(2, '0') + ":" + ScrimEndMin.ToString().PadLeft(2, '0') + ":00"), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            //CalendarBlock regelung

            if (DataCache.CurrentTeam.UseOnDays == 0)
            {
                times.Add(DateTime.ParseExact((sDate + " 00:00:00"), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
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

                        if (t1 < ScrimStartTime)
                        {
                            t1 = ScrimStartTime;
                        }
                        if (t2 > ScrimEndTime)
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
            }
            else
            {
                var fittingevents = events.Where(x => x.DateTo - x.DateFrom >= duration);

                foreach (var item in fittingevents)
                {
                    timespans.Add(new Response { StartDateTime = item.DateFrom.GetValueOrDefault(), EndDateTime = item.DateTo.GetValueOrDefault() });
                }
            }

            return timespans;
        }



        public IEnumerable<Window> GetMeetingWindows(IEnumerable<Player> players, TimeSpan meetingDuration)
        {
            var windows = new List<Window>();
            var responses = players.SelectMany(x => x.Responses).Where(x => x.EndDateTime - x.StartDateTime >= meetingDuration);

            foreach (var time in (responses.Select(x => x.StartDateTime)).Distinct())
            {
                var matches = players.Select(x => new
                {
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
                var matches = players.Select(x => new
                {
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
