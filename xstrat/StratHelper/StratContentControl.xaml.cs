using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using xstrat.Core;
using xstrat.StratHelper;

namespace XStrat
{
    /// <summary>
    /// Interaction logic for StratContentControl.xaml
    /// </summary>
    public partial class StratContentControl : UserControl
    {
        public bool IsDragging { get; set; }
        Point Start { get; set; }
        public bool Selection { get; set; }
        public bool Locked { get; set; } = false;
        public string UserID { get; set; }

        public StratContentControl()
        {
            InitializeComponent();
            this.PreviewMouseLeftButtonUp += StratContentControl_MouseLeftButtonDown;
            this.PreviewMouseDown += StratContentControl_PreviewMouseDown;
        }

        private void StratContentControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsDragging = false;
            Start = Mouse.GetPosition(Globals.wnd);
        }

        private void StratContentControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var End = Mouse.GetPosition(Globals.wnd);

            IsDragging = (End - Start).Length > 10;
            if(!IsDragging) HandleClick();
        }

        private void HandleClick()
        {
            if (Locked) return;

            if (xStratHelper.stratView.CurrentToolTip == xstrat.MVVM.View.ToolTip.Eraser)
            {
                xStratHelper.stratView.RequestRemove(this);
                return;
            }
            if (xStratHelper.stratView.CurrentToolTip != xstrat.MVVM.View.ToolTip.Cursor) return;
            if (!Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                xStratHelper.stratView.DeselectAll();
                Selector.SetIsSelected(this, true);
            }
            else
            {
                Selector.SetIsSelected(this, true);
            }
        }

        //private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if(Locked) return;

        //    if (xStratHelper.stratView.CurrentToolTip != xstrat.MVVM.View.ToolTip.Cursor) return;
        //    Selection = !Selection;
        //    if (!Keyboard.IsKeyDown(Key.LeftShift))
        //    {
        //        xStratHelper.stratView.DeselectAll();
        //        Selector.SetIsSelected(this, Selection);
        //    }
        //    Selector.SetIsSelected(this, Selection);
        ////    LastDown = DateTime.Now;

        //}

        //private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
            
        //}

    }
}
