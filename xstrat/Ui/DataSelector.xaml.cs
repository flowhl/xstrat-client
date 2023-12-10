using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using xstrat.Models.Supabase;
using xstrat.MVVM.View;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for TeammateSelector.xaml
    /// </summary>
    public partial class DataSelector : UserControl
    {
        // Declare the event using the delegate
        public event SelectionChangedEventHandler SelectionChanged;
        // Define a delegate for the event handler
        public delegate void SelectionChangedEventHandler(object sender, EventArgs e);

        public UserData selectedUser { get; set; } = null;
        public Models.Supabase.Game selectedGame { get; set; } = null;
        public OffDayType selectedOffDayType = null;
        public CalendarFilterType selectedCalendarFilterType { get; set; } = null;
        public Models.Supabase.Map SelectedMap { get; set; } = null;
        public ScrimMode selectedScrimMode { get; set; } = null;
        public EventType selectedEventType { get; set; } = null;
        public Models.Supabase.Operator selectedOperator { get; set; } = null;
        public MatchReplayFolder selectedReplayFolder { get; set; } = null;

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
            Loaded += DataSelector_Loaded;
        }

        private void DataSelector_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUI();
            CBox.SelectionChanged += CB_SelectionChanged;
        }

        public DataSelector(DataSelectorTypes Type)
        {
            InitializeComponent();
            Loaded += DataSelector_Loaded;
            this.Type = Type;
        }

        public void SelectIndexWhenLoaded(int index)
        {
            if (index < 0) return;
            indexToSelect = index;
        }

        public void SelectIndex(int index)
        {
            if (index < 0) return;
            indexToSelect = index;
            CBox.SelectedIndex = index;
        }

        public void SelectUserID(string id)
        {
            if (Type != DataSelectorTypes.Teammates) return;
            indexToSelect = CBox.Items.IndexOf(CBox.Items.OfType<ListBoxItem>().Where(x => x.Name == DataCache.CurrentTeamMates.Where(x => x.Id == id).FirstOrDefault()?.Name));
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
            if (Type == DataSelectorTypes.Teammates)
            {
                CBox.Items.Clear();
                foreach (var item in DataCache.CurrentTeamMates)
                {
                    CBox.Items.Add(item.Name);
                }
            }
            else if (Type == DataSelectorTypes.Game)
            {
                CBox.Items.Clear();
                foreach (var item in DataCache.CurrentGames)
                {
                    CBox.Items.Add(item.Name);
                }
            }
            else if (Type == DataSelectorTypes.OffdayType)
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
                foreach (var item in DataCache.CurrentMaps)
                {
                    CBox.Items.Add(item.Name);
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
                foreach (var item in DataCache.CurrentOperators)
                {
                    CBox.Items.Add(item.Name);
                }
            }
            else if (Type == DataSelectorTypes.DefenseOperators)
            {
                CBox.Items.Clear();
                foreach (var item in DataCache.CurrentOperators.Where(x => x.Typ == 0))
                {
                    CBox.Items.Add(item.Name);
                }
            }
            else if (Type == DataSelectorTypes.AttackOperators)
            {
                CBox.Items.Clear();
                foreach (var item in DataCache.CurrentOperators.Where(x => x.Typ == 1))
                {
                    CBox.Items.Add(item.Name);
                }
            }
            else if(Type == DataSelectorTypes.Replays)
            {
                CBox.Items.Clear();
                foreach (var item in DataCache.ReplayFolders)
                {
                    CBox.Items.Add(item.Title + "|" + item.FolderName);
                }
            }
            if (indexToSelect > -1) CBox.SelectedIndex = indexToSelect;
            if (!string.IsNullOrEmpty(valueToSelect)) CBox.SelectedValue = valueToSelect;
        }


        private void CB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CBox.SelectedIndex < 0) return;
            if (Type == DataSelectorTypes.Teammates)
            {
                selectedUser = DataCache.CurrentTeamMates[CBox.SelectedIndex];
            }
            else if (Type == DataSelectorTypes.Game)
            {
                selectedGame = DataCache.CurrentGames[CBox.SelectedIndex];
            }
            else if (Type == DataSelectorTypes.OffdayType)
            {
                selectedOffDayType = Globals.OffDayTypes[CBox.SelectedIndex];
            }
            else if (Type == DataSelectorTypes.CalendarFilter)
            {
                selectedCalendarFilterType = Globals.CalendarFilterTypes[CBox.SelectedIndex];
            }
            else if (Type == DataSelectorTypes.Map)
            {
                SelectedMap = DataCache.CurrentMaps[CBox.SelectedIndex];
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
                selectedOperator = DataCache.CurrentOperators.Where(x => x.Name == CBox.SelectedItem.ToString()).FirstOrDefault();
            }
            else if (Type == DataSelectorTypes.DefenseOperators)
            {
                selectedOperator = DataCache.CurrentOperators.Where(x => x.Name == CBox.SelectedItem.ToString()).FirstOrDefault();
            }
            else if (Type == DataSelectorTypes.AttackOperators)
            {
                selectedOperator = DataCache.CurrentOperators.Where(x => x.Name == CBox.SelectedItem.ToString()).FirstOrDefault();
            }
            else if (Type == DataSelectorTypes.Replays)
            {
                selectedReplayFolder = DataCache.ReplayFolders.FirstOrDefault(x => (x.Title + "|" + x.FolderName) == CBox.SelectedItem.ToString());
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
        Replays
    }
}
