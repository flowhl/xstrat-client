using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DiagramDesigner
{
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            StratHelper.window = this;
        }

        private void OnClick(object sender, RoutedEventArgs args)
        {
            CheckBox selectionCheckBox = sender as CheckBox;
            if (selectionCheckBox != null && selectionCheckBox.IsChecked == true)
            {
                foreach (Control child in DesignerCanvas.Children)
                {
                    Selector.SetIsSelected(child, true);
                }
            }
            else
            {
                foreach (Control child in DesignerCanvas.Children)
                {
                    Selector.SetIsSelected(child, false);
                }
            }
        }
        public void DeselectAll()
        {
            foreach (Control child in DesignerCanvas.Children)
            {
                Selector.SetIsSelected(child, false);
            }
        }
        public void DragMove(Point p)
        {
            foreach (Control child in DesignerCanvas.Children)
            {
                if (Selector.GetIsSelected(child))
                {
                    Canvas.SetLeft(child, Canvas.GetLeft(child) + p.X);
                    Canvas.SetTop(child, Canvas.GetTop(child) + p.Y);
                }
            }

        }
    }
}
