using Microsoft.VisualBasic;
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
using xstrat.MVVM.View;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for TeamDashboard.xaml
    /// </summary>
    public partial class TeamDashboard : UserControl
    {
        public List<User> teammates { get; set; } = new List<User>();
        public teamInfo TeamInfo { get; set; }
        public TeamDashboard()
        {
            InitializeComponent();
            Retrieve();
        }
        public async void Retrieve()
        {
            await RetrieveTeamMatesAsync();
            await RetrieveTeamInfoAsync();
            await CheckAdmin();
            await RetrieveColorAsync();
            DependencyObject ucParent = this.Parent;

            while (!(ucParent is UserControl))
            {
                ucParent = LogicalTreeHelper.GetParent(ucParent);
            }
            if(ucParent is UserControl)
            {
                var uc = (TeamView)ucParent;
                await uc.WaitForAPIAsync();
            }

        }

        private async Task CheckAdmin()
        {
            (bool, string) result = await ApiHandler.VerifyAdmin();
            if (result.Item1)
            {
                AdminButtons.Visibility = Visibility.Visible;
            }
            else
            {
                AdminButtons.Visibility = Visibility.Collapsed;
            }
        }

        private async void LeaveBtn_Click(object sender, RoutedEventArgs e)
        {
            (bool, string) result = await ApiHandler.LeaveTeam();
            if (result.Item1)
            {
                Notify.sendSuccess("Left successfully");
                Retrieve();
            }
            else
            {
                Notify.sendError( result.Item2);
            }
        }

        private void JoinPWAdminBtn_Click(object sender, RoutedEventArgs e)
        {
            JoinPWAdminBtn_ClickAsync();
        }

        private void RenameAdminBtn_Click(object sender, RoutedEventArgs e)
        {
            string input = Interaction.InputBox("New name:", "Rename Team");
            RenameAdminBtn_ClickAsync(input);
        }
        private void DeleteAdminBtn_Click(object sender, RoutedEventArgs e)
        {
            DeleteAdminBtn_ClickAsync();
        }
        
        private async Task RetrieveTeamMatesAsync()
        {
            var result = await ApiHandler.TeamMembers();
            if (result.Item1)
            {
                string response = result.Item2;
                //convert to json instance
                JObject json = JObject.Parse(response);
                var data = json.SelectToken("data").ToString();
                if (data != null && data != "")
                {
                    teammates.Clear();
                    List<xstrat.Json.User> rList = JsonConvert.DeserializeObject<List<Json.User>>(data);
                    teammates = rList;
                }
                else
                {
                    Notify.sendError("Teammates could not be loaded");
                    throw new Exception("Teammates could not be loaded");
                }
                MemberSP.Children.Clear();
                foreach (var user in teammates)
                {
                    var newItem = new Teammate(user.name, user.id, user.color);
                    newItem.Height = 30;
                    newItem.Width = 250;
                    MemberSP.Children.Add(newItem);
                }
            }
            else
            {
                Notify.sendError("Teammates could not be loaded");
            }
        }
        private async Task RetrieveTeamInfoAsync()
        {
            var result = await ApiHandler.TeamInfo();
            TeamName.Content = "Create or join a team";
            AdminName.Content = "Admin: ";
            GameName.Content = "Game: ";
            if(TeamInfo != null)
            {
                TeamInfo.admin_name = null;
                TeamInfo.game_name = null;
                TeamInfo.team_name = "Create or join a team";
            }            
            if (result.Item1)
            {
                string response = result.Item2;
                //convert to json instance
                JObject json = JObject.Parse(response);
                var data = json.SelectToken("data").ToString();
                if (data != null && data != "")
                {
                    try
                    {
                        TeamInfo = JsonConvert.DeserializeObject<List<teamInfo>>(data).First();
                    }
                    catch(Exception ex)
                    {
                        Logger.Log("No Teaminfo found! " + ex.Message);
                    }
                }
                else
                {
                    Notify.sendError("Team info could not be loaded");
                    throw new Exception("Team info could not be loaded");
                }
                if (TeamInfo != null)
                {
                    TeamName.Content = TeamInfo.team_name;
                    AdminName.Content = "Admin: " +TeamInfo.admin_name;
                    GameName.Content = "Game: "+ TeamInfo.game_name;
                }
            }
            else
            {
                Notify.sendError("Team info could not be loaded");
            }
        }

        private async Task RetrieveColorAsync()
        {
            var result = await ApiHandler.GetColor();
            if (result.Item1)
            {
                JObject json = JObject.Parse(result.Item2);
                var data = json.SelectToken("data").ToString();
                if (data != null && data != "")
                {
                    try
                    {
                        JColor color = JsonConvert.DeserializeObject<List<JColor>>(data).First();
                        ColorPickerUI.SelectedColor = (Color)ColorConverter.ConvertFromString(color.color);
                    }
                    catch (Exception ex)
                    {
                        Notify.sendError("No Color found!");
                        Logger.Log("No Color found! " + ex.Message);
                    }
                }
            }
            else
            {
                Notify.sendError(result.Item2);
            }
        }
        private async Task SaveColorAsync()
        {
            var result = await ApiHandler.SetColor(HexConverter(ColorPickerUI.SelectedColor));
            if (result.Item1)
            {
                Notify.sendSuccess("Changed color successfully");
                Retrieve();
            }
            else
            {
                Notify.sendError(result.Item2);
            }
        }

        private async Task DeleteAdminBtn_ClickAsync()
        {
            var result = await ApiHandler.DeleteTeam();
            if (result.Item1)
            {
                Notify.sendSuccess("Deleted successfully");
                Retrieve();
            }
            else
            {
                Notify.sendError(result.Item2);
            }
        }
        private async Task JoinPWAdminBtn_ClickAsync()
        {
            var result = await ApiHandler.TeamJoinpassword();
            if (result.Item1)
            {
                JObject json = JObject.Parse(result.Item2);
                var data = json.SelectToken("data").ToString();
                if (data != null && data != "")
                {
                    try
                    {
                        JoinPw joinPw = JsonConvert.DeserializeObject<List<JoinPw>>(data).First();
                        MessageBox.Show("Team ID: " + joinPw.id.ToString() + "\nJoin password: " + joinPw.join_password , "Your teams' join credentials:");
                        Notify.sendSuccess("Copied the join password to your clipboard");
                        Clipboard.SetText(joinPw.join_password);
                    }
                    catch (Exception ex)
                    {
                        Notify.sendError("No JoinPW found!");
                        Logger.Log("No JoinPW found! " + ex.Message);
                    }
                }
            }
            else
            {
                Notify.sendError(result.Item2);
            }
        }
        private async Task RenameAdminBtn_ClickAsync(string newname)
        {
            var result = await ApiHandler.RenameTeam(newname);
            if (result.Item1)
            {
                Notify.sendSuccess("Renamed successfully");
                Retrieve();
            }
            else
            {
                Notify.sendError( result.Item2);
            }
        }

        private void SaveColor_Click(object sender, RoutedEventArgs e)
        {
            SaveColorAsync();
        }
        private String HexConverter(System.Windows.Media.Color c)
        {
            return new ColorConverter().ConvertToString(c);
        }
    }
}
