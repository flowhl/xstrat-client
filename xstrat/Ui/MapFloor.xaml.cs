using System;
using System.Collections.Generic;
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

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for MapFloor.xaml
    /// </summary>
    public partial class MapFloor : UserControl
    {
        public string rawContent { get; set; }
        public MapFloor(string content)
        {
            InitializeComponent();
            rawContent = content;
        }
        private void _decodeString()
        {
            var split = rawContent.Replace("<Image:", "").Split("></Image>".ToCharArray());
            string imgSource = split.First();
            string wallscontent = split.Last();
            //image
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(imgSource, UriKind.Absolute);
            bi.EndInit();
            StratImage.Source = bi;

            //TODO generator erstellen und so. Siehe notepad
            //walls
            var wallsRaw = wallscontent.Split("<Wall".ToCharArray()).ToList<string>();
            string[] paras;
            foreach (var item in wallsRaw)
            {
                paras = item.Replace("/>", "").Split(';');
                if(paras.Length == 9)
                {
                    try
                    {
                        int x = int.Parse(item[0].ToString());
                        int y = int.Parse(item[1].ToString());
                        int orientation = int.Parse(item[2].ToString());
                        Wallstates state0 = (Wallstates)int.Parse(item[3].ToString());
                        Wallstates state1 = (Wallstates)int.Parse(item[4].ToString());
                        Wallstates state2 = (Wallstates)int.Parse(item[5].ToString());
                        Wallstates state3 = (Wallstates)int.Parse(item[6].ToString());
                        Wallstates state4 = (Wallstates)int.Parse(item[7].ToString());
                        Wallstates state5 = (Wallstates)int.Parse(item[8].ToString());
                        Wallstates[] states = new Wallstates[] { state0, state1, state2, state3, state4, state5 };
                        var newWall = new WallControl(states);
                        var index = WallCanvas.Children.Add(newWall);
                        var element = WallCanvas.Children[index];
                        double xe = x;
                        double ye = y;
                        Canvas.SetLeft(element, xe);
                        Canvas.SetTop(element, ye);
                    }
                    catch(Exception ex) 
                    {
                        Notify.sendError(ex.Message);
                        Logger.Log("Error on MapFloor :" + ex.Message);
                    }

                }
            }
        }
    }
}
