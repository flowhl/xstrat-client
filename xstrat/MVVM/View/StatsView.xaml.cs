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

namespace xstrat.MVVM.View
{
    /// <summary>
    /// Interaction logic for StatsView.xaml
    /// </summary>
    /// 

    public partial class StatsView : UserControl
    {
        #region Graph stuff

        #region General

        //Radial 
        public ISeries[] RadialSeries { get; set; } = new ISeries[] { };

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

        #endregion

        #region Scrim
        #endregion

        #region Ranked

        #endregion

        #endregion

        public StatsView()
        {
            BuildGraphs();
            InitializeComponent();
            PlayerList.Children.Clear();
            foreach (var user in Globals.teammates)
            {
                ColorDisplay newCD = new ColorDisplay(user.color.Replace("#", "#80").ToSolidColorBrush(), user.name);
                PlayerList.Children.Add(newCD);
            }
            Loaded += StatsView_Loaded;
            StatsDataSource.OnUpdateStats += StatsDataSource_OnUpdateStats;
        }

        private void StatsDataSource_OnUpdateStats(object sender, EventArgs e)
        {
            BuildGraphs();
        }

        private void StatsView_Loaded(object sender, RoutedEventArgs e)
        {

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


            //General
            //Radial
            List<PolarLineSeries<int>> radiallines = new List<PolarLineSeries<int>>();

            int max_userscrimpercentage = 0;
            int max_maxmmr = 0;
            int max_currmmr = 0;
            int max_kd = 0;
            int max_mmrchange = 0;
            int max_winrate = 0;

            foreach (var user in Globals.teammates)
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
                        kd = (deaths == 0 ) ? 0 : (int)((kills / deaths) * 100);
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


                if(userscrimpercentage > max_userscrimpercentage)
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

            foreach (var entry in radiallines)
            {
                List<int> linevalues = entry.Values.ToList();
                double userscrimpercentage = linevalues[0];
                double maxmmr  = linevalues[1];
                double currmmr  = linevalues[2];
                double kd  = linevalues[3];
                double mmrchange  = linevalues[4];
                double winrate = linevalues[5];

                userscrimpercentage = (max_userscrimpercentage == 0) ? 0 : (int)((double)(userscrimpercentage / max_userscrimpercentage) * global_max);
                maxmmr = (max_maxmmr == 0) ? 0 : (int)((double)(maxmmr / max_maxmmr) * global_max);
                currmmr = (max_currmmr == 0) ? 0 : (int)((double)(currmmr / max_currmmr) * global_max);
                kd = (max_kd == 0) ? 0 : (int)((double)(kd / max_kd) * global_max);
                mmrchange = (max_mmrchange == 0) ? 0 : (int)((double)(mmrchange / max_mmrchange) * global_max);
                winrate = (max_winrate == 0) ? 0 : (int)((double)(winrate / max_winrate) * global_max);

                entry.Values = new[] { (int)userscrimpercentage, (int)maxmmr, (int)currmmr, (int)kd, (int)mmrchange, (int)winrate };
            }
            

            RadialSeries = radiallines.ToArray();
        }

    }
}
       

