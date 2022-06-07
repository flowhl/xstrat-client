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
        public StratMakerView()
        {
            InitializeComponent();
            _loadMaps();
        }

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
    }
}
