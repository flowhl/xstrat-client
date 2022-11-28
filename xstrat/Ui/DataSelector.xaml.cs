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
        public EventType selectedEventType { get; set; } = null;
        public Operator selectedOperator { get; set; } = null;

        public DataSelectorTypes Type { get; set; } = 0;
        public int indexToSelect = -1;
        public string valueToSelect = string.Empty;

        /// <summary>
        /// type:
        /// 1 - teammates
        /// 2 - game
        /// 3 - offdaytype
        /// 4 - calendar filter
        /// 5 - Map
        /// 6 - ScrimMode
        /// 7 - EventType
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


        public DataSelector(DataSelectorTypes Type)
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                UpdateUI();
            };
        }

        public void SelectIndexWhenLoaded(int index)
        {
            if(index < 0) return;
            indexToSelect = index;
        }

        public void SelectIndex(int index)
        {
            if (index < 0) return;
            indexToSelect = index;
            CBox.SelectedIndex = index;
        }

        public void SelectUserID(int uid)
        {
            if (Type != DataSelectorTypes.Teammates) return;
            indexToSelect = CBox.Items.IndexOf(CBox.Items.OfType<ListBoxItem>().Where(x => x.Name == Globals.teammates.Where(x => x.id == uid).FirstOrDefault()?.name));
            SelectIndex(indexToSelect);
        }
        public void SelectOperator(string val)
        {
            if (Type != DataSelectorTypes.AttackOperators && Type != DataSelectorTypes.DefenseOperators && Type != DataSelectorTypes.AllOperators) return;
            SelectValue(val);
        }
        public void SelectValue(string val)
        {
            valueToSelect = val;
            CBox.SelectedValue = val;
        }

        public void UpdateUI()
        {
            if(Type == DataSelectorTypes.Teammates)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.teammates)
                {
                    CBox.Items.Add(item.name);
                }
            }
            else if (Type == DataSelectorTypes.Game)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.games)
                {
                    CBox.Items.Add(item.name);
                }
            }
            else if(Type == DataSelectorTypes.OffdayType)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.OffDayTypes)
                {
                    CBox.Items.Add(item.name);
                }
            }
            else if (Type == DataSelectorTypes.CalendarFilter)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.CalendarFilterTypes)
                {
                    CBox.Items.Add(item.name);
                }
            }
            else if (Type == DataSelectorTypes.Map)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.Maps)
                {
                    CBox.Items.Add(item.name);
                }
            }
            else if (Type == DataSelectorTypes.ScrimMode)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.ScrimModes)
                {
                    CBox.Items.Add(item.name);
                }
            }
            else if (Type == DataSelectorTypes.EventType)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.EventTypes)
                {
                    CBox.Items.Add(item.name);
                }
            }
            else if (Type == DataSelectorTypes.AllOperators)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.Operators)
                {
                    CBox.Items.Add(item.name);
                }
            }
            else if (Type == DataSelectorTypes.DefenseOperators)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.Operators.Where(x => x.type == 0))
                {
                    CBox.Items.Add(item.name);
                }
            }
            else if (Type == DataSelectorTypes.AttackOperators)
            {
                CBox.Items.Clear();
                foreach (var item in Globals.Operators.Where(x => x.type == 1))
                {
                    CBox.Items.Add(item.name);
                }
            }
            if(indexToSelect > -1) CBox.SelectedIndex = indexToSelect;
            if (!string.IsNullOrEmpty(valueToSelect)) CBox.SelectedValue = valueToSelect;
        }


        private void CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CBox.SelectedIndex < 0) return;
            if(Type == DataSelectorTypes.Teammates)
            {
                selectedUser = Globals.teammates[CBox.SelectedIndex];
            }
            else if(Type == DataSelectorTypes.Game)
            {
                selectedGame = Globals.games[CBox.SelectedIndex];
            }
            else if(Type == DataSelectorTypes.OffdayType)
            {
                selectedOffDayType = Globals.OffDayTypes[CBox.SelectedIndex];
            }
            else if (Type == DataSelectorTypes.CalendarFilter)
            {
                selectedCalendarFilterType = Globals.CalendarFilterTypes[CBox.SelectedIndex];
            }
            else if (Type == DataSelectorTypes.Map)
            {
                selectedMap = Globals.Maps[CBox.SelectedIndex];
            }
            else if (Type == DataSelectorTypes.ScrimMode)
            {
                selectedScrimMode = Globals.ScrimModes[CBox.SelectedIndex];
            }
            else if (Type == DataSelectorTypes.EventType)
            {
                selectedEventType = Globals.EventTypes[CBox.SelectedIndex];
            }
            else if (Type == DataSelectorTypes.AllOperators)
            {
                selectedOperator = Globals.Operators.Where(x => x.name == CBox.SelectedItem.ToString()).FirstOrDefault();
            }
            else if (Type == DataSelectorTypes.DefenseOperators)
            {
                selectedOperator = Globals.Operators.Where(x => x.name == CBox.SelectedItem.ToString()).FirstOrDefault();
            }
            else if (Type == DataSelectorTypes.AttackOperators)
            {
                selectedOperator = Globals.Operators.Where(x => x.name == CBox.SelectedItem.ToString()).FirstOrDefault();
            }
        }
    }
    public enum DataSelectorTypes
    {
        Teammates,
        Game,
        OffdayType,
        CalendarFilter,
        Map,
        ScrimMode,
        EventType,
        AttackOperators,
        DefenseOperators,
        AllOperators,
    }
}
