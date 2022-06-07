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
            RememberMe.setStatus(SettingsHandler.StayLoggedin);
            email.Text = SettingsHandler.LastLoginMail;
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
                (bool, string) result = await ApiHandler.LoginAsync(_email, _password);
                if (result.Item1)
                {
                    string response = result.Item2;
                    //convert to json instance
                    JObject json = JObject.Parse(response);
                    string baerer = json.SelectToken("token").ToString();
                    SettingsHandler.StayLoggedin = RememberMe.getStatus();
                    SettingsHandler.LastLoginMail = email.Text.Trim();
                    wnd.LoginComplete(baerer);
                }
                else
                {
                    Error.Content = "invalid user or password: " + result.Item2;
                    return;
                }
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
