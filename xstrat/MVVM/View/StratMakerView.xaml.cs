using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.Xml;
using xstrat.Core;
using xstrat.Json;
using xstrat.StratHelper;
using xstrat.Ui;
using XStrat;
using static System.Windows.Forms.AxHost;
using System.ComponentModel;
using System.Data;
using JetBrains.Annotations;
using SkiaSharp;
using static WPFSpark.MonitorHelper;
using Microsoft.Win32;
using Path = System.Windows.Shapes.Path;
using System.Runtime.CompilerServices;
using System.Drawing;
using Point = System.Windows.Point;
using Image = System.Windows.Controls.Image;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Rectangle = System.Windows.Shapes.Rectangle;
using xstrat.Models.Supabase;
using Newtonsoft.Json;
using static SkiaSharp.HarfBuzz.SKShaper;
using LiveChartsCore.Kernel;
using System.Net;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for StratMakerView.xaml
    /// </summary>
    public partial class StratMakerView : StateUserControl
    {
        private List<XMap> maps = new List<XMap>();

        public Models.Supabase.Strat CurrentStrat { get; set; }

        Image draggedItem;

        public ToolTip CurrentToolTip
        {

            get
            {
                return StratMakerToolTipHelper.CurrentToolTip;
            }
            set
            {
                StratMakerToolTipHelper.CurrentToolTip = value;
            }
        }
        public Brush CurrentBrush = null;
        public string CurrentBrushUser
        {
            get
            {
                return StratMakerToolTipHelper.CurrentBrushUser;
            }
            set
            {
                StratMakerToolTipHelper.CurrentBrushUser = value;
            }
        }
        public bool isMouseDown = false;
        public int BrushSize { get; set; } = 10;

        private bool floor0;
        public bool Floor0
        {
            get { return floor0; }
            set
            {
                floor0 = value;
                HasChanges = true;
            }
        }

        private bool floor1;
        public bool Floor1
        {
            get { return floor1; }
            set
            {
                floor1 = value;
                HasChanges = true;
            }
        }

        private bool floor2;
        public bool Floor2
        {
            get { return floor2; }
            set
            {
                floor2 = value;
                HasChanges = true;
            }
        }

        private bool floor3;
        public bool Floor3
        {
            get { return floor3; }
            set
            {
                floor3 = value;
                HasChanges = true;
            }
        }


        public string map_id;
        public string pos_id;

        public double TranslateXLast { get; set; }
        public double TranslateYLast { get; set; }

        public double IconSize { get; set; }

        public StratMakerView()
        {
            InitializeComponent();
            Loaded += StratMakerView_Loaded;
            HasChanges = false;
        }

        private void StratMakerView_Loaded(object sender, RoutedEventArgs e)
        {
            xStratHelper.stratView = this;
            xStratHelper.WEMode = false;

            Globals.wnd.KeyDown += KeyDown;
            CurrentToolTip = View.ToolTip.Cursor;
            CurrentBrushUser = null;
            Opened();
        }

        private void Opened()
        {
            UpdateTopBar();
            ToolTipChanged(View.ToolTip.Cursor);
            LoadColorButtons();
            UpdateFloorButtons();
            LoadDragItems();
            ZoomControl.MouseLeftButtonDown += ZC_MouseLeftButtonDown;
        }

        public Point? ClickPoint1 = null;

        private void ZC_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point MousePosition = e.GetPosition(DrawingLayer);
            MouseClickedOnCanvas(MousePosition);
        }

        public void MouseClickedOnCanvas(Point MousePosition)
        {

            //catch when chlick
            var mouseover = DrawingLayer.Children.OfType<StratContentControl>().Where(x => x.IsMouseCaptureWithin);
            if (mouseover != null && mouseover.Count() > 0) return;

            //cursor
            if (CurrentToolTip == View.ToolTip.Cursor) DeselectAll();

            //text
            if (CurrentToolTip == View.ToolTip.Text)
            {
                CreateText(MousePosition);
            }

            //Arrow
            if (CurrentToolTip == View.ToolTip.Arrow)
            {
                CreateArrow(MousePosition);
            }

            //Rectangle
            if (CurrentToolTip == View.ToolTip.Rectangle)
            {
                CreateRectangle(MousePosition);
            }

            //Circle
            if (CurrentToolTip == View.ToolTip.Circle)
            {
                CreateCircle(MousePosition);
            }
            HasChanges = true;
        }

        public void CreateText(Point MousePosition)
        {
            TextControl txt = new TextControl();
            txt.MainContent.Text = "Enter Text";

            StratContentControl newcc = new StratContentControl();
            newcc.Content = txt;
            newcc.Height = 100;
            newcc.Width = 300;
            newcc.Padding = new Thickness(1);
            newcc.Style = this.FindResource("DesignerItemStyle") as Style;
            newcc.BorderBrush = Brushes.Transparent;
            newcc.BorderThickness = new Thickness(2);
            newcc.Tag = CurrentBrushUser;

            DrawingLayer.Children.Add(newcc);

            Canvas.SetLeft(newcc, MousePosition.X - 150);
            Canvas.SetTop(newcc, MousePosition.Y - 50);
        }

        public void CreateArrow(Point MousePosition)
        {
            if (ClickPoint1 == null)
            {
                ClickPoint1 = MousePosition;
            }
            else
            {
                Path arrow = CreateArrow(ClickPoint1.GetValueOrDefault(), MousePosition, 20);

                // Add the arrow to the canvas
                DrawingLayer.Children.Add(arrow);

                arrow.PreviewMouseLeftButtonDown += Arrow_MouseLeftButtonDown;

                ClickPoint1 = null;
            }
        }

        public void CreateRectangle(Point MousePosition)
        {
            if (ClickPoint1 == null)
            {
                ClickPoint1 = MousePosition;
            }
            else
            {
                Point startPoint = ClickPoint1.GetValueOrDefault();
                Point endPoint = MousePosition;

                // calculate the minimum and maximum X and Y values
                double minX = Math.Min(startPoint.X, endPoint.X);
                double maxX = Math.Max(startPoint.X, endPoint.X);
                double minY = Math.Min(startPoint.Y, endPoint.Y);
                double maxY = Math.Max(startPoint.Y, endPoint.Y);

                var scc = new StratContentControl();

                scc.Padding = new Thickness(1);
                scc.Style = this.FindResource("DesignerItemStyle") as Style;
                scc.BorderBrush = Brushes.Transparent;
                scc.BorderThickness = new Thickness(2);
                scc.UserID = CurrentBrushUser;

                // create a new rectangle object
                Rectangle rectangle = new Rectangle();
                rectangle.IsHitTestVisible = false;

                // set the width and height of the rectangle
                scc.Width = maxX - minX;
                scc.Height = maxY - minY;

                scc.Content = rectangle;


                // Add the rect to the canvas
                DrawingLayer.Children.Add(scc);

                // set the position of the rectangle on the canvas
                Canvas.SetLeft(scc, minX);
                Canvas.SetTop(scc, minY);

                // set the fill color of the rectangle
                //rectangle.Fill = CurrentBrush;
                rectangle.StrokeThickness = 4;
                rectangle.Stroke = CurrentBrush;
                rectangle.Tag = CurrentBrushUser;

                ClickPoint1 = null;
            }
        }

        public void CreateCircle(Point MousePosition)
        {
            if (ClickPoint1 == null)
            {
                ClickPoint1 = MousePosition;
            }
            else
            {
                Point startPoint = ClickPoint1.GetValueOrDefault();
                Point endPoint = MousePosition;

                double radius = Math.Max(Math.Abs(startPoint.X - endPoint.X), Math.Abs(startPoint.Y - endPoint.Y));

                var scc = new StratContentControl();

                scc.Padding = new Thickness(1);
                scc.Style = this.FindResource("DesignerItemStyle") as Style;
                scc.BorderBrush = Brushes.Transparent;
                scc.BorderThickness = new Thickness(2);

                Ellipse circle = new Ellipse();
                circle.IsHitTestVisible = false;

                scc.Content = circle;

                scc.Width = radius * 2;
                scc.Height = radius * 2;


                // set the fill color of the rectangle
                //circle.Fill = CurrentBrush;
                circle.StrokeThickness = 4;
                circle.Stroke = CurrentBrush;
                circle.Tag = CurrentBrushUser;
                scc.Tag = CurrentBrushUser;
                scc.UserID = CurrentBrushUser;

                // Add the circle to the canvas
                DrawingLayer.Children.Add(scc);

                // set the position of the rectangle on the canvas
                Canvas.SetLeft(scc, startPoint.X - radius);
                Canvas.SetTop(scc, startPoint.Y - radius);

                ClickPoint1 = null;
            }
        }


        private void Arrow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolTip == View.ToolTip.Eraser)
            {
                DrawingLayer.Children.Remove(sender as Path);
            }
        }


        #region loading ui

        private void LoadMapImages()
        {
            MapStack.Children.Clear();
            WallsLayer.Children.Clear();
            DrawingLayer.Children.Clear();

            if (map_id.IsNullOrEmpty()) return;

            List<int> floors = new List<int>();
            if (Floor0) floors.Add(0);
            if (Floor1) floors.Add(1);
            if (Floor2) floors.Add(2);
            if (Floor3) floors.Add(3);

            string game_id = DataCache.CurrentTeam.GameID;

            double offset = 0;

            foreach (var floor in floors)
            {
                XmlDocument svgFile;


                svgFile = Globals.GetSCVDocumentForMapAndFloor(map_id, floor);

                if (svgFile == null) continue;

                var SVGContent = Globals.GetSvgContent(svgFile, game_id, map_id, floor);

                var newimage = Globals.GetImageForSVG(SVGContent);

                if (newimage != null)
                {
                    //add image
                    MapStack.Children.Add(newimage);
                    Canvas.SetLeft(newimage, offset);

                    //add Walls
                    CreateWallsBeta(SVGContent, offset);

                    offset += SVGContent.ViewBoxDimensions.X;
                }


                //    var newimage = Globals.GetImageForFloorAndMap(game_id, map_id, floor);
                //    MapStack.Children.Add(newimage);

                //    WallsLayer.Children.Clear();

                //    //add walls here
                //    //var objects = xStratHelper.GetWallObjects(map_id, floor);
                //    var objects = null;
                //    foreach (var obj in objects)
                //    {
                //        try
                //        {
                //            if (obj.type == 0)
                //            {
                //                var newpos = new Point();
                //                newpos.X = obj.position_x + offset.X;
                //                newpos.Y = obj.position_y + offset.Y;

                //                WallControl newwc = new WallControl();
                //                newwc.Height = 19;
                //                newwc.Name = obj.uid;
                //                newwc.Width = obj.width;
                //                newwc.RenderTransform = obj.rotate;

                //                WallsLayer.Children.Add(newwc);

                //                Canvas.SetLeft(newwc, newpos.X);
                //                Canvas.SetTop(newwc, newpos.Y);

                //                SetBrushToItem();
                //            }
                //            if (obj.type == 1)
                //            {
                //                var newpos = new Point();
                //                newpos.X = obj.position_x + offset.X;
                //                newpos.Y = obj.position_y + offset.Y;

                //                HatchControl newhc = new HatchControl();
                //                newhc.Name = obj.uid;
                //                newhc.RenderTransform = obj.rotate;
                //                newhc.Height = 86;
                //                newhc.Width = 86;

                //                WallsLayer.Children.Add(newhc);

                //                Canvas.SetLeft(newhc, newpos.X);
                //                Canvas.SetTop(newhc, newpos.Y);

                //                SetBrushToItem();
                //            }
                //        }
                //        catch (Exception ex)
                //        {
                //            Notify.sendError("Error creating ContentControl for image: " + ex.Message);
                //        }
                //    }

                //    offset.X += 4000 / 1.3;
                //    DeselectAll();
            }

        }

        private void CreateWallsBeta(SvgContent svg, double offset = 0)
        {
            if (svg == null) return;

            var Walls = svg.Rects.Where(x => x.style.Contains("#linear-gradient"));

            foreach (var Wall in Walls)
            {
                if (Globals.isWithin5Percent(Wall.width, Wall.height))
                {
                    //Hatch
                    HatchControl hatch = new HatchControl();

                    Point pos = new Point(Wall.x, Wall.y);

                    hatch.Width = Wall.width;
                    hatch.Height = Wall.height;

                    hatch.Name = Wall.uid;

                    //transform
                    if (Wall.hasTransform)
                    {
                        hatch.RenderTransformOrigin = new Point(0.5, 0.5);
                        hatch.RenderTransform = new RotateTransform(Wall.rotation);
                    }

                    WallsLayer.Children.Add(hatch);

                    Canvas.SetLeft(hatch, pos.X + offset);
                    Canvas.SetTop(hatch, pos.Y);
                }
                else
                {
                    //Wall

                    WallControl wall = new WallControl();

                    Point pos = new Point(Wall.x, Wall.y);

                    wall.Width = Wall.width;
                    wall.Height = Wall.height;

                    wall.Name = Wall.uid;

                    //transform
                    if (Wall.hasTransform)
                    {
                        wall.RenderTransformOrigin = new Point(0.5, 0.5);
                        wall.RenderTransform = new RotateTransform(Wall.rotation);
                    }

                    WallsLayer.Children.Add(wall);

                    Canvas.SetLeft(wall, pos.X + offset);
                    Canvas.SetTop(wall, pos.Y);
                }
            }
        }

        #endregion

        private void StratMakerView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            deleteFromCanvasLoop = false;
        }

        private bool deleteFromCanvasLoop = false;

        private void DeleteFromCanvas(Point point)
        {
            var dList = DrawingLayer.Children.OfType<UIElement>().Where(x => Math.Abs(Mouse.GetPosition(x).X) < BrushSize && Math.Abs(Mouse.GetPosition(x).Y) < BrushSize).ToList();
            dList.ForEach(x => DrawingLayer.Children.Remove(x));
        }

        #region Drag Items

        private void LoadDragItems()
        {
            if (DataCache.CurrentTeam.GameID == DataCache.CurrentGames.Where(x => x.Name == "Rainbow Six Siege").FirstOrDefault().Id)
            {
                IconsSP.Children.Clear();
                string folder = Globals.XStratInstallPath + @"/Images/Icons/";
                var allItems = Directory.GetFiles(folder, "*.png").Where(x => x != null && System.IO.Path.GetFileName(x).StartsWith("r6_")).ToList();
                foreach (var item in allItems)
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

                newpos.X -= IconSize / 2;
                newpos.Y -= IconSize / 2;

                Image newimg = new Image();
                newimg.IsHitTestVisible = false;
                newimg.Source = draggedItem.Source;
                newimg.Tag = CurrentBrushUser;

                var border = new Border();
                border.BorderThickness = new Thickness(2);
                border.BorderBrush = CurrentBrush;
                border.Tag = CurrentBrushUser;
                border.Child = newimg;

                StratContentControl newcc = new StratContentControl();
                newcc.Content = border;
                newcc.Height = IconSize;
                newcc.Width = IconSize;
                newcc.Padding = new Thickness(1);
                newcc.Style = this.FindResource("DesignerItemStyle") as Style;
                newcc.BorderBrush = CurrentBrush;
                newcc.BorderThickness = new Thickness(2);
                newcc.Tag = CurrentBrushUser;

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
            HasChanges = true;
        }

        private void SetBrushToItem()
        {
            var items = DrawingLayer.Children.OfType<UIElement>().Where(x => Selector.GetIsSelected(x));
            foreach (var item in items)
            {
                // Image or Text
                if (item is StratContentControl)
                {
                    if ((item as StratContentControl).Content is Border)
                    {
                        ((item as StratContentControl).Content as Border).BorderBrush = CurrentBrush;
                        ((item as StratContentControl).Content as Border).Tag = CurrentBrushUser;
                        (item as StratContentControl).UserID = CurrentBrushUser;
                        (item as StratContentControl).Tag = CurrentBrushUser;
                    }
                    if ((item as StratContentControl).Content is TextControl)
                    {
                        TextControl textControl = (TextControl)(item as StratContentControl).Content;
                        textControl.BorderBrush = CurrentBrush;
                        (item as StratContentControl).UserID = CurrentBrushUser;
                        (item as StratContentControl).Tag = CurrentBrushUser;
                        textControl.Tag = CurrentBrushUser;
                    }
                    if ((item as StratContentControl).Content is Ellipse)
                    {
                        var ellipse = (Ellipse)(item as StratContentControl).Content;
                        ellipse.Tag = CurrentBrushUser;
                        ellipse.Stroke = CurrentBrush;
                        (item as StratContentControl).UserID = CurrentBrushUser;
                        (item as StratContentControl).Tag = CurrentBrushUser;
                    }
                    if ((item as StratContentControl).Content is Rectangle)
                    {
                        var rect = (Rectangle)(item as StratContentControl).Content;
                        rect.Tag = CurrentBrushUser;
                        rect.Stroke = CurrentBrush;
                        (item as StratContentControl).UserID = CurrentBrushUser;
                        (item as StratContentControl).Tag = CurrentBrushUser;
                    }

                }

                //Drawing
                if (item is Ellipse)
                {
                    var ellipse = (Ellipse)item;
                    ellipse.Tag = CurrentBrushUser;
                    ellipse.Stroke = CurrentBrush;
                }

                //Arrow
                if (item is Path)
                {
                    var path = (Path)item;
                    path.Tag = CurrentBrushUser;
                    path.Stroke = CurrentBrush;
                    path.Fill = CurrentBrush;
                }
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
            foreach (var teamMate in DataCache.CurrentTeamMates)
            {
                Button newBtn = new Button();
                newBtn.Name = "Color_" + Globals.RemoveIllegalCharactersFromName(teamMate.Name.RemoveAllWhitespace());
                newBtn.Tag = teamMate;
                newBtn.Background = teamMate.Color.ToSolidColorBrush();
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
            UserData teammate = DataCache.CurrentTeamMates.Where(x => Globals.RemoveIllegalCharactersFromName(x.Name).RemoveAllWhitespace() == user).FirstOrDefault();
            CurrentBrush = teammate.Color.ToSolidColorBrush();
            CurrentBrushUser = teammate.Id.ToString();
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
            ClickPoint1 = null;

            switch (tip)
            {
                case View.ToolTip.Cursor:
                    CurrentToolTip = View.ToolTip.Cursor;
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                    DeselectAllToolTips();
                    DrawingLayer.Children.OfType<StratContentControl>().Where(x => x.Content is TextControl).ToList().ForEach(x => (x.Content as TextControl).Locked = false);
                    BtnCursor.BorderThickness = new Thickness(1);
                    break;
                case View.ToolTip.Eraser:
                    CurrentToolTip = View.ToolTip.Eraser;
                    DrawingLayer.Children.OfType<StratContentControl>().Where(x => x.Content is TextControl).ToList().ForEach(x => (x.Content as TextControl).Locked = true);
                    DeselectAllToolTips();
                    BtnEraser.BorderThickness = new Thickness(1);
                    break;
                case View.ToolTip.Text:
                    CurrentToolTip = View.ToolTip.Text;
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.IBeam;
                    DrawingLayer.Children.OfType<StratContentControl>().Where(x => x.Content is TextControl).ToList().ForEach(x => (x.Content as TextControl).Locked = true);
                    DeselectAllToolTips();
                    BtnText.BorderThickness = new Thickness(1);
                    break;
                case View.ToolTip.Node:
                    CurrentToolTip = View.ToolTip.Node;
                    DrawingLayer.Children.OfType<StratContentControl>().Where(x => x.Content is TextControl).ToList().ForEach(x => (x.Content as TextControl).Locked = true);
                    DeselectAllToolTips();
                    BtnNodes.BorderThickness = new Thickness(1);
                    break;
                case View.ToolTip.Arrow:
                    CurrentToolTip = View.ToolTip.Arrow;
                    DrawingLayer.Children.OfType<StratContentControl>().Where(x => x.Content is TextControl).ToList().ForEach(x => (x.Content as TextControl).Locked = true);
                    DeselectAllToolTips();
                    BtnArrow.BorderThickness = new Thickness(1);
                    break;
                case View.ToolTip.Circle:
                    CurrentToolTip = View.ToolTip.Circle;
                    DrawingLayer.Children.OfType<StratContentControl>().Where(x => x.Content is TextControl).ToList().ForEach(x => (x.Content as TextControl).Locked = true);
                    DeselectAllToolTips();
                    BtnCircle.BorderThickness = new Thickness(1);
                    break;
                case View.ToolTip.Rectangle:
                    CurrentToolTip = View.ToolTip.Rectangle;
                    DrawingLayer.Children.OfType<StratContentControl>().Where(x => x.Content is TextControl).ToList().ForEach(x => (x.Content as TextControl).Locked = true);
                    DeselectAllToolTips();
                    BtnRectangle.BorderThickness = new Thickness(1);
                    break;
                case View.ToolTip.Brush:
                    CurrentToolTip = View.ToolTip.Brush;
                    DrawingLayer.Children.OfType<StratContentControl>().Where(x => x.Content is TextControl).ToList().ForEach(x => (x.Content as TextControl).Locked = true);
                    DeselectAllToolTips();
                    BtnBrush.BorderThickness = new Thickness(1);
                    break;
                default:
                    CurrentToolTip = View.ToolTip.Cursor;
                    DrawingLayer.Children.OfType<StratContentControl>().Where(x => x.Content is TextControl).ToList().ForEach(x => (x.Content as TextControl).Locked = true);
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

        #region ToolTips Methods:

        public Path CreateArrow(Point pointStart, Point pointEnd, double arrowWidth, string UserID = null)
        {
            // Create a path to represent the arrow
            Path arrowPath = new Path();

            if (UserID == null)
            {
                // Set the stroke and fill colors for the path
                arrowPath.Stroke = CurrentBrush;
                arrowPath.Fill = CurrentBrush;
                arrowPath.Tag = CurrentBrushUser;
            }
            else
            {
                arrowPath.Stroke = Globals.GetUserColorBrush(UserID);
                arrowPath.Fill = Globals.GetUserColorBrush(UserID);
            }


            // Create a geometry group to hold the arrow head and tail geometries
            GeometryGroup arrowGeometryGroup = new GeometryGroup();

            // Create a line geometry for the tail of the arrow
            LineGeometry arrowTail = new LineGeometry();
            arrowTail.StartPoint = pointStart;  // Start point of the line
            arrowTail.EndPoint = pointEnd;  // End point of the line

            // Add the line geometry to the geometry group
            arrowGeometryGroup.Children.Add(arrowTail);

            // Calculate the angle of the arrow tail
            double angle = Math.Atan2(pointEnd.Y - pointStart.Y, pointEnd.X - pointStart.X);

            // Calculate the points for the arrow head
            Point pointTop = new Point(pointEnd.X - arrowWidth * Math.Cos(angle - Math.PI / 6), pointEnd.Y - arrowWidth * Math.Sin(angle - Math.PI / 6));
            Point pointBottom = new Point(pointEnd.X - arrowWidth * Math.Cos(angle + Math.PI / 6), pointEnd.Y - arrowWidth * Math.Sin(angle + Math.PI / 6));

            // Create a polyline geometry for the head of the arrow
            PolyLineSegment arrowHead = new PolyLineSegment();
            arrowHead.Points.Add(pointEnd);  // End point of the line
            arrowHead.Points.Add(pointTop);  // Point for the top of the arrow head
            arrowHead.Points.Add(pointBottom);  // Point for the bottom of the arrow head
            arrowHead.Points.Add(pointEnd);  // End point of the line

            // Create a path figure to hold the polyline geometry
            PathFigure arrowHeadFigure = new PathFigure();
            arrowHeadFigure.StartPoint = pointEnd;  // Start point of the line (same as tail)
            arrowHeadFigure.Segments.Add(arrowHead);  // Add the polyline geometry to the figure

            // Create a path geometry to hold the path figure
            PathGeometry arrowHeadGeometry = new PathGeometry();
            arrowHeadGeometry.Figures.Add(arrowHeadFigure);

            // Add the path geometry to the geometry group
            arrowGeometryGroup.Children.Add(arrowHeadGeometry);

            // Set the geometry group as the data for the path
            arrowPath.Data = arrowGeometryGroup;

            HasChanges = true;

            return arrowPath;
        }

        #endregion

        private void Newcc_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolTip == View.ToolTip.Eraser)
            {
                RequestRemove(sender as StratContentControl);
            }
        }

        #region Load Save

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        public void LoadStrat(string id)
        {
            DrawingLayer.Children.Clear();
            WallsLayer.Children.Clear();
            MapStack.Children.Clear();

            CurrentStrat = DataCache.CurrentStrats.Where(x => x.Id == id).FirstOrDefault();
            if (CurrentStrat == null) return;

            //body

            map_id = CurrentStrat.MapId;
            TxtMapName.Content = DataCache.CurrentMaps.Where(x => x.Id == CurrentStrat.MapId).FirstOrDefault()?.Name;
            TxtCreatedBy.Content = DataCache.CurrentTeamMates.Where(x => x.Id == CurrentStrat.CreatedUser).FirstOrDefault()?.Name;
            TxtCreatedOn.Content = CurrentStrat.CreatedAt.ToString().Replace("-", "/").Replace("T", " ");
            TxtLastEdit.Content = CurrentStrat.LastEdit?.ToString().Replace("-", "/").Replace("T", " ");
            TxtVersion.Content = CurrentStrat.Version;

            //content

            if (string.IsNullOrEmpty(CurrentStrat.Content)) return;

            StratContent content = GetStratContentFromString(CurrentStrat.Content);

            Floor0 = content.Floors.Contains(0);
            Floor1 = content.Floors.Contains(1);
            Floor2 = content.Floors.Contains(2);
            Floor3 = content.Floors.Contains(3);
            UpdateFloorButtons();

            AttBanSelector.SelectValue(content?.BanAtt?.Name ?? "");
            DefBanSelector.SelectValue(content?.BanDef?.Name ?? "");

            LoadMapImages();

            var wallsList = WallsLayer.Children.OfType<WallControl>().ToList();
            var hatchList = WallsLayer.Children.OfType<HatchControl>().ToList();

            foreach (var wall in content.Wallstatus)
            {
                var w = wallsList.Where(x => x.Name == wall.Wall_UID).FirstOrDefault();
                if (w == null) continue;
                w.states = wall.States;
                w.User_ID = wall.User_ID;
                w.UpdateUI();
                w.UpdateColor();
            }
            foreach (var hatch in content.Hatchstatus)
            {
                var h = hatchList.Where(x => x.Name == hatch.Hatch_UID).FirstOrDefault();
                if (h == null) continue;
                h.states = hatch.States;
                h.User_ID = hatch.User_ID;
                h.UpdateUI();
                h.UpdateColor();
            }

            LoadAssignmentTable(content.AssignmentTable);

            LoadDragNDropItems(content.DragNDropObjs);

            //IconSizeSlider.Value = content.IconSize;
            //IconSize = content.IconSize;
            //RescaleIcons(content.IconSize);

            Kommentar.Text = content.Comment;
            HasChanges = false;
        }

        public override void Save(bool silent = false)
        {
            SaveStratAsync();
            base.Save();
        }

        public async Task SaveStratAsync()
        {
            if (CurrentStrat == null)
            {
                Notify.sendError("No current strat");
                return;
            }

            StratContent content = new StratContent(); //build here

            content.Comment = Kommentar.Text;
            content.Wallstatus = GetWallObjs();
            content.Hatchstatus = GetHatchObjs();
            content.DragNDropObjs = GetDragNDropObjs();
            content.IconSize = IconSize;

            List<int> floors = new List<int>();
            if (Floor0) floors.Add(0);
            if (Floor1) floors.Add(1);
            if (Floor2) floors.Add(2);
            if (Floor3) floors.Add(3);

            content.Floors = floors;
            content.BanDef = DefBanSelector.selectedOperator;
            content.BanAtt = AttBanSelector.selectedOperator;
            content.AssignmentTable = GetAssignmentTable();

            string scontent = content.SerializeToString();

            if (string.IsNullOrEmpty(scontent))
            {
                Notify.sendError("Content cannot be empty");
            }
            else
            {

                var result = await ApiHandler.SaveStrat(CurrentStrat.Id, CurrentStrat.Name, map_id, pos_id, CurrentStrat.Version + 1, scontent);
                if (result)
                {
                    Notify.sendSuccess("Strat saved successfully");
                }
                else
                {
                    Notify.sendError("Could not save strat");
                }
            }
            HasChanges = false;
        }

        public AssignmentTableModel GetAssignmentTable()
        {
            var table = new AssignmentTableModel();

            table.Rows.Add(new AssignmentTableDataRow { User_ID = Player1.selectedUser?.Id, Gadgets = Gadget1.Text, Loadout = Loadout1.Text, Position = Position1.Text });
            table.Rows.Add(new AssignmentTableDataRow { User_ID = Player2.selectedUser?.Id, Gadgets = Gadget2.Text, Loadout = Loadout2.Text, Position = Position2.Text });
            table.Rows.Add(new AssignmentTableDataRow { User_ID = Player3.selectedUser?.Id, Gadgets = Gadget3.Text, Loadout = Loadout3.Text, Position = Position3.Text });
            table.Rows.Add(new AssignmentTableDataRow { User_ID = Player4.selectedUser?.Id, Gadgets = Gadget4.Text, Loadout = Loadout4.Text, Position = Position4.Text });
            table.Rows.Add(new AssignmentTableDataRow { User_ID = Player5.selectedUser?.Id, Gadgets = Gadget5.Text, Loadout = Loadout5.Text, Position = Position5.Text });

            return table;
        }

        public void LoadAssignmentTable(AssignmentTableModel table)
        {
            if (table == null) return;

            Player1.SelectValue(DataCache.CurrentTeamMates.Where(x => x.Id == table.Rows[0]?.User_ID).FirstOrDefault()?.Name ?? "");
            Gadget1.Text = table.Rows[0].Gadgets;
            Loadout1.Text = table.Rows[0].Loadout;
            Position1.Text = table.Rows[0].Position;

            Player2.SelectValue(DataCache.CurrentTeamMates.Where(x => x.Id == table.Rows[1]?.User_ID).FirstOrDefault()?.Name ?? "");
            Gadget2.Text = table.Rows[1].Gadgets;
            Loadout2.Text = table.Rows[1].Loadout;
            Position2.Text = table.Rows[1].Position;

            Player3.SelectValue(DataCache.CurrentTeamMates.Where(x => x.Id == table.Rows[2]?.User_ID).FirstOrDefault()?.Name ?? "");
            Gadget3.Text = table.Rows[2].Gadgets;
            Loadout3.Text = table.Rows[2].Loadout;
            Position3.Text = table.Rows[2].Position;

            Player4.SelectValue(DataCache.CurrentTeamMates.Where(x => x.Id == table.Rows[3]?.User_ID).FirstOrDefault()?.Name ?? "");
            Gadget4.Text = table.Rows[3].Gadgets;
            Loadout4.Text = table.Rows[3].Loadout;
            Position4.Text = table.Rows[3].Position;

            Player5.SelectValue(DataCache.CurrentTeamMates.Where(x => x.Id == table.Rows[4]?.User_ID).FirstOrDefault()?.Name ?? "");
            Gadget5.Text = table.Rows[4].Gadgets;
            Loadout5.Text = table.Rows[4].Loadout;
            Position5.Text = table.Rows[4].Position;
        }

        public List<DragNDropObj> GetDragNDropObjs()
        {
            List<DragNDropObj> result = new List<DragNDropObj>();

            //Position pos
            //int user_id
            //type type
            //string image
            //double diameter

            foreach (var item in DrawingLayer.Children)
            {
                var newEntry = new DragNDropObj();

                // Image or Text
                if (item is StratContentControl)
                {
                    if ((item as StratContentControl).Content is Border)
                    {
                        newEntry.Pos = new Point(Canvas.GetLeft(item as StratContentControl), Canvas.GetTop(item as StratContentControl));
                        newEntry.UserID = (item as StratContentControl).UserID;
                        Image image = ((item as StratContentControl).Content as Border).Child as Image;
                        newEntry.Image = GetRelativePathForImage(((BitmapImage)image.Source).UriSource);
                        newEntry.Type = DragNDropObjType.Image;
                        newEntry.Diameter = image.ActualWidth;
                    }
                    if ((item as StratContentControl).Content is TextControl)
                    {
                        newEntry.Pos = new Point(Canvas.GetLeft(item as StratContentControl), Canvas.GetTop(item as StratContentControl));
                        newEntry.Width = (item as StratContentControl).Width;
                        newEntry.Height = (item as StratContentControl).Height;
                        newEntry.TextContent = ((item as StratContentControl).Content as TextControl).MainContent.Text;
                        newEntry.Type = DragNDropObjType.Text;
                        newEntry.UserID = (item as StratContentControl).UserID;
                    }
                    if ((item as StratContentControl).Content is Ellipse)
                    {
                        newEntry.Pos = new Point(Canvas.GetLeft(item as StratContentControl), Canvas.GetTop(item as StratContentControl));
                        newEntry.Width = (item as StratContentControl).Width;
                        newEntry.Height = (item as StratContentControl).Height;
                        newEntry.Diameter = (item as StratContentControl).Height;
                        newEntry.Type = DragNDropObjType.Circle;
                        newEntry.UserID = (item as StratContentControl).UserID;
                    }

                    if ((item as StratContentControl).Content is Rectangle)
                    {
                        newEntry.Pos = new Point(Canvas.GetLeft(item as StratContentControl), Canvas.GetTop(item as StratContentControl));
                        newEntry.Width = (item as StratContentControl).Width;
                        newEntry.Height = (item as StratContentControl).Height;
                        newEntry.Type = DragNDropObjType.Rectangle;
                        newEntry.UserID = (item as StratContentControl).UserID;
                    }
                }

                //Drawing
                if (item is Ellipse)
                {
                    newEntry.Pos = new Point(Canvas.GetLeft(item as Ellipse), Canvas.GetTop(item as Ellipse));
                    newEntry.Diameter = (item as Ellipse).Width;
                    newEntry.Type = DragNDropObjType.DrawingCircle;
                    newEntry.UserID = (item as Ellipse).Tag.ToString();
                }

                //Arrow
                if (item is Path)
                {
                    newEntry.Pos = new Point(Canvas.GetLeft(item as Path), Canvas.GetTop(item as Path));

                    var g = ((item as Path).Data as GeometryGroup).Children.First();
                    if (g == null) continue;

                    newEntry.ArrowGeometryStart = (g as LineGeometry).StartPoint;
                    newEntry.ArrowGeometryEnd = (g as LineGeometry).EndPoint;

                    newEntry.UserID = (item as Path).Tag.ToString();
                    newEntry.Type = DragNDropObjType.Arrow;
                }
                if (newEntry != null) result.Add(newEntry);
            }

            return result;
        }

        public string GetRelativePathForImage(Uri image)
        {
            string folder = Globals.XStratInstallPath + @"/Images/Icons/";

            Uri baseFolder = new Uri(folder);

            if (image == null) return null;

            var relativeUri = image.MakeRelativeUri(baseFolder).ToString();

            if (relativeUri == "./")
            {
                return System.IO.Path.GetFileName(image.AbsolutePath);
            }
            return null;
        }

        public void LoadDragNDropItems(List<DragNDropObj> list)
        {
            string ImageFolder = Globals.XStratInstallPath + @"/Images/Icons/";

            foreach (var item in list)
            {
                if (item.Type == DragNDropObjType.Image)
                {
                    if (item.Image.IsNullOrEmpty())
                    {
                        Notify.sendWarn($"could not find image path for image: {item.UserID} | {item.Height} | {item.Width}");
                        continue;
                    }

                    Point newpos = item.Pos;

                    Image newimg = new Image();
                    newimg.IsHitTestVisible = false;
                    newimg.Source = new BitmapImage(new Uri(ImageFolder + item.Image, UriKind.Absolute));

                    var border = new Border();
                    border.BorderThickness = new Thickness(2);
                    border.BorderBrush = CurrentBrush;
                    border.Tag = CurrentBrushUser;
                    border.Child = newimg;

                    StratContentControl newcc = new StratContentControl();
                    newcc.Content = border;
                    newcc.Height = 50;
                    newcc.Width = 50;
                    newcc.Padding = new Thickness(1);
                    newcc.Style = this.FindResource("DesignerItemStyle") as Style;
                    newcc.BorderBrush = item.UserID?.ToSolidColorBrush();
                    newcc.BorderThickness = new Thickness(2);
                    newcc.PreviewMouseLeftButtonDown += Newcc_MouseLeftButtonDown;
                    newcc.UserID = item.UserID;
                    newcc.Tag = item.UserID;

                    DrawingLayer.Children.Add(newcc);

                    Canvas.SetLeft(newcc, newpos.X);
                    Canvas.SetTop(newcc, newpos.Y);
                }

                if (item.Type == DragNDropObjType.Circle)
                {
                    var scc = new StratContentControl();

                    scc.Padding = new Thickness(1);
                    scc.Style = this.FindResource("DesignerItemStyle") as Style;
                    scc.BorderBrush = Brushes.Transparent;
                    scc.BorderThickness = new Thickness(2);

                    Ellipse circle = new Ellipse();
                    circle.IsHitTestVisible = false;

                    scc.Content = circle;

                    scc.Width = item.Width;
                    scc.Height = item.Height;


                    // set the fill color of the rectangle
                    //circle.Fill = CurrentBrush;
                    circle.StrokeThickness = 4;
                    circle.Stroke = Globals.GetUserColorBrush(item.UserID); ;
                    circle.Tag = item.UserID;
                    scc.Tag = item.UserID;
                    scc.UserID = item.UserID;

                    // Add the circle to the canvas
                    DrawingLayer.Children.Add(scc);

                    // set the position of the rectangle on the canvas
                    Canvas.SetLeft(scc, item.Pos.X);
                    Canvas.SetTop(scc, item.Pos.Y);
                }

                if (item.Type == DragNDropObjType.DrawingCircle)
                {
                    var ellipse = new Ellipse();
                    ellipse.Fill = Globals.GetUserColorBrush(item.UserID);
                    ellipse.Width = item.Diameter;
                    ellipse.Height = item.Diameter;
                    ellipse.Tag = item.UserID;
                    DrawingLayer.Children.Add(ellipse);
                    Canvas.SetLeft(ellipse, item.Pos.X);
                    Canvas.SetTop(ellipse, item.Pos.Y);
                }

                if (item.Type == DragNDropObjType.Rectangle)
                {
                    var scc = new StratContentControl();

                    scc.Padding = new Thickness(1);
                    scc.Style = this.FindResource("DesignerItemStyle") as Style;
                    scc.BorderBrush = Brushes.Transparent;
                    scc.BorderThickness = new Thickness(2);

                    Rectangle rect = new Rectangle();
                    rect.IsHitTestVisible = false;

                    scc.Content = rect;

                    scc.Width = item.Width;
                    scc.Height = item.Height;


                    // set the fill color of the rectangle
                    //circle.Fill = CurrentBrush;
                    rect.StrokeThickness = 4;
                    rect.Stroke = Globals.GetUserColorBrush(item.UserID); ;
                    rect.Tag = item.UserID;
                    scc.Tag = item.UserID;
                    scc.UserID = item.UserID;

                    // Add the circle to the canvas
                    DrawingLayer.Children.Add(scc);

                    // set the position of the rectangle on the canvas
                    Canvas.SetLeft(scc, item.Pos.X);
                    Canvas.SetTop(scc, item.Pos.Y);
                }

                if (item.Type == DragNDropObjType.Text)
                {
                    TextControl txt = new TextControl();
                    txt.MainContent.Text = item.TextContent;

                    StratContentControl newcc = new StratContentControl();
                    newcc.Content = txt;
                    newcc.Height = 100;
                    newcc.Width = 300;
                    newcc.Padding = new Thickness(1);
                    newcc.Style = this.FindResource("DesignerItemStyle") as Style;
                    newcc.BorderBrush = Brushes.Transparent;
                    newcc.BorderThickness = new Thickness(2);
                    newcc.UserID = item.UserID;
                    newcc.Tag = item.UserID;

                    DrawingLayer.Children.Add(newcc);

                    Canvas.SetLeft(newcc, item.Pos.X);
                    Canvas.SetTop(newcc, item.Pos.Y);
                }

                if (item.Type == DragNDropObjType.Arrow)
                {
                    var arrow = CreateArrow(item.ArrowGeometryStart, item.ArrowGeometryEnd, 20, item.UserID);
                    DrawingLayer.Children.Add(arrow);
                    Canvas.SetLeft(arrow, item.Pos.X);
                    Canvas.SetTop(arrow, item.Pos.Y);
                    arrow.Tag = item.UserID;
                }
            }
            DeselectAll();
        }

        public List<WallObj> GetWallObjs()
        {
            List<WallObj> wallObjs = new List<WallObj>();

            foreach (var wall in WallsLayer.Children.OfType<WallControl>())
            {
                wallObjs.Add(new WallObj { States = wall.states, Wall_UID = wall.Name.Replace("SCC_", ""), User_ID = wall.User_ID });
            }
            return wallObjs;
        }

        public List<HatchObj> GetHatchObjs()
        {
            List<HatchObj> hatchObjs = new List<HatchObj>();

            foreach (var hatch in WallsLayer.Children.OfType<HatchControl>())
            {
                hatchObjs.Add(new HatchObj { States = hatch.states, Hatch_UID = hatch.Name.Replace("SCC_", ""), User_ID = hatch.User_ID });
            }

            return hatchObjs;
        }

        private void UpdateTopBar()
        {
            Menu.Items.Clear();
            var thickness = new Thickness(0, 0, 0, 0);
            foreach (var map in DataCache.CurrentMaps.Where(x => x.Floor0SVG.IsNotNullOrEmpty() || x.Floor1SVG.IsNotNullOrEmpty() || x.Floor2SVG.IsNotNullOrEmpty() || x.Floor3SVG.IsNotNullOrEmpty()).OrderBy(x => x.Name))
            {
                var mapItem = new MenuItem();
                //mapItem.Name = map.name + "_MapItem";
                mapItem.Header = map.Name;
                mapItem.Template = Application.Current.Resources["Menu_SubMenu_Template"] as ControlTemplate;
                List<MenuItem> subitems = new List<MenuItem>();
                foreach (var pos in DataCache.CurrentPositions.Where(x => x.MapId == map.Id))
                {
                    var posItem = new MenuItem();
                    //posItem.Name = pos.name + "PositionItem";
                    posItem.Header = pos.Name;
                    posItem.Template = Application.Current.Resources["Menu_SubMenu_Template"] as ControlTemplate;
                    foreach (var strat in DataCache.CurrentStrats.Where(x => x.PositionId == pos.Id))
                    {
                        var stratItem = new MenuItem();
                        stratItem.Header = strat.Name;
                        stratItem.Tag = strat;
                        stratItem.Template = Application.Current.Resources["Item_Template"] as ControlTemplate;
                        stratItem.Click += StratItem_Click;
                        posItem.Items.Add(stratItem);
                    }
                    var addItem = new MenuItem();
                    addItem.Header = "New Strat";
                    addItem.Tag = string.Format("create_{0}_{1}", map.Id, pos.Id);
                    addItem.Template = Application.Current.Resources["Item_Template"] as ControlTemplate;
                    addItem.Click += StratItem_Click;
                    posItem.Items.Add(addItem);

                    mapItem.Items.Add(posItem);
                }
                Menu.Items.Add(mapItem);
            }
        }

        private void StratItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem sendObj = sender as MenuItem;
            if (sendObj == null) return;

            //check if pending changes
            var res = AllowExit();
            if (!res) return;

            if (sendObj.Tag.ToString().StartsWith("create_"))
            {
                var split = sendObj.Tag.ToString().Split('_');
                string map_id = split[1];
                string pos_id = split[2];
                CreateStrat(map_id, pos_id);
            }
            else
            {
                Strat strat = sendObj.Tag as Strat;
                LoadStrat(strat.Id);
            }
        }

        private async void CreateStrat(string map, string pos)
        {
            string inputString = Microsoft.VisualBasic.Interaction.InputBox("Name", "Name your strat", "");
            if (string.IsNullOrEmpty(inputString))
            {
                Notify.sendError("Invalid name");
                return;
            }

            //create scrim here
            (bool, string) result = await ApiHandler.NewStrat(inputString, DataCache.CurrentTeam.GameID, map, pos, 1, "");
            if (result.Item1)
            {
                Notify.sendSuccess("Strat created successfully");
                JObject jsonObj = JObject.Parse(result.Item2);
                Strat newStrat = JsonConvert.DeserializeObject<Strat>(jsonObj["model"].ToString());
                CurrentStrat = newStrat;
                Refresh();
            }
            else
            {
                Notify.sendError("Could not save new strat: " + result.Item2);
            }
        }


        public async void Refresh()
        {
            ApiHandler.Waiting();
            DataCache.RetrieveStrats();
            //await Task.Delay(1000);
            UpdateTopBar();
            if (CurrentStrat != null)
            {
                if (CurrentStrat.Id.IsNotNullOrEmpty())
                {
                    LoadStrat(CurrentStrat.Id);
                }
            }
            ApiHandler.EndWaiting();
        }

        public StratContent GetStratContentFromString(string input)
        {
            input = Globals.DecompressString(input);

            //migration
            input = MigrateOldStratContent(input);

            using (var stringReader = new System.IO.StringReader(input))
            {
                var serializer = new XmlSerializer(typeof(StratContent));
                return serializer.Deserialize(stringReader) as StratContent;
            }
        }

        public string MigrateOldStratContent(string content)
        {
            content = content.Replace("user_id>", "User_ID>");
            content = content.Replace("loadout>", "Loadout>");
            content = content.Replace("gadgets>", "Gadgets>");
            content = content.Replace("position>", "Position>");
            content = content.Replace("hatch_uid>", "Hatch_UID>");
            content = content.Replace("wall_uid>", "Wall_UID>");
            content = content.Replace("wall_UID>", "Wall_UID>");
            content = content.Replace("states>", "States>");
            content = content.Replace("assignmentTable>", "AssignmentTableModel>");
            content = content.Replace("floors>", "Floors>");
            content = content.Replace("states>", "States>");
            content = content.Replace("wallstatus>", "Wallstatus>");
            content = content.Replace("hatchstatus>", "Hatchstatus>");
            content = content.Replace("dragNDropObjs>", "DragNDropObjs>");

            return content;
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
                DragDrop.DoDragDrop(sender as Image, sender as Image, DragDropEffects.Move);
            }
        }

        #region Buttons

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
            LoadMapImages();
        }

        private void BtnFloor1_Click(object sender, RoutedEventArgs e)
        {
            Floor1 = !Floor1;
            UpdateFloorButtons();
            ZoomControl.Focus();
            LoadMapImages();
        }

        private void BtnFloor2_Click(object sender, RoutedEventArgs e)
        {
            Floor2 = !Floor2;
            UpdateFloorButtons();
            ZoomControl.Focus();
            LoadMapImages();
        }

        private void BtnFloor3_Click(object sender, RoutedEventArgs e)
        {
            Floor3 = !Floor3;
            UpdateFloorButtons();
            ZoomControl.Focus();
            LoadMapImages();
        }

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            //check if pending changes
            var res = AllowExit();
            if (!res) return;
            Refresh();
        }

        private async void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentStrat == null)
            {
                Notify.sendWarn("There is no strat loaded");
                return;
            }
            bool admin = await ApiHandler.GetAdminStatus();
            if (!admin)
            {
                Notify.sendWarn("Only the team admin is allowed to delete strats");
                return;
            }
            var res = await ApiHandler.DeleteStrat(CurrentStrat.Id);
            if (res)
            {
                CurrentStrat = null;
                UpdateTopBar();
                LoadStrat(null);

                map_id = null;
                TxtMapName.Content = null;
                TxtCreatedBy.Content = null;
                TxtCreatedOn.Content = null;
                TxtLastEdit.Content = null;
                TxtVersion.Content = null;

                Floor0 = false;
                Floor1 = false;
                Floor2 = false;
                Floor3 = false;

                UpdateFloorButtons();

                Notify.sendSuccess("Deleted sucessfully");
            }
        }

        #endregion

        #region drawing
        private Point previousMousePosition;

        private void DrawingLayer_MouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentToolTip == View.ToolTip.Brush && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point mousepoint = e.GetPosition(DrawingLayer);
                var circle = paintCircle(CurrentBrush, mousepoint);
                circle.Tag = CurrentBrushUser;
                e.Handled = true;
            }
            if (CurrentToolTip == View.ToolTip.Eraser && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                ////V1
                //    Point currentMousePosition = e.GetPosition(DrawingLayer);

                //    double distance = (currentMousePosition - previousMousePosition).Length;
                //    if (distance > 0)
                //    {
                //        // Determine the range of the eraser based on the distance
                //        Rect eraserRect = new Rect(currentMousePosition, previousMousePosition);
                //        eraserRect.Inflate(10, 10); // Increase the size of the eraser

                //        // Iterate through all the objects in the canvas and check if they intersect with the eraser
                //        for (int i = DrawingLayer.Children.Count - 1; i >= 0; i--)
                //        {
                //            UIElement element = DrawingLayer.Children[i];

                //            if (element is StratContentControl && (element as StratContentControl).Content is UIElement) element = (element as StratContentControl).Content as UIElement;

                //            Rect elementRect = VisualTreeHelper.GetDescendantBounds(element);
                //            elementRect = element.TransformToAncestor(DrawingLayer).TransformBounds(elementRect);

                //            if (eraserRect.IntersectsWith(elementRect))
                //            {
                //                // Remove the object from the canvas
                //                DrawingLayer.Children.RemoveAt(i);
                //            }
                //        }

                //        // Update the canvas
                //        DrawingLayer.InvalidateVisual();
                //    }

                //    previousMousePosition = currentMousePosition;

                Point currentMousePosition = e.GetPosition(DrawingLayer);

                double distance = (currentMousePosition - previousMousePosition).Length;
                if (distance > 0)
                {
                    // Determine the range of the eraser based on the distance
                    double eraserRadius = BrushSize;
                    Ellipse eraser = new Ellipse
                    {
                        Width = eraserRadius * 2,
                        Height = eraserRadius * 2,
                        Fill = Brushes.White,
                        Opacity = 0.01, // make the eraser invisible
                        StrokeThickness = 1,
                        Stroke = Brushes.Black
                    };

                    // Position the eraser at the current mouse position
                    Canvas.SetLeft(eraser, currentMousePosition.X - eraserRadius);
                    Canvas.SetTop(eraser, currentMousePosition.Y - eraserRadius);

                    // Create a Geometry object that represents the area that should be erased
                    EllipseGeometry eraserGeometry = new EllipseGeometry(currentMousePosition, eraserRadius, eraserRadius);

                    // Iterate through all the objects in the canvas and check if they intersect with the eraser
                    for (int i = DrawingLayer.Children.Count - 1; i >= 0; i--)
                    {
                        UIElement element = DrawingLayer.Children[i] as UIElement;

                        if (element == null) continue;

                        if (element is Ellipse)
                        {
                            var el = element as Ellipse;
                            double X = Canvas.GetLeft(el) + el.Width;
                            double Y = Canvas.GetTop(el) + el.Height;

                            if (Math.Abs(X - currentMousePosition.X) < eraserRadius && Math.Abs(Y - currentMousePosition.Y) < eraserRadius)
                            {
                                // Remove the object from the canvas
                                DrawingLayer.Children.RemoveAt(i);
                            }
                        }
                        else
                        {
                            Rect elementRect = VisualTreeHelper.GetDescendantBounds(element);
                            elementRect = element.TransformToAncestor(DrawingLayer).TransformBounds(elementRect);

                            // Check if the element intersects with the eraser shape
                            Geometry elementGeometry = new RectangleGeometry(elementRect);
                            if (elementGeometry != null && elementGeometry.FillContains(eraserGeometry))
                            {
                                // Remove the object from the canvas
                                DrawingLayer.Children.RemoveAt(i);
                            }
                        }
                    }
                    // Update the canvas
                    DrawingLayer.InvalidateVisual();
                }

                previousMousePosition = currentMousePosition;
            }


        }


        private Ellipse paintCircle(Brush circleColor, Point position)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Fill = circleColor;
            ellipse.Width = BrushSize;
            ellipse.Height = BrushSize;
            Canvas.SetTop(ellipse, position.Y);
            Canvas.SetLeft(ellipse, position.X);
            DrawingLayer.Children.Add(ellipse);
            return ellipse;
        }

        private void BrushSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BrushSize = (int)BrushSlider.Value;
        }

        #endregion

        #region Helpers for XStrathelper

        public void UpdateUserID()
        {
            foreach (var item in DrawingLayer.Children)
            {
                // Image or Text
                if (item is StratContentControl)
                {
                    var brush = Globals.GetUserColorBrush((item as StratContentControl).UserID);
                    if (brush == null) brush = Brushes.Red;
                    if ((item as StratContentControl).Content is Border)
                    {
                        ((item as StratContentControl).Content as Border).BorderBrush = brush;
                    }
                    if ((item as StratContentControl).Content is TextControl)
                    {
                        TextControl textControl = (TextControl)(item as StratContentControl).Content;
                        textControl.BorderBrush = brush;
                    }

                }

                //Drawing
                if (item is Ellipse)
                {
                    var ellipse = (Ellipse)item;
                    string userID = ellipse.Tag.ToString();
                    var brush = Globals.GetUserColorBrush(userID);
                    if (brush == null) brush = Brushes.Red;
                    ellipse.Stroke = brush;
                }

                //Arrow
                if (item is Path)
                {
                    var path = (Path)item;
                    string userID = path.Tag.ToString();
                    var brush = Globals.GetUserColorBrush(userID);
                    if (brush == null) brush = Brushes.Red;
                    path.Stroke = brush;
                    path.Fill = brush;
                }
            }

        }

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

        public void SelectAll()
        {
            foreach (Control child in DrawingLayer.Children.OfType<Control>())
            {
                Selector.SetIsSelected(child, true);
            }
        }

        public void RequestRemove(StratContentControl item)
        {
            DrawingLayer.Children.Remove(item);
        }
        #endregion



        private void ZoomControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CurrentToolTip == View.ToolTip.Brush)
            {
                e.Handled = true;
            }
            if (CurrentToolTip == View.ToolTip.Eraser)
            {
                deleteFromCanvasLoop = true;
                //DeleteFromCanvas();
                e.Handled = true;
            }
        }

        private void ZoomControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TranslateXLast = ZoomControl.TranslateX;
            TranslateYLast = ZoomControl.TranslateY;

            if (CurrentToolTip == View.ToolTip.Brush)
            {
                e.Handled = true;
            }
            if (CurrentToolTip == View.ToolTip.Eraser)
            {
                deleteFromCanvasLoop = true;
                //DeleteFromCanvas();
                e.Handled = true;
            }
        }

        private void IconSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            IconSize = e.NewValue;

            //scale all icons
            RescaleIcons(e.NewValue);
        }

        private void RescaleIcons(double size)
        {
            if (!IsLoaded || IconSizeSlider == null) return;
            foreach (StratContentControl item in DrawingLayer.Children.OfType<StratContentControl>().Where(x => x.Content is Border && Selector.GetIsSelected(x)))
            {
                double newsize = size;
                double oldsize = item.Width;

                double left = Canvas.GetLeft(item) + (item.Width / 2) - (newsize / 2);
                double top = Canvas.GetTop(item) + (item.Height / 2) - (newsize / 2);

                item.Width = newsize;
                item.Height = newsize;

                Canvas.SetLeft(item, left);
                Canvas.SetTop(item, top);
            }
        }

        private void ZoomControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (TranslateXLast != ZoomControl.TranslateX || TranslateYLast != ZoomControl.TranslateY)
            {
                ZoomControl.InvalidateVisual();
            }
        }

        #region Hotkeys

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DeselectAll();
                return;
            }

            if (e.Key == Key.A)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    SelectAll();
                    return;
                }
            }

            if (e.Key == Key.S)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    Save();
                    return;
                }
            }

            if (e.Key == Key.R)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    Refresh();
                    return;
                }

            }

            if (e.Key == Key.D1)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl)) ToolTipChanged(View.ToolTip.Cursor);
            }
            if (e.Key == Key.D2)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl)) ToolTipChanged(View.ToolTip.Brush);
            }
            if (e.Key == Key.D3)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl)) ToolTipChanged(View.ToolTip.Eraser);
            }
            if (e.Key == Key.D4)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl)) ToolTipChanged(View.ToolTip.Text);
            }
            if (e.Key == Key.D5)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl)) ToolTipChanged(View.ToolTip.Node);
            }
            if (e.Key == Key.D6)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl)) ToolTipChanged(View.ToolTip.Arrow);
            }
            if (e.Key == Key.D7)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl)) ToolTipChanged(View.ToolTip.Circle);
            }
            if (e.Key == Key.D8)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl)) ToolTipChanged(View.ToolTip.Rectangle);
            }

        }

        #endregion

        #region Print

        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(MapContent);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(MapContent);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                pngEncoder.Save(ms);
                ms.Close();

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PNG | *.png";
                saveFileDialog.CheckFileExists = false;
                saveFileDialog.CheckPathExists = false;
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (saveFileDialog.ShowDialog() == true)
                {
                    string filename = saveFileDialog.FileName;
                    System.IO.File.WriteAllBytes(filename, ms.ToArray());
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }

    #region Sub Classes
    public enum ToolTip
    {
        Cursor, Eraser, Text, Node, Arrow, Circle, Rectangle, Brush
    }

    [Serializable]
    public class StratContent
    {
        public List<WallObj> Wallstatus { get; set; }
        public List<HatchObj> Hatchstatus { get; set; }
        public List<DragNDropObj> DragNDropObjs { get; set; }
        public string Comment { get; set; }

        public List<int> Floors { get; set; }
        
        public double IconSize { get; set; }
        public AssignmentTableModel AssignmentTable { get; set; }
        public Operator BanDef { get; set; }
        public Operator BanAtt { get; set; }


        public StratContent()
        {
            Wallstatus = new List<WallObj>();
            Hatchstatus = new List<HatchObj>();
            DragNDropObjs = new List<DragNDropObj>();
            Comment = "";
            Floors = new List<int>();
        }
    }

    public class WallObj
    {
        public string Wall_UID { get; set; }
        public string User_ID { get; set; }
        public WallStates[] States { get; set; }
    }
    public class HatchObj
    {
        public string Hatch_UID { get; set; }
        public string User_ID { get; set; }
        public HatchStates[] States { get; set; }
    }

    public class AssignmentTableModel
    {
        public List<AssignmentTableDataRow> Rows = new List<AssignmentTableDataRow>();
    }

    public class AssignmentTableDataRow
    {
        public string User_ID { get; set; }
        public string Loadout { get; set; }
        public string Gadgets { get; set; }
        public string Position { get; set; }
    }

    public class DragNDropObj
    {
        public Point Pos { get; set; }
        public string UserID { get; set; }
        public DragNDropObjType Type { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string TextContent { get; set; }
        public Point ArrowGeometryStart { get; set; }
        public Point ArrowGeometryEnd { get; set; }
        public string Image { get; set; }
        public double Diameter { get; set; }
    }

    public enum DragNDropObjType
    {
        Image,
        Circle,
        Text,
        Arrow,
        Rectangle,
        DrawingCircle,
    }

    public static class StratMakerToolTipHelper
    {
        public static ToolTip CurrentToolTip { get; set; }
        public static string CurrentBrushUser { get; set; }
    }

    #endregion
}
