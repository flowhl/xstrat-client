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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        MainWindow wnd = (MainWindow)Application.Current.MainWindow;
        public LoginView()
        {
            InitializeComponent();
            if (wnd.NewlyRegistered)
            {
                Error.Content = "Please confirm the email to verify your account";
            }
            else
            {
                Error.Content = null;
            }
            RememberMe.setStatus(SettingsHandler.Settings.StayLoggedin);
            email.Text = SettingsHandler.Settings.LastLoginMail;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void ForgotPW_MouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            wnd.Register();
        }
        private async void Login()
        {
            var _email = email.Text;
            var _password = password.Password;
            if(_email != null && _password != null && _email != "" && _password != "")
            {
                var session = await ApiHandler.LoginAsync(_email, _password);
                if (session == null)
                {
                    return;
                }

                ApiHandler.CurrentSession = session;

                string baerer = session.AccessToken;
                string user_id = session.User.Id;

                if(user_id.IsNotNullOrEmpty())
                {
                    SettingsHandler.Settings.CurrentUserId = user_id;
                }
                SettingsHandler.Settings.StayLoggedin = RememberMe.getStatus();
                SettingsHandler.Settings.LastLoginMail = email.Text.Trim();
                SettingsHandler.Save();
                wnd.LoginComplete(baerer);
                
            }
            else
            {
                Error.Content = "invalid input";
                return;
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Login();
            }
        }
    }
}
