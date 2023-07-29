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
            //txtGreeting.Content = GetGreeting();
        }

        public string GetGreeting()
        {
            string name = DataCache.CurrentUser?.Name;

            if (string.IsNullOrEmpty(name)) return "";

            TimeSpan morning_start = new TimeSpan(4, 0, 0); //10 o'clock
            TimeSpan morning_end = new TimeSpan(12, 0, 0); //12 o'clock

            TimeSpan evening_start = new TimeSpan(18, 0, 0); //18 o'clock
            TimeSpan evening_end = new TimeSpan(23, 59, 0); //24 o'clock

            TimeSpan now = DateTime.Now.TimeOfDay;

            if ((now > morning_start) && (now < morning_end))
            {
                return "Good Morning, " + name + "!";
            }

            if((now > evening_start) && (now < evening_end))
            {
                return "Good Evening, " + name + "!";
            }

            int rnd = new Random().Next(0, 10);

            switch (rnd)
            {
                case 0:
                    return "Hi " + name + "!";
                case 1:
                    return "Welcome back " + name + "!";
                case 2:
                    return "Hey " + name+ "!";
                case 3:
                    return "Nice to see you " + name+ "!";
                case 4:
                    return "Hope you are doing well " + name+ "!";
                case 5:
                    return "Great to see you " + name+ "!";
                case 6:
                    return "Thank you for being here " + name+ "!";
                case 7:
                    return "Hello " + name+ "!";
                case 8:
                    return "Welcome back " + name + "!";
                case 9:
                    return "Great to see you " + name + "!";

                default:
                    break;
            }

            return "";
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

        //private void AddItems()
        //{
        //    AddToPanel(new SkinSwitcherControl());
        //    AddToPanel(new TeamMatesControl());

        //    var li = new Viewbox();
        //    li.StretchDirection = StretchDirection.Both;
        //    li.Stretch = Stretch.Uniform;
        //    li.Child = new LicenseInfo(true);
        //    li.Width = 400;
        //    li.Height = 275;
        //    li.Margin = new Thickness(10);
        //    Row1.Children.Add(li);
        //}

        private void AddToPanel(Control e)
        {
            e.Margin = new Thickness(10);
            Row1.Children.Add(e);
        }
    }
}
