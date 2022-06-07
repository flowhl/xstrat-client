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
    /// Interaction logic for ScrimFinderControl.xaml
    /// </summary>
    public partial class ScrimFinderControl : UserControl
    {
        #region btn properties
        public bool mobtn { get; set; } = true;
        public bool tubtn { get; set; } = true;
        public bool webtn { get; set; } = true;
        public bool thbtn { get; set; } = true;
        public bool frbtn { get; set; } = true;
        public bool sabtn { get; set; } = true;
        public bool subtn { get; set; } = true;

        private Brush disabledBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#202020"));
        private Brush enabledBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#336cb5"));

        #endregion

        public List<User> Users { get; set; } = new List<User>();
        public CalendarFilterType FilterType { get; set; }
        public int PlayerCount { get; set; } = 5;

        public ScrimFinderControl()
        {
            InitializeComponent();
            CalendarTypeSelector.CBox.SelectionChanged += CBox_SelectionChanged;
            UpdateButtonColors();
            Loaded += ScrimFinderControl_Loaded;
            DurHour.Value = 2;
            ToHour.Value = 23;
            FromHour.Value = 20;
        }

        private void ScrimFinderControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Fill Player selection list for Calendar filter
            foreach (var user in Globals.teammates)
            {
                UserCheckbox userCheckbox = new UserCheckbox(user);
                Playerlist.Children.Add(userCheckbox);
            }

            //set default number
            PlayerAmount.Value = PlayerCount;
        }

        private void CBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CalendarTypeSelector != null && CalendarTypeSelector.selectedCalendarFilterType != null)
            {
                var type = CalendarTypeSelector.selectedCalendarFilterType.id;
                if (type == 0) //min players
                {
                    PlayerAmount.Visibility = Visibility.Visible;
                    Playerlist.Visibility = Visibility.Collapsed;
                }
                if (type == 1) //specific
                {
                    PlayerAmount.Visibility = Visibility.Collapsed;
                    Playerlist.Visibility = Visibility.Visible;
                }
                if (type == 2) //specific min
                {
                    PlayerAmount.Visibility = Visibility.Collapsed;
                    Playerlist.Visibility = Visibility.Visible;
                }
                if (type == 3) // everyone
                {
                    PlayerAmount.Visibility = Visibility.Collapsed;
                    Playerlist.Visibility = Visibility.Collapsed;
                }
            }
        }

        #region Day buttons
        private void MoBtn_Click(object sender, RoutedEventArgs e)
        {
            mobtn = !mobtn;
            UpdateButtonColors();
        }

        private void TuBtn_Click(object sender, RoutedEventArgs e)
        {
            tubtn = !tubtn;
            UpdateButtonColors();
        }

        private void WeBtn_Click(object sender, RoutedEventArgs e)
        {
            webtn = !webtn;
            UpdateButtonColors();
        }

        private void ThBtn_Click(object sender, RoutedEventArgs e)
        {
            thbtn = !thbtn;
            UpdateButtonColors();
        }

        private void FrBtn_Click(object sender, RoutedEventArgs e)
        {
            frbtn = !frbtn;
            UpdateButtonColors();
        }

        private void SaBtn_Click(object sender, RoutedEventArgs e)
        {
            sabtn = !sabtn;
            UpdateButtonColors();
        }

        private void SuBtn_Click(object sender, RoutedEventArgs e)
        {
            subtn = !subtn;
            UpdateButtonColors();
        }
        private void UpdateButtonColors()
        {
            if (mobtn)
            {
                MoBtn.Background = enabledBrush;
            }
            else
            {
                MoBtn.Background = disabledBrush;
            }

            if (tubtn)
            {
                TuBtn.Background = enabledBrush;
            }
            else
            {
                TuBtn.Background = disabledBrush;
            }

            if (webtn)
            {
                WeBtn.Background = enabledBrush;
            }
            else
            {
                WeBtn.Background = disabledBrush;
            }

            if (thbtn)
            {
                ThBtn.Background = enabledBrush;
            }
            else
            {
                ThBtn.Background = disabledBrush;
            }

            if (frbtn)
            {
                FrBtn.Background = enabledBrush;
            }
            else
            {
                FrBtn.Background = disabledBrush;
            }

            if (sabtn)
            {
                SaBtn.Background = enabledBrush;
            }
            else
            {
                SaBtn.Background = disabledBrush;
            }

            if (subtn)
            {
                SuBtn.Background = enabledBrush;
            }
            else
            {
                SuBtn.Background = disabledBrush;
            }
        }

        #endregion

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            FilterType = CalendarTypeSelector.selectedCalendarFilterType;
            Users.Clear();
            foreach (UserCheckbox uc in Playerlist.Children)
            {
                if (uc.UserCheckboxItem.IsChecked.GetValueOrDefault(false))
                {
                    Users.Add(uc.User);
                }
            }
            PlayerCount = PlayerAmount.Value;

            DependencyObject ucParent = this.Parent;
            if (ucParent != null)
            {
                while (!(ucParent is UserControl))
                {
                    ucParent = LogicalTreeHelper.GetParent(ucParent);
                }
                var sv = ucParent as ScrimView;
                sv.SearchButtonClicked();
            }
        }

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.Window window = new Core.Window
            {
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddHours(3),
                AvailablePlayers = new List<Player>()
            };
            var response = new ScrimWindow(window);
            response.Show();
        }
    }
}
