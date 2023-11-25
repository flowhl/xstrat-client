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
using xstrat.Models.Supabase;
using xstrat;
using xstrat.Ui;
using static xstrat.Ui.ScrimWindow;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for CalendarView.xaml
    /// </summary>
    public partial class CalendarView : StateUserControl, INotifyPropertyChanged
    {
        private List<Models.Supabase.CalendarBlock> offDays = new List<Models.Supabase.CalendarBlock>();
        private List<Models.Supabase.CalendarEvent> scrims = new List<Models.Supabase.CalendarEvent>();
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
            offDays = DataCache.CurrentCalendarBlocks;

            foreach (var od in offDays)
            {
                MakeCalendarEntry(od);
            }

            await Task.Delay(100);

            CalendarMonthUI.DrawDays();
        }

        private async void RetrieveScrims()
        {
            scrims = DataCache.CurrentCalendarEvents;

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
                        Events.Add(new CalendarEntry() { DateFrom = od.Start.GetValueOrDefault().SetTime(0,0,0), DateTo = od.End.GetValueOrDefault().SetTime(23,59,59), Label = GetLabel(od), Typ = 1, User = DataCache.CurrentTeamMates.Where(x => x.Id == od.UserId).FirstOrDefault() });
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

        private void MakeCalendarEntry(Models.Supabase.CalendarEvent scrim)
        {
            try
            {
                if (scrim.Start != null && scrim.End != null)
                {
                    Events.Add(new CalendarEntry() { DateFrom = scrim.Start, DateTo = scrim.End, Label = GetLabel(scrim), Typ = 0, Scrim = scrim });
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

            string outputTime = od.Start.GetValueOrDefault().ToString("HH:mm") + "-" + od.End.GetValueOrDefault().ToString("HH:mm");
            //return Globals.UserIdToName(od.user_id.GetValueOrDefault()) + " | " + stitle + ": " + sstart + "-" + send;
            return Globals.UserIdToName(od.UserId) + " | " + outputTime + " | " + stitle;
        }

        private string GetLabel(Models.Supabase.CalendarEvent sc)
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
            return Globals.EventTypes[sc.EventType].name + ": " + sc.OpponentName + " | " + sstart + "-" + send;

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
