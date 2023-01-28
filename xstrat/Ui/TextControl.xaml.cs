using Mono.Cecil.Cil;
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
using xstrat.StratHelper;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for TextControl.xaml
    /// </summary>
    public partial class TextControl : UserControl
    {
        public bool Locked
        {
            set
            {
                MainContent.IsReadOnly = value;
            }
        }
        public TextControl()
        {
            InitializeComponent();
            MouseLeftButtonDown += TextControl_MouseLeftButtonDown;
            MainContent.TextChanged += MainContent_TextChanged;
        }

        private void MainContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(MainContent.Text))
            {
                MainContent.Text = "Text";
            }
        }

        private void TextControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(xStratHelper.stratView.CurrentToolTip == MVVM.View.ToolTip.Text)
            {
                MainContent.IsReadOnly = false;
                Focus();
                MainContent.SelectAll();
            }
        }
    }
}
