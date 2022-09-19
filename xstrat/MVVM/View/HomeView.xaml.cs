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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using xstrat.Core;
using xstrat.Ui;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            Loaded += HomeView_Loaded;
        }

        private void HomeView_Loaded(object sender, RoutedEventArgs e)
        {
            AddItems();
            
        }

        //var rnd = new Random();
        //for (int i = 0; i < 535; i++)
        //{
        //    int h = rnd.Next(20, 35) * 10;
        //    int w = rnd.Next(20, 35) * 10;
        //    Color randomColor = Color.FromArgb(Convert.ToByte(rnd.Next(255,256)), Convert.ToByte(rnd.Next(256)), Convert.ToByte(rnd.Next(256)), Convert.ToByte(rnd.Next(256)));

        //    var rc = new Rectangle();
        //    rc.Height = h;
        //    rc.Width = w;
        //    rc.Fill = randomColor.ToString().ToSolidColorBrush();

        //    WrapPanel.Children.Add(rc);

        //}

        private void AddItems()
        {
            AddToPanel(new SkinSwitcherControl());
            AddToPanel(new TeamMatesControl());

            var li = new Viewbox();
            li.StretchDirection = StretchDirection.Both;
            li.Stretch = Stretch.Uniform;
            li.Child = new LicenseInfo(true);
            li.Width = 400;
            li.Height = 275;
            li.Margin = new Thickness(10);
            WrapPanel.Children.Add(li);
        }

        private void AddToPanel(Control e)
        {
            e.Margin = new Thickness(10);
            WrapPanel.Children.Add(e);
        }
    }
}
