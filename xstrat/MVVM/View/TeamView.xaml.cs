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
using xstrat.Ui;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for TeamView.xaml
    /// </summary>
    public partial class TeamView : StateUserControl
    {
        public TeamView()
        {
            InitializeComponent();
            TDashboard.Loaded += TDashboard_Loaded;
        }

        private void TDashboard_Loaded(object sender, RoutedEventArgs e)
        {
            WaitForAPIAsync();
        }

        public async Task WaitForAPIAsync()
        {
            await Task.Delay(500);
            if (TDashboard.TeamInfo != null && TDashboard.TeamInfo.Name != null && TDashboard.TeamInfo.Name != "" && TDashboard.TeamInfo.Name != "Create or join a team")
            {
                JoinCreatePanel.Visibility = Visibility.Collapsed;
                TeamDashboard.Visibility = Visibility.Visible;
            }
            else
            {
                JoinCreatePanel.Visibility = Visibility.Visible;
                TeamDashboard.Visibility = Visibility.Collapsed;
            }
        }

        private async void JoinBtn_ClickAsync()
        {
            string id = team_id.Text;
            string pw = password.Text;
            bool result = await ApiHandler.JoinTeam(id, pw);
            if (result)
            {
                Notify.sendSuccess("Joined successfully");
                TDashboard.Reload();
                TDashboard.Retrieve(); // fix stuff not loading here
                await Task.Delay(500);
                WaitForAPIAsync();
            }
        }

        private async void CreateBtn_ClickAsync()
        {
            string name = team_name.Text.ToString();
            if(GameSelector.selectedGame != null)
            {
                string game_id = GameSelector.selectedGame.Id;

                var result = await ApiHandler.NewTeam(name, game_id);
                if (result)
                {
                    Notify.sendSuccess("Created successfully");
                    TDashboard.Retrieve();
                    await WaitForAPIAsync();
                }
                else
                {
                    Notify.sendError("Could not create Team");
                }
            }
            else
            {
                Notify.sendWarn("Select a game first");
            }

        }

        private void JoinBtn_Click(object sender, RoutedEventArgs e)
        {
            JoinBtn_ClickAsync();
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateBtn_ClickAsync();
        }


    }
}
