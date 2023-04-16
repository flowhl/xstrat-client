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

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for SkinSwitcherControl.xaml
    /// </summary>
    public partial class SkinSwitcherControl : UserControl
    {
        SolidColorBrush Off = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        SolidColorBrush On = new SolidColorBrush(Color.FromRgb(240, 222, 45));

        string docPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"xstrat\skinswitcher");
        string normalPath;
        string noSkinPath;

        public SkinSwitcherControl()
        {
            InitializeComponent();
            normalPath = System.IO.Path.Combine(docPath, @"normal");
            noSkinPath = System.IO.Path.Combine(docPath, @"noSkins");
            Directory.CreateDirectory(docPath);
            Directory.CreateDirectory(normalPath);
            Directory.CreateDirectory(noSkinPath);
            TS.setStatus(SettingsHandler.Settings.SkinSwitcherStatus);

        }

        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo dir1 = new DirectoryInfo(noSkinPath);
            DirectoryInfo dir2 = new DirectoryInfo(normalPath);
            DirectoryInfo ubidir = new DirectoryInfo(SettingsHandler.Settings.SkinSwitcherPath);
            if (SettingsHandler.Settings.SkinSwitcherPath != null && SettingsHandler.Settings.SkinSwitcherPath != "")
            {
                Error.Content = null;
                if (SettingsHandler.Settings.SkinSwitcherStatus)
                {
                    UbiClear();
                    CopyFiles(dir2, ubidir); //disable skins

                }
                else
                {
                    UbiClear();
                    CopyFiles(dir1, ubidir); //enable skins
                }
                Notify.sendSuccess("You can restart your game now");
            }
            else
            {
                Notify.sendError("Please set the correct folder in the settings!");
            }

        }

        private void TS_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (TS.Toggled1 == true)
            {
                SettingsHandler.Settings.SkinSwitcherStatus = true;
            }
            else
            {
                SettingsHandler.Settings.SkinSwitcherStatus = false;
            }
            SettingsHandler.Save();
        }

        private void CurrentToNoSkin_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Override current noskin-config?", "Override?", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                DirectoryInfo dir1 = new DirectoryInfo(noSkinPath);
                DirectoryInfo ubidir = new DirectoryInfo(SettingsHandler.Settings.SkinSwitcherPath);
                CopyFiles(ubidir, dir1);
                Notify.sendSuccess("Copied files");
            }
        }

        private void CurrentToSkin_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Override current skin-config?", "Override?", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                DirectoryInfo dir2 = new DirectoryInfo(normalPath);
                DirectoryInfo ubidir = new DirectoryInfo(SettingsHandler.Settings.SkinSwitcherPath);
                CopyFiles(ubidir, dir2);
                Notify.sendSuccess("Copied files");
            }
        }

        private void UbiClear()
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(SettingsHandler.Settings.SkinSwitcherPath);

                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    dir.Delete(true);
                }

            }
            catch (Exception ex)
            {
                Logger.Log("error while deleting files: " + ex.Message);
                Notify.sendError("error while deleting files: " + ex.Message);
            }

        }
        public static void CopyFiles(DirectoryInfo source, DirectoryInfo target)
        {
            try
            {
                foreach (DirectoryInfo dir in source.GetDirectories())
                    CopyFiles(dir, target.CreateSubdirectory(dir.Name));
                foreach (FileInfo file in source.GetFiles())
                    file.CopyTo(System.IO.Path.Combine(target.FullName, file.Name), true);
            }
            catch (Exception ex)
            {
                Logger.Log("error while copying files: " + ex.Message);
                Notify.sendError("error while copying files: " + ex.Message);
            }
        }
    }
}
