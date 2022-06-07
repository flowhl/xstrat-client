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
using xstrat.Ui;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for SkinSwitcherView.xaml
    /// </summary>
    public partial class SkinSwitcherView : UserControl
    {
        SolidColorBrush Off = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        SolidColorBrush On = new SolidColorBrush(Color.FromRgb(240, 222, 45));

        string docPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "xstrat/skinswitcher");
        string normalPath;
        string noSkinPath;

        public SkinSwitcherView()
        {
            InitializeComponent();
            normalPath = System.IO.Path.Combine(docPath, @"\normal");
            noSkinPath = docPath + @"\noSkins";
            Directory.CreateDirectory(docPath);
            Directory.CreateDirectory(normalPath);
            Directory.CreateDirectory(noSkinPath);
            TS.setStatus(SettingsHandler.SkinSwitcherStatus);
            
        }

        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo dir1 = new DirectoryInfo(noSkinPath);
            DirectoryInfo dir2 = new DirectoryInfo(normalPath);
            DirectoryInfo ubidir = new DirectoryInfo(SettingsHandler.SkinSwitcherPath);
            if(SettingsHandler.SkinSwitcherPath != null && SettingsHandler.SkinSwitcherPath != "")
            {
                Error.Content = null;
                if (SettingsHandler.SkinSwitcherStatus)
                {
                    UbiClear();
                    CopyFiles(dir2, ubidir); //disable skins

                }
                else
                {
                    UbiClear();
                    CopyFiles(dir1, ubidir); //enable skins
                }
            }
            else
            {
                Error.Content = "Please set the correct folder in the settings!";
            }
           
        }

        private void TS_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (TS.Toggled1 == true)
            {
                SettingsHandler.SkinSwitcherStatus = true;
            }
            else
            {
                SettingsHandler.SkinSwitcherStatus = false;
            }
            SettingsHandler.Save();
        }

        private void CurrentToNoSkin_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo dir1 = new DirectoryInfo(noSkinPath);
            DirectoryInfo ubidir = new DirectoryInfo(SettingsHandler.SkinSwitcherPath);
            CopyFiles(ubidir, dir1);
        }

        private void CurrentToSkin_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo dir2 = new DirectoryInfo(normalPath);
            DirectoryInfo ubidir = new DirectoryInfo(SettingsHandler.SkinSwitcherPath);
            CopyFiles(ubidir, dir2);
        }

        private void UbiClear()
        {
            DirectoryInfo directory = new DirectoryInfo(SettingsHandler.SkinSwitcherPath);

            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                dir.Delete(true);
            }
        }
        public static void CopyFiles(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFiles(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(System.IO.Path.Combine(target.FullName, file.Name), true);
        }
    }
}
