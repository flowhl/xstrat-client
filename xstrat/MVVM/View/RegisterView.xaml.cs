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

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : StateUserControl
    {
        //public char[] forbiddenchars = "(){}[]|`¬¦! \"£$%^&*\"<>:;#~_-+=,@.".ToCharArray();
        MainWindow wnd = (MainWindow)Application.Current.MainWindow;
        public RegisterView()
        {
            InitializeComponent();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            Error.Content = "";
            var _email = email.Text;
            var _password = password.Password;
            var _repeat_password = RepeatPassword.Password;
            var _username = username.Text;
            if (string.IsNullOrEmpty(_email))
            {
                Error.Content = "Email cannot be empty";
                return;
            }
            if (string.IsNullOrEmpty(_username))
            {
                Error.Content = "Username cannot be empty";
                return;
            }
            if (string.IsNullOrEmpty(_password))
            {
                Error.Content = "Password cannot be empty";
                return;
            }

            if (_password != _repeat_password)
            {
                Error.Content = "Passwords do not match";
                return;
            }

            if (!(_email.Contains("@") && _email.Contains(".")))
            {
                Error.Content = "Email invalid";
                return;
            }

            if (_username.Length <= 5)
            {
                Error.Content = "Name has to be longer than 5 characters";
                return;
            }

            if (_password.Length <= 8)
            {
                Error.Content = "Password has to be longer than 8 characters";
                return;
            }

            //if (containsChars(_password))
            //{
            //    Error.Content = "Password cannot contain (){}[]|`¬¦! \"£$%^&*\"<>:;#~_-+=,@.";
            //    return;
            //}

            RegisterAsync();
        }

        /// <summary>
        /// UI Registration -> calls APIHandler
        /// </summary>
        private async void RegisterAsync()
        {
            var _email = email.Text;
            var _password = password.Password;
            var _username = username.Text;

            (bool, string) result = await ApiHandler.RegisterAsync(_username, _email, _password);
            if (result.Item1)
            {
                SettingsHandler.Settings.LastLoginMail = _email;
                SettingsHandler.Save();
                wnd.RegisterComplete();
            }
            else
            {
                Error.Content = "Registration error: " + result.Item2;
                return;
            }

        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            wnd.RegisterComplete();
        }
        //private bool containsChars(string input)
        //{
        //    foreach (var c in forbiddenchars)
        //    {
        //        if (input.Contains(c))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RegisterBtn_Click(this, EventArgs.Empty as RoutedEventArgs);
            }
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Brush red = "#D64251".ToSolidColorBrush();
            Brush orange = "#d66242".ToSolidColorBrush();
            Brush green = "#4ab859".ToSolidColorBrush();

            string pw = password.Password;
            if (string.IsNullOrEmpty(pw)) return;

            double lengthpart = ((double)pw.Length) / 22.0;
            int pixels = (int)(lengthpart * 240.0);
            if (pixels > 240) pixels = 240;
            if (pixels < 0) pixels = 0;

            StrengthSlider.Width = pixels;

            if (pw.Length > 12)
            {
                StrengthSlider.Background = green;
                return;
            }
            if (pw.Length > 8)
            {
                StrengthSlider.Background = orange;
                return;
            }
            StrengthSlider.Background = red;
        }
    }
}
