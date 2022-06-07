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
            
            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o => { if(wnd.IsLoaded) CurrentView = HomeVM;});
            SettingsViewCommand = new RelayCommand(o => { if (wnd.IsLoaded) CurrentView = SettingsVM; });
            AboutViewCommand = new RelayCommand(o => { if (wnd.IsLoaded) CurrentView = AboutVM; });
            RegisterViewCommand = new RelayCommand(o => { CurrentView = RegisterVM; });
            LoginViewCommand = new RelayCommand(o => { CurrentView = LoginVM; });
            SkinSwitcherViewCommand = new RelayCommand(o => { if (wnd.IsLoaded) CurrentView = SkinSwitcherVM; });
            RoutinesViewCommand = new RelayCommand(o => { if (wnd.IsLoaded) CurrentView = RoutinesVM; });
            StratMakerViewCommand = new RelayCommand(o => { if (wnd.IsLoaded) CurrentView = StratMakerVM; });
            TeamViewCommand = new RelayCommand(o => { if (wnd.IsLoaded) CurrentView = TeamVM; });
            CalendarViewCommand = new RelayCommand(o => { if (wnd.IsLoaded) CurrentView = CalendarVM; });
            ScrimViewCommand = new RelayCommand(o => { if (wnd.IsLoaded) CurrentView = ScrimVM; });
            StatsViewCommand = new RelayCommand(o => { if (wnd.IsLoaded) CurrentView = StatsVM; });
        }
    }
}
