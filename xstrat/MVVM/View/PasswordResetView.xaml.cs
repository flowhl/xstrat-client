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
    /// Interaction logic for PasswordResetView.xaml
    /// </summary>
    public partial class PasswordResetView : UserControl
    {
        public PasswordResetView()
        {
            InitializeComponent();
            Loaded += PasswordResetView_Loaded;
        }

        private void PasswordResetView_Loaded(object sender, RoutedEventArgs e)
        {
            bPassword.Visibility = Visibility.Collapsed;
            bPasswordRepeat.Visibility = Visibility.Collapsed;
            ChangePasswordBtn.Visibility = Visibility.Collapsed;
        }

        private void ChangePasswordBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ResetTokenBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
