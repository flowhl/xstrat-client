using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using xstrat.Json;
using xstrat.MVVM.ViewModel;
using SVGImage.SVG;
using System.Reflection;
using System.Globalization;
using System.Xml;
using xstrat.MVVM.View;
using JetBrains.Annotations;
using static WPFSpark.MonitorHelper;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using System.Xml.Linq;
using SkiaSharp.Views.WPF;
using NuGet;
using xstrat.Models.Supabase;

namespace xstrat.Core
{
    public class Player
    {
        public ICollection<Response> Responses { get; set; }
        public string Id { get; set; }
    }

    public static class Extensions
    {
        public static T[] Append<T>(this T[] array, T item)
        {
            if (array == null)
            {
                return new T[] { item };
            }
            T[] result = new T[array.Length + 1];
            array.CopyTo(result, 0);
            result[array.Length] = item;
            return result;
        }
        public static T FindVisualParent<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindVisualParent<T>(parentObject);
            }
        }
    }

    public class Response
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

    }

    public class Window
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public IEnumerable<Player> AvailablePlayers { get; set; }

    }
    public static class StatsDataSource
    {
        public static DateTime LastRetrieve;
        public delegate void StatsUpdateHandler(object sender, EventArgs e);
        public static event StatsUpdateHandler OnUpdateStats;

        public static List<StatsResponse> PlayerStats = new List<StatsResponse>();
        public static List<List<StatsBySeasonDetail>> PlayerAllSeasonStats = new List<List<StatsBySeasonDetail>>();
        public static List<List<ScrimParticipationResult>> PlayerScrimParticipation = new List<List<ScrimParticipationResult>>();
        public static List<PlayerScrimParticipationPercentage> PlayerScrimParticipationPercentages = new List<PlayerScrimParticipationPercentage>();

        public async static Task Init()
        {
            await RetrieveData();
        }

        public static async Task RetrieveData()
        {
            if ((DateTime.Now - LastRetrieve).TotalMinutes <= 5) return;
            Globals.wnd.SetLoadingStatus("Loading Stats Data");
            await StartRetrieveStatsData();
            Globals.wnd.SetLoadingStatus("Loading Stats Data for season");
            await StartRetrieveStatsAllSeasons();
            LastRetrieve = DateTime.Now;
            Globals.wnd.SetLoadingStatus("");
        }

        #region stats
        public static void DataRetrieved()
        {
            CalculatePlayerScrimPercentage();
        }
        private static void CalculatePlayerScrimPercentage()
        {
            PlayerScrimParticipationPercentages.Clear();
            foreach (var player in PlayerScrimParticipation)
            {
                if (player.Count > 0)
                {
                    string user_id = player.FirstOrDefault().user_id;
                    int type0 = 0;
                    int type1 = 0;
                    int type2 = 0;
                    int count = 0;

                    foreach (var scrim in player)
                    {
                        count++;
                        if (scrim.response_typ == 0)
                        {
                            type0++;
                        }
                        if (scrim.response_typ == 1)
                        {
                            type1++;
                        }
                        if (scrim.response_typ == 2)
                        {
                            type2++;
                        }
                    }
                    if (user_id != null)
                    {
                        PlayerScrimParticipationPercentages.Add(new PlayerScrimParticipationPercentage
                        (
                            user_id,
                            count,
                            (double)type0 / count,
                            (double)type1 / count,
                            (double)type2 / count,
                            type0,
                            type1,
                            type2
                        ));
                    }
                }

            }
        }
        public static async Task RetrieveStatsDataAsync(string ubisoft_id, string user_id)
        {
            //TODO: Implement tracker
            //if (!string.IsNullOrEmpty(ubisoft_id) && user_id.IsNotNullOrEmpty())
            //{
            //    try
            //    {
            //        (bool, string) result = await ApiHandler.GetStats(ubisoft_id);
            //        if (result.Item1)
            //        {
            //            string response = result.Item2;
            //            //convert to json instance
            //            JObject json = JObject.Parse(response);
            //            var data = json.SelectToken("data").ToString();
            //            if (data != null && data != "")
            //            {
            //                StatsResponse sr = JsonConvert.DeserializeObject<StatsResponse>(data);
            //                sr.StatsResponseDetails.Values.First().xstrat_user_id = user_id;
            //                if (sr != null)
            //                {
            //                    PlayerStats.Add(sr);
            //                }
            //            }
            //            else
            //            {
            //                Notify.sendError("Playerstats could not be loaded");
            //                throw new Exception("Playerstats could not be loaded");
            //            }
            //        }
            //        else
            //        {
            //            return;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Notify.sendError(ex.Message);
            //    }

            //    //scrim stats
            //    if (user_id.IsNotNullOrEmpty())
            //    {
            //        try
            //        {
            //            (bool, string) result = await ApiHandler.GetScrimParticipation(user_id);
            //            if (result.Item1)
            //            {
            //                string response = result.Item2;
            //                //convert to json instance
            //                JObject json = JObject.Parse(response);
            //                var data = json.SelectToken("data").ToString();
            //                if (data != null && data != "")
            //                {
            //                    List<ScrimParticipationResult> sr = JsonConvert.DeserializeObject<List<ScrimParticipationResult>>(data);
            //                    if (sr != null)
            //                    {
            //                        PlayerScrimParticipation.Add(sr);
            //                    }
            //                }
            //                else
            //                {
            //                    Notify.sendError("Playerstats could not be loaded");
            //                    throw new Exception("Playerstats could not be loaded");
            //                }
            //            }
            //            else
            //            {
            //                return;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            Notify.sendError(ex.Message);
            //        }
            //    }

            //}
            //return;
        }
        public static async Task RetrieveStatsAllSeasons(string ubisoft_id, string user_id)
        {
            //if (!string.IsNullOrEmpty(ubisoft_id) && user_id.IsNotNullOrEmpty())
            //{
            //    try
            //    {
            //        (bool, string) result = await ApiHandler.GetStatsByAllSeason(ubisoft_id);
            //        if (result.Item1)
            //        {
            //            string response = result.Item2;
            //            //convert to json instance
            //            JObject json = JObject.Parse(response);
            //            var data = "[ \n\r " + json.SelectToken("data").ToString().Replace("[", "").Replace("]", "") + "\n\r ]";
            //            if (data != null && data != "")
            //            {
            //                var sr = JsonConvert.DeserializeObject<List<StatsBySeasonDetail>>(data);

            //                sr.ForEach(x => x.xstrat_user_id = user_id);

            //                if (sr.Count > 0)
            //                {
            //                    PlayerAllSeasonStats.Add(sr);
            //                }
            //            }
            //            else
            //            {
            //                Notify.sendError("Playerstats could not be loaded");
            //                throw new Exception("Playerstats could not be loaded");
            //            }
            //        }
            //        else
            //        {
            //            return;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Notify.sendError(ex.Message);
            //    }

            //}
            //return;
        }
        public static async Task StartRetrieveStatsData()
        {
            PlayerStats.Clear();
            PlayerScrimParticipation.Clear();
            foreach (var user in DataCache.CurrentTeamMates)
            {
                //playerstats
                if (user.UbisoftId != null && user.UbisoftId != "")
                {
                    await RetrieveStatsDataAsync(user.UbisoftId, user.Id);
                }
            }
            DataRetrieved();
        }
        public static async Task StartRetrieveStatsAllSeasons()
        {
            PlayerAllSeasonStats.Clear();
            foreach (var user in DataCache.CurrentTeamMates)
            {
                if (user.UbisoftId != null && user.UbisoftId != "")
                {
                    await RetrieveStatsAllSeasons(user.UbisoftId, user.Id);
                }
            }
        }
        #endregion
    }
    public static class Globals
    {
        public static MainWindow wnd = (MainWindow)Application.Current.MainWindow;
        public static List<OffDayType> OffDayTypes = new List<OffDayType>();
        public static List<CalendarFilterType> CalendarFilterTypes = new List<CalendarFilterType>();

        public static List<ScrimMode> ScrimModes = new List<ScrimMode>();
        public static List<EventType> EventTypes = new List<EventType>();

        public static DateTime lastEventClicked { get; set; }

        public static event EventHandler<EventArgs> OnDataRetrieved;

        public static void CallOnDataRetrieved()
        {
            OnDataRetrieved(null, new EventArgs());
        }

        public static event EventHandler<CalendarEventCreatedArgs> CalendarEventCreated;

        public static void CallCalendarEventCreated(DateTime Date)
        {
            CalendarEventCreated(null, new CalendarEventCreatedArgs { Date = Date });
        }
        public static string XStratInstallPath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public static string[] SeasonNames = new string[]{
            "Current Season", // API ID = 0
            "Black Ice", //  Feburary 2, 2016
            "Dust Line", // May 10, 2016
            "Skull Rain", // August 2, 2016
            "Red Crow", // November 17, 2016
            "Velvet Shell", // February 7, 2017
            "Health", // June 7, 2017
            "Blood Orchid", // September 5, 2017
            "White Noise", // December 5, 2017
            "Chimera", // March 6, 2018
            "Para Bellum", // June 7, 2018
            "Grim Sky", // September 4, 2018
            "Wind Bastion", // December 4, 2018
            "Burnt Horizon", // March 6, 2019
            "Phantom Sight", // June 11, 2019
            "Ember Rise", // September 11, 2019
            "Shifting Tides", // December 3, 2019
            "Void Edge", // March 20, 2020
            "Steel Wave", // June 16, 2020
            "Shadow Legacy", // September 10, 2020
            "Neon Dawn", // December 1, 2020
            "Crimson Heist", // March 16, 2021
            "North Star", // June 14, 2021
            "Crystal Guard", // September 7, 2021
            "High Calibre", // November 30, 2021
            "Demon Veil", // March 15, 2022
            "Vector Glare", // June 14, 2022
            "Brutal Swarm", // September 6, 2022
            "Solar Raid", //Y7S4 2022-12-06 id=28
            };

        public static string[] IconPaths = new string[]{
            "g_Alibi.png"
            ,"g_AruniGate.png"
            ,"g_Bandit.png"
            ,"g_Barb.png"
            ,"g_Barricade.png"
            ,"g_BulletProofCam.png"
            ,"g_BulletProofCamArrow.png"
            ,"g_Castle.png"
            ,"g_Claymore.png"
            ,"g_Drone.png"
            ,"g_Echo.png"
            ,"g_Ela.png"
            ,"g_Frost.png"
            ,"g_Goyo.png"
            ,"g_GoyoShieldTopDown.png"
            ,"g_ImpactGrenade.png"
            ,"g_Jager.png"
            ,"g_Kaid.png"
            ,"g_Kapkan.png"
            ,"g_Lesion.png"
            ,"g_Maestro.png"
            ,"g_MelusiBanshee.png"
            ,"g_Mira.png"
            ,"g_Mozzie.png"
            ,"g_Mute.png"
            ,"g_NitroCell.png"
            ,"g_Nomad.png"
            ,"g_Osa.png"
            ,"g_ProximityMine.png"
            ,"g_Shield.png"
            ,"g_ShieldTopDown.png"
            ,"g_Smoke.png"
            ,"g_Thunderbird.png"
            ,"g_ToxicBabe.png"
            ,"g_ToxicBabeSmoke.png"
            ,"g_TraxStingers.png"
            ,"g_Valkyrie.png"
            ,"g_Wamai.png"
            ,"g_Zero.png"
            ,"op_ace.png"
            ,"op_alibi.png"
            ,"op_amaru.png"
            ,"op_aruni.png"
            ,"op_ash.png"
            ,"op_azami.png"
            ,"op_bandit.png"
            ,"op_blackbeard.png"
            ,"op_blitz.png"
            ,"op_buck.png"
            ,"op_capitao.png"
            ,"op_castle.png"
            ,"op_caveira.png"
            ,"op_clash.png"
            ,"op_doc.png"
            ,"op_dokkaebi.png"
            ,"op_echo.png"
            ,"op_ela.png"
            ,"op_finka.png"
            ,"op_flores.png"
            ,"op_frost.png"
            ,"op_fuze.png"
            ,"op_glaz.png"
            ,"op_goyo.png"
            ,"op_gridlock.png"
            ,"op_hibana.png"
            ,"op_iana.png"
            ,"op_iq.png"
            ,"op_jackal.png"
            ,"op_jager.png"
            ,"op_kaid.png"
            ,"op_kali.png"
            ,"op_kapkan.png"
            ,"op_lesion.png"
            ,"op_lion.png"
            ,"op_maestro.png"
            ,"op_maverick.png"
            ,"op_melusi.png"
            ,"op_mira.png"
            ,"op_montagne.png"
            ,"op_mozzie.png"
            ,"op_mute.png"
            ,"op_nokk.png"
            ,"op_nomad.png"
            ,"op_oryx.png"
            ,"op_osa.png"
            ,"op_pulse.png"
            ,"op_recruit_blue.png"
            ,"op_recruit_green.png"
            ,"op_recruit_orange.png"
            ,"op_recruit_red.png"
            ,"op_recruit_yellow.png"
            ,"op_rook.png"
            ,"op_sledge.png"
            ,"op_smoke.png"
            ,"op_tachanka.png"
            ,"op_thatcher.png"
            ,"op_thermite.png"
            ,"op_thorn.png"
            ,"op_thunderbird.png"
            ,"op_twitch.png"
            ,"op_valkyrie.png"
            ,"op_vigil.png"
            ,"op_wamai.png"
            ,"op_warden.png"
            ,"op_ying.png"
            ,"op_zero.png"
            ,"op_zofia.png"
        };

        //public enum Icons
        //{
        //      op_ace
        //    , op_alibi
        //    , op_amaru
        //    , op_aruni
        //    , op_ash
        //    , op_azami
        //    , op_bandit
        //    , op_blackbeard
        //    , op_blitz
        //    , op_buck
        //    , op_capitao
        //    , op_castle
        //    , op_caveira
        //    , op_clash
        //    , op_doc
        //    , op_dokkaebi
        //    , op_echo
        //    , op_ela
        //    , op_finka
        //    , op_flores
        //    , op_frost
        //    , op_fuze
        //    , op_glaz
        //    , op_goyo
        //    , op_gridlock
        //    , op_hibana
        //    , op_iana
        //    , op_iq
        //    , op_jackal
        //    , op_jager
        //    , op_kaid
        //    , op_kali
        //    , op_kapkan
        //    , op_lesion
        //    , op_lion
        //    , op_maestro
        //    , op_maverick
        //    , op_melusi
        //    , op_mira
        //    , op_montagne
        //    , op_mozzie
        //    , op_mute
        //    , op_nokk
        //    , op_nomad
        //    , op_oryx
        //    , op_osa
        //    , op_pulse
        //    , op_recruit_blue
        //    , op_recruit_green
        //    , op_recruit_orange
        //    , op_recruit_red
        //    , op_recruit_yellow
        //    , op_rook
        //    , op_sledge
        //    , op_smoke
        //    , op_tachanka
        //    , op_thatcher
        //    , op_thermite
        //    , op_thorn
        //    , op_thunderbird
        //    , op_twitch
        //    , op_valkyrie
        //    , op_vigil
        //    , op_wamai
        //    , op_warden
        //    , op_ying
        //    , op_zero
        //    , op_zofia
        //}

        private static int lastcustomuserid;

        public static int LastCustomUserId
        {
            get
            {
                lastcustomuserid++;
                return lastcustomuserid;
            }
            private set { lastcustomuserid = value; }
        }

        public static List<Tuple<int, string>> customUserIdsAndNames { get; set; } = new List<Tuple<int, string>>();

        public static string UserIdToName(string id)
        {
            var teammate = DataCache.CurrentTeamMates.Where(x => x.Id == id).FirstOrDefault();
            if (teammate == null) return null;
            return teammate.Name;
        }

        public async static void Init()
        {
            if (wnd.IsLoggedIn)
            {
                wnd.SetLoadingStatus("Retrieving team mates");
                RetrieveOffDayTypes();
                wnd.SetLoadingStatus("Retrieving team mates");
                RetrieveCalendarFilterTypes();
                RetrieveScrimModes();
                wnd.SetLoadingStatus("Retrieving scrimmodes");
                RetrieveEventTypes();
                wnd.SetLoadingStatus("");
                CallOnDataRetrieved();
            }
        }
    private static void RetrieveOffDayTypes()
    {
        OffDayTypes.Clear();
        OffDayTypes.Add(new OffDayType(0, "exactly"));
        OffDayTypes.Add(new OffDayType(1, "entire day"));
        OffDayTypes.Add(new OffDayType(2, "weekly"));
        OffDayTypes.Add(new OffDayType(3, "every second week"));
        OffDayTypes.Add(new OffDayType(4, "monthly"));
        OffDayTypes.Add(new OffDayType(5, "daily"));
    }
    private static void RetrieveCalendarFilterTypes()
    {
        CalendarFilterTypes.Clear();
        CalendarFilterTypes.Add(new CalendarFilterType(0, "min players"));
        CalendarFilterTypes.Add(new CalendarFilterType(1, "specific players"));
        CalendarFilterTypes.Add(new CalendarFilterType(2, "min specific players"));
        CalendarFilterTypes.Add(new CalendarFilterType(3, "everyone"));
    }
    private static void RetrieveScrimModes()
    {
        ScrimModes.Clear();
        ScrimModes.Add(new ScrimMode(0, "Normal"));
        ScrimModes.Add(new ScrimMode(1, "6+6"));
        ScrimModes.Add(new ScrimMode(2, "2-2-2"));
        ScrimModes.Add(new ScrimMode(3, "4+4"));
    }
    private static void RetrieveEventTypes()
    {
        EventTypes.Clear();
        EventTypes.Add(new EventType(0, "Scrim"));
        EventTypes.Add(new EventType(1, "Strats"));
        EventTypes.Add(new EventType(2, "Training"));
        EventTypes.Add(new EventType(3, "Dryrun"));
        EventTypes.Add(new EventType(4, "Warmup"));
        EventTypes.Add(new EventType(5, "Match"));
        EventTypes.Add(new EventType(6, "Cup"));
    }
    
    public static string GetUserIdFromName(string name)
    {
        var rows = DataCache.CurrentTeamMates.Where(x => x.Name.ToUpper().StartsWith(name.ToUpper()));
        if (rows.Any()) return rows.First().Id;
        return null;
    }
    public static SolidColorBrush ToSolidColorBrush(this string hex_code)
    {
        return (SolidColorBrush)new BrushConverter().ConvertFromString(hex_code);
    }

    #region SVG

    public static string GetPathFromId(int game_id, int map_id, int floor_id)
    {
        string fileName = game_id + "_" + map_id + "_" + floor_id + ".svg";
        string file = Environment.CurrentDirectory + @"/Images/Maps/" + fileName;
        if (!File.Exists(file)) return null;
        return file;
    }

    public static XmlDocument GetSCVDocumentForMapAndFloor(string map_id, int floor_id = 0)
    {
        var map = DataCache.CurrentMaps.Where(x => x.Id == map_id).FirstOrDefault();
        if (map == null) return null;
        if (floor_id < 0 || floor_id > 4) return null;

        string svg = null;
        if (floor_id == 0 && !string.IsNullOrEmpty(map.Floor0SVG))
        {
            svg = map.Floor0SVG;
        }
        if (floor_id == 1 && !string.IsNullOrEmpty(map.Floor1SVG))
        {
            svg = map.Floor1SVG;
        }
        if (floor_id == 2 && !string.IsNullOrEmpty(map.Floor2SVG))
        {
            svg = map.Floor2SVG;
        }
        if (floor_id == 3 && !string.IsNullOrEmpty(map.Floor3SVG))
        {
            svg = map.Floor3SVG;
        }

        if (string.IsNullOrEmpty(svg)) return null;

        var xml = new XmlDocument();
        xml.LoadXml(svg);
        return xml;
    }

    public static XmlDocument GetSVGDocumentFromPath(string path)
    {
        if (!File.Exists(path)) return null;

        //load svg
        XmlDocument svgDocument = new XmlDocument();
        svgDocument.Load(path);
        return svgDocument;
    }

    public static SvgContent GetSvgContent(XmlDocument svgDocument, string game_id, string map_id, int floor_id)
    {
        #region Generate svgContent
        SvgContent svgContent = new SvgContent();

        svgContent.Rects = new List<SvgRect>();

        svgContent.Name = svgDocument.Name;

        svgContent.XmlDocument = svgDocument;
        #endregion
        #region remove walls

        var XDocumentNoWalls = XDocument.Parse(svgDocument.OuterXml);

        var rectsToRemove = XDocumentNoWalls.Descendants().Where(e => e.Name.LocalName == "rect" && e.Attribute("style").Value.Contains("linear-gradient")).ToList();

        foreach (var rect in rectsToRemove)
        {
            rect.Remove();
        }

        string svgStringNoWalls = XDocumentNoWalls.ToString();

        XmlDocument XmlDocumentNoWalls = new XmlDocument();
        XmlDocumentNoWalls.LoadXml(svgStringNoWalls);

        svgContent.XmlDocumentNoWalls = XmlDocumentNoWalls;

        #endregion
        #region Get Viewbox

        XmlNode svg = svgDocument.ChildNodes[1];
        string viewBox = svg.Attributes["viewBox"].Value;

        string[] values = viewBox.Split(' ');

        #endregion
        #region Viewbox analysis

        double vx = Globals.GetDouble(values[0], 0.0);
        double vy = Globals.GetDouble(values[1], 0.0);
        double vwidth = Globals.GetDouble(values[2], 0.0);
        double vheight = Globals.GetDouble(values[3], 0.0);

        svgContent.ViewBoxDimensions = new Point(vwidth, vheight);
        svgContent.ViewBoxPosition = new Point(vx, vy);

        #endregion
        #region Generate Rectangles

        XmlNodeList rectangles = svgDocument.GetElementsByTagName("rect");

        foreach (XmlNode rectangle in rectangles)
        {
            var attributes = rectangle.Attributes;

            double x = Globals.GetDouble(attributes["x"]?.Value ?? "0", 0);
            double y = Globals.GetDouble(attributes["y"]?.Value ?? "0", 0);
            double width = Globals.GetDouble(attributes["width"].Value, 0);
            double height = Globals.GetDouble(attributes["height"].Value, 0);
            string style = attributes["style"].Value;
            string transform = attributes["transform"]?.Value;

            string stringToHash = x + "_" + y + "_" + width + "_" + height;
            string hash = string.Format("{0:X}", stringToHash.GetHashCode());

            string uid = "_" + game_id.Replace("-", "") + "_" + map_id.Replace("-", "") + "_" + floor_id + "_" + hash;

            double translateX = 0;
            double translateY = 0;
            double rotation = 0;
            bool hasTransform = false;

            if (!string.IsNullOrEmpty(transform))
            {
                hasTransform = true;

                // Parse the transform attribute
                string[] transformValues = transform.Split(' ');
                translateX = Globals.GetDouble(transformValues[0].Substring(10), 0);
                translateY = Globals.GetDouble(transformValues[1].Substring(0, transformValues[1].Length - 1), 0);
                rotation = Globals.GetDouble(transformValues[2].Substring(7).Replace(")", ""), 0);
            }


            SvgRect svgRect = new SvgRect { height = height, width = width, x = x, y = y, style = style, transform = transform, uid = uid, translateX = translateX, translateY = translateY, rotation = rotation, hasTransform = hasTransform };

            svgContent.Rects.Add(svgRect);
        }
        #endregion
        return svgContent;
    }

    public static SVGImage.SVG.SVGImage GetImageForSVG(SvgContent svg)
    {
        if (svg == null) return null;


        //generate svg viewer
        var image = new SVGImage.SVG.SVGImage();

        var stream = new MemoryStream();
        svg.XmlDocumentNoWalls.Save(stream);
        stream.Position = 0;

        image.SetImage(stream);

        image.Height = svg.ViewBoxDimensions.Y;
        image.Width = svg.ViewBoxDimensions.X;

        return image;
    }

    public static SVGImage.SVG.SVGImage GetImageForFloorAndMap(int game_id, int map_id, int floor_id)
    {
        //get file name
        string fileName = game_id + "_" + map_id + "_" + floor_id + ".svg";
        string file = Environment.CurrentDirectory + @"/Images/Maps/" + fileName;
        if (!File.Exists(file)) return null;

        //generate svg viewer
        var image = new SVGImage.SVG.SVGImage();
        image.SetImage(File.OpenRead(file));

        //analyse svg to get dimensions:
        XmlDocument svgDocument = new XmlDocument();
        svgDocument.Load(file);

        XmlNode svg = svgDocument.ChildNodes[1];
        string viewBox = svg.Attributes["viewBox"].Value;

        string[] values = viewBox.Split(' ');

        double vx = Globals.GetDouble(values[0], 0.0);
        double vy = Globals.GetDouble(values[1], 0.0);
        double vwidth = Globals.GetDouble(values[2], 0.0);
        double vheight = Globals.GetDouble(values[3], 0.0);

        //set image size
        image.Height = vheight;
        image.Width = vwidth;

        return image;
    }

    #endregion

    public class CalendarEventCreatedArgs : EventArgs
    {
        public DateTime Date { get; set; }
    }

    public static string SerializeToString<T>(this T toSerialize)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

        using (StringWriter textWriter = new StringWriter())
        {
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }
    }

    // Serialize an object to XML and save it to a file
    public static void SerializeToFile<T>(this T obj, string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (TextWriter writer = new StreamWriter(path))
        {
            serializer.Serialize(writer, obj);
        }
    }

    // Deserialize an object from an XML file
    public static T DeserializeFromFile<T>(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        using (TextReader reader = new StreamReader(path))
        {
            return (T)serializer.Deserialize(reader);
        }
    }

    public static void CopyTo(Stream src, Stream dest)
    {
        byte[] bytes = new byte[4096];

        int cnt;

        while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
        {
            dest.Write(bytes, 0, cnt);
        }
    }


    /// <summary>
    /// Compresses the string.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    public static string CompressString(string text)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        var memoryStream = new MemoryStream();
        using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
        {
            gZipStream.Write(buffer, 0, buffer.Length);
        }

        memoryStream.Position = 0;

        var compressedData = new byte[memoryStream.Length];
        memoryStream.Read(compressedData, 0, compressedData.Length);

        var gZipBuffer = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
        return Convert.ToBase64String(gZipBuffer);
    }

    /// <summary>
    /// Decompresses the string.
    /// </summary>
    /// <param name="compressedText">The compressed text.</param>
    /// <returns></returns>
    public static string DecompressString(string compressedText)
    {
        try
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
        catch
        {
            return compressedText;
        }
    }

    public static string RemoveIllegalCharactersFromName(string input)
    {
        Regex rgx = new Regex("[^a-zA-Z0-9 -]");
        return rgx.Replace(input, "");
    }

    public static double GetDouble(string value, double defaultValue)
    {
        double result;

        value = value.Replace(",", ".");

        //Try parsing in the current culture
        if (!double.TryParse(value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
        {
            result = defaultValue;
        }

        return result;
    }

    public static bool isWithin5Percent(double x, double y)
    {
        double difference = Math.Abs(x - y);
        double threshold = y * 0.05;
        return difference < threshold;
    }

    static public void CopyFolder(string sourceFolder, string destFolder)
    {
        if (!Directory.Exists(destFolder))
            Directory.CreateDirectory(destFolder);
        string[] files = Directory.GetFiles(sourceFolder);
        foreach (string file in files)
        {
            string name = Path.GetFileName(file);
            string dest = Path.Combine(destFolder, name);
            File.Copy(file, dest);
        }
        string[] folders = Directory.GetDirectories(sourceFolder);
        foreach (string folder in folders)
        {
            string name = Path.GetFileName(folder);
            string dest = Path.Combine(destFolder, name);
            CopyFolder(folder, dest);
        }
    }
}

public class SvgRect
{
    public string uid { get; set; }
    public double x { get; set; }
    public double y { get; set; }
    public double width { get; set; }
    public double height { get; set; }
    public string style { get; set; }
    public string transform { get; set; }

    public double translateX { get; set; }
    public double translateY { get; set; }
    public double rotation { get; set; }
    public bool hasTransform { get; set; }
}

public class SvgContent
{
    public string Name { get; set; }
    public List<SvgRect> Rects { get; set; }
    public Point ViewBoxPosition { get; set; }
    public Point ViewBoxDimensions { get; set; }
    public XmlDocument XmlDocument { get; set; }
    public XmlDocument XmlDocumentNoWalls { get; set; }
}
}
