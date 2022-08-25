using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for StratMakerView.xaml
    /// </summary>
    public partial class StratMakerView : UserControl
    {
        private List<XMap> maps = new List<XMap>();

        public ToolTip CurrentToolTip;
        public Brush CurrentBrush = null;
        public bool isMouseDown = false;
        public int BrushSize { get; set; } = 15;

        public bool Floor0 { get; set; }
        public bool Floor1 { get; set; }
        public bool Floor2 { get; set; }
        public bool Floor3 { get; set; }

        public StratMakerView()
        {
            InitializeComponent();
            Opened();
        }

        private void Opened()
        {
            _loadMaps();
            ToolTipChanged(View.ToolTip.Cursor);
            LoadColorButtons();
            UpdateFloorButtons();
        }

        private void UpdateFloorButtons()
        {
            if (Floor0)
            {
                BtnFloor0.Background = "#336CB5".ToSolidColorBrush();
            }
            if (Floor1)
            {
                BtnFloor1.Background = "#336CB5".ToSolidColorBrush();
            }
            if (Floor2)
            {
                BtnFloor2.Background = "#336CB5".ToSolidColorBrush();
            }
            if (Floor3)
            {
                BtnFloor3.Background = "#336CB5".ToSolidColorBrush();
            }
            if (!Floor0)
            {
                BtnFloor0.Background = "#202020".ToSolidColorBrush();
            }
            if (!Floor1)
            {
                BtnFloor1.Background = "#202020".ToSolidColorBrush();
            }
            if (!Floor2)
            {
                BtnFloor2.Background = "#202020".ToSolidColorBrush();
            }
            if (!Floor3)
            {
                BtnFloor3.Background = "#202020".ToSolidColorBrush();
            }
        }

        private void LoadColorButtons()
        {
            foreach (var user in Globals.teammates)
            {
                Button newBtn = new Button();
                newBtn.Name = "Color_" + user.name;
                newBtn.Tag = user;
                newBtn.Background = user.color.ToSolidColorBrush();
                newBtn.BorderThickness = new Thickness(0);
                newBtn.BorderBrush = "#336CB5".ToSolidColorBrush();
                newBtn.Height = 25;
                newBtn.Width = 25;
                newBtn.Margin = new Thickness(3);
                newBtn.Click += ColorBtnClicked;

                var icon = new PackIcon();
                icon.Kind = PackIconKind.Brush;
                icon.Foreground = "#202020".ToSolidColorBrush();

                newBtn.Content = icon;

                SPColors.Children.Add(newBtn);
            }

        }

        private void ColorBtnClicked(object sender, RoutedEventArgs e)
        {
            string user = (sender as Button).Name.Replace("Color_", "");
            User teammate = Globals.getUserFromId(Globals.getUserIdFromName(user));
            CurrentBrush = teammate.color.ToSolidColorBrush();
            DeselectAllColors();
            (sender as Button).BorderThickness = new Thickness(1);
            ToolTipChanged(View.ToolTip.Brush);
        }

        private void DeselectAllColors()
        {
            foreach (var item in SPColors.Children)
            {
                (item as Button).BorderThickness = new Thickness(0);
            }
        }

        #region ToolTips

        private void ToolTipChanged(ToolTip tip)
        {
            switch (tip)
            {
                case View.ToolTip.Cursor:
                    CurrentToolTip = View.ToolTip.Cursor;
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                    DeselectAllToolTips();
                    BtnCursor.BorderThickness = new Thickness(1);
                    break;
                case View.ToolTip.Eraser:
                    CurrentToolTip = View.ToolTip.Eraser;
                    DeselectAllToolTips();
                    BtnEraser.BorderThickness = new Thickness(1);
                    break;
                case View.ToolTip.Text:
                    CurrentToolTip = View.ToolTip.Text;
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.IBeam;
                    DeselectAllToolTips();
                    BtnText.BorderThickness = new Thickness(1);
                    break;
                case View.ToolTip.Node:
                    CurrentToolTip = View.ToolTip.Node;
                    DeselectAllToolTips();
                    BtnNodes.BorderThickness = new Thickness(1);
                    break;
                case View.ToolTip.Arrow:
                    CurrentToolTip = View.ToolTip.Arrow;
                    DeselectAllToolTips();
                    BtnArrow.BorderThickness = new Thickness(1);
                    break;
                case View.ToolTip.Circle:
                    CurrentToolTip = View.ToolTip.Circle;
                    DeselectAllToolTips();
                    BtnCircle.BorderThickness = new Thickness(1);
                    break;
                case View.ToolTip.Rectangle:
                    CurrentToolTip = View.ToolTip.Rectangle;
                    DeselectAllToolTips();
                    BtnRectangle.BorderThickness = new Thickness(1);
                    break;
                case View.ToolTip.Brush:
                    CurrentToolTip = View.ToolTip.Brush;
                    DeselectAllToolTips();
                    break;
                default:
                    CurrentToolTip = View.ToolTip.Cursor;
                    DeselectAllToolTips();
                    break;  
            }
            if(tip != View.ToolTip.Brush)
            {
                DeselectAllColors();
            }

        }

        private void DeselectAllToolTips()
        {
            BtnCursor.BorderThickness = new Thickness(0);
            BtnEraser.BorderThickness = new Thickness(0);
            BtnText.BorderThickness = new Thickness(0);
            BtnNodes.BorderThickness = new Thickness(0);
            BtnArrow.BorderThickness = new Thickness(0);
            BtnCircle.BorderThickness = new Thickness(0);
            BtnRectangle.BorderThickness = new Thickness(0);

        }

        #endregion

        private void _loadMaps()
        {
            maps.Add(new XMap());
            UpdateTopBar();
        }

        private void UpdateTopBar()
        {
            var thickness = new Thickness(0,0,0,0);
            foreach (var map in maps)
            {
                var mapItem = new MenuItem();
                //mapItem.Name = map.Name + "MapItem";
                mapItem.BorderThickness = thickness;
                mapItem.Header = map.Name;
                mapItem.Height = 30;
                mapItem.Width = 200;
                
                List<MenuItem> subitems = new List<MenuItem>();
                foreach (var pos in map.positions)
                {
                    var posItem = new MenuItem();
                    //posItem.Name = pos.name + "PositionItem";
                    posItem.Header = pos.name;
                    posItem.BorderThickness = thickness;
                    posItem.Height = 30;
                    posItem.Width = 200;
                    foreach (var strat in pos.strats)
                    {
                        var stratItem = new MenuItem();
                        //stratItem.Name = strat.name + "StratItem";
                        stratItem.BorderThickness = thickness;
                        stratItem.Header = strat.name;
                        stratItem.Height = 30;
                        stratItem.Width = 200;
                        posItem.Items.Add(stratItem);
                    }
                    mapItem.Items.Add(posItem);
                }
                Menu.Items.Add(mapItem);
            }
        }
        private void MapSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {/*
            ImageStack.Children.Clear();
            var index = MapSelector.SelectedIndex;
            var images = StratHandler.getFloorsByListPos(index);
            foreach (var image in images)
            {

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(image.Item2, UriKind.Absolute);
                bi.EndInit();

                Image img = new Image();
                img.Source = bi;
                img.Height = 1080;
                img.Width = 1920;
                ImageStack.Children.Add(img);
            */
        }

        private void WallsLayer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void WallsLayer_PreviewMouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(Alibi, Alibi, DragDropEffects.Move);
            }
        }

        private void ZoomControl_Drop(object sender, DragEventArgs e)
        {

        }

        private void BtnUndo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnRedo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCursor_Click(object sender, RoutedEventArgs e)
        {
            ToolTipChanged(View.ToolTip.Cursor);
        }

        private void BtnBrush_Click(object sender, RoutedEventArgs e)
        {
            ToolTipChanged(View.ToolTip.Brush);
        }

        private void BtnCircle_Click(object sender, RoutedEventArgs e)
        {
            ToolTipChanged(View.ToolTip.Circle);
        }

        private void BtnText_Click(object sender, RoutedEventArgs e)
        {
            ToolTipChanged(View.ToolTip.Text);
        }

        private void BtnNodes_Click(object sender, RoutedEventArgs e)
        {
            ToolTipChanged(View.ToolTip.Node);
        }

        private void BtnEraser_Click(object sender, RoutedEventArgs e)
        {
            ToolTipChanged(View.ToolTip.Eraser);
        }

        private void BtnArrow_Click(object sender, RoutedEventArgs e)
        {
            ToolTipChanged(View.ToolTip.Arrow);
        }

        private void BtnRectangle_Click(object sender, RoutedEventArgs e)
        {
            ToolTipChanged(View.ToolTip.Rectangle);
        }

        private void BtnFloor0_Click(object sender, RoutedEventArgs e)
        {
            Floor0 = !Floor0;
            UpdateFloorButtons();
            ZoomControl.Focus();
        }

        private void BtnFloor1_Click(object sender, RoutedEventArgs e)
        {
            Floor1 = !Floor1;
            UpdateFloorButtons();
            ZoomControl.Focus();
        }

        private void BtnFloor2_Click(object sender, RoutedEventArgs e)
        {
            Floor2 = !Floor2;
            UpdateFloorButtons();
            ZoomControl.Focus();
        }

        private void BtnFloor3_Click(object sender, RoutedEventArgs e)
        {
            Floor3 = !Floor3;
            UpdateFloorButtons();
            ZoomControl.Focus();
        }

        #region drawing

        private void DrawingLayer_MouseMove(object sender, MouseEventArgs e)
        {
            if(CurrentToolTip == View.ToolTip.Brush && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point mousepoint = e.GetPosition(DrawingLayer);
                paintCircle(CurrentBrush, mousepoint);
            }
        }

        private void paintCircle(Brush circleColor, Point position)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = circleColor;
            ellipse.Width = BrushSize / ZoomControl.Zoom;
            ellipse.Height = BrushSize / ZoomControl.Zoom;
            Canvas.SetTop(ellipse, position.Y);
            Canvas.SetLeft(ellipse, position.X);
            DrawingLayer.Children.Add(ellipse);
        }

        #endregion

        private void BrushSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BrushSize = (int)BrushSlider.Value;
        }
    }
    public enum ToolTip
    {
        Cursor, Eraser, Text, Node, Arrow, Circle, Rectangle, Brush
    }
    public class StratContent
    {

    }
}
