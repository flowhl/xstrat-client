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
using xstrat.StratHelper;

namespace XStrat
{
    /// <summary>
    /// Interaction logic for StratContentControl.xaml
    /// </summary>
    public partial class StratContentControl : UserControl
    {
        DateTime LastDown;
        public bool Selection { get; set; }

        public StratContentControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (xStratHelper.WEMode)
            {
                if (xStratHelper.editorView.CurrentToolTip != xstrat.MVVM.View.ToolTip.Cursor) return;
                Selection = !Selection;
                if (!Keyboard.IsKeyDown(Key.LeftShift))
                {
                    xStratHelper.editorView.DeselectAll();
                    Selector.SetIsSelected(this, Selection);
                }
                Selector.SetIsSelected(this, Selection);
                LastDown = DateTime.Now;
            }
            else
            {
                if (xStratHelper.stratView.CurrentToolTip != xstrat.MVVM.View.ToolTip.Cursor) return;
                Selection = !Selection;
                if (!Keyboard.IsKeyDown(Key.LeftShift))
                {
                    xStratHelper.stratView.DeselectAll();
                    Selector.SetIsSelected(this, Selection);
                }
                Selector.SetIsSelected(this, Selection);
                LastDown = DateTime.Now;
            }
        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (xStratHelper.WEMode)
            {
                if (xStratHelper.editorView.CurrentToolTip == xstrat.MVVM.View.ToolTip.Eraser)
                {
                    xStratHelper.editorView.RequestRemove(this);
                    return;
                }
                if (xStratHelper.editorView.CurrentToolTip != xstrat.MVVM.View.ToolTip.Cursor) return;
                if (!Selection)
                {
                    Selector.SetIsSelected(this, true);
                    LastDown = DateTime.Now;
                }
            }
            else
            {
                if (xStratHelper.stratView.CurrentToolTip == xstrat.MVVM.View.ToolTip.Eraser)
                {
                    xStratHelper.stratView.RequestRemove(this);
                    return;
                }
                if (xStratHelper.stratView.CurrentToolTip != xstrat.MVVM.View.ToolTip.Cursor) return;
                if (!Selection)
                {
                    Selector.SetIsSelected(this, true);
                    LastDown = DateTime.Now;
                }
            }

            
        }

        private void UserControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (xStratHelper.WEMode)
            {
                if (xStratHelper.editorView.CurrentToolTip != xstrat.MVVM.View.ToolTip.Cursor) return;
                if (!Selection)
                {
                    if (!Keyboard.IsKeyDown(Key.LeftShift) && (DateTime.Now - LastDown).TotalMilliseconds < 100)
                    {
                        xStratHelper.editorView.DeselectAll();
                        Selector.SetIsSelected(this, true);
                    }
                    Selector.SetIsSelected(this, true);
                }
            }
            else
            {
                if (xStratHelper.stratView.CurrentToolTip != xstrat.MVVM.View.ToolTip.Cursor) return;
                if (!Selection)
                {
                    if (!Keyboard.IsKeyDown(Key.LeftShift) && (DateTime.Now - LastDown).TotalMilliseconds < 100)
                    {
                        xStratHelper.stratView.DeselectAll();
                        Selector.SetIsSelected(this, true);
                    }
                    Selector.SetIsSelected(this, true);
                }
            }

        }

    }
}
