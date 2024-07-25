using System;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xstrat.Core;
using System.Net.Http;
using xstrat.Json;
using xstrat.Ui;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using xstrat.Models.API;
using Newtonsoft.Json;
using NuGet;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using xstrat.Models.Supabase;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace xstrat
{
    public static class ApiHandler
    {
        #region Properties and Session functions

        /// <summary>
        /// creates restclient instance
        /// </summary>
        public async static void Initialize()
        {
            //if (Client == null)
            //{

            //    if (SettingsHandler.Settings.APIURL != null)
            //    {
            //        var options = new RestClientOptions
            //        {
            //            MaxTimeout = 10000,
            //            BaseUrl = new Uri(SettingsHandler.Settings.APIURL),
            //        };
            //        Client = new RestClient(options);
            //    }
            //    else
            //    {
            //        Notify.sendError("Please enter a proper url to reach the server. Use https://app.xstrat.app/ as default");
            //    }
            //}
            //var request =RestHandler.GetRequest("/", Method.Get);
            //

            //var response = Client.Execute<RestResponse>(request);

            //if (response.StatusCode != System.Net.HttpStatusCode.OK) //success
            //{
            //    Notify.sendError("Api could not be reached. Please check your connection and restart");
            //    //await Task.Delay(5000);
            //    ////MessageBox.Show("Api could not be reached. Please check your connection");
            //    //App.Current.Shutdown();
            //}
            ////RenewSessionQueue();
        }


        #endregion
        #region User
        /// <summary>
        /// User Signup
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_email"></param>
        /// <param name="_pw"></param>
        /// <returns>
        /// (bool success, string error)
        /// </returns>
        public static async Task<(bool, string)> RegisterAsync(string _name, string _email, string _pw)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("user/signup", Method.Post);
                
                request.AddJsonBody(new { email = _email, password = _pw, username = _name });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    return (true, "");
                }
                EndWaiting();
                string msg = string.Empty;
                if(response.Content.IsNotNullOrEmpty())                
                msg = ExtractMessage(response.Content);
                else
                    msg = response.ToString();
                Notify.sendError(msg, true);
                return (false, "db error");
            }
        }

        /// <summary>
        /// User Sign In
        /// </summary>
        /// <param name="_email"></param>
        /// <param name="_pw"></param>
        /// <returns></returns>
        public static async Task<Session> SignInAsync(string _email, string _pw)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("User/SignIn", Method.Post);
                
                //request.AddJsonBody(new { email = _email, password = _pw });
                request.AddJsonBody(new { email = _email, password = _pw });

                var response = client.Execute<RestResponse>(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var session = JsonConvert.DeserializeObject<Session>(response.Content);
                    RestHandler.CurrentSession = session;
                    DataCache.RetrieveUser();
                    EndWaiting();
                    return session;
                }
                EndWaiting();
                Notify.sendError("Error logging in: " + response.Content, true);
                return null;
            }
        }

        public static async Task<bool> RequestPasswordResetEmailAsync()
        {
            //using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            //{
            //    Waiting();
            //    var request = RestHandler.GetRequest("User/RequestEmail", Method.Post);
            //    var response = client.Execute<RestResponse>(request);

            //    if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            //    {
            //        var userData = JsonConvert.DeserializeObject<Models.Supabase.UserData>(response.Content);
            //        EndWaiting();
            //        return userData;
            //    }
            //    EndWaiting();
            //    Notify.sendError("Error getting user data in: " + response.Content);
            //    return false;
            //}
            return false;
        }

        public static async Task<Models.Supabase.UserData> GetUserDataAsync()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request = RestHandler.GetRequest("User/Data", Method.Get);
                var response = client.Execute<RestResponse>(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var userData = JsonConvert.DeserializeObject<Models.Supabase.UserData>(response.Content);
                    EndWaiting();
                    return userData;
                }
                EndWaiting();
                Notify.sendError("Error getting user data in: " + response.Content);
                return null;
            }

        }

        public static async Task<Session> RenewSessionAsync(string access, string refresh)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                var request = RestHandler.GetRequest("user/refresh", Method.Post);
                request.Authenticator = new JwtAuthenticator(access);

                request.AddJsonBody(new { refresh_token = refresh });

                var response = client.Execute<RestResponse>(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var session = JsonConvert.DeserializeObject<Session>(response.Content);
                    return session;
                }

                Logger.Log("Error renewing Session: " + response.Content);
                return null;
            }
        }


        /// <summary>
        /// verifies the api token by api call
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<bool> VerifyTokenAsync()
        {
            Waiting();
            var userData = await GetUserDataAsync();
            EndWaiting();
            return userData != null;
        }

        /// <summary>
        /// sends email to reset password - not implemented yet!
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static async Task<bool> ResetEmail()
        {
            Waiting();
            throw new NotImplementedException();
            EndWaiting();
        }

        public static async Task<bool> SetColor(string icolor)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("user/color", Method.Post);

                string scolor = icolor.Replace("#FF", "#");

                string jsonBody = JsonConvert.SerializeObject(new { color = scolor });
                jsonBody = jsonBody.Replace(@"\", "");
                request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    DataCache.RetrieveUser();
                    DataCache.RetrieveTeamMates();
                    return true;
                }
                EndWaiting();
                return false;
            }
        }

        public static async Task<bool> SetDiscordId(string discord)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("user/discord", Method.Post);
                
                request.AddJsonBody(new { discord = discord });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    DataCache.RetrieveUser();
                    return true;
                }
                EndWaiting();
                return false;
            }
        }

        public static bool SetUbisoftID(string ubisoft_id)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request = RestHandler.GetRequest("user/ubisoft", Method.Post);

                request.AddJsonBody(new { ubisoft_id = ubisoft_id });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    return true;
                }
                EndWaiting();
                return false;
            }
        }

        #endregion

        #region Static data

        #region Maps
        public static async Task<List<Models.Supabase.Map>> GetMapsAsync()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("maps", Method.Get);
                
                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var maps = JsonConvert.DeserializeObject<List<Models.Supabase.Map>>(response.Content);

                    EndWaiting();
                    return maps;
                }
                EndWaiting();
                return null;
            }
        }
        #endregion

        #region Operators
        public static async Task<List<Models.Supabase.Operator>> GetOperatorsAsync()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("operator", Method.Get);
                
                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var operators = JsonConvert.DeserializeObject<List<Models.Supabase.Operator>>(response.Content);

                    EndWaiting();
                    return operators;
                }
                EndWaiting();
                return null;
            }
        }
        #endregion

        #region Positions
        public static async Task<List<Models.Supabase.Position>> GetPositionsAsync()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("position", Method.Get);
                
                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var positions = JsonConvert.DeserializeObject<List<Models.Supabase.Position>>(response.Content);

                    EndWaiting();
                    return positions;
                }
                EndWaiting();
                return null;
            }
        }
        #endregion

        #region Games

        public static async Task<List<Models.Supabase.Game>> GetGamesAsync()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("game", Method.Get);
                
                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var game = JsonConvert.DeserializeObject<List<Models.Supabase.Game>>(response.Content);

                    EndWaiting();
                    return game;
                }
                EndWaiting();
                return null;
            }
        }

        #endregion

        #endregion

        #region Team

        public static async Task<bool> GetAdminStatus()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("user/teamadminstatus", Method.Get);
                
                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var isAdmin = JsonConvert.DeserializeObject<bool>(response.Content);

                    EndWaiting();
                    return isAdmin;
                }
                EndWaiting();
                return false;
            }
        }

        public static async Task<bool> JoinTeam(string id, string pw)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("team/join", Method.Post);
                
                request.AddJsonBody(new { password = pw, team_id = id });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    DataCache.RetrieveTeam();
                    DataCache.RetrieveTeamMates();
                    EndWaiting();
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Notify.sendError("Invalid id or password");
                    EndWaiting();
                    return false;
                }
                EndWaiting();
                Notify.sendError("DB Error");
                return false;
            }
        }

        public static async Task LeaveTeam()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("team/leave", Method.Post);
                

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    DataCache.RetrieveTeam();
                    DataCache.RetrieveTeamMates();
                    Notify.sendSuccess("Team left sucessfully");
                }
                else
                {
                    Notify.sendError("You cannot leave the team as you are the team admin");
                }
                EndWaiting();
            }
        }

        public static async Task<bool> NewTeam(string name, string igame_id)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                var request =RestHandler.GetRequest("team/new", Method.Post);
                
                request.AddJsonBody(new { name = name, game_id = igame_id });

                var response = client.Execute<RestResponse>(request);   
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    DataCache.RetrieveTeam();
                    DataCache.RetrieveTeamMates();
                    EndWaiting();
                    return true;
                }
                EndWaiting();
                return false;
            }
        }

        public static async Task<bool> RenameTeamAsync(string _newname)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                var request =RestHandler.GetRequest("team/rename", Method.Post);
                
                request.AddJsonBody(new { name = _newname });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    DataCache.RetrieveTeam();
                    DataCache.RetrieveTeamMates();
                    EndWaiting();
                    return true;
                }
                EndWaiting();
                return false;
            }
        }

        public static async Task<List<Models.Supabase.UserData>> GetTeamMembersAsync()
        {
            Waiting();
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {

                var request =RestHandler.GetRequest("team/members", Method.Get);
                request.Timeout = 1000;
                
                var response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var members = JsonConvert.DeserializeObject<List<UserData>>(response.Content);

                    EndWaiting();
                    return members;
                }
                EndWaiting();
                return null;
            }

        }

        public static async Task<Models.Supabase.Team> GetTeamInfoAsync()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("team", Method.Get);
                
                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var team = JsonConvert.DeserializeObject<Models.Supabase.Team>(response.Content);

                    EndWaiting();
                    return team;
                }
                EndWaiting();
                return null;
            }

        }

        public static async Task<bool> DeleteTeam()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("team/delete", Method.Post);
                

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    DataCache.RetrieveTeam();
                    DataCache.RetrieveTeamMates();
                    EndWaiting();
                    return true;
                }
                EndWaiting();
                return false;
            }
        }

        public static async Task<bool> SetDiscordAdminSettings(string webhook, int sn_created, int sn_changed, int sn_weekly, int sn_soon, int sn_delay, int use_on_days)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("team/adminsettings", Method.Post);
                
                request.AddJsonBody(new { webhook = webhook, sn_created = sn_created, sn_changed = sn_changed, sn_weekly = sn_weekly, sn_soon = sn_soon, sn_delay = sn_delay, use_on_days = use_on_days });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    DataCache.RetrieveTeam();
                    DataCache.RetrieveTeamMates();
                    EndWaiting();
                    return true;
                }
                EndWaiting();
                return false;
            }
        }

        #endregion
        #region Routines

        /// <summary>
        /// adds new routine to db by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> NewRoutine()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("routine/new", Method.Post);
                

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    DataCache.RetrieveRoutines();
                    EndWaiting();
                    return true;
                }
                EndWaiting();
                return false;
            }

        }

        /// <summary>
        /// deletes routine by api call
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteRoutine(string id)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("routine/delete", Method.Post);
                
                request.AddJsonBody(new { routine_id = id });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    DataCache.RetrieveRoutines();
                    EndWaiting();
                    return true;
                }
                EndWaiting();
                return false;
            }
        }

        /// <summary>
        /// Loads all routines by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Models.Supabase.Routine>> GetRoutinesAsync()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("routine", Method.Get);
                
                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var routines = JsonConvert.DeserializeObject<List<Models.Supabase.Routine>>(response.Content);

                    EndWaiting();
                    return routines;
                }
                EndWaiting();
                return null;
            }
        }

        /// <summary>
        /// Saves routine by api call
        /// </summary>
        /// <param name="ntitle"></param>
        /// <param name="ncontent"></param>
        /// <param name="n_id"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateRoutineAsync(string ntitle, string ncontent, string n_id)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("routine/update", Method.Post);
                
                request.AddJsonBody(new { title = ntitle, content = ncontent, routine_id = n_id });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    DataCache.RetrieveRoutines();
                    EndWaiting();
                    return true;
                }
                EndWaiting();
                return false;
            }
        }
        #endregion

        #region OffDays
        /// <summary>
        /// adds new routine to db by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> NewOffDay(int typ, string title, string start, string end)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("calendarblock/new", Method.Post);
                
                request.AddJsonBody(new { typ = typ, title = title, start = start, end = end });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    DataCache.RetrieveCalendarBlocks();
                    return true;
                }
                EndWaiting();
                return false;
            }
        }

        /// <summary>
        /// deletes routine by api call
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> DeleteOffDay(string id)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("calendarblock/delete", Method.Delete);
                
                request.AddJsonBody(new { block_id = id });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    DataCache.RetrieveCalendarBlocks();
                    return (true, response.Content);
                }
                EndWaiting();
                return (false, "db error");
            }
        }


        /// <summary>
        /// Loads all routines by api call
        /// </summary>
        /// <returns></returns>
        public static List<CalendarBlock>GetUserOffDays()
        {
            return DataCache.CurrentCalendarBlocks.Where(x => x.UserId == DataCache.CurrentUser.Id).ToList();
        }

        /// <summary>
        /// Loads all routines by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<List<CalendarBlock>> GetTeamOffDays()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                    var request =RestHandler.GetRequest("calendarblock", Method.Get);
                

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var blocks = JsonConvert.DeserializeObject<List<CalendarBlock>>(response.Content);
                    EndWaiting();
                    return blocks;
                }
                EndWaiting();
                return null;
            }
        }

        /// <summary>
        /// Saves routine by api call
        /// </summary>
        /// <param name="ntitle"></param>
        /// <param name="ncontent"></param>
        /// <param name="n_id"></param>
        /// <returns></returns>
        public static async Task<bool> SaveOffDay(string id, int typ, string title, DateTime start, DateTime end)
        {
            start= start.ToLocalTime();
            end = end.ToLocalTime();
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("calendarblock/update", Method.Post);
                
                if (typ == 1)
                {
                    start = start.SetTime(0, 0, 0).ToUniversalTime();
                    end = end.SetTime(23, 59, 59).ToUniversalTime();
                }

                request.AddJsonBody(new { id = id, typ = typ, title = title, start = start, end = end });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    DataCache.RetrieveCalendarBlocks();
                    return true;
                }
                else
                {
                    Notify.sendError(response.Content);
                }
                EndWaiting();
                return false;
            }
        }
        #endregion

        #region Scrims

        /// <summary>
        /// adds new scrim to db by api call
        /// time = timestringfrom + | + timestringto
        /// </summary>
        /// <returns></returns>
        public static async Task<CalendarEvent> NewScrim(int typ, string title, string opponent_name, DateTime start, DateTime end, int event_type)
        {
            start = start.ToLocalTime();
            end = end.ToLocalTime();
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("calendarevent/new", Method.Post);
                
                request.AddJsonBody(new { typ = typ, title = title, opponent_name = opponent_name, start = start, end = end, event_type = event_type });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var calendarEvent = JsonConvert.DeserializeObject<CalendarEvent>(response.Content);
                    DataCache.RetrieveCalendarEvents();
                    EndWaiting();
                    return calendarEvent;
                }
                else
                {
                    Notify.sendError(response.Content);
                }
                EndWaiting();
                return null;
            }
        }

        /// <summary>
        /// deletes routine by api call
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteScrim(string id)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("calendarevent/delete", Method.Delete);
                
                request.AddJsonBody(new { id = id });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    DataCache.RetrieveCalendarEvents();
                    EndWaiting();
                    return true;
                }
                else
                {
                    Notify.sendError(response.Content);
                }
                EndWaiting();
                return false;
            }
        }


        /// <summary>
        /// Loads all routines by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<List<CalendarEvent>> GetTeamScrims()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("calendarevent", Method.Get);
                

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var cal = JsonConvert.DeserializeObject<List<CalendarEvent>>(response.Content);
                    EndWaiting();
                    return cal;
                }
                EndWaiting();
                return null;
            }
        }

        /// <summary>
        /// Saves routine by api call
        /// </summary>
        /// <param name="ntitle"></param>
        /// <param name="ncontent"></param>
        /// <param name="n_id"></param>
        /// <returns></returns>
        public static async Task<bool> SaveScrim(string id, string title, string comment, DateTime start, DateTime end, string opponent_name, string map_1_id, string map_2_id, string map_3_id, int typ, int event_type)
        {
            start = start.ToLocalTime();
            end = end.ToLocalTime();
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("calendarevent/update", Method.Post);
                
                request.AddJsonBody(new { id = id, title = title, comment = comment, time_start = start, time_end = end, opponent_name = opponent_name, map_1_id = map_1_id, map_2_id = map_2_id, map_3_id = map_3_id, typ = typ, event_type = event_type });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    DataCache.RetrieveCalendarEvents();
                    EndWaiting();
                    return true;
                }
                else
                {
                    Notify.sendError(response.Content);
                }
                EndWaiting();
                return false;
            }
        }

        /// <summary>
        /// sets the scrim response:
        /// 0 default
        /// 1 accept
        /// 2 deny
        /// </summary>
        /// <param name="scrim_id"></param>
        /// <param name="typ"></param>
        /// <returns></returns>
        public static async Task<bool> SetScrimResponse(string scrim_id, int typ)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("calendarevent/response", Method.Post);
                
                request.AddJsonBody(new { event_id = scrim_id, response_type = typ });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    DataCache.RetrieveCalendarEventResponses();
                    return true;
                }
                else
                {
                    Notify.sendError(response.Content);
                }
                EndWaiting();
                return false;
            }
        }

        /// <summary>
        /// gets the scrim response:
        /// 0 default
        /// 1 accept
        /// 2 deny
        /// </summary>
        /// <returns></returns>
        public static async Task<List<CalendarEventResponse>> GetCalendarEventResponsesAsync()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("calendarevent/responses", Method.Get);
                

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    var responses = JsonConvert.DeserializeObject<List<CalendarEventResponse>>(response.Content);
                    EndWaiting();
                    return responses;
                }
                else
                {
                    Notify.sendError(response.Content);
                }
                EndWaiting();
                return null;
            }
        }

        #endregion

        #region tracker
        /*
        /// <summary>
        /// gets the stats response:
        /// </summary>
        /// <param name="username"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> GetStats(string username, string region = "emea")
        {

            var CacheResponse = GetCachedResponse(username);
            if (!string.IsNullOrEmpty(CacheResponse.Item2))
            {
                return CacheResponse;
            }
            else
            {
                Waiting();
                var request =RestHandler.GetRequest("tracker/stats", Method.Post);
                
                request.AddJsonBody(new { username = username, region = region });

                var response = Client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    AddToCache(username, (true, response.Content), 20);
                    return (true, response.Content);
                }
                EndWaiting();
                return (false, "db error");
            }
        }

        public static async Task<(bool, string)> GetScrimParticipation(string user_id)
        {

            var CacheResponse = GetCachedResponse("");
            if (!string.IsNullOrEmpty(CacheResponse.Item2))
            {
                return CacheResponse;
            }
            else
            {
                Waiting();
                var request =RestHandler.GetRequest("user/scrimresponsestats", Method.Post);
                
                request.AddJsonBody(new { user_id = user_id });

                var response = Client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    AddToCache(user_id.ToString(), (true, response.Content), 10);
                    return (true, response.Content);
                }
                EndWaiting();
                return (false, "db error");
            }
        }

        /// <summary>
        /// gets the stats by season response:
        /// </summary>
        /// <param name="username"></param>
        /// <param name="season"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> GetStatsByAllSeason(string username, string region = "emea")
        {

            var CacheResponse = GetCachedResponse(username);
            if (!string.IsNullOrEmpty(CacheResponse.Item2))
            {
                return CacheResponse;
            }
            else
            {
                Waiting();
                var request =RestHandler.GetRequest("tracker/statsbyallseasons", Method.Post);
                
                request.AddJsonBody(new { username = username, region = region });

                var response = Client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    AddToCache(username, (true, response.Content), 20);
                    return (true, response.Content);
                }
                EndWaiting();
                return (false, "db error");
            }
        }

        /// <summary>
        /// gets the stats by operator response:
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> GetStatsByOperator(string username)
        {

            var CacheResponse = GetCachedResponse("");
            if (!string.IsNullOrEmpty(CacheResponse.Item2))
            {
                return CacheResponse;
            }
            else
            {
                Waiting();
                var request =RestHandler.GetRequest("tracker/statsbyoperator", Method.Post);
                
                request.AddJsonBody(new { username = username, team = "attacker,defender" });

                var response = Client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    AddToCache(username, (true, response.Content), 20);
                    return (true, response.Content);
                }
                EndWaiting();
                return (false, "db error");
            }
        }
        */
        #endregion

        #region license
        /*
        /// <summary>
        /// gets the stats by operator response:
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> GetLicenseStatus()
        {
            var CacheResponse = GetCachedResponse("");
            if (!string.IsNullOrEmpty(CacheResponse.Item2))
            {
                return CacheResponse;
            }
            else
            {
                Waiting();
                var request =RestHandler.GetRequest("/license/status", Method.Get);
                

                var response = Client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    AddToCache("", (true, response.Content), 1);
                    return (true, response.Content);
                }
                EndWaiting();
                return (false, "db error");
            }
        }

        /// <summary>
        /// gets the stats by operator response:
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> ActivateLicense(string key)
        {
            RemoveFromCache("GetLicenseStatus");
            Waiting();
            var request =RestHandler.GetRequest("/license/activate", Method.Post);
            
            request.AddJsonBody(new { key = key });

            var response = Client.Execute<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest) //success
            {
                EndWaiting();
                return (false, "License could not be activated. Please contact support");
            }
            EndWaiting();
            return (false, "db error");
        }
        */
        #endregion

        #region strats
        /// <summary>
        /// adds new scrim to db by api call
        /// time = timestringfrom + | + timestringto
        /// </summary>
        /// <returns></returns>
        public static async Task<(bool, string)> NewStrat(string name, string game_id, string map_id, string position_id, int version, string content)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("strat/new", Method.Post);
                
                request.AddJsonBody(new { name = name, game_id = game_id, map_id = map_id, position_id = position_id, version = version, content = content });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    DataCache.RetrieveStrats();
                    return (true, response.Content);
                }
                EndWaiting();
                return (false, "db error");
            }
        }

        /// <summary>
        /// deletes routine by api call
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteStrat(string strat_id)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                Waiting();
                var request =RestHandler.GetRequest("strat/delete", Method.Post);
                
                request.AddJsonBody(new { strat_id = strat_id });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    DataCache.RetrieveStrats();
                    return true;
                }
                EndWaiting();
                return false;
            }
        }


        /// <summary>
        /// Loads all routines by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Strat>> GetStrats()
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                var request =RestHandler.GetRequest("strat", Method.Get);
                

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    var strats = JsonConvert.DeserializeObject<List<Strat>>(response.Content);
                    return strats;
                }
                else
                {
                    Notify.sendError(response.Content);
                }
                EndWaiting();
                return null;
            }
        }


        /// <summary>
        /// Saves routine by api call
        /// </summary>
        /// <param name="ntitle"></param>
        /// <param name="ncontent"></param>
        /// <param name="n_id"></param>
        /// <returns></returns>
        public static async Task<bool> SaveStrat(string strat_id, string name, string map_id, string position_id, int version, string content)
        {
            using (RestClient client = new RestClient(SettingsHandler.Settings.APIURL))
            {
                //compress
                content = Globals.CompressString(content);

                Waiting();
                var request =RestHandler.GetRequest("strat/update", Method.Post);
                
                request.AddJsonBody(new { strat_id = strat_id, name = name, map_id = map_id, position_id = position_id, version = version, content = content });

                var response = client.Execute<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    DataCache.RetrieveStrats();
                    return true;
                }
                else
                {
                    Notify.sendError(response.Content);
                }
                EndWaiting();
                return false;
            }
        }

        #endregion


        #region helper methodes

        /// <summary>
        /// Sets waiting cursor
        /// </summary>
        public static void Waiting()
        {
            Cursor.Current = Cursors.WaitCursor;
        }

        /// <summary>
        /// Removes waiting cursor
        /// </summary>
        public static void EndWaiting()
        {
            Cursor.Current = Cursors.Default;
        }

        public static string ExtractMessage(string jsonString)
        {
            if(string.IsNullOrEmpty(jsonString))
            {
                return null;
            }
            // Remove the outer escaped quotes
            string trimmedJsonString = jsonString.Trim('"');

            // Unescape the inner content
            string unescapedJsonString = System.Text.RegularExpressions.Regex.Unescape(trimmedJsonString);

            // Parse the unescaped JSON string
            var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(unescapedJsonString);

            // Extract the "msg" value
            string message = jsonObject["msg"].ToString();

            return message;
        }

        #endregion
    }
}
