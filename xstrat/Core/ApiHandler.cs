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

namespace xstrat
{
    public static class ApiHandler
    {
        #region Properties and Session functions
        public static RestClient Client;

        public static Session CurrentSession;

        /// <summary>
        /// creates restclient instance
        /// </summary>
        public async static void Initialize()
        {
            if (Client == null)
            {

                if (SettingsHandler.Settings.APIURL != null)
                {
                    Client = new RestClient(SettingsHandler.Settings.APIURL);
                }
                else
                {
                    Notify.sendError("Please enter a proper url to reach the server. Use https://app.xstrat.app/ as default");
                }
            }
            var request = new RestRequest("/", Method.Get);
            request.RequestFormat = DataFormat.Json;

            var response = await Client.ExecuteAsync<RestResponse>(request);

            if (response.StatusCode != System.Net.HttpStatusCode.OK) //success
            {
                Notify.sendError("Api could not be reached. Please check your connection and restart");
                //await Task.Delay(5000);
                ////MessageBox.Show("Api could not be reached. Please check your connection");
                //App.Current.Shutdown();
            }
            RenewSessionQueue();
        }

        public static async void RenewSessionQueue()
        {
            while (true)
            {
                if (CurrentSession != null)
                {
                    if (CurrentSession.ExpiresIn <= 600)
                    {
                        var newSession = await RenewSessionAsync();
                        if (newSession != null)
                        {
                            CurrentSession = newSession;
                            Client.Authenticator = new JwtAuthenticator(CurrentSession.AccessToken);
                        }
                        else
                        {
                            Notify.sendError("Could not renew session token");
                        }
                    }
                }
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// adds baerer_token to client if not allready
        /// </summary>
        /// <param name="token"></param>
        public static void AddBearer(string token)
        {
            if (Client.Authenticator == null)
            {
                Client.Authenticator = new JwtAuthenticator(token);
                CurrentSession = new Session { AccessToken = token };
                var task = RenewSessionAsync();
                task.Wait();
                CurrentSession = task.Result;
                Client.Authenticator = new JwtAuthenticator(CurrentSession.AccessToken);
            }
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
            Waiting();
            var request = new RestRequest("user/signup", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { email = _email, password = _pw });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, "");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict) //duplicate
            {
                EndWaiting();
                return (false, "email allready registered");
            }
            EndWaiting();
            return (false, "db error");
        }

        /// <summary>
        /// User Sign In
        /// </summary>
        /// <param name="_email"></param>
        /// <param name="_pw"></param>
        /// <returns></returns>
        public static async Task<Session> SignInAsync(string _email, string _pw)
        {
            Waiting();
            var request = new RestRequest("user/signin", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { email = _email, password = _pw });

            var response = await Client.ExecuteAsync<RestResponse>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var session = JsonConvert.DeserializeObject<Session>(response.Content);
                CurrentSession = session;
                Client.Authenticator = new JwtAuthenticator(CurrentSession.AccessToken);
                DataCache.RetrieveUser();
                EndWaiting();
                return session;
            }
            EndWaiting();
            Notify.sendError("Error logging in: " + response.Content);
            return null;
        }

        public static async Task<Models.Supabase.UserData> GetUserDataAsync()
        {
            Waiting();
            var request = new RestRequest("user/data", Method.Post);
            request.RequestFormat = DataFormat.Json;

            var response = await Client.ExecuteAsync<RestResponse>(request);

            var userData = JsonConvert.DeserializeObject<Models.Supabase.UserData>(response.Content);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return userData;
            }
            EndWaiting();
            Notify.sendError("Error getting user data in: " + response.Content);
            return null;
        }

        public static async Task<Session> RenewSessionAsync()
        {
            if (CurrentSession == null) return null;
            var request = new RestRequest("user/refresh", Method.Post);
            request.RequestFormat = DataFormat.Json;

            var response = await Client.ExecuteAsync<RestResponse>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var session = JsonConvert.DeserializeObject<Session>(response.Content);
                return session;
            }
            return null;
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

        public static async Task<bool> SetColor(string color)
        {
            Waiting();
            var request = new RestRequest("user/color", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { color = color.Replace("#FF", "#") });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                DataCache.RetrieveUser();
                return true;
            }
            EndWaiting();
            return false;
        }

        public static async Task<bool> SetDiscordId(string discord)
        {
            Waiting();
            var request = new RestRequest("user/discord", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { discord = discord });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                DataCache.RetrieveUser();
                return true;
            }
            EndWaiting();
            return false;
        }

        public static async Task<bool> SetUbisoftID(string ubisoft_id)
        {
            Waiting();
            var request = new RestRequest("user/ubisoft", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { ubisoft_id = ubisoft_id });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return true;
            }
            EndWaiting();
            return false;
        }

        #endregion

        #region Static data

        #region Maps
        public static async Task<List<Models.Supabase.Map>> GetMapsAsync()
        {
            Waiting();
            var request = new RestRequest("maps", Method.Get);
            request.RequestFormat = DataFormat.Json;
            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var maps = JsonConvert.DeserializeObject<List<Models.Supabase.Map>>(response.Content);

                EndWaiting();
                return maps;
            }
            EndWaiting();
            return null;
        }
        #endregion

        #region Operators
        public static async Task<List<Models.Supabase.Operator>> GetOperatorsAsync()
        {
            Waiting();
            var request = new RestRequest("operators", Method.Get);
            request.RequestFormat = DataFormat.Json;
            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var operators = JsonConvert.DeserializeObject<List<Models.Supabase.Operator>>(response.Content);

                EndWaiting();
                return operators;
            }
            EndWaiting();
            return null;
        }
        #endregion

        #region Positions
        public static async Task<List<Models.Supabase.Position>> GetPositionsAsync()
        {
            Waiting();
            var request = new RestRequest("position", Method.Get);
            request.RequestFormat = DataFormat.Json;
            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var positions = JsonConvert.DeserializeObject<List<Models.Supabase.Position>>(response.Content);

                EndWaiting();
                return positions;
            }
            EndWaiting();
            return null;
        }
        #endregion

        #region Games

        public static async Task<List<Models.Supabase.Game>> GetGamesAsync()
        {
            Waiting();
            var request = new RestRequest("game", Method.Get);
            request.RequestFormat = DataFormat.Json;
            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var game = JsonConvert.DeserializeObject<List<Models.Supabase.Game>>(response.Content);

                EndWaiting();
                return game;
            }
            EndWaiting();
            return null;
        }

        #endregion

        #endregion

        #region Team

        public static async Task<bool> GetAdminStatus()
        {
            Waiting();
            var request = new RestRequest("user/teamadminstatus", Method.Get);
            request.RequestFormat = DataFormat.Json;
            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var isAdmin = JsonConvert.DeserializeObject<bool>(response.Content);

                EndWaiting();
                return isAdmin;
            }
            EndWaiting();
            return false;
        }

        public static async Task<bool> JoinTeam(string id, string pw)
        {
            Waiting();
            var request = new RestRequest("team/join", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { password = pw, team_id = id });

            var response = await Client.ExecuteAsync<RestResponse>(request);
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

        public static async Task LeaveTeam()
        {
            Waiting();
            var request = new RestRequest("team/leave", Method.Get);
            request.RequestFormat = DataFormat.Json;

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                DataCache.RetrieveTeam();
                DataCache.RetrieveTeamMates();
                Notify.sendSuccess("Team left sucessfully");
            }
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Notify.sendError("You cannot leave the team as you are the team admin");
            }
            EndWaiting();
        }

        public static async Task<bool> NewTeam(string name, string igame_id)
        {
            var request = new RestRequest("team/new", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { name = name, game_id = igame_id });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created) //success
            {
                DataCache.RetrieveTeam();
                DataCache.RetrieveTeamMates();
                EndWaiting();
                return true;
            }
            EndWaiting();
            return false;
        }

        public static async Task<bool> RenameTeamAsync(string _newname)
        {
            var request = new RestRequest("team/rename", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { name = _newname });

            var response = await Client.ExecuteAsync<RestResponse>(request);
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

        public static async Task<List<Models.Supabase.UserData>> GetTeamMembersAsync()
        {
            Waiting();
            var request = new RestRequest("team/members", Method.Get);
            request.RequestFormat = DataFormat.Json;
            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var members = JsonConvert.DeserializeObject<List<UserData>>(response.Content);

                EndWaiting();
                return members;
            }
            EndWaiting();
            return null;
        }

        public static async Task<Models.Supabase.Team> GetTeamInfoAsync()
        {
            Waiting();
            var request = new RestRequest("team", Method.Get);
            request.RequestFormat = DataFormat.Json;
            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var team = JsonConvert.DeserializeObject<Models.Supabase.Team>(response.Content);

                EndWaiting();
                return team;
            }
            EndWaiting();
            return null;

        }

        public static async Task<bool> DeleteTeam()
        {
            Waiting();
            var request = new RestRequest("team/delete", Method.Post);
            request.RequestFormat = DataFormat.Json;

            var response = await Client.ExecuteAsync<RestResponse>(request);
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

        public static async Task<bool> SetDiscordAdminSettings(string webhook, int sn_created, int sn_changed, int sn_weekly, int sn_soon, int sn_delay, int use_on_days)
        {
            Waiting();
            var request = new RestRequest("team/adminsettings", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { webhook = webhook, sn_created = sn_created, sn_changed = sn_changed, sn_weekly = sn_weekly, sn_soon = sn_soon, sn_delay = sn_delay, use_on_days = use_on_days });

            var response = await Client.ExecuteAsync<RestResponse>(request);
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

        #endregion
        #region Routines

        /// <summary>
        /// adds new routine to db by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> NewRoutine()
        {
            Waiting();
            var request = new RestRequest("routine/new", Method.Post);
            request.RequestFormat = DataFormat.Json;

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                DataCache.RetrieveRoutines();
                EndWaiting();
                return true;
            }
            EndWaiting();
            return false;

        }

        /// <summary>
        /// deletes routine by api call
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteRoutine(string id)
        {
            Waiting();
            var request = new RestRequest("routine/delete", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { routine_id = id });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                DataCache.RetrieveRoutines();
                EndWaiting();
                return true;
            }
            EndWaiting();
            return false;
        }

        /// <summary>
        /// Loads all routines by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Models.Supabase.Routine>> GetRoutinesAsync()
        {
            Waiting();
            var request = new RestRequest("routine", Method.Get);
            request.RequestFormat = DataFormat.Json;
            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var routines = JsonConvert.DeserializeObject<List<Models.Supabase.Routine>>(response.Content);

                EndWaiting();
                return routines;
            }
            EndWaiting();
            return null;
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
            Waiting();
            var request = new RestRequest("routine/update", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { title = ntitle, content = ncontent, routine_id = n_id });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                DataCache.RetrieveRoutines();
                EndWaiting();
                return true;
            }
            EndWaiting();
            return false;
        }
        #endregion

        #region OffDays
        /// <summary>
        /// adds new routine to db by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<(bool, string)> NewOffDay(int typ, string title, string start, string end)
        {
            Waiting();
            var request = new RestRequest("calendarblock/new", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { typ = typ, title = title, start = start, end = end });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
        }

        /// <summary>
        /// deletes routine by api call
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> DeleteOffDay(string id)
        {
            Waiting();
            var request = new RestRequest("calendarblock/delete", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { event_id = id });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
        }


        /// <summary>
        /// Loads all routines by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<List<CalendarBlock>> GetUserOffDays()
        {
            var userCalendarBlocks = (await GetTeamOffDays()).Where(x => x.Id == DataCache.CurrentUser.Id);
            return userCalendarBlocks.ToList();
        }

        /// <summary>
        /// Loads all routines by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<List<CalendarBlock>> GetTeamOffDays()
        {

            Waiting();
            var request = new RestRequest("calendarblock", Method.Get);
            request.RequestFormat = DataFormat.Json;

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var blocks = JsonConvert.DeserializeObject<List<CalendarBlock>>(response.Content);
                EndWaiting();
                return blocks;
            }
            EndWaiting();
            return null;
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
            Waiting();
            var request = new RestRequest("calendarblock/update", Method.Post);
            request.RequestFormat = DataFormat.Json;
            if (typ == 1)
            {
                start = start.SetTime(0, 0, 0);
                end = end.SetTime(23, 59, 59);
            }

            request.AddJsonBody(new { id = id, typ = typ, title = title, start = start, end = end });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return true;
            }
            EndWaiting();
            return false;
        }
        #endregion

        #region Scrims

        /// <summary>
        /// adds new scrim to db by api call
        /// time = timestringfrom + | + timestringto
        /// </summary>
        /// <returns></returns>
        public static async Task<CalendarEvent> NewScrim(int typ, string title, string opponent_name, string time_start, string time_end, int event_type)
        {
            Waiting();
            var request = new RestRequest("calendarevent/new", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { typ = typ, title = title, opponent_name = opponent_name, time_start = time_start, time_end = time_end, event_type = event_type });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var calendarEvent = JsonConvert.DeserializeObject<CalendarEvent>(response.Content);
                EndWaiting();
                return calendarEvent;
            }
            EndWaiting();
            return null;
        }

        /// <summary>
        /// deletes routine by api call
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteScrim(string id)
        {
            Waiting();
            var request = new RestRequest("calendarevent/delete", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { scrim_id = id });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return true;
            }
            EndWaiting();
            return false;
        }


        /// <summary>
        /// Loads all routines by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> GetTeamScrims()
        {
            Waiting();
            var request = new RestRequest("calendarevent", Method.Get);
            request.RequestFormat = DataFormat.Json;

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return true;
            }
            EndWaiting();
            return false;
        }

        /// <summary>
        /// Saves routine by api call
        /// </summary>
        /// <param name="ntitle"></param>
        /// <param name="ncontent"></param>
        /// <param name="n_id"></param>
        /// <returns></returns>
        public static async Task<bool> SaveScrim(string id, string title, string comment, string time_start, string time_end, string opponent_name, string map_1_id, string map_2_id, string map_3_id, int typ, int event_type)
        {
            Waiting();
            var request = new RestRequest("calendarevent/update", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { scrim_id = id, title = title, comment = comment, time_start = time_start, time_end = time_end, opponent_name = opponent_name, map_1_id = map_1_id, map_2_id = map_2_id, map_3_id = map_3_id, typ = typ, event_type = event_type });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return true;
            }
            EndWaiting();
            return false;
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
        public static async Task<bool> SetScrimResponse(int scrim_id, int typ)
        {
            Waiting();
            var request = new RestRequest("calendarevent/response", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { scrim_id = scrim_id, typ = typ });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return true;
            }
            EndWaiting();
            return false;
        }

        /// <summary>
        /// gets the scrim response:
        /// 0 default
        /// 1 accept
        /// 2 deny
        /// </summary>
        /// <returns></returns>
        public static async Task<List<CalendarEventResponse>> GetScrimResponse()
        {
            Waiting();
            var request = new RestRequest("calendarevent/responses", Method.Get);
            request.RequestFormat = DataFormat.Json;

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var responses = JsonConvert.DeserializeObject<List<CalendarEventResponse>>(response.Content);
                EndWaiting();
                return responses;
            }
            EndWaiting();
            return null;
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
                var request = new RestRequest("tracker/stats", Method.Post);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { username = username, region = region });

                var response = await Client.ExecuteAsync<RestResponse>(request);
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
                var request = new RestRequest("user/scrimresponsestats", Method.Post);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { user_id = user_id });

                var response = await Client.ExecuteAsync<RestResponse>(request);
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
                var request = new RestRequest("tracker/statsbyallseasons", Method.Post);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { username = username, region = region });

                var response = await Client.ExecuteAsync<RestResponse>(request);
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
                var request = new RestRequest("tracker/statsbyoperator", Method.Post);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { username = username, team = "attacker,defender" });

                var response = await Client.ExecuteAsync<RestResponse>(request);
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
                var request = new RestRequest("/license/status", Method.Get);
                request.RequestFormat = DataFormat.Json;

                var response = await Client.ExecuteAsync<RestResponse>(request);
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
            var request = new RestRequest("/license/activate", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { key = key });

            var response = await Client.ExecuteAsync<RestResponse>(request);
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
            Waiting();
            var request = new RestRequest("strat/new", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { name = name, game_id = game_id, map_id = map_id, position_id = position_id, version = version, content = content });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
        }

        /// <summary>
        /// deletes routine by api call
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteStrat(string strat_id)
        {
            Waiting();
            var request = new RestRequest("strat/delete", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { strat_id = strat_id });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return true;
            }
            EndWaiting();
            return false;
        }


        /// <summary>
        /// Loads all routines by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Strat>> GetStrats()
        {
            var request = new RestRequest("strat", Method.Post);
            request.RequestFormat = DataFormat.Json;

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                var strats = JsonConvert.DeserializeObject<List<Strat>>(response.Content);
                return strats;
            }
            EndWaiting();
            return null;
        }


        /// <summary>
        /// Saves routine by api call
        /// </summary>
        /// <param name="ntitle"></param>
        /// <param name="ncontent"></param>
        /// <param name="n_id"></param>
        /// <returns></returns>
        public static async Task<bool> SaveStrat(int strat_id, string name, int map_id, int position_id, int version, string content)
        {
            //compress
            content = Globals.CompressString(content);

            Waiting();
            var request = new RestRequest("strat/save", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { strat_id = strat_id, name = name, map_id = map_id, position_id = position_id, version = version, content = content });

            var response = await Client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return true;
            }
            EndWaiting();
            return false;
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

        #endregion
    }
}
