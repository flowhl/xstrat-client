using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    /// Interaction logic for LicenseInfo.xaml
    /// </summary>
    public partial class LicenseInfo : UserControl
    {
        public int Subscribed { get; set; }
        public DateTime? End { get; set; }

        public LicenseInfo()
        {
            InitializeComponent();
            Loaded += LicenseInfo_Loaded;
        }
        public LicenseInfo(bool hideEnterOption)
        {
            InitializeComponent();
            Loaded += LicenseInfo_Loaded;
            if (hideEnterOption)
            {
                EnterPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void LicenseInfo_Loaded(object sender, RoutedEventArgs e)
        {
            RetrieveStatus();
        }

        private async void RetrieveStatus()
        {
            (bool, string) result = await ApiHandler.GetLicenseStatus();
            if (result.Item1)
            {
                string response = result.Item2;
                //convert to json instance
                JObject json = JObject.Parse(response);
                var data = json.SelectToken("data").ToString();
                xstrat.Json.LicenseStatus _content = JsonConvert.DeserializeObject<List<xstrat.Json.LicenseStatus>>(data).FirstOrDefault();
                Subscribed = _content.subscribed;
                if(_content.subscription_end != null && _content.subscription_end != string.Empty) End = DateTime.Parse(_content.subscription_end);
                else
                {
                    End = null;
                }
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if(Subscribed == 1)
            {
                LicenseStatusIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Check;
                LicenseStatusText.Content = "Premium";
                ExpiryText.Content = End.ToString();
            }
            else
            {
                LicenseStatusIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Close;
                LicenseStatusText.Content = "Free";
                ExpiryText.Content = "Never";
            }
        }

        private void ActivateBtn_Click(object sender, RoutedEventArgs e)
        {
            ActivateLicense();
        }

        private async void ActivateLicense()
        {
            string key = SearchTermTextBox.Text.Trim();
            (bool, string) result = await ApiHandler.ActivateLicense(key);
            if (result.Item1)
            {
                Notify.sendSuccess(result.Item2);
                RetrieveStatus();
            }
            else
            {
                Notify.sendError(result.Item2);
            }
        }

        private void GetLicenseBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://xstrat.app/store");
        }
    }
}
