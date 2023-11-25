using xstrat.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using xstrat.MVVM.View;

namespace xstrat.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        MainWindow wnd = (MainWindow)Application.Current.MainWindow;
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand SettingsViewCommand { get; set; }
        public RelayCommand AboutViewCommand { get; set; }
        public RelayCommand RegisterViewCommand { get; set; }
        public RelayCommand LoginViewCommand { get; set; }
        public RelayCommand SkinSwitcherViewCommand { get; set; }
        public RelayCommand RoutinesViewCommand { get; set; }
        public RelayCommand StratMakerViewCommand { get; set; }
        public RelayCommand TeamViewCommand { get; set; }
        public RelayCommand CalendarViewCommand { get; set; }
        public RelayCommand ScrimViewCommand { get; set; }
        public RelayCommand StatsViewCommand { get; set; }
        public RelayCommand LicenseViewCommand { get; set; }
        public RelayCommand LoadingViewCommand { get; set; }
        public RelayCommand WallEditorViewCommand { get; set; }
        public RelayCommand ReplayViewCommand { get; set; }
        public RelayCommand OffdayViewCommand { get; set; }
        public RelayCommand PasswordResetViewCommand { get; set; }


        public HomeViewModel HomeVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }
        public AboutViewModel AboutVM { get; set; }
        public RegisterViewModel RegisterVM { get; set; }
        public LoginViewModel LoginVM { get; set; }
        public SkinSwitcherViewModel SkinSwitcherVM { get; set; }
        public RoutinesViewModel RoutinesVM { get; set; }
        public StratMakerViewModel StratMakerVM { get; set; }
        public TeamViewModel TeamVM { get; set; }
        public CalendarViewModel CalendarVM { get; set; }
        public ScrimViewModel ScrimVM { get; set; }
        public StatsViewModel StatsVM { get; set; }
        public LicenseViewModel LicenseVM { get; set; }
        public LoadingViewModel LoadingVM { get; set; }
        public WallEditorViewModel WallEditorVM { get; set; }
        public ReplayViewModel ReplayVM { get; set; }
        public OffdayViewModel OffdayVM { get; set; }
        public PasswordResetViewModel PasswordResetVM { get; set; }

        public object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            HomeVM = new HomeViewModel();
            SettingsVM = new SettingsViewModel();
            AboutVM = new AboutViewModel();
            LoginVM = new LoginViewModel();
            RegisterVM = new RegisterViewModel();
            SkinSwitcherVM = new SkinSwitcherViewModel();
            RoutinesVM = new RoutinesViewModel();
            StratMakerVM = new StratMakerViewModel();
            TeamVM = new TeamViewModel();
            CalendarVM = new CalendarViewModel();
            ScrimVM = new ScrimViewModel();
            StatsVM = new StatsViewModel();
            LicenseVM = new LicenseViewModel();
            LoadingVM = new LoadingViewModel();
            WallEditorVM = new WallEditorViewModel();
            ReplayVM = new ReplayViewModel();
            OffdayVM = new OffdayViewModel();
            PasswordResetVM = new PasswordResetViewModel();

            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o => { if(wnd.IsLoaded && wnd.IsLoggedIn && wnd.FinishedLoading && IsAllowedToExit()) CurrentView = HomeVM;});
            SettingsViewCommand = new RelayCommand(o => { if (wnd.IsLoaded && IsAllowedToExit()) CurrentView = SettingsVM; });
            AboutViewCommand = new RelayCommand(o => { if (wnd.IsLoaded && wnd.IsLoggedIn && wnd.FinishedLoading && IsAllowedToExit()) CurrentView = AboutVM; });
            RegisterViewCommand = new RelayCommand(o => { CurrentView = RegisterVM; });
            LoginViewCommand = new RelayCommand(o => { CurrentView = LoginVM; });
            SkinSwitcherViewCommand = new RelayCommand(o => { if (wnd.IsLoaded && wnd.IsLoggedIn && wnd.FinishedLoading && IsAllowedToExit()) CurrentView = SkinSwitcherVM; });
            RoutinesViewCommand = new RelayCommand(o => { if (wnd.IsLoaded && wnd.IsLoggedIn && wnd.FinishedLoading && IsAllowedToExit()) CurrentView = RoutinesVM; });
            StratMakerViewCommand = new RelayCommand(o => { if (wnd.IsLoaded && wnd.IsLoggedIn && wnd.FinishedLoading && IsAllowedToExit()) CurrentView = StratMakerVM; });
            TeamViewCommand = new RelayCommand(o => { if (wnd.IsLoaded && wnd.IsLoggedIn && wnd.FinishedLoading && IsAllowedToExit()) CurrentView = TeamVM; });
            CalendarViewCommand = new RelayCommand(o => { if (wnd.IsLoaded && wnd.IsLoggedIn && wnd.FinishedLoading && IsAllowedToExit()) CurrentView = CalendarVM; });
            ScrimViewCommand = new RelayCommand(o => { if (wnd.IsLoaded && wnd.IsLoggedIn && wnd.FinishedLoading && IsAllowedToExit()) CurrentView = ScrimVM; });
            StatsViewCommand = new RelayCommand(o => { if (wnd.IsLoaded && wnd.IsLoggedIn && wnd.FinishedLoading && IsAllowedToExit()) CurrentView = StatsVM; });
            LicenseViewCommand = new RelayCommand(o => { if (wnd.IsLoaded && wnd.IsLoggedIn && wnd.FinishedLoading && IsAllowedToExit()) CurrentView = LicenseVM; });
            LoadingViewCommand = new RelayCommand(o => { CurrentView = LoadingVM; });
            WallEditorViewCommand = new RelayCommand(o => { CurrentView = WallEditorVM; });
            ReplayViewCommand = new RelayCommand(o => { if (wnd.IsLoaded && wnd.IsLoggedIn && wnd.FinishedLoading && IsAllowedToExit()) CurrentView = ReplayVM; });
            OffdayViewCommand = new RelayCommand(o => { if (wnd.IsLoaded && wnd.IsLoggedIn && wnd.FinishedLoading && IsAllowedToExit()) CurrentView = OffdayVM; });
            PasswordResetViewCommand = new RelayCommand(o => { CurrentView = PasswordResetVM; });
        }

        public bool IsAllowedToExit()
        {
            var view = wnd.CurrentView;
            if (view != null)
            {
                return view.AllowExit();
            }
            return true;
        }
    }
}
