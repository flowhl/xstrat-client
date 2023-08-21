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
using Globals = xstrat.Core.Globals;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for TeamDashboard.xaml
    /// </summary>
    public partial class TeamDashboard : UserControl
    {
        public Models.Supabase.Team TeamInfo;
        public TeamDashboard()
        {
            InitializeComponent();
            Loaded += TeamDashboard_Loaded;
        }

        private void TeamDashboard_Loaded(object sender, RoutedEventArgs e)
        {
            Retrieve();
        }

        public async void Retrieve()
        {
            TeamName.Content = "Create or join a team";
            AdminName.Content = "Admin: ";
            GameName.Content = "Game: ";
            TeamInfo = DataCache.CurrentTeam;
            await CheckAdmin();
            await RetrieveColorAsync();
            DependencyObject ucParent = this.Parent;

            while (!(ucParent is UserControl))
            {
                ucParent = LogicalTreeHelper.GetParent(ucParent);
            }
            if (ucParent is UserControl)
            {
                var uc = (TeamView)ucParent;
                await uc.WaitForAPIAsync();
            }
            if (TeamInfo != null)
            {
                TeamName.Content = TeamInfo.Name;
                AdminName.Content = "Admin: " + TeamInfo.AdminName;
                GameName.Content = "Game: " + TeamInfo.GameName;
            }

        }

        private async Task CheckAdmin()
        {
            bool admin = await ApiHandler.GetAdminStatus();
            if (admin)
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
            await ApiHandler.LeaveTeam();
            Reload();
            Retrieve();
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

        private async Task RetrieveColorAsync()
        {

            ColorPickerUI.SelectedColor = (Color)ColorConverter.ConvertFromString(DataCache.CurrentUser?.Color ?? "#FF1234");
        }
        private async Task SaveColorAsync()
        {
            var result = await ApiHandler.SetColor(HexConverter(ColorPickerUI.SelectedColor));
            if (result)
            {
                Notify.sendSuccess("Changed color successfully");
            }
            else
            {
                Notify.sendError("Could not set color");
            }
        }

        private async Task DeleteAdminBtn_ClickAsync()
        {
            var result = await ApiHandler.DeleteTeam();
            if (result)
            {
                Notify.sendSuccess("Deleted successfully");
                Retrieve();
            }
            else
            {
                Notify.sendError("Could not delete team. Make sure to be team admin.");
            }
        }
        private async Task JoinPWAdminBtn_ClickAsync()
        {
            if(DataCache.CurrentTeam == null || DataCache.CurrentTeam.Password.IsNullOrEmpty())
            {
                Notify.sendError("No Team or Join Password found!");
                Logger.Log("No Team or Join Password found!");
                return;
            }

            if(!await ApiHandler.GetAdminStatus())
            {
                Notify.sendError("You have to be teamadmin to use this feature");
                Logger.Log("You have to be teamadmin to use this feature");
                return;
            }

            //MessageBox.Show("Team ID: " + DataCache.CurrentTeam.Id + "\nJoin password: " + DataCache.CurrentTeam.Password, "Your teams' join credentials:");
            Notify.sendSuccess("Copied the team id and join password to your clipboard");
            Clipboard.SetText($"Team ID: {DataCache.CurrentTeam.Id} | Join password: {DataCache.CurrentTeam.Password}");
        }
        private async Task RenameAdminBtn_ClickAsync(string newname)
        {
            var result = await ApiHandler.RenameTeamAsync(newname);
            if (result)
            {
                Notify.sendSuccess("Renamed successfully");
                Retrieve();
            }
            else
            {
                Notify.sendError("Could not rename team, make sure to be team admin");
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

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            Reload();
        }
        public void Reload()
        {
            DataCache.RetrieveTeam();
            DataCache.RetrieveTeamMates();
        }
    }
}
