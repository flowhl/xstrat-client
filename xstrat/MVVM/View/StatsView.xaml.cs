using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using CommunityToolkit.Mvvm.ComponentModel;
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
using xstrat.Json;
using xstrat.Ui;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using LiveChartsCore.Measure;

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for StatsView.xaml
    /// </summary>
    /// 

    [ObservableObject]
    public partial class StatsView : UserControl
    {
        #region Graph stuff

        #region General

        //Radial 

        public ObservableCollection<ISeries> RadialSeries { get; set; } = new ObservableCollection<ISeries> { };

        public PolarAxis[] RadialAngleAxes { get; set; } =
        {
            new PolarAxis
            {
                LabelsRotation = LiveCharts.TangentAngle,
                LabelsBackground = LiveChartsCore.Drawing.LvcColor.Empty,
                LabelsPaint =  new SolidColorPaint(new SKColor(245, 245, 245)),
                ShowSeparatorLines = false,
                Labels = new[] { "Scrim participation", "Max MMR", "Current MMR","MMR per game", "KD", "Winrate" }
            }
        };

        //Bars

        public ObservableCollection<ISeries> GeneralBarSeries { get; set; } = new ObservableCollection<ISeries> { };

        public ObservableCollection<Axis> GeneralBarXAxes { get; set; } =  new ObservableCollection<Axis>();

        #endregion

        #region Scrim

        //Bars

        public ObservableCollection<ISeries> ScrimBarSeries { get; set; } = new ObservableCollection<ISeries> { };

        //History

        public ObservableCollection<Axis> ScrimHistoryXAxes { get; set; } = new ObservableCollection<Axis>();

        public ObservableCollection<ISeries> ScrimHistorySeries { get; set; } = new ObservableCollection<ISeries> { };

        #endregion

        #region Ranked

        #endregion

        #endregion

        public List<User> users = new List<User>();

        private List<User> customUsers = new List<User>();

        public StatsView()
        {
            InitializeComponent();
            PlayerList.Children.Clear();
            foreach (var user in Globals.teammates)
            {
                ColorDisplay newCD = new ColorDisplay(user.color.Replace("#", "#80").ToSolidColorBrush(), user.name, true);
                PlayerList.Children.Add(newCD);
                newCD.ColorDisplayCheckstatusChanged += NewCD_ColorDisplayCheckstatusChanged;
            }
            Loaded += StatsView_Loaded;
            StatsDataSource.OnUpdateStats += StatsDataSource_OnUpdateStats;
        }

        private void NewCD_ColorDisplayCheckstatusChanged(object sender, EventArgs e)
        {
            UpdateUsers();
        }

        private async void AddCustomUser(string name)
        {
            bool needretrieve = true;
            int? id;
            var urow = Globals.customUserIdsAndNames.Where(x => x.Item2.ToUpper().Trim() == name.ToUpper().Trim());
            if (urow.Any())
            {
                id = urow.FirstOrDefault().Item1;
                needretrieve = false;
            }
            else
            {
                id = Globals.LastCustomUserId;
            }
            if(id != null)
            {
                User newuser = new User { id = id.GetValueOrDefault(), color = "#336cb5", name = name, ubisoft_id = name };
                customUsers.Add(newuser);
                ColorDisplay newcd = new ColorDisplay(newuser.color.Replace("#", "#80").ToSolidColorBrush(), newuser.name, true);
                PlayerList.Children.Add(newcd);
                newcd.ColorDisplayCheckstatusChanged += NewCD_ColorDisplayCheckstatusChanged;
                if (needretrieve)
                {
                    await StatsDataSource.RetrieveStatsAllSeasons(newuser.ubisoft_id, newuser.id);
                    await StatsDataSource.RetrieveStatsDataAsync(newuser.ubisoft_id, newuser.id);
                }
                UpdateUsers();
                newcd.SetStatus(true);
            }
        }

        public void UpdateUsers()
        {
            users.Clear();
            if (team)
            {
                foreach (var plc in PlayerList.Children)
                {
                    ColorDisplay cd = plc as ColorDisplay;
                    if (cd.Status)
                    {
                        int uid = Globals.getUserIdFromName(cd.NameInput);
                        if (uid >= 0) 
                        {
                            users.Add(Globals.getUserFromId(uid)); 
                        }
                        else
                        {
                            var crows = customUsers.Where(x => x.name == cd.NameInput);
                            if (!crows.Any())
                            {
                                users.Add(new User { id = -1, name = cd.NameInput, color = "#336cb5" });
                            }
                            else
                            {
                                users.Add(crows.FirstOrDefault());
                            }
                        }
                    }
                }
            }
            else
            {
                users.Add(Globals.currentUser);
                foreach (var plc in PlayerList.Children)
                {
                    ColorDisplay cd = plc as ColorDisplay;
                    if(cd.NameInput == Globals.currentUser.name)
                    {
                        cd.SetStatus(true);
                    }
                }
            }
            BuildGraphs();
        }

        public bool team { get; set; }

        private void StatsDataSource_OnUpdateStats(object sender, EventArgs e)
        {
            UpdateUsers();
        }

        private void StatsView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUsers();
        }

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            RetrieveData();
        }


        public void RetrieveData()
        {            
            StatsDataSource.RetrieveData();
        }

        public void BuildGraphs()
        {
            var allSeasons = StatsDataSource.PlayerAllSeasonStats;
            var Player = StatsDataSource.PlayerStats;
            var ScrimPercentage = StatsDataSource.PlayerScrimParticipationPercentages;
            var ScrimParticipation = StatsDataSource.PlayerScrimParticipation;


            //General
            #region Radial
            //Radial
            List<PolarLineSeries<int>> radiallines = new List<PolarLineSeries<int>>();

            int max_userscrimpercentage = 0;
            int max_maxmmr = 0;
            int max_currmmr = 0;
            int max_kd = 0;
            int max_mmrchange = 0;
            int max_winrate = 0;

            foreach (var user in users)
            {
                int userscrimpercentage = 0;
                int maxmmr = 0;
                int currmmr = 0;
                int kd = 0;
                int mmrchange = 0;
                int winrate = 0;
                var percRow = ScrimPercentage.Where(x => x.user_id == user.id);
                if (percRow.Any())
                {
                    userscrimpercentage = (int)(percRow.FirstOrDefault().type1ratio * 100);
                }

                var seasonsrow = allSeasons.Where(x => x.First().xstrat_user_id == user.id);

                if (seasonsrow.Any())
                {
                    var currentseason = seasonsrow.First().First();
                    maxmmr = currentseason.max_mmr.GetValueOrDefault(0);
                    currmmr = currentseason.mmr.GetValueOrDefault(0);
                    if (currentseason.deaths != null)
                    {
                        double kills = (double)currentseason.kills.GetValueOrDefault(0);
                        double deaths = (double)currentseason.deaths.GetValueOrDefault(0);
                        kd = (deaths == 0) ? 0 : (int)((kills / deaths) * 100);
                    }
                    mmrchange = Math.Abs(currentseason.last_match_mmr_change.GetValueOrDefault(0));

                    double wins = currentseason.wins.GetValueOrDefault(0);
                    double total = (currentseason.wins.GetValueOrDefault(0) + currentseason.losses.GetValueOrDefault(0) + currentseason.abandons.GetValueOrDefault(0));
                    winrate = (total == 0) ? 0 : (int)(wins / total * 100);
                }

                SKColor ucolor = SKColors.DarkGray;
                SKColor.TryParse(user.color.Replace("#", "#4A"), out ucolor);

                PolarLineSeries<int> entry = new PolarLineSeries<int>
                {
                    Values = new[] { userscrimpercentage, maxmmr, currmmr, kd, mmrchange, winrate },
                    LineSmoothness = 2,
                    GeometrySize = 0,
                    Fill = new SolidColorPaint(ucolor),
                    Stroke = new SolidColorPaint(SKColor.Empty),
                    Name = user.name,
                };


                if (userscrimpercentage > max_userscrimpercentage)
                {
                    max_userscrimpercentage = userscrimpercentage;
                }
                if (maxmmr > max_maxmmr)
                {
                    max_maxmmr = maxmmr;
                }
                if (currmmr > max_currmmr)
                {
                    max_currmmr = currmmr;
                }
                if (kd > max_kd)
                {
                    max_kd = kd;
                }
                if (mmrchange > max_mmrchange)
                {
                    max_mmrchange = mmrchange;
                }
                if (winrate > max_winrate)
                {
                    max_winrate = winrate;
                }

                radiallines.Add(entry);

            }

            //balancing: -> maximum aller werte abgleichen
            int global_max = Math.Max(max_userscrimpercentage, Math.Max(max_maxmmr, Math.Max(max_currmmr, Math.Max(max_kd, Math.Max(max_mmrchange, max_winrate)))));
            //int global_max = 1000;

            foreach (var entry in radiallines)
            {
                List<int> linevalues = entry.Values.ToList();
                double userscrimpercentage = linevalues[0];
                double maxmmr = linevalues[1];
                double currmmr = linevalues[2];
                double kd = linevalues[3];
                double mmrchange = linevalues[4];
                double winrate = linevalues[5];

                userscrimpercentage = (max_userscrimpercentage == 0) ? 0 : (int)((double)(userscrimpercentage / max_userscrimpercentage) * global_max);
                maxmmr = (max_maxmmr == 0) ? 0 : (int)((double)(maxmmr / max_maxmmr) * global_max);
                currmmr = (max_currmmr == 0) ? 0 : (int)((double)(currmmr / max_currmmr) * global_max);
                kd = (max_kd == 0) ? 0 : (int)((double)(kd / max_kd) * global_max);
                mmrchange = (max_mmrchange == 0) ? 0 : (int)((double)(mmrchange / max_mmrchange) * global_max);
                winrate = (max_winrate == 0) ? 0 : (int)((double)(winrate / max_winrate) * global_max);

                entry.Values = new[] { (int)userscrimpercentage, (int)maxmmr, (int)currmmr, (int)kd, (int)mmrchange, (int)winrate };
            }

            RadialSeries.Clear();
            radiallines.ForEach(x => RadialSeries.Add(x));
            //RadialSeries = radiallines;
            #endregion

            #region GeneralBars

            GeneralBarSeries.Clear();

            List<double> l_currmmr = new List<double>();
            List<double> l_maxmmr = new List<double>();
            List<double> l_mmrchange = new List<double>();

            List<string> categories = new List<string>();
            foreach (var user in users)
            {
                categories.Add(user.name);

                var seasonsrow = allSeasons.Where(x => x.First().xstrat_user_id == user.id);
                if (seasonsrow.Any())
                {
                    var currentseason = seasonsrow.First().First();
                    l_currmmr.Add(Math.Abs(currentseason.mmr.GetValueOrDefault(0)));
                    l_maxmmr.Add(Math.Abs(currentseason.max_mmr.GetValueOrDefault(0)));
                    l_mmrchange.Add(Math.Abs(currentseason.last_match_mmr_change.GetValueOrDefault(0)));
                }
                else
                {
                    l_currmmr.Add(0);
                    l_maxmmr.Add(0);
                    l_mmrchange.Add(0);
                }
            }

            GeneralBarXAxes.Clear();

            GeneralBarXAxes.Add(new Axis
            {
                Labels = categories.ToArray()
            });

            GeneralBarSeries.Add(new ColumnSeries<double>
            {
                Values = l_currmmr.ToArray(),
                Fill = new SolidColorPaint(SKColor.Parse("#2D9ED5")),
                Stroke = new SolidColorPaint(SKColor.Empty),
                Name = "Current MMR",
            });

            GeneralBarSeries.Add(new ColumnSeries<double>
            {
                Values = l_maxmmr.ToArray(),
                Fill = new SolidColorPaint(SKColor.Parse("#D64251")),
                Stroke = new SolidColorPaint(SKColor.Empty),
                Name = "Max MMR",
            });

            GeneralBarSeries.Add(new ColumnSeries<double>
            {
                Values = l_mmrchange.ToArray(),
                Fill = new SolidColorPaint(SKColor.Parse("#4ab859")),
                Stroke = new SolidColorPaint(SKColor.Empty),
                Name = "MMR per match",
            });


            #endregion

            #region ScrimLayeredBar

            ScrimBarSeries.Clear();

            List<int> l_ignore = new List<int>();
            List<int> l_accept = new List<int>();
            List<int> l_deny = new List<int>();

            foreach (var user in users)
            {
                var scrimpercrow = ScrimPercentage.Where(x => x.user_id == user.id);
                if (scrimpercrow.Any())
                {
                    l_ignore.Add(scrimpercrow.FirstOrDefault().type0count);
                    l_accept.Add(scrimpercrow.FirstOrDefault().type1count);
                    l_deny.Add(scrimpercrow.FirstOrDefault().type2count);
                }
                else
                {
                    l_currmmr.Add(0);
                    l_maxmmr.Add(0);
                    l_mmrchange.Add(0);
                }
            }

            ScrimBarSeries.Add(new StackedColumnSeries<int>
            {
                Values = l_ignore,
                Stroke = null,
                DataLabelsPaint = new SolidColorPaint(new SKColor(45, 45, 45)),
                DataLabelsSize = 14,
                DataLabelsPosition = DataLabelsPosition.Middle,
                Fill = new SolidColorPaint(SKColor.Parse("#2D9ED5")),
                Name = "Ignored Scrims"
            });

            ScrimBarSeries.Add(new StackedColumnSeries<int>
            {
                Values = l_accept,
                Stroke = null,
                DataLabelsPaint = new SolidColorPaint(new SKColor(45, 45, 45)),
                DataLabelsSize = 14,
                Fill = new SolidColorPaint(SKColor.Parse("#4ab859")),
                DataLabelsPosition = DataLabelsPosition.Middle,
                Name = "Accepted Scrims"
            });

            ScrimBarSeries.Add(new StackedColumnSeries<int>
            {
                Values = l_deny,
                Stroke = null,
                DataLabelsPaint = new SolidColorPaint(new SKColor(45, 45, 45)),
                DataLabelsSize = 14,
                Fill = new SolidColorPaint(SKColor.Parse("#D64251")),
                DataLabelsPosition = DataLabelsPosition.Middle,
                Name = "Denied Scrims"
            });

            #endregion

            #region ScrimHistory
            ScrimHistorySeries.Clear();
            List<string> historylabels = new List<string>();
            List<Tuple<int, List<int>>> ScrimHistoryData = new List<Tuple<int, List<int>>>();
            foreach (var user in users)
            {
                ScrimHistoryData.Add(new Tuple<int, List<int>>(user.id, new List<int>()));
            }

            DateTime time = DateTime.Now.AddDays(-100);
            for (int i = 0; i < 101; i++)
            {
                foreach (var userentry in ScrimParticipation)
                {
                    var resultrows = userentry.Where(x => x.response_typ == 1 && x.time_start != null && x.time_start.StartsWith(time.Year+"/"+time.Month.ToString().PadLeft(2,'0')+"/"+time.Day.ToString().PadLeft(2, '0')));
                    var historyrows = ScrimHistoryData.Where(x => x.Item1 == userentry.FirstOrDefault().user_id);
                    if (historyrows.Any())
                    {
                        historyrows.FirstOrDefault().Item2.Add(resultrows.Count());
                    }
                    
                }
                historylabels.Add(time.Day + ". " + time.Month + ".");
                time =  time.AddDays(1);
            }

            foreach (var data in ScrimHistoryData)
            {
                ScrimHistorySeries.Add(new LineSeries<int>
                {
                    Name = Globals.UserIdToName(data.Item1),
                    Values = data.Item2,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColor.Parse(Globals.getUserFromId(data.Item1).color)),
                    GeometrySize = 0,
                });
            }
            ScrimHistoryXAxes.Clear();
            ScrimHistoryXAxes.Add(new Axis
            {
                Labels = historylabels
            });

            #endregion
        }

        private void TeamToggle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            team = !TeamToggle.getStatus();
            if (team)
            {
                foreach (var plc in PlayerList.Children)
                {
                    ColorDisplay cd = plc as ColorDisplay;
                    cd.SetStatus(true);
                }
            }
            UpdateUsers();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            string inputString = Microsoft.VisualBasic.Interaction.InputBox("Add Name", "Add player to track", "");
            if(!Globals.teammates.Where(x => x.name.ToUpper().Trim() == inputString.ToUpper().Trim()).Any())
            {
                if(!customUsers.Where(x => x.name.ToUpper().Trim() == inputString.ToUpper().Trim()).Any())
                {
                    AddCustomUser(inputString);
                }
            }
        }
    }
}
       

