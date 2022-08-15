using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using xstrat.MVVM.View;
using xstrat.Core;
using xstrat.MVVM.ViewModel;
using Squirrel;
using System;
using System.Threading;
using System.Globalization;

namespace xstrat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        MainViewModel mv;
        public bool NewlyRegistered = false;
        public bool IsLoggedIn = false;
        UpdateManager manager;
        public MainWindow()
        {
            InitializeComponent();
            SettingsHandler.Initialize();
            ApiHandler.Initialize();
            mv = (MainViewModel)DataContext;
            Task loginTask = LoginWindowAsync();
            Loaded += MainWindow_Loaded;
            StateChanged += MainWindow_StateChanged;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                ButtonFullscreenIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Fullscreen;
            }
            else
            {
                ButtonFullscreenIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.FullscreenExit;
            }
        }

        private async void CheckForUpdate()
        {
            if(manager != null)
            {
                var updateinfo = await manager.CheckForUpdate();
                if(updateinfo.ReleasesToApply.Count > 0)
                {
                    await manager.UpdateApp();
                    //MessageBox.Show("New Update found. Please restart your client to install");
                    Notify.sendInfo("New Update found. Please restart your client to install");
                }
            }
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/flowhl/xstrat-client");
                CheckForUpdate();
            }
            catch(Exception ex)
            {
                Logger.Log(ex.Message);
            }
            finally
            {
                Globals.Init();
            }
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
        }

        /// <summary>
        /// drag window around
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        /// <summary>
        /// minimize button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// close button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// show login window
        /// </summary>
        /// <returns></returns>
        private async Task LoginWindowAsync()
        {
            if(SettingsHandler.StayLoggedin == true && SettingsHandler.token != null && SettingsHandler.token != "")
            {
                bool verified = await ApiHandler.VerifyToken(SettingsHandler.token);
                if(verified)
                {
                    IsLoggedIn = true;
                    return;
                }
            }
            mv.CurrentView = new LoginView();
        }

        /// <summary>
        /// show register window
        /// </summary>
        public void Register()
        {
            mv.CurrentView = new RegisterView();
        }

        public void LoginComplete(string token)
        {
            if (SettingsHandler.StayLoggedin)
            {
                SettingsHandler.token = token;
                SettingsHandler.Save();
            }
            ApiHandler.AddBearer(token);
            NewlyRegistered = false;
            IsLoggedIn = true;
            Globals.Init();
            mv.CurrentView = new HomeView();
        }
        public void RegisterComplete()
        {
            NewlyRegistered = true;
            mv.CurrentView = new LoginView();
        }

        private void ButtonFullscreen_Click(object sender, RoutedEventArgs e)
        {
            if(WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }
    }
}
