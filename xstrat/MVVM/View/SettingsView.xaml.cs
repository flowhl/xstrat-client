using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using xstrat.Core;
using xstrat.Json;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            Loaded += SettingsView_Loaded;
            
        }

        private void SettingsView_Loaded(object sender, RoutedEventArgs e)
        {
            SkinSwitcherPathDisplay.Text = SettingsHandler.Settings.SkinSwitcherPath;
            RememberMeSettings.setStatus(SettingsHandler.Settings.StayLoggedin);
            RetrieveDiscordID();
            RetrieveUbisoftID();
            if (SettingsHandler.Settings.APIURL != null)
            {
                APIText.Text = SettingsHandler.Settings.APIURL;
            }
            if (!Globals.AdminUser)
            {
                DcAdminView.Visibility = Visibility.Collapsed;
                TeamAdminSettingsBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                DcAdminView.Visibility = Visibility.Visible;
                TeamAdminSettingsBorder.Visibility = Visibility.Visible;
                RetrieveTeamSettingsData();
            }
        }

        internal class Credential
        {
            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("password")]
            public string Password { get; set; }
        }

        

        private void RetrieveUbisoftID()
        {

            UbiIDText.Text = DataCache.CurrentUser.UbisoftId;
        }

        private async Task SaveUbisoftIDAsync()
        {
            if (UbiIDText.Text != null && UbiIDText.Text != string.Empty)
            {
                var result = await ApiHandler.SetUbisoftID(UbiIDText.Text);
                if (result)
                {
                    Notify.sendSuccess("Changed Ubisoft-ID successfully");
                }
                else
                {
                    Notify.sendSuccess("Could not update Ubisoft-ID");
                }
            }
            else
            {
                Notify.sendWarn("Discord ID cannot be empty and has to be digits only");
            }
        }

        private void SkinSwitcherPickPathBtn_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = @"C:\Program Files (x86)\Ubisoft\Ubisoft Game Launcher\savegames";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var path = dialog.FileName;
                SettingsHandler.Settings.SkinSwitcherPath = path;
                SkinSwitcherPathDisplay.Text = path;
                SettingsHandler.Save();
            }
        }

        private void Save_BtnClick(object sender, RoutedEventArgs e)
        {
            SettingsHandler.Settings.StayLoggedin = RememberMeSettings.getStatus();
            SettingsHandler.Settings.APIURL = APIText.Text;
            SettingsHandler.Save();
            SaveDiscordIDAsync();
            SaveUbisoftIDAsync();
            if (Globals.AdminUser)
            {
                SaveDiscordData();
            }
        }

        private void RetrieveDiscordID()
        {
            DCId.Text = DataCache.CurrentUser.DiscordId;
        }
        private async Task SaveDiscordIDAsync()
        {
            if(DCId.Text != null && DCId.Text != string.Empty && IsDigitsOnly(DCId.Text))
            {
                var result = await ApiHandler.SetDiscordId(DCId.Text);
                if (result)
                {
                    Notify.sendSuccess("Changed Discord-ID successfully");
                }
                else
                {
                    Notify.sendError("Could not update Discord-ID");
                }
            }
            else
            {
                Notify.sendWarn("Discord-ID cannot be empty and has to be digits only");
            }
        }

        private void RetrieveTeamSettingsData()
        {
            
            DCWebhook.Text = DataCache.CurrentTeam.Webhook;
            SwNew.setStatus(StringToBool(DataCache.CurrentTeam.NotifyCreated.ToString()));
            SwTimeChanged.setStatus(StringToBool(DataCache.CurrentTeam.NotifyChanged.ToString()));
            SwWeeklySummary.setStatus(StringToBool(DataCache.CurrentTeam.NotifyWeekly.ToString()));
            SwStartingSoon.setStatus(StringToBool(DataCache.CurrentTeam.NotifySoon.ToString()));
            SWUseOnDays.setStatus(StringToBool(DataCache.CurrentTeam.UseOnDays.ToString()));
            ScrimTimeDelay.Value = DataCache.CurrentTeam.NotifyDelay;
        }
        private async Task SaveDiscordData()
        {
            if (DCWebhook.Text != null && DCWebhook.Text != string.Empty)
            {
                var result = await ApiHandler.SetDiscordAdminSettings(DCWebhook.Text, Convert.ToInt32(SwNew.getStatus()), Convert.ToInt32(SwTimeChanged.getStatus()), Convert.ToInt32(SwWeeklySummary.getStatus()), Convert.ToInt32(SwStartingSoon.getStatus()), ScrimTimeDelay.Value, Convert.ToInt32(SWUseOnDays.getStatus()));
                if (result)
                {
                    Notify.sendSuccess("Changed Discord admin settings successfully");
                }
                else
                {
                    Notify.sendError("Could not change Discord Admin Settings");
                }
            }
            else
            {
                Notify.sendWarn("Discord Webhook cannot be empty");
            }
        }

        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
        private bool StringToBool(string input)
        {
            return (input.Trim() == "1");
        }

        private void DcHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://support.discord.com/hc/en-us/articles/206346498-Where-can-I-find-my-User-Server-Message-ID-");
        }

        private void ReplayPickPathBtn_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = @"C:\Program Files (x86)\Ubisoft\Ubisoft Game Launcher\games\Tom Clancy's Rainbow Six Siege\MatchReplay";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var path = dialog.FileName;
                SettingsHandler.Settings.GameReplayPath = path;
                ReplayPathDisplay.Text = path;
                SettingsHandler.Save();
            }
        }
    }
}
