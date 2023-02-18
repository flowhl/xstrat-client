﻿using MaterialDesignThemes.Wpf;
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

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for StratMakerView.xaml
    /// </summary>
    public partial class StratMakerView : UserControl
    {
        private List<XMap> maps = new List<XMap>();

        public Strat currentStrat { get; set; }

        Image draggedItem;

        public ToolTip CurrentToolTip;
        public Brush CurrentBrush = null;
        public bool isMouseDown = false;
        public int BrushSize { get; set; } = 10;

        public bool Floor0 { get; set; }
        public bool Floor1 { get; set; }
        public bool Floor2 { get; set; }
        public bool Floor3 { get; set; }

        public int map_id;
        public int pos_id;

        public double TranslateXLast { get; set; }
        public double TranslateYLast { get; set; }

        public double IconSize { get; set; }

        public StratMakerView()
        {
            InitializeComponent();
            Loaded += StratMakerView_Loaded;
        }

        private void StratMakerView_Loaded(object sender, RoutedEventArgs e)
        {
            xStratHelper.stratView = this;
            xStratHelper.WEMode = false;

            Globals.wnd.KeyDown += KeyDown;

            Opened();
        }

        private void Opened()
        {
            UpdateTopBar();
            ToolTipChanged(View.ToolTip.Cursor);
            LoadColorButtons();
            UpdateFloorButtons();
            LoadDragItems();
            //ZoomControl.ZoomChanged += ZoomControl_ZoomChanged;
            MouseLeftButtonDown += StratMakerView_MouseLeftButtonDown;
            //MouseLeftButtonUp += StratMakerView_MouseLeftButtonUp;
            ZoomControl.MouseLeftButtonDown += ZC_MouseLeftButtonDown;
            
        }

        public Point? ArrowPoint1 = null;

        private void ZC_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //catch when chlick
            var mouseover = DrawingLayer.Children.OfType<StratContentControl>().Where(x => x.IsMouseCaptureWithin);
            if (mouseover != null && mouseover.Count() > 0) return;

            //cursor
            if (CurrentToolTip == View.ToolTip.Cursor) DeselectAll();

            //text
            if (CurrentToolTip == View.ToolTip.Text)
            {
                var newpos = e.GetPosition(DrawingLayer);

                TextControl txt = new TextControl();
                txt.MainContent.Text = "Text";

                StratContentControl newcc = new StratContentControl();
                newcc.Content = txt;
                newcc.Height = 100;
                newcc.Width = 300;
                newcc.Padding = new Thickness(1);
                newcc.Style = this.FindResource("DesignerItemStyle") as Style;
                newcc.BorderBrush = Brushes.Transparent;
                newcc.BorderThickness = new Thickness(2);

                DrawingLayer.Children.Add(newcc);

                Canvas.SetLeft(newcc, newpos.X);
                Canvas.SetTop(newcc, newpos.Y);
            }

            //Arrow
            if (CurrentToolTip == View.ToolTip.Arrow)
            {
                if(ArrowPoint1 == null)
                {
                    ArrowPoint1 = Mouse.GetPosition(DrawingLayer);
                }
                else
                {
                    Path arrow = CreateArrow(ArrowPoint1.GetValueOrDefault(), Mouse.GetPosition(DrawingLayer), 20);

                    // Add the arrow to the canvas
                    DrawingLayer.Children.Add(arrow);

                    arrow.PreviewMouseLeftButtonDown+= Arrow_MouseLeftButtonDown;

                    ArrowPoint1 = null;
                }
            }
        }

        private void Arrow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(CurrentToolTip == View.ToolTip.Eraser)
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

            if (map_id < 0) return;

            List<int> floors = new List<int>();
            if (Floor0) floors.Add(0);
            if (Floor1) floors.Add(1);
            if (Floor2) floors.Add(2);
            if (Floor3) floors.Add(3);

            int game_id = Globals.games.Where(x => x.name == Globals.teamInfo.game_name).FirstOrDefault().id;

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
                    Canvas.SetLeft(newimage,offset);

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
            //if (!DrawingLayer.Children.OfType<StratContentControl>().Where(x => (x.PointFromScreen(Mouse.GetPosition(this)) - x.PointToScreen(new Point(0, 0))).Length < 20).Any())
            //{
            //    DeselectAll();
            //}
            if (CurrentToolTip == View.ToolTip.Eraser)
            {
                DeleteFromCanvas();
            }

        }

        #region Drag Items

        private void LoadDragItems()
        {
            if(Globals.teamInfo.game_name == "R6 Siege")
            {
                IconsSP.Children.Clear();
                string folder = Globals.XStratInstallPath + @"/Images/Icons/";
                var allItems = Directory.GetFiles(folder, "*.png").Where(x => x != null && System.IO.Path.GetFileName(x).StartsWith("r6_")).ToList();
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

                newpos.X -= IconSize / 2;
                newpos.Y -= IconSize / 2;

                Image newimg = new Image();
                newimg.IsHitTestVisible = false;
                newimg.Source = draggedItem.Source;

                StratContentControl newcc = new StratContentControl();
                newcc.Content = newimg;
                newcc.Height = IconSize;
                newcc.Width = IconSize;
                newcc.Padding = new Thickness(1);
                newcc.Style = this.FindResource("DesignerItemStyle") as Style;
                newcc.BorderBrush = CurrentBrush;
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
                newBtn.Name = "Color_" + Globals.RemoveIllegalCharactersFromName(user.name);
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
                    BtnBrush.BorderThickness = new Thickness(1);
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

        #region ToolTips Methods:

        public Path CreateArrow(Point pointStart, Point pointEnd, double arrowWidth, Brush brush = null)
        {
            // Create a path to represent the arrow
            Path arrowPath = new Path();

            if(brush == null)
            {
                // Set the stroke and fill colors for the path
                arrowPath.Stroke = CurrentBrush;
                arrowPath.Fill = CurrentBrush;
            }
            else
            {
                arrowPath.Stroke = brush;
                arrowPath.Fill = brush;
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
            SaveStratAsync();
        }

        public void LoadStrat(int id)
        {
            DrawingLayer.Children.Clear();
            WallsLayer.Children.Clear();
            MapStack.Children.Clear();

            currentStrat = Globals.strats.Where(x => x.id == id).FirstOrDefault();
            if (currentStrat == null) return;

            map_id = currentStrat.map_id;

            //body

            map_id = currentStrat.map_id;
            TxtMapName.Content = Globals.Maps.Where(x => x.id == currentStrat.map_id).FirstOrDefault()?.name;
            TxtCreatedBy.Content = Globals.teammates.Where(x => x.id == currentStrat.created_by).FirstOrDefault()?.name;
            TxtCreatedOn.Content = currentStrat.created_date.ToString().Replace("-","/").Replace("T", " ");
            TxtLastEdit.Content = currentStrat.last_edit_time?.ToString().Replace("-", "/").Replace("T", " ");
            TxtVersion.Content = currentStrat.version;
            
            //content

            if (string.IsNullOrEmpty(currentStrat.content)) return;

            StratContent content = GetStratContentFromString(currentStrat.content);

            Floor0 = content.floors.Contains(0);
            Floor1 = content.floors.Contains(1);
            Floor2 = content.floors.Contains(2);
            Floor3 = content.floors.Contains(3);
            UpdateFloorButtons();

            AttBanSelector.SelectValue(content?.banAtt?.name ?? "");
            DefBanSelector.SelectValue(content?.banDef?.name ?? "");

            LoadMapImages();

            var wallsList = WallsLayer.Children.OfType<WallControl>().ToList();
            var hatchList = WallsLayer.Children.OfType<HatchControl>().ToList();

            foreach (var wall in content.wallstatus)
            {
                var w = wallsList.Where(x => x.Name == wall.wall_uid).FirstOrDefault();
                if (w == null) continue;
                w.states = wall.states;
                w.UpdateUI();
                //TODO user einfügen
            }
            foreach (var hatch in content.hatchstatus)
            {
                var h = hatchList.Where(x => x.Name == hatch.hatch_uid).FirstOrDefault();
                if (h == null) continue;
                h.states = hatch.states;
                h.UpdateUI();
                //TODO user einfügen
            }

            LoadAssignmentTable(content.assignmentTable);

            LoadDragNDropItems(content.dragNDropObjs);

            IconSizeSlider.Value = content.IconSize;
            IconSize = content.IconSize;
            RescaleAllIcons(content.IconSize);

            Kommentar.Text = content.comment;
        }

        public async Task SaveStratAsync()
        {
            if(currentStrat == null)
            {
                Notify.sendError("No current strat");
                return;
            }

            StratContent content = new StratContent(); //build here
            
            content.comment = Kommentar.Text;
            content.wallstatus = GetWallObjs();
            content.hatchstatus = GetHatchObjs();
            content.dragNDropObjs = GetDragNDropObjs();
            content.IconSize = IconSize;

            List<int> floors = new List<int>();
            if (Floor0) floors.Add(0);
            if (Floor1) floors.Add(1);
            if (Floor2) floors.Add(2);
            if (Floor3) floors.Add(3);

            content.floors = floors;
            content.banDef = DefBanSelector.selectedOperator;
            content.banAtt = AttBanSelector.selectedOperator;
            content.assignmentTable = GetAssignmentTable();

            string scontent = content.SerializeObject();

            if (string.IsNullOrEmpty(scontent))
            {
                Notify.sendError("Content cannot be empty");
            }
            else
            {

                (bool, string) result = await ApiHandler.SaveStrat(currentStrat.id, currentStrat.name, map_id, pos_id, currentStrat.version + 1, scontent);
                if (result.Item1)
                {
                    Notify.sendSuccess("Strat saved successfully");
                }
                else
                {
                    Notify.sendError("Could not save strat: " + result.Item2);
                }
            }
        }

        public AssignmentTable GetAssignmentTable()
        {
            var table = new AssignmentTable();

            table.Rows.Add(new AssignmentTableDataRow { User_id = Player1.selectedUser?.id ?? -1, gadgets = Gadget1.Text, loadout = Loadout1.Text, position = Position1.Text });
            table.Rows.Add(new AssignmentTableDataRow { User_id = Player2.selectedUser?.id ?? -1, gadgets = Gadget2.Text, loadout = Loadout2.Text, position = Position2.Text });
            table.Rows.Add(new AssignmentTableDataRow { User_id = Player3.selectedUser?.id ?? -1, gadgets = Gadget3.Text, loadout = Loadout3.Text, position = Position3.Text });
            table.Rows.Add(new AssignmentTableDataRow { User_id = Player4.selectedUser?.id ?? -1, gadgets = Gadget4.Text, loadout = Loadout4.Text, position = Position4.Text });
            table.Rows.Add(new AssignmentTableDataRow { User_id = Player5.selectedUser?.id ?? -1, gadgets = Gadget5.Text, loadout = Loadout5.Text, position = Position5.Text });

            return table;
        }

        public void LoadAssignmentTable(AssignmentTable table)
        {
            Player1.SelectValue(Globals.teammates.Where(x => x.id == table.Rows[0].User_id).FirstOrDefault()?.name ?? "");
            Gadget1.Text = table.Rows[0].gadgets;
            Loadout1.Text = table.Rows[0].loadout;
            Position1.Text = table.Rows[0].position;

            Player2.SelectValue(Globals.teammates.Where(x => x.id == table.Rows[1].User_id).FirstOrDefault()?.name ?? "");
            Gadget2.Text = table.Rows[1].gadgets;
            Loadout2.Text = table.Rows[1].loadout;
            Position2.Text = table.Rows[1].position;

            Player3.SelectValue(Globals.teammates.Where(x => x.id == table.Rows[2].User_id).FirstOrDefault()?.name ?? "");
            Gadget3.Text = table.Rows[2].gadgets;
            Loadout3.Text = table.Rows[2].loadout;
            Position3.Text = table.Rows[2].position;

            Player4.SelectValue(Globals.teammates.Where(x => x.id == table.Rows[3].User_id).FirstOrDefault()?.name ?? "");
            Gadget4.Text = table.Rows[3].gadgets;
            Loadout4.Text = table.Rows[3].loadout;
            Position4.Text = table.Rows[3].position;

            Player5.SelectValue(Globals.teammates.Where(x => x.id == table.Rows[4].User_id).FirstOrDefault()?.name ?? "");
            Gadget5.Text = table.Rows[4].gadgets;
            Loadout5.Text = table.Rows[4].loadout;
            Position5.Text = table.Rows[4].position;
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
                if(item is StratContentControl)
                {
                    if((item as StratContentControl).Content is Image)
                    {
                        newEntry.pos = new Point(Canvas.GetLeft(item as StratContentControl), Canvas.GetTop(item as StratContentControl));
                        newEntry.brush = (item as StratContentControl).BorderBrush.ToString();
                        Image image = ((item as StratContentControl).Content as Image);
                        newEntry.image = GetRelativePathForImage(((BitmapImage)image.Source).UriSource);
                        newEntry.type = DragNDropObjType.Image;
                    }
                    if ((item as StratContentControl).Content is TextControl)
                    {
                        newEntry.pos = new Point(Canvas.GetLeft(item as StratContentControl), Canvas.GetTop(item as StratContentControl));
                        newEntry.width = (item as StratContentControl).Width;
                        newEntry.height = (item as StratContentControl).Height;
                        newEntry.textContent = ((item as StratContentControl).Content as TextControl).MainContent.Text;
                        newEntry.type = DragNDropObjType.Text;
                    }

                }

                //Drawing
                if(item is Ellipse)
                {
                    newEntry.pos = new Point(Canvas.GetLeft(item as Ellipse), Canvas.GetTop(item as Ellipse));
                    newEntry.diameter = (item as Ellipse).Width;
                    newEntry.brush = (item as Ellipse).Fill.ToString();
                    newEntry.type = DragNDropObjType.Circle;
                }

                //Arrow
                if(item is Path)
                {
                    newEntry.pos = new Point(Canvas.GetLeft(item as Path), Canvas.GetTop(item as Path));                    

                    var g = ((item as Path).Data as GeometryGroup).Children.First();
                    if (g == null) continue;

                    newEntry.arrowGeometryStart = (g as LineGeometry).StartPoint;
                    newEntry.arrowGeometryEnd = (g as LineGeometry).EndPoint;

                    newEntry.brush = (item as Path).Fill.ToString();
                    newEntry.type = DragNDropObjType.Arrow;
                }
                if(newEntry != null) result.Add(newEntry);
            }

            return result;
        }

        public string GetRelativePathForImage(Uri image)
        {
            string folder = Globals.XStratInstallPath + @"/Images/Icons/";
            
            Uri baseFolder = new Uri(folder);
            
            if (image == null) return null;

            var relativeUri = image.MakeRelativeUri(baseFolder).ToString();

            if(relativeUri == "./")
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
                if(item.type == DragNDropObjType.Image)
                {
                    Point newpos = item.pos;

                    Image newimg = new Image();
                    newimg.IsHitTestVisible = false;
                    newimg.Source = new BitmapImage(new Uri(ImageFolder + item.image, UriKind.Absolute));

                    StratContentControl newcc = new StratContentControl();
                    newcc.Content = newimg;
                    newcc.Height = 50;
                    newcc.Width = 50;
                    newcc.Padding = new Thickness(1);
                    newcc.Style = this.FindResource("DesignerItemStyle") as Style;
                    newcc.BorderBrush = item.brush.ToSolidColorBrush();
                    newcc.BorderThickness = new Thickness(2);
                    newcc.PreviewMouseLeftButtonDown += Newcc_MouseLeftButtonDown;

                    DrawingLayer.Children.Add(newcc);

                    Canvas.SetLeft(newcc, newpos.X);
                    Canvas.SetTop(newcc, newpos.Y);
                }

                if(item.type == DragNDropObjType.Circle)
                {
                    var ellipse = new Ellipse();
                    ellipse.Fill = item.brush.ToSolidColorBrush();
                    ellipse.Width = item.diameter;
                    ellipse.Height = item.diameter;
                    DrawingLayer.Children.Add(ellipse);
                    Canvas.SetLeft(ellipse, item.pos.X);
                    Canvas.SetTop(ellipse, item.pos.Y);
                }

                if(item.type == DragNDropObjType.Text)
                {
                    TextControl txt = new TextControl();
                    txt.MainContent.Text = "Text";

                    StratContentControl newcc = new StratContentControl();
                    newcc.Content = txt;
                    newcc.Height = 100;
                    newcc.Width = 300;
                    newcc.Padding = new Thickness(1);
                    newcc.Style = this.FindResource("DesignerItemStyle") as Style;
                    newcc.BorderBrush = Brushes.Transparent;
                    newcc.BorderThickness = new Thickness(2);

                    DrawingLayer.Children.Add(newcc);

                    Canvas.SetLeft(newcc, item.pos.X);
                    Canvas.SetTop(newcc, item.pos.Y);
                }

                if(item.type == DragNDropObjType.Arrow)
                {
                    var arrow = CreateArrow(item.arrowGeometryStart, item.arrowGeometryEnd, 20, item.brush.ToSolidColorBrush());
                    DrawingLayer.Children.Add(arrow);
                    Canvas.SetLeft(arrow, item.pos.X);
                    Canvas.SetTop(arrow, item.pos.Y);

                }
            }
            DeselectAll();
        }

        public List<WallObj> GetWallObjs()
        {
            List<WallObj> wallObjs = new List<WallObj>();

            foreach (var wall in WallsLayer.Children.OfType<WallControl>())
            {
                wallObjs.Add(new WallObj { states = wall.states, wall_uid = wall.Name.Replace("SCC_", "") });
            }
            return wallObjs;
        }

        public List<HatchObj> GetHatchObjs()
        {
            List<HatchObj> hatchObjs = new List<HatchObj>();

            foreach (var hatch in WallsLayer.Children.OfType<HatchControl>())
            {
                hatchObjs.Add(new HatchObj { states = hatch.states, hatch_uid = hatch.Name.Replace("SCC_", "") });
            }

            return hatchObjs;
        }

        private void UpdateTopBar()
        {
            Menu.Items.Clear();
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
                    var addItem = new MenuItem();
                    addItem.Header = "New Strat";
                    addItem.Tag = string.Format("create_{0}_{1}", map.id, pos.id);
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

            if (sendObj.Tag.ToString().StartsWith("create_"))
            {
                var split = sendObj.Tag.ToString().Split('_');
                int map_id = -1;
                int pos_id = -1;
                int.TryParse(split[1], out map_id);
                int.TryParse(split[2], out pos_id);
                CreateStrat(map_id, pos_id);                
            }
            else
            {           
                Strat strat = sendObj.Tag as Strat;
                LoadStrat(strat.id);
            }
        }
        
        private async void CreateStrat(int map, int pos)
        {
            string inputString = Microsoft.VisualBasic.Interaction.InputBox("Name", "Name your strat", "");
            if (string.IsNullOrEmpty(inputString)) 
            {
                Notify.sendError("Invalid name");
                return;
            }

            //create scrim here
            (bool, string) result = await ApiHandler.NewStrat(inputString, Globals.Game_id(),map, pos, 1, "");
            if (result.Item1)
            {
                Notify.sendSuccess("Strat created successfully");
                JObject json = JObject.Parse(result.Item2);
                int strat_id = json.SelectToken("data").SelectToken("insertId").Value<int>();
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
            await Globals.RetrieveStrats();
            await Task.Delay(1000);
            UpdateTopBar();
            if (currentStrat != null)
            {
                LoadStrat(currentStrat.id);
            }
            ApiHandler.EndWaiting();
        }

        public StratContent GetStratContentFromString(string input)
        {
            input = Globals.DecompressString(input);
            using (var stringReader = new System.IO.StringReader(input))
            {
                var serializer = new XmlSerializer(typeof(StratContent));
                return serializer.Deserialize(stringReader) as StratContent;
            }
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
            if(e.LeftButton == MouseButtonState.Pressed)
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
            Refresh();
        }
        #endregion

        #region drawing

        private void DrawingLayer_MouseMove(object sender, MouseEventArgs e)
        {
            if(CurrentToolTip == View.ToolTip.Brush && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point mousepoint = e.GetPosition(DrawingLayer);
                paintCircle(CurrentBrush, mousepoint);
                e.Handled = true;
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

        private void BrushSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BrushSize = (int)BrushSlider.Value;
        }

        #endregion

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
            if(CurrentToolTip == View.ToolTip.Eraser)
            {
                deleteFromCanvasLoop = true;
                DeleteFromCanvas();
                e.Handled = true;
            }
        }

        private void ZoomControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TranslateXLast = ZoomControl.TranslateX;
            TranslateYLast = ZoomControl.TranslateY;

            if(CurrentToolTip == View.ToolTip.Brush)
            {
                e.Handled = true;
            }
            if (CurrentToolTip == View.ToolTip.Eraser)
            {
                deleteFromCanvasLoop = true;
                DeleteFromCanvas();
                e.Handled = true;
            }
        }

        private void IconSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            IconSize = e.NewValue;

            //scale all icons
            RescaleAllIcons(e.NewValue);
        }

        private void RescaleAllIcons(double size)
        {
            if (!IsLoaded || IconSizeSlider == null) return;
            foreach (StratContentControl item in DrawingLayer.Children.OfType<StratContentControl>())
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
            if(TranslateXLast != ZoomControl.TranslateX || TranslateYLast != ZoomControl.TranslateY)
            {
                ZoomControl.InvalidateVisual();
            }
        }

        #region Hotkeys

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                DeselectAll();
                return;
            }

            if(e.Key == Key.A)
            {
                if(Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    SelectAll();
                    return;
                }
            }

            if (e.Key == Key.S)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    SaveStratAsync();
                    return;
                }
            }

            if (e.Key == Key.R)
            {
                if(Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    Refresh();
                    return;
                }

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
    public enum ToolTip
    {
        Cursor, Eraser, Text, Node, Arrow, Circle, Rectangle, Brush
    }
    [Serializable]
    public class StratContent
    {
        public List<WallObj> wallstatus { get; set; }
        public List<HatchObj> hatchstatus { get; set; }
        public List<DragNDropObj> dragNDropObjs { get; set; }
        public string comment { get; set; }
        public List<int> floors { get; set; }
        public double IconSize { get; set; }
        public AssignmentTable assignmentTable { get; set; }
        public Operator banDef { get; set; }
        public Operator banAtt { get; set; }


        public StratContent()
        {
            wallstatus = new List<WallObj>();
            hatchstatus = new List<HatchObj>();
            dragNDropObjs = new List<DragNDropObj>();
            comment = "";
            floors = new List<int>();
        }

    }

    public class WallObj
    {
        public string wall_uid { get; set; }
        public int user_id { get; set; }
        public Wallstates[] states { get; set; }
    }
    public class HatchObj
    {
        public string hatch_uid { get; set; }
        public int user_id { get; set; }
        public Hatchstates[] states { get; set; }
    }

    public class AssignmentTable
    {
        public List<AssignmentTableDataRow> Rows = new List<AssignmentTableDataRow>();
    }

    public class AssignmentTableDataRow
    {
        public int User_id { get; set; }
        public string loadout { get; set; }
        public string gadgets { get; set; }
        public string position { get; set; }
    }



    public class DragNDropObj
    {
        public Point pos { get; set; }
        public string brush { get; set; }
        public DragNDropObjType type { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public string textContent{ get; set; }
        public Point arrowGeometryStart { get; set; }
        public Point arrowGeometryEnd { get; set; }
        public string image { get; set; }
        public double diameter { get; set; }
    }

    public enum DragNDropObjType 
    {
        Image,
        Circle,
        Text,
        Arrow
    }
    

}
