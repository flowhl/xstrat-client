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
using XStrat;
using System.Linq;
using System.Windows.Controls;

namespace xstrat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        MainViewModel mv;

        public LoadingView lv;

        public bool NewlyRegistered = false;
        public bool IsLoggedIn = false;
        public bool FinishedLoading = false;
        UpdateManager manager;

        DateTime startTime = DateTime.Now;

        int counterToUnlockEditor = 0;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Notify.CleanUp();
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
            Globals.OnDataRetrieved += Globals_OnDataRetrieved;
            mv = (MainViewModel)DataContext;
            mv.CurrentView = mv.LoadingVM;
            SettingsHandler.Initialize();
            ApiHandler.Initialize();
            Task loginTask = LoginWindowAsync();

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

            StateChanged += MainWindow_StateChanged;
            WallEditorBtn.Visibility = Visibility.Collapsed;
        }

        private void Globals_OnDataRetrieved(object sender, EventArgs e)
        {
            EndLoading();
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
                if (this.WindowState == WindowState.Maximized)
                {
                    // Store the current window position and size
                    double left = this.Left;
                    double top = this.Top;
                    double width = this.Width;
                    double height = this.Height;

                    this.WindowState = WindowState.Normal;

                    // Calculate the new window position based on mouse position
                    double mouseX = e.GetPosition(this).X;
                    double mouseY = e.GetPosition(this).Y;
                    double newLeft = mouseX - (width * mouseX / this.ActualWidth);
                    double newTop = mouseY - (height * mouseY / this.ActualHeight);

                    // Set the new window position
                    this.Left = newLeft;
                    this.Top = newTop;
                }
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
            if(SettingsHandler.Settings.StayLoggedin == true && SettingsHandler.Settings.Token != null && SettingsHandler.Settings.Token != "")
            {
                bool verified = await ApiHandler.VerifyToken(SettingsHandler.Settings.Token);
                if(verified)
                {
                    Notify.ResumeLogging();
                    //EndLoading();
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
            if (SettingsHandler.Settings.StayLoggedin)
            {
                SettingsHandler.Settings.Token = token;
                SettingsHandler.Save();
            }
            ApiHandler.AddBearer(token);
            NewlyRegistered = false;
            //EndLoading();
            IsLoggedIn = true;
            Notify.ResumeLogging();
            Globals.Init();
        }
        public void RegisterComplete()
        {
            NewlyRegistered = true;
            mv.CurrentView = new LoginView();
        }


        public async void EndLoading()
        {
            int secondsToLoad = (int)(DateTime.Now - startTime).TotalSeconds;

            var rnd = new Random();
            int val = rnd.Next(2, 4);

            if (secondsToLoad < val)
            {
                secondsToLoad = val - secondsToLoad;
                await Task.Delay(secondsToLoad * 1000);
            }
            FinishedLoading = true;

            SPRadioButtons.Children.OfType<RadioButton>().ToList().ForEach(x => x.IsChecked = false);

            mv.CurrentView = mv.HomeVM;
            //mv.CurrentView = mv.LoadingVM;
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
        public void SetLoadingStatus(string message)
        {
            if(lv != null)
            {
                lv.SetStatusMessage(message);
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(counterToUnlockEditor > 5)
            {
                WallEditorBtn.Visibility = Visibility.Visible;
            }
            counterToUnlockEditor++;
        }
    }
}
