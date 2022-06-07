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

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for Routine.xaml
    /// </summary>
    public partial class Routine : UserControl
    {
        public string Head { get; set; }
        public string createdDate;
        public int ID;
        public EventHandler<RoutineButtonClicked> MoveButtonEvent;

        public Routine(string header, string createdDate, int id)
        {
            InitializeComponent();
            Head = header;
            this.createdDate = createdDate.Replace("T", " ");
            var charsToRemove = new string[] { "T", "Z"};
            foreach (var c in charsToRemove)
            {
                this.createdDate = this.createdDate.Replace(c, string.Empty);
            }
            ID = id;
            UpdateUI();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (MoveButtonEvent != null)
            {
                MoveButtonEvent(sender, new RoutineButtonClicked(0, this));
            }
        }

        private void UpdateUI()
        {
            Header_Textbox.Text = Head;
            CreatedOnLabel.Content = "Created on: " + createdDate;
        }

        private void Header_Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Head = Header_Textbox.Text;
        }

        private void AddBelowButton_Click(object sender, RoutedEventArgs e)
        {
            if (MoveButtonEvent != null)
            {
                MoveButtonEvent(sender, new RoutineButtonClicked(1, this));
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MoveButtonEvent != null)
            {
                MoveButtonEvent(sender, new RoutineButtonClicked(-1, this));
            }
        }
    }
}
