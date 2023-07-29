using MaterialDesignThemes.Wpf;
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
using xstrat.Models.Supabase;
using xstrat.MVVM.View;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for ScrimControl.xaml
    /// </summary>
    public partial class ScrimControl : UserControl
    {
        private SolidColorBrush _normalBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#303030");
        private SolidColorBrush _acceptedBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#3b9347");
        private SolidColorBrush _deniedBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#ab3440");
        private SolidColorBrush _normalBorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#262626");
        private SolidColorBrush _acceptedBorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#2F7538");
        private SolidColorBrush _deniedBorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#882933");

        public CalendarEvent Scrim { get; set; }

        /// <summary>
        /// 0 neutral
        /// 1 accept
        /// 2 deny
        /// </summary>
        public int Status = 0;

        public ScrimControl()
        {
            InitializeComponent();
        }

        public ScrimControl(CalendarEvent scrim)
        {
            InitializeComponent();
            LoadScrim(scrim);
        }
        
        public void LoadScrim(CalendarEvent scrim)
        {
            this.Scrim = scrim;
            if(Scrim != null)
            {
                EventTypeLabel.Content = Globals.EventTypes[scrim.EventType].name + ":";
                TxtTitle.Content = Scrim.Title;
                TxtStart.Content = Scrim.Start.ToString().Split(' ')[1];
                TxtEnd.Content = Scrim.End.ToString().Split(' ')[1];
                TxtDate.Content = Scrim.Start.ToString().Split(' ')[0]; ;
                TxtTimeLeft.Content = TimeLeft();
                TxtMode.Content = Globals.ScrimModes.Where(x => x.id == Scrim.Typ).FirstOrDefault().name;
                if(Scrim.Map3Id.IsNotNullOrEmpty())
                {
                    TxtMap3.Content = DataCache.CurrentMaps.Where(x => x.Id == Scrim.Map3Id).FirstOrDefault().Name;
                }
                if (Scrim.Map2Id.IsNotNullOrEmpty())
                {
                    TxtMap2.Content = DataCache.CurrentMaps.Where(x => x.Id == Scrim.Map2Id).FirstOrDefault().Name;
                }
                if (Scrim.Map1Id.IsNotNullOrEmpty())
                {
                    TxtMap1.Content = DataCache.CurrentMaps.Where(x => x.Id == Scrim.Map1Id).FirstOrDefault().Name;
                }
                TxtEnemyName.Content = "Against: " + Scrim.OpponentName;
                TxtDescription.Text = Scrim.Comment;

                ParticipantsSP.Children.Clear();

                //TODO UserList
                //string acc_users = Scrim.acc_user_list;
                //if(acc_users != null && acc_users != "")
                //{
                //    var ulist = acc_users.Split(';');
                //    foreach (var item in ulist)
                //    {
                //        string uid = null;
                //        try
                //        {
                //            uid = item.ToString();
                //        }
                //        catch
                //        {
                //            continue;
                //        }
                //        if (uid.IsNullOrEmpty()) continue;

                //        UserData user = DataCache.CurrentTeamMates.Where(x => x.Id == uid).FirstOrDefault();
                //        if (user == null) continue;
                //        var control = new ScrimParticipationControl(1, user.Name);
                //        control.Margin = new Thickness(5, 0, 0, 0);
                //        ParticipantsSP.Children.Add(control);
                //    }
                //}


                //string deny_users = Scrim.deny_user_list;
                //if (deny_users != null && deny_users != "")
                //{
                //    var ulist = deny_users.Split(';');
                //    foreach (var item in ulist)
                //    {
                //        string uid = null;
                //        try
                //        {
                //            uid = item.ToString();
                //        }
                //        catch
                //        {
                //            continue;
                //        }
                //        if (uid.IsNullOrEmpty()) continue;

                //        UserData user = DataCache.CurrentTeamMates.Where(x => x.Id == uid).FirstOrDefault();
                //        if (user == null) continue;
                //        var control = new ScrimParticipationControl(2, user.Name);
                //        control.Margin = new Thickness(5, 0, 0, 0);
                //        ParticipantsSP.Children.Add(control);
                //    }
                //}


                //string ign_users = Scrim.ign_user_list;
                //if (ign_users != null && ign_users != "")
                //{
                //    var ulist = ign_users.Split(';');
                //    foreach (var item in ulist)
                //    {
                //        string uid = null;
                //        try
                //        {
                //            uid = item.ToString();
                //        }
                //        catch
                //        {
                //            continue;
                //        }
                //        if (uid.IsNullOrEmpty()) continue;

                //        UserData user = DataCache.CurrentTeamMates.Where(x => x.Id == uid).FirstOrDefault();
                //        if (user == null) continue;
                //        var control = new ScrimParticipationControl(0, user.Name);
                //        control.Margin = new Thickness(5, 0, 0, 0);
                //        ParticipantsSP.Children.Add(control);
                //    }
                //}

            }
            //TODO: Fix
            //Status = scrim.response_typ.GetValueOrDefault(0);
            StatusChanged();
        }

        private void AcceptBtn_Click(object sender, RoutedEventArgs e)
        {
            if(Status == 1)
            {
                Status = 0;
            }
            else
            {
                Status = 1;
            }
            StatusChanged();
        }

        private void DenyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Status == 2)
            {
                Status = 0;
            }
            else
            {
                Status = 2;
            }
            StatusChanged();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var responseWindow = new ScrimWindow(Scrim);
            responseWindow.Show();
            responseWindow.Closed += ResponseWindow_Closed;            
        }

        private void ResponseWindow_Closed(object sender, EventArgs e)
        {
            DependencyObject ucParent = this.Parent;
            if (ucParent != null)
            {
                while (!(ucParent is UserControl))
                {
                    ucParent = LogicalTreeHelper.GetParent(ucParent);
                }
                var sv = ucParent as ScrimView;
                sv.RetrieveScrims();
                sv.UpdateScrimList();
            }
        }

        private async void StatusChanged()
        {
            switch (Status)
            {
                case 0:
                    MainBorder.Background = _normalBrush;
                    ContentBorder1.Background = _normalBorderBrush;
                    ContentBorder2.Background = _normalBorderBrush;
                    ContentBorder3.Background = _normalBorderBrush;
                    ContentBorder4.Background = _normalBorderBrush;
                    ContentBorder5.Background = _normalBorderBrush;
                    ContentBorder6.Background = _normalBorderBrush;
                    break;
                case 1:
                    MainBorder.Background = _acceptedBrush;
                    ContentBorder1.Background = _acceptedBorderBrush;
                    ContentBorder2.Background = _acceptedBorderBrush;
                    ContentBorder3.Background = _acceptedBorderBrush;
                    ContentBorder4.Background = _acceptedBorderBrush;
                    ContentBorder5.Background = _acceptedBorderBrush;
                    ContentBorder6.Background = _acceptedBorderBrush;
                    break;
                case 2:
                    MainBorder.Background = _deniedBrush;
                    ContentBorder1.Background = _deniedBorderBrush;
                    ContentBorder2.Background = _deniedBorderBrush;
                    ContentBorder3.Background = _deniedBorderBrush;
                    ContentBorder4.Background = _deniedBorderBrush;
                    ContentBorder5.Background = _deniedBorderBrush;
                    ContentBorder6.Background = _deniedBorderBrush;
                    break;
                default:
                    break;
            }
            var result = await ApiHandler.SetScrimResponse(Scrim.Id, Status);
            if (result)
            {
                Logger.Log("Set Response");
            }
            else
            {
                Notify.sendError("Scrim status could not be loaded");
                throw new Exception("Scrim status could not be loaded");
            }

        }
        private string TimeLeft()
        {
            string result = string.Empty;
            var left = (Scrim.Start ?? DateTime.Now) - DateTime.Now;
            if (left.Days > 0) result += left.Days + "D ";
            if (left.Hours > 0) result += left.Hours + "H ";
            if (left.Minutes > 0) result += left.Minutes + "M";



            return result;
        }
    }
}
