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

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        MainWindow wnd = (MainWindow)Application.Current.MainWindow;
        public RegisterView()
        {
            InitializeComponent();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
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
            if (_email != null && _password != null && _username != null && _email != "" && _password != "" && _username != "" && _email.Contains("@") && _email.Contains("."))
            {
                (bool, string) result = await ApiHandler.RegisterAsync(_username, _email, _password);
                if (result.Item1)
                {
                    wnd.RegisterComplete();
                }
                else
                {
                    Error.Content = "Registration error: " + result.Item2;
                    return;
                }
            }
            else
            {
                Error.Content =  "Invalid input";
                return;
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            wnd.RegisterComplete();
        }
    }
}
