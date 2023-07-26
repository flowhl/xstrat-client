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

        public Scrim Scrim { get; set; }

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

        public ScrimControl(Scrim scrim)
        {
            InitializeComponent();
            LoadScrim(scrim);
        }
        
        public void LoadScrim(Scrim scrim)
        {
            this.Scrim = scrim;
            if(Scrim != null)
            {
                EventTypeLabel.Content = Globals.EventTypes[scrim.event_type].name + ":";
                TxtTitle.Content = Scrim.title;
                TxtStart.Content = Scrim.time_start.Split(' ')[1];
                TxtEnd.Content = Scrim.time_end.Split(' ')[1];
                TxtDate.Content = Scrim.time_start.Split(' ')[0]; ;
                TxtTimeLeft.Content = TimeLeft();
                TxtMode.Content = Globals.ScrimModes.Where(x => x.id == Scrim.typ).FirstOrDefault().name;
                if(Scrim.map_3_id >= 0)
                {
                    TxtMap3.Content = Globals.Maps.Where(x => x.Id == Scrim.map_3_id).FirstOrDefault().name;
                }
                if (Scrim.map_2_id >= 0)
                {
                    TxtMap2.Content = Globals.Maps.Where(x => x.Id == Scrim.map_2_id).FirstOrDefault().name;
                }
                if (Scrim.map_1_id >= 0)
                {
                    TxtMap1.Content = Globals.Maps.Where(x => x.Id == Scrim.map_1_id).FirstOrDefault().name;
                }
                TxtEnemyName.Content = "Against: " + Scrim.opponent_name;
                TxtDescription.Text = Scrim.comment;

                ParticipantsSP.Children.Clear();

                string acc_users = Scrim.acc_user_list;
                if(acc_users != null && acc_users != "")
                {
                    var ulist = acc_users.Split(';');
                    foreach (var item in ulist)
                    {
                        int uid = -1;
                        try
                        {
                            uid = int.Parse(item);
                        }
                        catch
                        {
                            continue;
                        }
                        if (uid == -1) continue;

                        User user = Globals.getUserFromId(uid);
                        if (user == null) continue;
                        var control = new ScrimParticipationControl(1, user.name);
                        control.Margin = new Thickness(5, 0, 0, 0);
                        ParticipantsSP.Children.Add(control);
                    }
                }


                string deny_users = Scrim.deny_user_list;
                if (deny_users != null && deny_users != "")
                {
                    var ulist = deny_users.Split(';');
                    foreach (var item in ulist)
                    {
                        int uid = -1;
                        try
                        {
                            uid = int.Parse(item);
                        }
                        catch
                        {
                            continue;
                        }
                        if (uid == -1) continue;

                        User user = Globals.getUserFromId(uid);
                        if (user == null) continue;
                        var control = new ScrimParticipationControl(2, user.name);
                        control.Margin = new Thickness(5, 0, 0, 0);
                        ParticipantsSP.Children.Add(control);
                    }
                }


                string ign_users = Scrim.ign_user_list;
                if (ign_users != null && ign_users != "")
                {
                    var ulist = ign_users.Split(';');
                    foreach (var item in ulist)
                    {
                        int uid = -1;
                        try
                        {
                            uid = int.Parse(item);
                        }
                        catch
                        {
                            continue;
                        }
                        if (uid == -1) continue;


                        User user = Globals.getUserFromId(uid);
                        if (user == null) continue;
                        var control = new ScrimParticipationControl(0, user.name);
                        control.Margin = new Thickness(5, 0, 0, 0);
                        ParticipantsSP.Children.Add(control);
                    }
                }

            }
            Status = scrim.response_typ.GetValueOrDefault(0);
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
            var result = await ApiHandler.SetScrimResponse(Scrim.id, Status);
            if (result.Item1)
            {
                Logger.Log(result.Item2);
            }
            else
            {
                Notify.sendError("Scrim status could not be loaded: " + result.Item2);
                throw new Exception("Scrim status could not be loaded: " + result.Item2);
            }

        }
        private string TimeLeft()
        {
            string result = string.Empty;
            DateTime start = DateTime.ParseExact((Scrim.time_start), "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            var left = start - DateTime.Now;
            if (left.Days > 0) result += left.Days + "D ";
            if (left.Hours > 0) result += left.Hours + "H ";
            if (left.Minutes > 0) result += left.Minutes + "M";



            return result;
        }
    }
}
