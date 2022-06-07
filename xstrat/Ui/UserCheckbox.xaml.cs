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
using xstrat.Json;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for UserCheckbox.xaml
    /// </summary>
    public partial class UserCheckbox : UserControl
    {
        public User User { get; set; }
        public UserCheckbox(User user)
        {
            InitializeComponent();
            User = user;
            UserCheckboxItem.Content = User.name;
            UserCheckboxItem.IsChecked = true;
        }
    }
}
