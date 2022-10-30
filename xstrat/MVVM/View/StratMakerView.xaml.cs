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
            MouseLeftButtonUp += StratMakerView_MouseLeftButtonUp;
        }


        #region loading ui

        private void LoadMapImages()
        {
            MapStack.Children.Clear();
            if (map_id < 0) return;

            List<int> floors = new List<int>();
            if (Floor0) floors.Add(0);
            if (Floor1) floors.Add(1);
            if (Floor2) floors.Add(2);
            if (Floor3) floors.Add(3);

            int game_id = Globals.games.Where(x => x.name == Globals.TeamInfo.game_name).FirstOrDefault().id;

            Point offset = new Point(0,0);

            foreach (var floor in floors)
            {
                var newimage = Globals.GetImageForFloorAndMap(game_id, map_id, floor);
                MapStack.Children.Add(newimage);

                WallsLayer.Children.Clear();

                //add walls here
                var objects = xStratHelper.GetWallObjects(map_id, floor);
                foreach (var obj in objects)
                {
                    try
                    {
                        if (obj.type == 0)
                        {
                            var newpos = new Point();
                            newpos.X = obj.position_x + offset.X;
                            newpos.Y = obj.position_y + offset.Y;

                            WallControl newwc = new WallControl();
                            newwc.Height = 19;
                            newwc.Name = obj.uid;
                            newwc.Width = obj.width;
                            newwc.RenderTransform = obj.rotate;

                            WallsLayer.Children.Add(newwc);

                            Canvas.SetLeft(newwc, newpos.X);
                            Canvas.SetTop(newwc, newpos.Y);

                            SetBrushToItem();
                        }
                        if (obj.type == 1)
                        {
                            var newpos = new Point();
                            newpos.X = obj.position_x + offset.X;
                            newpos.Y = obj.position_y + offset.Y;

                            HatchControl newhc = new HatchControl();
                            newhc.Name = obj.uid;
                            newhc.RenderTransform = obj.rotate;
                            newhc.Height = 86;
                            newhc.Width = 86;

                            WallsLayer.Children.Add(newhc);

                            Canvas.SetLeft(newhc, newpos.X);
                            Canvas.SetTop(newhc, newpos.Y);

                            SetBrushToItem();
                        }
                    }
                    catch (Exception ex)
                    {
                        Notify.sendError("Error creating ContentControl for image: " + ex.Message);
                    }
                }

                offset.X += 4000 / 1.3;
                DeselectAll();
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
                if(item is StratContentControl)
                {
                    newEntry.pos = new Point(Canvas.GetLeft(item as StratContentControl), Canvas.GetTop(item as StratContentControl));
                    newEntry.UserID = (item as StratContentControl).UserID;
                    Image image = ((item as StratContentControl).Content as Image);
                    newEntry.image = ((BitmapImage)image.Source).UriSource.AbsolutePath;
                    newEntry.type = DragNDropObjType.image;
                }
                if(item is Ellipse)
                {
                    newEntry.pos = new Point(Canvas.GetLeft(item as Ellipse), Canvas.GetTop(item as Ellipse));
                    newEntry.diameter = (item as Ellipse).Width;
                    newEntry.UserID = Globals.teammates.Where(x => x.color.ToSolidColorBrush().ToString() == (item as Ellipse).Fill.ToString().ToSolidColorBrush().ToString()).FirstOrDefault()?.id ?? -1;
                    newEntry.type = DragNDropObjType.Circle;
                }
                if(newEntry != null) result.Add(newEntry);
            }

            return result;
        }

        public void LoadDragNDropItems(List<DragNDropObj> list)
        {
            foreach (var item in list)
            {
                if(item.type == DragNDropObjType.image)
                {
                    Point newpos = item.pos;

                    Image newimg = new Image();
                    newimg.IsHitTestVisible = false;
                    newimg.Source = new BitmapImage(new Uri(item.image, UriKind.Absolute));

                    StratContentControl newcc = new StratContentControl();
                    newcc.Content = newimg;
                    newcc.Height = 50;
                    newcc.Width = 50;
                    newcc.Padding = new Thickness(1);
                    newcc.Style = this.FindResource("DesignerItemStyle") as Style;
                    newcc.BorderBrush = Brushes.Transparent;
                    newcc.BorderThickness = new Thickness(2);
                    newcc.UserID = item.UserID;

                    DrawingLayer.Children.Add(newcc);

                    Canvas.SetLeft(newcc, newpos.X);
                    Canvas.SetTop(newcc, newpos.Y);

                    DeselectAll();
                }

                if(item.type == DragNDropObjType.Circle)
                {
                    var ellipse = new Ellipse();
                    ellipse.Fill = Globals.teammates.Where(x => x.id == item.UserID).FirstOrDefault()?.color?.ToSolidColorBrush() ?? Brushes.Red;
                    ellipse.Width = item.diameter;
                    ellipse.Height = item.diameter;
                    DrawingLayer.Children.Add(ellipse);
                    Canvas.SetLeft(ellipse, item.pos.X);
                    Canvas.SetTop(ellipse, item.pos.Y);
                }
            }
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

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        public async void Refresh()
        {
            ApiHandler.Waiting();
            await Globals.RetrieveStrats();
            await Task.Delay(1000);
            UpdateTopBar();
            if(currentStrat != null)
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

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveStratAsync();
        }

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



    public class DragNDropObj
    {
        public Point pos { get; set; }
        public int UserID { get; set; }
        public DragNDropObjType type { get; set; }
        public string image { get; set; }
        public double diameter { get; set; }
    }

    public enum DragNDropObjType 
    {
        image,
        Circle
    }
    

}
