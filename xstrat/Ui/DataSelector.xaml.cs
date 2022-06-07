using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for TeammateSelector.xaml
    /// </summary>
    public partial class DataSelector : UserControl
    {  
       
        public User selectedUser { get; set; } = null;
        public Game selectedGame { get; set; } = null;
        public OffDayType selectedOffDayType = null;
        public CalendarFilterType selectedCalendarFilterType { get; set; } = null;
        public Map selectedMap { get; set; } = null;
        public ScrimMode selectedScrimMode { get; set; } = null;
        public int type { get; set; } = 0;



        /// <summary>
        /// type:
        /// 1 - teammates
        /// 2 - game
        /// 3 - offdaytype
        /// 4 - calendar filter
        /// 5 - Map
        /// 6 - ScrimMode
        /// </summary>
        /// <param name="type"></param>
        public DataSelector()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                UpdateUI();
            };

        }


        /// <summary>
        /// type:
        /// 1 - teammates
        /// 2 - game
        /// 3 - offdaytype
        /// 4 - calendar filter
        /// 5 - Map
        /// 6 - ScrimMode
        /// </summary>
        /// <param name="type"></param>
        public DataSelector(int type_id)
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                UpdateUI();
            };
        }

        public void SelectIndex(int index)
        {
            CBox.SelectedIndex = index;
        }

        private void UpdateUI()
        {
            if(type == 1)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.teammates)
                {
                    CBox.Items.Add(item.name);
                }
                CBox.SelectedIndex = 0;
            }
            else if (type == 2)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.games)
                {
                    CBox.Items.Add(item.name);
                }
                CBox.SelectedIndex = 0;
            }
            else if(type == 3)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.OffDayTypes)
                {
                    CBox.Items.Add(item.name);
                }
                CBox.SelectedIndex = 0;

            }
            else if (type == 4)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.CalendarFilterTypes)
                {
                    CBox.Items.Add(item.name);
                }
                CBox.SelectedIndex = 0;
            }
            else if (type == 5)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.Maps)
                {
                    CBox.Items.Add(item.name);
                }
            }
            else if (type == 6)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.ScrimModes)
                {
                    CBox.Items.Add(item.name);
                }
                CBox.SelectedIndex = 0;
            }
        }


        private void CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(type == 1)
            {
                selectedUser = Globals.teammates[CBox.SelectedIndex];
            }
            else if(type == 2)
            {
                selectedGame = Globals.games[CBox.SelectedIndex];
            }
            else if(type == 3)
            {
                selectedOffDayType = Globals.OffDayTypes[CBox.SelectedIndex];
            }
            else if (type == 4)
            {
                selectedCalendarFilterType = Globals.CalendarFilterTypes[CBox.SelectedIndex];
            }
            else if (type == 5)
            {
                selectedMap = Globals.Maps[CBox.SelectedIndex];
            }
            else if (type == 6)
            {
                selectedScrimMode = Globals.ScrimModes[CBox.SelectedIndex];
            }
        }
    }
}
