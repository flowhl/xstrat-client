using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using xstrat.Json;
using xstrat.StratHelper;
using xstrat.Ui;
using XStrat;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for WallEditorView.xaml
    /// </summary>
    public partial class WallEditorView : UserControl
    {
        private List<XMap> maps = new List<XMap>();

        public ToolTip CurrentToolTip;
        public Brush CurrentBrush = null;
        public bool isMouseDown = false;
        public int BrushSize { get; set; } = 10;

        public bool Floor0 { get; set; }
        public bool Floor1 { get; set; }
        public bool Floor2 { get; set; }
        public bool Floor3 { get; set; }

        public int map_id;
        public int floor_id;

        public WallEditorView()
        {
            InitializeComponent();
            Loaded += WallEditorView_Loaded;
        }

        private void WallEditorView_Loaded(object sender, RoutedEventArgs e)
        {
            //ZoomViewbox.Width = 100;
            //ZoomViewbox.Height = 100;
            xStratHelper.editorView = this;
            xStratHelper.WEMode = true;
            Opened();
        }

        private void Opened()
        {
            ToolTipChanged(View.ToolTip.Cursor);
            UpdateFloorButtons();
            LoadDragItems();
            //ZoomControl.ZoomChanged += ZoomControl_ZoomChanged;
            MouseLeftButtonDown += StratMakerView_MouseLeftButtonDown;
            MouseLeftButtonUp += StratMakerView_MouseLeftButtonUp;
            MapSelector.CBox.SelectionChanged += MapSelector_SelectionChanged;
        }

        private void MapSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectMap();
        }

        #region loading ui

        private void SelectMap()
        {
            if (MapSelector.selectedMap != null)
            {
                map_id = MapSelector.selectedMap.id;
            }
            else
            {
                map_id = -1;
            }
            //LoadMapImages();
        }

        private void LoadMapImages()
        {
            MapStack.Children.Clear();
            if (floor_id < 0) return;
            if (map_id < 0) return;
            int game_id = Globals.games.Where(x => x.name == Globals.TeamInfo.game_name).FirstOrDefault().id;
            var newimage = Globals.GetImageForFloorAndMap(game_id, map_id, floor_id);
            MapStack.Children.Add(newimage);
        }

        #endregion

        private void StratMakerView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            deleteFromCanvasLoop = false;
        }

        private bool deleteFromCanvasLoop = false;

        private async void DeleteFromCanvas()
        {
            while (deleteFromCanvasLoop)
            {
                var dList = DrawingLayer.Children.OfType<UIElement>().Where(x => Math.Abs(Mouse.GetPosition(x).X) < BrushSize && Math.Abs(Mouse.GetPosition(x).Y) < BrushSize).ToList();
                dList.ForEach(x => DrawingLayer.Children.Remove(x));
                await Task.Delay(5);
            }
        }

        private void StratMakerView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            deleteFromCanvasLoop = true;
            if (!DrawingLayer.Children.OfType<StratContentControl>().Where(x => x.IsMouseOver).Any())
            {
                DeselectAll();
            }
            if (CurrentToolTip == View.ToolTip.Eraser)
            {
                DeleteFromCanvas();
            }

        }


        private void ZoomControl_ZoomChanged(object sender, EventArgs e)
        {
            if (ZoomSlider != null)
            {
                //ZoomSlider.Value = ZoomControl.ZoomValue * 100;
            }
        }

        #region Drag Items

        private void LoadDragItems()
        {
            if (Globals.TeamInfo.game_name == "R6 Siege")
            {
                //create wall item
                WallControl newWc = new WallControl();
                newWc.MouseMove += Image_MouseMove;
                newWc.MouseLeftButtonDown += NewImg_MouseLeftButtonDown;
                newWc.Name = GetNextWallID();
                newWc.Margin = new Thickness(10);
                IconsSP.Children.Add(newWc);
            }
        }

        private void NewImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ZoomControl_Drop(object sender, DragEventArgs e)
        {
            try
            {
                var newpos = e.GetPosition(DrawingLayer);

                newpos.X -= 15;
                newpos.Y -= 4.5;

                WallControl newwc = new WallControl();
                newwc.Height = 19;
                newwc.IsHitTestVisible = false;
                newwc.Visibility = Visibility.Visible;

                //Height="9" Width="60" Panel.ZIndex="2"

                StratContentControl newcc = new StratContentControl();
                newcc.Content = newwc;
                newcc.Height = 19;
                newcc.Width = 30;
                newcc.Padding = new Thickness(1);
                newcc.Style = this.FindResource("DesignerItemStyle") as Style;
                newcc.BorderBrush = Brushes.Aqua;
                newcc.BorderThickness = new Thickness(2);

                DrawingLayer.Children.Add(newcc);

                Canvas.SetLeft(newcc, newpos.X);
                Canvas.SetTop(newcc, newpos.Y);

                DeselectAll();
                Selector.SetIsSelected(newcc, true);
                SetBrushToItem();
            }
            catch (Exception ex)
            {
                Notify.sendError("Error creating ContentControl for image: " + ex.Message);
            }
        }

        private void SetBrushToItem()
        {
            var items = DrawingLayer.Children.OfType<StratContentControl>().Where(x => Selector.GetIsSelected(x));
            foreach (var item in items)
            {
                item.BorderBrush = CurrentBrush;
            }
        }

        #endregion

        #region Colors

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

        private void DeselectFloors()
        {
            floor_id = 0;
            Floor0 = false;
            Floor1 = false;
            Floor2 = false;
            Floor3 = false;
        }


        #endregion

        #region ToolTips

        public void ToolTipChanged(ToolTip tip)
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
                default:
                    CurrentToolTip = View.ToolTip.Cursor;
                    DeselectAllToolTips();
                    break;
            }
        }

        private void DeselectAllToolTips()
        {
            BtnCursor.BorderThickness = new Thickness(0);
            BtnEraser.BorderThickness = new Thickness(0);
        }

        #endregion

        private void WallsLayer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void WallsLayer_PreviewMouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(sender as WallControl, sender as WallControl, DragDropEffects.Move);
            }
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

        private void BtnEraser_Click(object sender, RoutedEventArgs e)
        {
            ToolTipChanged(View.ToolTip.Eraser);
        }

        private void BtnFloor0_Click(object sender, RoutedEventArgs e)
        {
            DeselectFloors();
            Floor0 = !Floor0;
            UpdateFloorButtons();
            //ZoomControl.Focus();
            floor_id = 0;
            LoadMapImages();
        }

        private void BtnFloor1_Click(object sender, RoutedEventArgs e)
        {
            DeselectFloors();
            Floor1 = !Floor1;
            UpdateFloorButtons();
            //ZoomControl.Focus();
            floor_id = 1;
            LoadMapImages();
        }

        private void BtnFloor2_Click(object sender, RoutedEventArgs e)
        {
            DeselectFloors();
            Floor2 = !Floor2;
            UpdateFloorButtons();
            //ZoomControl.Focus();
            floor_id = 2;
            LoadMapImages();
        }

        private void BtnFloor3_Click(object sender, RoutedEventArgs e)
        {
            DeselectFloors();
            Floor3 = !Floor3;
            UpdateFloorButtons();
            //ZoomControl.Focus();
            floor_id = 3;
            LoadMapImages();
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if (ZoomControl != null && ZoomSlider.Value != ZoomControl.ZoomValue * 100)
            //{
            //    ZoomControl.Zoom(ZoomSlider.Value);
            //}
        }

        #region Helpers for XStrathelper

        public void PointDragMove(Point p)
        {
            foreach (Control child in DrawingLayer.Children.OfType<Control>())
            {
                if (Selector.GetIsSelected(child))
                {
                    Canvas.SetLeft(child, Canvas.GetLeft(child) + p.X);
                    Canvas.SetTop(child, Canvas.GetTop(child) + p.Y);
                }
            }

        }

        public void DeselectAll()
        {
            foreach (Control child in DrawingLayer.Children.OfType<Control>())
            {
                Selector.SetIsSelected(child, false);
            }
        }

        public void RequestRemove(StratContentControl item)
        {
            DrawingLayer.Children.Remove(item);
        }
        #endregion

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        private string GetNextWallID()
        {
            return "";
        }
    }
}
