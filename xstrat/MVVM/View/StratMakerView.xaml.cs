using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using XStrat;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for StratMakerView.xaml
    /// </summary>
    public partial class StratMakerView : UserControl
    {
        private List<XMap> maps = new List<XMap>();

        Image draggedItem;

        public ToolTip CurrentToolTip;
        public Brush CurrentBrush = null;
        public bool isMouseDown = false;
        public int BrushSize { get; set; } = 10;

        public bool Floor0 { get; set; }
        public bool Floor1 { get; set; }
        public bool Floor2 { get; set; }
        public bool Floor3 { get; set; }

        public StratMakerView()
        {
            InitializeComponent();
            Loaded += StratMakerView_Loaded;
        }

        private void StratMakerView_Loaded(object sender, RoutedEventArgs e)
        {
            xStratHelper.stratView = this;
            xStratHelper.WEMode = false;
            Opened();
        }

        private void Opened()
        {
            _loadMaps();
            ToolTipChanged(View.ToolTip.Cursor);
            LoadColorButtons();
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
            string name = StratFileHelper.GetImagePathForSpot(MapSelector.selectedMap.id, 1);
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
            if(!DrawingLayer.Children.OfType<StratContentControl>().Where(x => x.IsMouseOver).Any())
            {
                DeselectAll();
            }
            if(CurrentToolTip == View.ToolTip.Eraser)
            {
                DeleteFromCanvas();
            }

        }

        #region Drag Items

        private void LoadDragItems()
        {
            if(Globals.TeamInfo.game_name == "R6 Siege")
            {
                IconsSP.Children.Clear();
                string folder = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"/Images/R6_Icons/";
                var allItems = Directory.GetFiles(folder, "*.png").ToList();
                foreach(var item in allItems)
                {
                    if (item == null || item == "") continue;
                    //<Image x:Name="Alibi" Source="/Images/R6_Icons/g_Alibi.png" MouseMove="Image_MouseMove"/>
                    Image newImg = new Image();
                    newImg.Source = new BitmapImage(new Uri(item, UriKind.Absolute));
                    newImg.MouseMove += Image_MouseMove;
                    newImg.MouseLeftButtonDown += NewImg_MouseLeftButtonDown;
                    newImg.Name = System.IO.Path.GetFileName(item.ToString()).Replace(".png", "");
                    newImg.Margin = new Thickness(10);
                    IconsSP.Children.Add(newImg);
                }
            }
        }
        
        private void NewImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedItem = sender as Image;
        }

        private void ZoomControl_Drop(object sender, DragEventArgs e)
        {
            if (draggedItem == null) return;
            try
            {

                var newpos = e.GetPosition(DrawingLayer);

                newpos.X -= 25;
                newpos.Y -= 25;

                Image newimg = new Image();
                newimg.IsHitTestVisible = false;
                newimg.Source = draggedItem.Source;

                StratContentControl newcc = new StratContentControl();
                newcc.Content = newimg;
                newcc.Height = 50;
                newcc.Width = 50;
                newcc.Padding = new Thickness(1);
                newcc.Style = this.FindResource("DesignerItemStyle") as Style;
                newcc.BorderBrush = Brushes.Transparent;
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
            draggedItem = null;
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
            var firstButton = SPColors.Children.OfType<Button>().FirstOrDefault();
            if (firstButton != null) ColorBtnClicked(firstButton, RoutedEventArgs.Empty as RoutedEventArgs);

        }

        private void ColorBtnClicked(object sender, RoutedEventArgs e)
        {
            string user = (sender as Button).Name.Replace("Color_", "");
            User teammate = Globals.getUserFromId(Globals.getUserIdFromName(user));
            CurrentBrush = teammate.color.ToSolidColorBrush();
            DeselectAllColors();
            (sender as Button).BorderThickness = new Thickness(2);
            SetBrushToItem();
        }

        public void DeselectAllColors()
        {
            foreach (var item in SPColors.Children)
            {
                (item as Button).BorderThickness = new Thickness(0);
            }
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
        }

        private void DeselectAllToolTips()
        {
            BtnCursor.BorderThickness = new Thickness(0);
            BtnBrush.BorderThickness = new Thickness(0);
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
            UpdateTopBar();
        }

        private void UpdateTopBar()
        {
            var thickness = new Thickness(0, 0, 0, 0);
            foreach (var map in Globals.Maps)
            {
                var mapItem = new MenuItem();
                //mapItem.Name = map.name + "_MapItem";
                mapItem.Header = map.name;
                mapItem.Template = Application.Current.Resources["Menu_SubMenu_Template"] as ControlTemplate;
                List<MenuItem> subitems = new List<MenuItem>();
                foreach (var pos in Globals.xPositions.Where(x => x.map_id == map.id))
                {
                    var posItem = new MenuItem();
                    //posItem.Name = pos.name + "PositionItem";
                    posItem.Header = pos.name;
                    posItem.Template = Application.Current.Resources["Menu_SubMenu_Template"] as ControlTemplate;
                    foreach (var strat in Globals.strats.Where(x => x.position_id == pos.id))
                    {
                        var stratItem = new MenuItem();
                        stratItem.Header = strat.name;
                        stratItem.Tag = strat;
                        stratItem.Template = Application.Current.Resources["Item_Template"] as ControlTemplate;
                        stratItem.Click += StratItem_Click;
                        posItem.Items.Add(stratItem);
                    }
                    mapItem.Items.Add(posItem);
                }
                Menu.Items.Add(mapItem);
            }
        }

        private void StratItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem sendObj = sender as MenuItem;
            if (sendObj == null) return;
            Strat strat = sendObj.Tag as Strat;
        }


        //private void MapSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{/*
        //    ImageStack.Children.Clear();
        //    var index = MapSelector.SelectedIndex;
        //    var images = StratHandler.getFloorsByListPos(index);
        //    foreach (var image in images)
        //    {

        //        BitmapImage bi = new BitmapImage();
        //        bi.BeginInit();
        //        bi.UriSource = new Uri(image.Item2, UriKind.Absolute);
        //        bi.EndInit();

        //        Image img = new Image();
        //        img.Source = bi;
        //        img.Height = 1080;
        //        img.Width = 1920;
        //        ImageStack.Children.Add(img);
        //    */
        //}

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
                DragDrop.DoDragDrop(sender as Image, sender as Image, DragDropEffects.Move);
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
            ellipse.Width = BrushSize;
            ellipse.Height = BrushSize;
            Canvas.SetTop(ellipse, position.Y);
            Canvas.SetLeft(ellipse, position.X);
            DrawingLayer.Children.Add(ellipse);
        }

        #endregion

        private void BrushSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BrushSize = (int)BrushSlider.Value;
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
    }
    public enum ToolTip
    {
        Cursor, Eraser, Text, Node, Arrow, Circle, Rectangle, Brush
    }

    public class StratContent
    {
        public List<WallObj> wallstatus { get; set; }
        public List<DragNDropObj> dragNDropObjs { get; set; }
        public string comment { get; set; }
        public List<int> floors { get; set; }

        public static StratContent Empyt = new StratContent();
        
        public StratContent()
        {
            wallstatus = new List<WallObj>();
            dragNDropObjs = new List<DragNDropObj>();
            comment = "";
            floors = new List<int>();
        }
    }

    public class WallObj
    {
        public int wallID { get; set; }
        public int user_id { get; set; }
        public Wallstates[] states { get; set; }
    }

    public class DragNDropObj
    {
        public Position pos { get; set; }
        public int user_id { get; set; }
        public string type { get; set; }
        public string image { get; set; }
        public double diameter { get; set; }
    }
    

}
