using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml;
using System.Xml.Linq;
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
        public bool isMouseDown = false;

        public bool Floor0 { get; set; }
        public bool Floor1 { get; set; }
        public bool Floor2 { get; set; }
        public bool Floor3 { get; set; }

        public string map_id;
        public int floor_id;

        public List<string> Walls { get; set; }
        public string SelectedWall { get; set; }
        public UserControl CurrentElement { get; set; }

        public WallEditorView()
        {
            InitializeComponent();
            Loaded += WallEditorView_Loaded;
        }

        private void WallEditorView_Loaded(object sender, RoutedEventArgs e)
        {
            WallList.SelectionChanged += WallList_SelectionChanged;
            xStratHelper.editorView = this;
            xStratHelper.WEMode = true;
            Opened();
        }

        private void WallList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedWall = WallList?.SelectedValue?.ToString() ?? null;
            CurrentElement = GetWallControlByName(SelectedWall);
            ResetBorderBrush();
            if (CurrentElement == null) return;
            CurrentElement.BorderBrush = Brushes.Red;
        }

        private void ResetBorderBrush()
        {
            var walls = WallsLayer.Children.OfType<WallControl>();
            foreach (var wall in walls)
            {
                wall.BorderBrush = Brushes.Transparent;
            }
            var hatches = WallsLayer.Children.OfType<HatchControl>();
            foreach (var hatch in hatches)
            {
                hatch.BorderBrush = Brushes.Transparent;
            }
        }

        private void Opened()
        {
            UpdateFloorButtons();
            MouseLeftButtonDown += StratMakerView_MouseLeftButtonDown;
            MouseLeftButtonUp += StratMakerView_MouseLeftButtonUp;
            MapSelector.CBox.SelectionChanged += MapSelector_SelectionChanged;
        }

        private void MapSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectMap();
        }

        #region loading ui

        private void UpdateWallsList()
        {
            List<string> list = new List<string>();
            foreach (var item in WallsLayer.Children.OfType<WallControl>())
            {
                list.Add(item.Name);
            }
            foreach (var item in WallsLayer.Children.OfType<HatchControl>())
            {
                list.Add(item.Name);
            }
            list.Sort();
            Walls = list;
            WallList.ItemsSource = Walls;
        }

        public UserControl GetWallControlByName(string name)
        {
            if (!Walls.Contains(name)) return null;
            var wall = WallsLayer.Children.OfType<WallControl>().Where(x => x.Name == name).FirstOrDefault();
            var hatch = WallsLayer.Children.OfType<HatchControl>().Where(x => x.Name == name).FirstOrDefault();
            if (wall != null) return wall;
            if (hatch != null) return hatch;
            return null;
        }

        private void SelectMap()
        {
            if (MapSelector.SelectedMap != null)
            {
                map_id = MapSelector.SelectedMap.Id;
            }
            else
            {
                map_id = null;
            }
            LoadMapImages();
        }

        private void LoadMapImages(string svgPath = null)
        {
            MapStack.Children.Clear();
            WallsLayer.Children.Clear();

            if (floor_id < 0) return;
            if (map_id.IsNullOrEmpty()) return;
            string game_id = DataCache.CurrentTeam.GameID;

            XmlDocument svgFile;

            if (string.IsNullOrEmpty(svgPath))
            {
                svgFile = Globals.GetSCVDocumentForMapAndFloor(map_id, floor_id);
            }
            else
            {
                svgFile = Globals.GetSVGDocumentFromPath(svgPath);
            }

            if (svgFile == null) return;

            var SVGContent = Globals.GetSvgContent(svgFile, game_id, map_id, floor_id);

            var newimage = Globals.GetImageForSVG(SVGContent);

            if (newimage != null)
            {
                //double widthToSet = 4000 * (MapStack.Children.Count);
                MapStack.Children.Add(newimage);
                //Canvas.SetLeft(newimage,widthToSet);
            }
            CreateWallsBeta(SVGContent);
            UpdateWallsList();
            ZoomControl.ZoomToFill();
        }

        #endregion

        #region Keyboard

        private void Border_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyPress();
        }


        private void HandleKeyPress()
        {
            if (Keyboard.IsKeyDown(Key.A))
            {
                MoveLeft();
            }
            else if (Keyboard.IsKeyDown(Key.D))
            {
                MoveRight();
            }
            else if (Keyboard.IsKeyDown(Key.W))
            {
                MoveUp();
            }
            else if (Keyboard.IsKeyDown(Key.S))
            {
                MoveDown();
            }
            else if (Keyboard.IsKeyDown(Key.R))
            {
                Rotate();
            }
            else if (Keyboard.IsKeyDown(Key.Y))
            {
                DecreaseWidth();
            }
            else if (Keyboard.IsKeyDown(Key.X))
            {
                IncreaseWidth();
            }
            else if (Keyboard.IsKeyDown(Key.Delete))
            {
                Delete();
            }
        }

        private void MoveLeft()
        {
            if (CurrentElement == null) return;
            int amount = 1;
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                amount = 10;
            }
            Canvas.SetLeft(CurrentElement, Canvas.GetLeft(CurrentElement) - amount);
        }

        private void MoveRight()
        {

            if (CurrentElement == null) return;
            int amount = 1;
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                amount = 10;
            }
            Canvas.SetLeft(CurrentElement, Canvas.GetLeft(CurrentElement) + amount);
        }

        private void MoveUp()
        {

            if (CurrentElement == null) return;
            int amount = 1;
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                amount = 10;
            }
            Canvas.SetTop(CurrentElement, Canvas.GetTop(CurrentElement) - amount);
        }

        private void MoveDown()
        {

            if (CurrentElement == null) return;
            int amount = 1;
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                amount = 10;
            }
            Canvas.SetTop(CurrentElement, Canvas.GetTop(CurrentElement) + amount);
        }

        private void Rotate()
        {
            if (CurrentElement == null) return;
            if (CurrentElement.RenderTransform == null) CurrentElement.RenderTransform = new RotateTransform(0.0);
            RotateTransform rotation = CurrentElement.RenderTransform as RotateTransform;
            double angle = rotation.Angle;
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    angle -= 1;
                }
                else
                {
                    angle -= 45;
                }
            }
            else
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    angle -= 1;
                }
                else
                {
                    angle -= 45;
                }
            }
            if (angle > 360) angle -= 360;
            if (angle < -360) angle += 360;
            CurrentElement.RenderTransform = rotation;
        }

        private void DecreaseWidth()
        {
            if (CurrentElement == null || CurrentElement is HatchControl) return;
            int amount = 1;
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                amount = 5;
            }
            CurrentElement.Width -= amount;
        }

        private void IncreaseWidth()
        {
            if (CurrentElement == null || CurrentElement is HatchControl) return;
            int amount = 1;
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                amount = 5;
            }
            CurrentElement.Width += amount;
        }

        private void Delete()
        {
            if (CurrentElement == null) return;
            WallsLayer.Children.Remove(CurrentElement);
            CurrentElement = null;
            UpdateWallsList();
        }
        #endregion


        private void StratMakerView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void StratMakerView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }


        #region Drag Items

        private void NewImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ZoomControl_Drop(object sender, DragEventArgs e)
        {

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

        #region Helpers for XStrathelper

        public void PointDragMove(Point p)
        {
            foreach (Control child in WallsLayer.Children.OfType<Control>())
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
            foreach (Control child in WallsLayer.Children.OfType<Control>())
            {
                Selector.SetIsSelected(child, false);
            }
        }

        public void RequestRemove(StratContentControl item)
        {
            WallsLayer.Children.Remove(item);
        }
        #endregion

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadMapImages();
        }
        
        private List<WallPositionObject> WallsToWallObjects()
        {
            List<WallPositionObject> wallObjects = new List<WallPositionObject>();

            foreach (var item in WallsLayer.Children.OfType<WallControl>())
            {
                int type = 0;
                var obj = new WallPositionObject { position_x = Canvas.GetLeft(item), type = type, position_y = Canvas.GetTop(item), rotate = item.RenderTransform as RotateTransform, uid = item.Name, width = item.Width };
                wallObjects.Add(obj);
            }

            foreach (var item in WallsLayer.Children.OfType<HatchControl>())
            {
                int type = 1;
                var obj = new WallPositionObject { position_x = Canvas.GetLeft(item), type = type, position_y = Canvas.GetTop(item), rotate = item.RenderTransform as RotateTransform, uid = item.Name, width = item.Width };
                wallObjects.Add(obj);
            }

            return wallObjects;
        }

        private void CreateWallsBeta(SvgContent svg)
        {
            if (svg == null) return;
            WallsLayer.Children.Clear();

            var Walls = svg.Rects.Where(x => x.style.Contains("#linear-gradient"));

            foreach (var Wall in Walls)
            {
                #region testcode
                //var rect = new Rectangle();

                //Point pos = new Point(Wall.x, Wall.y);

                //rect.Fill = Brushes.Pink;

                //rect.Width = Wall.width;
                //rect.Height = Wall.height;

                ////transform
                //if (Wall.hasTransform)
                //{
                //    //rect.RenderTransformOrigin = new Point(Wall.width / 2, Wall.height / 2);

                //    rect.RenderTransformOrigin = new Point(0.5, 0.5);
                //    rect.RenderTransform = new RotateTransform(Wall.rotation);
                //    //pos = new Point(Wall.translateX, Wall.translateY);

                //}

                //WallsLayer.Children.Add(rect);

                //Canvas.SetLeft(rect, pos.X);
                //Canvas.SetTop(rect, pos.Y);

                #endregion

                if (Globals.isWithin5Percent(Wall.width, Wall.height))
                {
                    //Hatch
                    HatchControl hatch = new HatchControl();

                    Point pos = new Point(Wall.x, Wall.y);

                    hatch.IsHitTestVisible = false;

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

                    Canvas.SetLeft(hatch, pos.X);
                    Canvas.SetTop(hatch, pos.Y);
                }
                else
                {
                    //Wall

                    WallControl wall = new WallControl();

                    Point pos = new Point(Wall.x, Wall.y);

                    wall.IsHitTestVisible = false;

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

                    Canvas.SetLeft(wall, pos.X);
                    Canvas.SetTop(wall, pos.Y);
                }
            }

            UpdateWallsList();
        }


        //private void WallObjectsToWalls()
        //{
        //    var objects = xStratHelper.GetWallObjects(map_id, floor_id);
        //    DrawingLayer.Children.Clear();
        //    foreach (var obj in objects)
        //    {
        //        try
        //        {
        //            if (obj.type == 0)
        //            {
        //                var newpos = new Point();
        //                newpos.X = obj.position_x;
        //                newpos.Y = obj.position_y;

        //                WallControl newwc = new WallControl();
        //                newwc.Height = 19;
        //                newwc.IsHitTestVisible = false;
        //                newwc.Width = obj.width;
        //                newwc.RenderTransform = obj.rotate;
        //                newwc.Name = obj.uid.Replace("SCC_", "");

        //                DrawingLayer.Children.Add(newwc);

        //                Canvas.SetLeft(newwc, newpos.X);
        //                Canvas.SetTop(newwc, newpos.Y);
        //            }
        //            if (obj.type == 1)
        //            {
        //                var newpos = new Point();
        //                newpos.X = obj.position_x;
        //                newpos.Y = obj.position_y;

        //                HatchControl newhc = new HatchControl();
        //                newhc.IsHitTestVisible = false;
        //                newhc.Name = obj.uid.Replace("SCC_", "");
        //                newhc.Height = 86;
        //                newhc.Width = 86;
        //                newhc.RenderTransform = obj.rotate;

        //                DrawingLayer.Children.Add(newhc);

        //                Canvas.SetLeft(newhc, newpos.X);
        //                Canvas.SetTop(newhc, newpos.Y);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Notify.sendError("Error creating ContentControl for image: " + ex.Message);
        //        }
        //    }
        //    UpdateWallsList();
        //}

        private void WallsLayer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void WallsLayer_PreviewMouseMove(object sender, MouseEventArgs e)
        {

        }

        private void LoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            string path = null;

            //open file dialog

            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "SVG files (*.svg) | *.svg";
            openFileDialog.CheckFileExists = true;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) path = openFileDialog.FileName;

            LoadMapImages(path);
        }
    }

    public class WallPositionObject
    {
        public string uid { get; set; }
        public int type { get; set; }
        public double position_x { get; set; }
        public double position_y { get; set; }
        public RotateTransform rotate { get; set; }
        public double width { get; set; }
    }

}
