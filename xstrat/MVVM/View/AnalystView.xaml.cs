using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using XStratApi.Models.Supabase;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for AnalystView.xaml
    /// </summary>
    public partial class AnalystView : UserControl
    {
        public StatsMatch Match { get; set; }

        public int CurrentPage
        {
            get { return tbControl.SelectedIndex; }
            set { tbControl.SelectedIndex = value; }
        }
        public bool IsLastPage { get { return CurrentPage == tbControl.Items.Count - 1; } }

        public AnalystView()
        {
            InitializeComponent();
            Loaded += AnalystView_Loaded;
        }

        private void AnalystView_Loaded(object sender, RoutedEventArgs e)
        {
            tbControl.SelectionChanged += TbControl_SelectionChanged;
            UpdatePageButtons();
            UpdateBindings();
        }

        public void UpdateBindings()
        {
            //dgMatch.ItemsSource = Match?.Rounds;
            dgTeams.ItemsSource = Match?.Teams;
            dgBans.ItemsSource = Match?.Bans;
            TabGeneral.DataContext = Match;
        }

        public void Finished()
        {
            MessageBox.Show("Finished");
        }
        
        public void PageChanged()
        {
            //Page 0 > 1
            if(CurrentPage == 1)
            {
                Match = new StatsMatch();
                var replay = dsReplay.selectedReplayFolder;
                Match.ImportDissect(replay.DissectReplay);
                UpdateBindings();
            }
        }

        public void Cancel()
        {
            MessageBox.Show("Cancel");
        }

        #region PageHandling

        private void TbControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePageButtons();
            PageChanged();
        }

        public void NextPage()
        {
            if (IsLastPage)
            {
                Finished();
            }
            else
            {
                CurrentPage++;
            }
            UpdatePageButtons();
            PageChanged();
        }

        public void PreviousPage()
        {
            CurrentPage--;
            UpdatePageButtons();
            PageChanged();
        }

        public void UpdatePageButtons()
        {
            if (IsLastPage)
            {
                BtnForward.Content = "Finish";
            }
            else
            {
                BtnForward.Content = "Next";
            }
        }
        #endregion

        #region Buttons

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            PreviousPage();
        }

        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            NextPage();
        }

        #endregion
    }
}
