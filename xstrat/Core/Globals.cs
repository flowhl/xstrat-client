using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using xstrat.Json;

namespace xstrat.Core
{
    public class Player
    {
        public ICollection<Response> Responses { get; set; }
        public int ID { get; set; }
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
        public delegate void StatsUpdateHandler(object sender, EventArgs e);
        public static event StatsUpdateHandler OnUpdateStats;

        public static List<StatsResponse> PlayerStats = new List<StatsResponse>();
        public static List<List<StatsBySeasonDetail>> PlayerAllSeasonStats = new List<List<StatsBySeasonDetail>>();
        public static List<List<ScrimParticipationResult>> PlayerScrimParticipation = new List<List<ScrimParticipationResult>>();
        public static List<PlayerScrimParticipationPercentage> PlayerScrimParticipationPercentages = new List<PlayerScrimParticipationPercentage>();

        public static void Init()
        {
            RetrieveData();
        }

        public static void RetrieveData()
        {
            StartRetrieveStatsData();
            StartRetrieveStatsAllSeasons();
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
                    int? user_id = player.FirstOrDefault().user_id;
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
                            user_id.GetValueOrDefault(0),
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
        public static async Task RetrieveStatsDataAsync(string ubisoft_id, int user_id = -1)
        {
            if(!string.IsNullOrEmpty(ubisoft_id) && user_id >= 0)
            {
                    try
                    {
                        (bool, string) result = await ApiHandler.GetStats(ubisoft_id);
                        if (result.Item1)
                        {
                            string response = result.Item2;
                            //convert to json instance
                            JObject json = JObject.Parse(response);
                            var data = json.SelectToken("data").ToString();
                            if (data != null && data != "")
                            {
                                StatsResponse sr = JsonConvert.DeserializeObject<StatsResponse>(data);
                                sr.StatsResponseDetails.Values.First().xstrat_user_id = user_id;
                                if (sr != null)
                                {
                                    PlayerStats.Add(sr);
                                }
                            }
                            else
                            {
                                Notify.sendError("Playerstats could not be loaded");
                                throw new Exception("Playerstats could not be loaded");
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Notify.sendError(ex.Message);
                    }

                //scrim stats
                if(user_id >= 0)
                {
                    try
                    {
                        (bool, string) result = await ApiHandler.GetScrimParticipation(user_id);
                        if (result.Item1)
                        {
                            string response = result.Item2;
                            //convert to json instance
                            JObject json = JObject.Parse(response);
                            var data = json.SelectToken("data").ToString();
                            if (data != null && data != "")
                            {
                                List<ScrimParticipationResult> sr = JsonConvert.DeserializeObject<List<ScrimParticipationResult>>(data);
                                if (sr != null)
                                {
                                    PlayerScrimParticipation.Add(sr);
                                }
                            }
                            else
                            {
                                Notify.sendError("Playerstats could not be loaded");
                                throw new Exception("Playerstats could not be loaded");
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Notify.sendError(ex.Message);
                    }
                }            
            
            }
            return;
        }
        public static async Task RetrieveStatsAllSeasons(string ubisoft_id, int user_id = -1)
        {
            if (!string.IsNullOrEmpty(ubisoft_id) && user_id >= 0)
            {
                try
                {
                    (bool, string) result = await ApiHandler.GetStatsByAllSeason(ubisoft_id);
                    if (result.Item1)
                    {
                        string response = result.Item2;
                        //convert to json instance
                        JObject json = JObject.Parse(response);
                        var data = "[ \n\r " + json.SelectToken("data").ToString().Replace("[", "").Replace("]", "") + "\n\r ]";
                        if (data != null && data != "")
                        {
                            var sr = JsonConvert.DeserializeObject<List<StatsBySeasonDetail>>(data);

                            sr.ForEach(x => x.xstrat_user_id = user_id);

                            if (sr.Count > 0)
                            {
                                PlayerAllSeasonStats.Add(sr);
                            }
                        }
                        else
                        {
                            Notify.sendError("Playerstats could not be loaded");
                            throw new Exception("Playerstats could not be loaded");
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Notify.sendError(ex.Message);
                }

            }
            return;
        }
        public static async Task StartRetrieveStatsData()
        {
            PlayerStats.Clear();
            PlayerScrimParticipation.Clear();
            foreach (var user in Globals.teammates)
            {
                //playerstats
                if (user.ubisoft_id != null && user.ubisoft_id != "")
                {
                    await RetrieveStatsDataAsync(user.ubisoft_id, user.id);
                }
            }
            DataRetrieved();
        }
        public static async Task StartRetrieveStatsAllSeasons()
        {
            PlayerAllSeasonStats.Clear();
            foreach (var user in Globals.teammates)
            {
                if (user.ubisoft_id != null && user.ubisoft_id != "")
                {
                    await RetrieveStatsAllSeasons(user.ubisoft_id, user.id);
                }
            }
        }
        #endregion
    }
    public static class Globals
    {
        public static string TeamName { get; set; }
        public static List<User> teammates { get; set; } = new List<User>();
        public static List<Game> games { get; set; } = new List<Game>();
        public static List<OffDayType> OffDayTypes = new List<OffDayType>();
        public static List<CalendarFilterType> CalendarFilterTypes = new List<CalendarFilterType>();
        public static List<Map> Maps = new List<Map>();
        public static List<ScrimMode> ScrimModes = new List<ScrimMode>();
        public static bool AdminUser = false;
        public static User currentUser { get; set; }

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
            "Vector Glare" // June 14, 2022
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

        public enum Icons
        {
            g_Alibi
            , g_AruniGate
            , g_Bandit
            , g_Barb
            , g_Barricade
            , g_BulletProofCam
            , g_BulletProofCamArrow
            , g_Castle
            , g_Claymore
            , g_Drone
            , g_Echo
            , g_Ela
            , g_Frost
            , g_Goyo
            , g_GoyoShieldTopDown
            , g_ImpactGrenade
            , g_Jager
            , g_Kaid
            , g_Kapkan
            , g_Lesion
            , g_Maestro
            , g_MelusiBanshee
            , g_Mira
            , g_Mozzie
            , g_Mute
            , g_NitroCell
            , g_Nomad
            , g_Osa
            , g_ProximityMine
            , g_Shield
            , g_ShieldTopDown
            , g_Smoke
            , g_Thunderbird
            , g_ToxicBabe
            , g_ToxicBabeSmoke
            , g_TraxStingers
            , g_Valkyrie
            , g_Wamai
            , g_Zero
            , op_ace
            , op_alibi
            , op_amaru
            , op_aruni
            , op_ash
            , op_azami
            , op_bandit
            , op_blackbeard
            , op_blitz
            , op_buck
            , op_capitao
            , op_castle
            , op_caveira
            , op_clash
            , op_doc
            , op_dokkaebi
            , op_echo
            , op_ela
            , op_finka
            , op_flores
            , op_frost
            , op_fuze
            , op_glaz
            , op_goyo
            , op_gridlock
            , op_hibana
            , op_iana
            , op_iq
            , op_jackal
            , op_jager
            , op_kaid
            , op_kali
            , op_kapkan
            , op_lesion
            , op_lion
            , op_maestro
            , op_maverick
            , op_melusi
            , op_mira
            , op_montagne
            , op_mozzie
            , op_mute
            , op_nokk
            , op_nomad
            , op_oryx
            , op_osa
            , op_pulse
            , op_recruit_blue
            , op_recruit_green
            , op_recruit_orange
            , op_recruit_red
            , op_recruit_yellow
            , op_rook
            , op_sledge
            , op_smoke
            , op_tachanka
            , op_thatcher
            , op_thermite
            , op_thorn
            , op_thunderbird
            , op_twitch
            , op_valkyrie
            , op_vigil
            , op_wamai
            , op_warden
            , op_ying
            , op_zero
            , op_zofia
        }

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

        public static string UserIdToName(int id)
        {
            return teammates.Where(x => x.id == id).First().name;
        }

        public static void Init()
        {
            RetrieveTeamMates();
            RetrieveGames();
            RetrieveOffDayTypes();
            RetrieveCalendarFilterTypes();
            RetrieveMaps();
            RetrieveScrimModes();
            RetrieveTeamName();
            RetrieveAdminStatusAsync();
        }


        private static async Task RetrieveAdminStatusAsync()
        {
            var result = await ApiHandler.GetAdminStatus();
            AdminUser = result.Item1;
        }

        private static async void RetrieveTeamName()
        {

        }
        private static async void RetrieveCurrentUser()
        {
            currentUser = getUserFromId(SettingsHandler.current_user_id);
        }

        private static async void RetrieveTeamMates()
        {
            var result = await ApiHandler.TeamMembers();
            if (result.Item1)
            {
                string resultJson = result.Item2;
                string response = result.Item2;
                //convert to json instance
                JObject json = JObject.Parse(response);
                var data = json.SelectToken("data").ToString();
                if (data != null && data != "")
                {
                    List<xstrat.Json.User> rList = JsonConvert.DeserializeObject<List<Json.User>>(data);
                    teammates.Clear();
                    teammates = rList;
                }
                else
                {
                    Notify.sendError("Teammates could not be loaded");
                    throw new Exception("Teammates could not be loaded");
                }
                StatsDataSource.Init();
                RetrieveCurrentUser();
                int max_id = 0;
                foreach (var mate in teammates)
                {
                    if(mate.id > max_id) max_id = mate.id;
                }
                lastcustomuserid = max_id;
            }
            else
            {
                Notify.sendError("Teammates could not be loaded");
            }
        }

        private static async void RetrieveGames()
        {
            var result = await ApiHandler.Games();
            if (result.Item1)
            {
                string resultJson = result.Item2;
                string response = result.Item2;
                //convert to json instance
                JObject json = JObject.Parse(response);
                var data = json.SelectToken("data").ToString();
                if (data != null && data != "")
                {
                    List<xstrat.Json.Game> rList = JsonConvert.DeserializeObject<List<Json.Game>>(data);
                    games.Clear();
                    games = rList;
                }
                else
                {
                    Notify.sendError("Games could not be loaded");
                }
            }
            else
            {
                Notify.sendError("Games could not be loaded");
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
        }
        private static void RetrieveCalendarFilterTypes()
        {
            CalendarFilterTypes.Clear();
            CalendarFilterTypes.Add(new CalendarFilterType(0, "min players"));
            CalendarFilterTypes.Add(new CalendarFilterType(1, "specific players"));
            CalendarFilterTypes.Add(new CalendarFilterType(2, "min specific players"));
            CalendarFilterTypes.Add(new CalendarFilterType(3, "everyone"));
        }
        private static async void RetrieveMaps()
        {
            var result = await ApiHandler.GetMaps();
            if (result.Item1)
            {
                string resultJson = result.Item2;
                string response = result.Item2;
                //convert to json instance
                JObject json = JObject.Parse(response);
                var data = json.SelectToken("data").ToString();
                if (data != null && data != "")
                {
                    List<xstrat.Json.Map> rList = JsonConvert.DeserializeObject<List<Json.Map>>(data);
                    Maps.Clear();
                    Maps = rList;
                }
                else
                {
                    Notify.sendError("Maps could not be loaded");
                }
            }
            else
            {
                Notify.sendError("Maps could not be loaded");
            }
        }
        private static void RetrieveScrimModes()
        {
            ScrimModes.Clear();
            ScrimModes.Add(new ScrimMode(0, "normal"));
            ScrimModes.Add(new ScrimMode(1, "6+6"));
            ScrimModes.Add(new ScrimMode(2, "2-2-2"));
            ScrimModes.Add(new ScrimMode(3, "4+4"));
        }

        public static User getUserFromId(int id)
        {
            var rows = teammates.Where(x => x.id == id);
            if (rows.Any()) return rows.First();
            return null;
        }
        public static int getUserIdFromName(string name)
        {
            var rows = teammates.Where(x => x.name.ToUpper().StartsWith(name.ToUpper()));
            if(rows.Any()) return rows.First().id;
            return -1;
        }
        public static SolidColorBrush ToSolidColorBrush(this string hex_code)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(hex_code);
        }
    }
}
