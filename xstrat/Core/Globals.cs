using DragonFruit.Six.Api.Accounts;
using DragonFruit.Six.Api.Accounts.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using xstrat.Json;

namespace xstrat.Core
{
    public class Player
    {
        public ICollection<Response> Responses { get; set; }
        public int ID { get; set; }
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
            return teammates.Where(x => x.id == id).First();
        }
        public static int getUserIdFromName(string name)
        {
            return teammates.Where(x => x.name.ToUpper().StartsWith(name.ToUpper())).First().id;
        }
    }
}
