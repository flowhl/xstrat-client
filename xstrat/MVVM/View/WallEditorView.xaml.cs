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

        public int? dropmode;

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
            LoadMapImages();
        }

        private void LoadMapImages()
        {
            MapStack.Children.Clear();
            if (floor_id < 0) return;
            if (map_id < 0) return;
            int game_id = Globals.games.Where(x => x.name == Globals.teamInfo.game_name).FirstOrDefault().id;
            var newimage = Globals.GetImageForFloorAndMap(game_id, map_id, floor_id);
            MapStack.Children.Add(newimage);
            WallObjectsToWalls();
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
            if (Globals.teamInfo.game_name == "R6 Siege")
            {
                var grid = new Grid();
                                
                //create wall item
                WallControl newWc = new WallControl();
                newWc.MouseMove += Image_MouseMove;
                newWc.MouseLeftButtonDown += NewImg_MouseLeftButtonDown;
                newWc.Name = "template";
                newWc.Margin = new Thickness(10);
                newWc.HorizontalAlignment = HorizontalAlignment.Center;
                newWc.VerticalAlignment = VerticalAlignment.Center;
                newWc.Height = 19;
                newWc.Width = 60;
                newWc.isLocked = true;

                grid.Children.Add(newWc);                

                IconsSP.Children.Add(grid);

                var grid2 = new Grid();

                //create wall item
                HatchControl newHc = new HatchControl();
                newHc.MouseMove += Image_MouseMove;
                newHc.MouseLeftButtonDown += NewImg_MouseLeftButtonDown;
                newHc.Name = "template";
                newHc.Margin = new Thickness(10);
                newHc.HorizontalAlignment = HorizontalAlignment.Center;
                newHc.VerticalAlignment = VerticalAlignment.Center;
                newHc.Height = 50;
                newHc.Width = 50;
                newHc.isLocked = true;

                grid2.Children.Add(newHc);

                IconsSP.Children.Add(grid2);
            }
        }

        private void NewImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ZoomControl_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if(dropmode == 0 )
                {

                    var newpos = e.GetPosition(DrawingLayer);

                    newpos.X -= 15;
                    newpos.Y -= 4.5;

                    string nameToSet = GetNextWallID();

                    WallControl newwc = new WallControl();
                    newwc.Height = 19;
                    newwc.IsHitTestVisible = false;
                    newwc.Name = nameToSet;

                    //Height="9" Width="60" Panel.ZIndex="2"

                    StratContentControl newcc = new StratContentControl();
                    newcc.Content = newwc;
                    newcc.Height = 19;
                    newcc.Width = 30;
                    newcc.Padding = new Thickness(1);
                    newcc.Style = this.FindResource("DesignerItemStyle") as Style;
                    newcc.BorderBrush = Brushes.Aqua;
                    newcc.BorderThickness = new Thickness(2);
                    newcc.Name = "SCC_" + nameToSet;

                    DrawingLayer.Children.Add(newcc);

                    Canvas.SetLeft(newcc, newpos.X);
                    Canvas.SetTop(newcc, newpos.Y);

                    DeselectAll();
                    Selector.SetIsSelected(newcc, true);
                    SetBrushToItem();
                }
                if(dropmode == 1)
                {
                    var newpos = e.GetPosition(DrawingLayer);

                    newpos.X -= 15;
                    newpos.Y -= 4.5;

                    string nameToSet = GetNextWallID();

                    HatchControl newwc = new HatchControl();
                    newwc.IsHitTestVisible = false;
                    newwc.Name = nameToSet;

                    //Height="9" Width="60" Panel.ZIndex="2"

                    StratContentControl newcc = new StratContentControl();
                    newcc.Content = newwc;
                    newcc.Height = 86;
                    newcc.Width = 86;
                    newcc.Padding = new Thickness(1);
                    newcc.Style = this.FindResource("DesignerItemStyle") as Style;
                    newcc.BorderBrush = Brushes.Aqua;
                    newcc.BorderThickness = new Thickness(0);
                    newcc.Name = "SCC_" + nameToSet;

                    DrawingLayer.Children.Add(newcc);

                    Canvas.SetLeft(newcc, newpos.X);
                    Canvas.SetTop(newcc, newpos.Y);

                    DeselectAll();
                    Selector.SetIsSelected(newcc, true);
                    SetBrushToItem();
                }
                dropmode = null;
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
                if(sender is WallControl)
                {
                    dropmode = 0;
                    DragDrop.DoDragDrop(sender as WallControl, sender as WallControl, DragDropEffects.Move);
                }
                if(sender is HatchControl)
                {
                    dropmode = 1;
                    DragDrop.DoDragDrop(sender as HatchControl, sender as HatchControl, DragDropEffects.Move);
                }
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
            floor_id = 0;
            LoadMapImages();
        }

        private void BtnFloor1_Click(object sender, RoutedEventArgs e)
        {
            DeselectFloors();
            Floor1 = !Floor1;
            UpdateFloorButtons();
            floor_id = 1;
            LoadMapImages();
        }

        private void BtnFloor2_Click(object sender, RoutedEventArgs e)
        {
            DeselectFloors();
            Floor2 = !Floor2;
            UpdateFloorButtons();
            floor_id = 2;
            LoadMapImages();
        }

        private void BtnFloor3_Click(object sender, RoutedEventArgs e)
        {
            DeselectFloors();
            Floor3 = !Floor3;
            UpdateFloorButtons();
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

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            WallObjectsToWalls();
        }
        private string GetNextWallID()
        {
            int uid = DrawingLayer.Children.OfType<StratContentControl>().Count();
            return "id_" + map_id + "_" + floor_id + "_" + uid;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            xStratHelper.SaveWallObjects(WallsToWallObjects(), map_id, floor_id);
            Notify.sendSuccess("Saved successfully");
        }

        private List<WallPositionObject> WallsToWallObjects()
        {
            List<WallPositionObject> wallObjects = new List<WallPositionObject>();

            foreach (var item in DrawingLayer.Children.OfType<StratContentControl>())
            {
                int type = 0;
                if (item.Content is HatchControl) type = 1;
                var obj = new WallPositionObject { position_x = Canvas.GetLeft(item), type = type, position_y = Canvas.GetTop(item), rotate = item.RenderTransform as RotateTransform, uid = item.Name.Replace("SCC_", ""), width = item.Width };
                wallObjects.Add(obj);
            }

            return wallObjects;
        }

        private void WallObjectsToWalls()
        {
            var objects = xStratHelper.GetWallObjects(map_id, floor_id);
            DrawingLayer.Children.Clear();
            foreach (var obj in objects)
            {
                try
                {
                    if(obj.type == 0)
                    {
                        var newpos = new Point();
                        newpos.X = obj.position_x;
                        newpos.Y = obj.position_y;

                        WallControl newwc = new WallControl();
                        newwc.Height = 19;
                        newwc.IsHitTestVisible = false;
                        newwc.Visibility = Visibility.Visible;
                        newwc.Name = obj.uid.Replace("SCC_", "");

                        StratContentControl newcc = new StratContentControl();
                        newcc.Content = newwc;
                        newcc.Height = 19;
                        newcc.Width = obj.width;
                        newcc.Padding = new Thickness(1);
                        newcc.Style = this.FindResource("DesignerItemStyle") as Style;
                        newcc.BorderBrush = Brushes.Aqua;
                        newcc.BorderThickness = new Thickness(2);
                        newcc.RenderTransform = obj.rotate;
                        newcc.Name = "SCC_" + obj.uid.Replace("SCC_", "");

                        DrawingLayer.Children.Add(newcc);

                        Canvas.SetLeft(newcc, newpos.X);
                        Canvas.SetTop(newcc, newpos.Y);

                        DeselectAll();
                        Selector.SetIsSelected(newcc, true);
                        SetBrushToItem();
                    }
                    if(obj.type == 1)
                    {
                        var newpos = new Point();
                        newpos.X = obj.position_x;
                        newpos.Y = obj.position_y;

                        HatchControl newhc = new HatchControl();
                        newhc.IsHitTestVisible = false;
                        newhc.Name = obj.uid.Replace("SCC_", "");

                        StratContentControl newcc = new StratContentControl();
                        newcc.Content = newhc;
                        newcc.Height = 86;
                        newcc.Width = 86;
                        newcc.Width = obj.width;
                        newcc.Padding = new Thickness(1);
                        newcc.Style = this.FindResource("DesignerItemStyle") as Style;
                        newcc.BorderBrush = Brushes.Aqua;
                        newcc.BorderThickness = new Thickness(2);
                        newcc.RenderTransform = obj.rotate;
                        newcc.Name = "SCC_" + obj.uid.Replace("SCC_", "");

                        DrawingLayer.Children.Add(newcc);

                        Canvas.SetLeft(newcc, newpos.X);
                        Canvas.SetTop(newcc, newpos.Y);

                        DeselectAll();
                        Selector.SetIsSelected(newcc, true);
                        SetBrushToItem();
                    }
                }
                catch (Exception ex)
                {
                    Notify.sendError("Error creating ContentControl for image: " + ex.Message);
                }
            }
        } 
    }

    public class WallPositionObject{
        public string uid { get; set; }
        public int type { get; set; }
        public double position_x { get; set; }
        public double position_y { get; set; }
        public RotateTransform rotate { get; set; }
        public double width { get; set; }
    }
}
