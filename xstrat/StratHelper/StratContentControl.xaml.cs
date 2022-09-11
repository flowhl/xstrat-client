using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using xstrat.StratHelper;

namespace XStrat
{
    /// <summary>
    /// Interaction logic for StratContentControl.xaml
    /// </summary>
    public partial class StratContentControl : UserControl
    {
        public bool Selection { get; set; }
        public StratContentControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Selection = !Selection;
            if (!Keyboard.IsKeyDown(Key.LeftShift))
            {
                xStratHelper.stratView.DeselectAll();
                Selector.SetIsSelected(this, Selection);
            }
            Selector.SetIsSelected(this, Selection);           

        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!Selection)
            {
                Selector.SetIsSelected(this, true);
            }
        }
    }
}
