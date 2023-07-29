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
using xstrat.Models.Supabase;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for Routine.xaml
    /// </summary>
    public partial class Routine : UserControl
    {
        public Models.Supabase.Routine routine { get; set; }
        public EventHandler<RoutineButtonClicked> MoveButtonEvent;

        public Routine(Models.Supabase.Routine _routine)
        {
            InitializeComponent();
            routine = _routine;
            UpdateUI();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (MoveButtonEvent != null)
            {
                MoveButtonEvent(sender, new RoutineButtonClicked(0, routine));
            }
        }

        private void UpdateUI()
        {
            Header_Textbox.Text = routine.Title;
            CreatedOnLabel.Content = "Created on: " + routine.CreatedAt.ToString();
        }

        private void Header_Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            routine.Title = Header_Textbox.Text;
        }

        private void AddBelowButton_Click(object sender, RoutedEventArgs e)
        {
            if (MoveButtonEvent != null)
            {
                MoveButtonEvent(sender, new RoutineButtonClicked(1, routine));
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MoveButtonEvent != null)
            {
                MoveButtonEvent(sender, new RoutineButtonClicked(-1, routine));
            }
        }
    }
}
