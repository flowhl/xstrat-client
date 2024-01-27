using System.IO;
using System.Windows;
using xstrat.Core;

namespace xstrat.Ui
{
    /// <summary>
    /// Interaction logic for EULAWindow.xaml
    /// </summary>
    public partial class EULAWindow : System.Windows.Window
    {
        public bool EULAAccepted { get; private set; }

        public EULAWindow()
        {
            InitializeComponent();
            Loaded += EULAWindow_Loaded;
        }

        private void EULAWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadEULAText();
        }

        private void LoadEULAText()
        {
            string eulaFilePath = System.IO.Path.Combine(Globals.XStratInstallPath, "license.txt");
            if(!File.Exists(eulaFilePath))
            {
                Notify.sendError("EULA file not found. Please reinstall the application.");
                this.Close();
                return;
            }
            txtEULA.Text = File.ReadAllText(eulaFilePath);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            EULAAccepted = chkAcceptEULA.IsChecked == true;
            this.Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
            e.Handled = true;
        }

    }
}
