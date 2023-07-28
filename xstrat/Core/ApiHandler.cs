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
        #region client functions
        public static RestClient client;

        public static List<ApiResponse> Cache = new List<ApiResponse>();

        public static Session CurrentSession;

        /// <summary>
        /// creates restclient instance
        /// </summary>
        public async static void Initialize()
        {
            if (client == null)
            {

                if (SettingsHandler.Settings.APIURL != null)
                {
                    client = new RestClient(SettingsHandler.Settings.APIURL);
                }
                else
                {
                    Notify.sendError("Please enter a proper url to reach the server. Use https://app.xstrat.app/ as default");
                }
            }
            var request = new RestRequest("/", Method.Get);
            request.RequestFormat = DataFormat.Json;

            var response = await client.ExecuteAsync<RestResponse>(request);

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
                            client.Authenticator = new JwtAuthenticator(CurrentSession.AccessToken);
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
            if (client.Authenticator == null)
            {
                client.Authenticator = new JwtAuthenticator(token);
                CurrentSession = new Session { AccessToken = token };
                var task = RenewSessionAsync();
                task.Wait();
                CurrentSession = task.Result;
                client.Authenticator = new JwtAuthenticator(CurrentSession.AccessToken);
            }
        }


        #endregion
        #region logins
        /// <summary>
        /// registers a new account
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

            var response = await client.ExecuteAsync<RestResponse>(request);
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
        /// Login by api call
        /// </summary>
        /// <param name="_email"></param>
        /// <param name="_pw"></param>
        /// <returns></returns>
        public static async Task<Session> LoginAsync(string _email, string _pw)
        {
            Waiting();
            var request = new RestRequest("user/signin", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { email = _email, password = _pw });

            var response = await client.ExecuteAsync<RestResponse>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var session = JsonConvert.DeserializeObject<Session>(response.Content);
                CurrentSession = session;
                client.Authenticator = new JwtAuthenticator(CurrentSession.AccessToken);
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

            var response = await client.ExecuteAsync<RestResponse>(request);

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

            var response = await client.ExecuteAsync<RestResponse>(request);

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

        public static async Task<List<Models.Supabase.Map>> GetMapsAsync()
        {
            Waiting();
            var request = new RestRequest("maps", Method.Get);
            request.RequestFormat = DataFormat.Json;
            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var maps = JsonConvert.DeserializeObject<List<Models.Supabase.Map>>(response.Content);

                EndWaiting();
                return maps;
            }
            EndWaiting();
            return null;
        }

        public static async Task<List<Models.Supabase.Operator>> GetOperatorsAsync()
        {
            Waiting();
            var request = new RestRequest("operators", Method.Get);
            request.RequestFormat = DataFormat.Json;
            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var operators = JsonConvert.DeserializeObject<List<Models.Supabase.Operator>>(response.Content);

                EndWaiting();
                return operators;
            }
            EndWaiting();
            return null;
        }

        public static async Task<List<Models.Supabase.Position>> GetPositionsAsync()
        {
            Waiting();
            var request = new RestRequest("position", Method.Get);
            request.RequestFormat = DataFormat.Json;
            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var positions = JsonConvert.DeserializeObject<List<Models.Supabase.Position>>(response.Content);

                EndWaiting();
                return positions;
            }
            EndWaiting();
            return null;
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
        #endregion
        #region Client requests
        public static async Task<List<Models.Supabase.Game>> GetGamesAsync()
        {
            Waiting();
            var request = new RestRequest("game", Method.Get);
            request.RequestFormat = DataFormat.Json;
            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var game = JsonConvert.DeserializeObject<List<Models.Supabase.Game>>(response.Content);

                EndWaiting();
                return game;
            }
            EndWaiting();
            return null;
        }

        public static async Task<bool> GetAdminStatus()
        {
            Waiting();
            var request = new RestRequest("user/teamadminstatus", Method.Get);
            request.RequestFormat = DataFormat.Json;
            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                var isAdmin = JsonConvert.DeserializeObject<bool>(response.Content);

                EndWaiting();
                return isAdmin;
            }
            EndWaiting();
            return false;
        }
        #endregion
        #region team
        public static async Task<(bool, string)> JoinTeam(string id, string pw)
        {
            Waiting();
            var request = new RestRequest("team/join", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { password = pw, team_id = id });

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                EndWaiting();
                return (false, "Invalid id or password");
            }
            EndWaiting();
            return (false, response.Content);
        }

        public static async Task LeaveTeam()
        {
            Waiting();
            var request = new RestRequest("team/leave", Method.Get);
            request.RequestFormat = DataFormat.Json;

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                Notify.sendSuccess("Team left sucessfully");
            }
            if(response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                Notify.sendError("You cannot leave the team as you are the team admin");
            }
            EndWaiting();
        }

        public static async Task<bool> NewTeam(string name, int igame_id)
        {
            var request = new RestRequest("team/new", Method.Post);
            request.RequestFormat = DataFormat.Json;
            var param = new NewParams { name = name, game_id = igame_id };
            request.AddJsonBody(param);

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created) //success
            {
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

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
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
            var response = await client.ExecuteAsync<RestResponse>(request);
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
            var response = await client.ExecuteAsync<RestResponse>(request);
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

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return true;
            }
            EndWaiting();
            return false;
        }
        public static async Task<(bool, string)> RenameTeam(string newname)
        {
            RemoveFromCache("TeamInfo");
            Waiting();
            var request = new RestRequest("team/rename ", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { newname = newname });

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, response.Content);
        }
        public static async Task<bool> SetColor(string color)
        {
            RemoveFromCache("GetColor");
            Waiting();
            var request = new RestRequest("user/color", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { color = color.Replace("#FF", "#") });

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
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

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
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

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return true;
            }
            EndWaiting();
            return false;
        }

        #endregion
        #region routines

        /// <summary>
        /// adds new routine to db by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<(bool, string)> NewRoutine()
        {
            Waiting();
            RemoveFromCache("GetRoutineContent");
            RemoveFromCache("GetAllRoutines");
            var request = new RestRequest("routines/new", Method.Post);
            request.RequestFormat = DataFormat.Json;

            var response = await client.ExecuteAsync<RestResponse>(request);
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
        public static async Task<(bool, string)> DeleteRoutine(int id)
        {
            RemoveFromCache("GetRoutineContent");
            RemoveFromCache("GetAllRoutines");
            Waiting();
            var request = new RestRequest("routines/delete", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { routine_id = id });

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
        }

        /// <summary>
        /// Gets routines content by api call
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> GetRoutineContent(int id)
        {
            var CacheResponse = GetCachedResponse(id.ToString());
            if (!string.IsNullOrEmpty(CacheResponse.Item2))
            {
                return CacheResponse;
            }
            else
            {
                Waiting();
                var request = new RestRequest("routines/content", Method.Post);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { routine_id = id });

                var response = await client.ExecuteAsync<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    AddToCache(id.ToString(), (true, response.Content), 20);
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
        public static async Task<(bool, string)> GetAllRoutines()
        {
            var CacheResponse = GetCachedResponse(MethodBase.GetCurrentMethod().Name, "");
            if (!string.IsNullOrEmpty(CacheResponse.Item2))
            {
                return CacheResponse;
            }
            else
            {
                Waiting();
                var request = new RestRequest("routines/all", Method.Get);
                request.RequestFormat = DataFormat.Json;

                var response = await client.ExecuteAsync<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    AddToCache("", (true, response.Content), 5);
                    return (true, response.Content);
                }
                EndWaiting();
                return (false, "db error");
            }
        }

        /// <summary>
        /// Saves routine by api call
        /// </summary>
        /// <param name="ntitle"></param>
        /// <param name="ncontent"></param>
        /// <param name="n_id"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> SaveRoutine(string ntitle, string ncontent, int n_id)
        {
            Waiting();
            RemoveFromCache("GetRoutineContent");
            RemoveFromCache("GetAllRoutines");
            var request = new RestRequest("routines/save", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { title = ntitle, content = ncontent, routine_id = n_id });

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
        }

        /// <summary>
        /// renames a routine by api call
        /// </summary>
        /// <param name="ntitle"></param>
        /// <param name="n_id"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> RenameRoutine(string ntitle, int n_id)
        {
            Waiting();
            RemoveFromCache("GetRoutineContent");
            RemoveFromCache("GetAllRoutines");
            var request = new RestRequest("routines/rename", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { title = ntitle, routine_id = n_id });

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
        }
        #endregion
        #region OffDays
        /// <summary>
        /// adds new routine to db by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<(bool, string)> NewOffDay(int typ, string title, string start, string end)
        {
            RemoveFromCache("GetUserOffDays");
            RemoveFromCache("GetTeamOffDays");
            Waiting();
            var request = new RestRequest("event/new", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { typ = typ, title = title, start = start, end = end });

            var response = await client.ExecuteAsync<RestResponse>(request);
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
        public static async Task<(bool, string)> DeleteOffDay(int id)
        {
            RemoveFromCache("GetUserOffDays");
            RemoveFromCache("GetTeamOffDays");
            Waiting();
            var request = new RestRequest("event/delete", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { event_id = id });

            var response = await client.ExecuteAsync<RestResponse>(request);
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
        public static async Task<(bool, string)> GetUserOffDays()
        {
            var CacheResponse = GetCachedResponse("GetUserOffDays");
            if (!string.IsNullOrEmpty(CacheResponse.Item2))
            {
                return CacheResponse;
            }
            else
            {
                Waiting();
                var request = new RestRequest("event/user", Method.Get);
                request.RequestFormat = DataFormat.Json;

                var response = await client.ExecuteAsync<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    AddToCache("GetUserOffDays", (true, response.Content), 1);
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
        public static async Task<(bool, string)> GetTeamOffDays()
        {

            var CacheResponse = GetCachedResponse("GetTeamOffDays");
            if (!string.IsNullOrEmpty(CacheResponse.Item2))
            {
                return CacheResponse;
            }
            else
            {
                Waiting();
                var request = new RestRequest("event/team", Method.Get);
                request.RequestFormat = DataFormat.Json;

                var response = await client.ExecuteAsync<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    AddToCache("GetTeamOffDays", (true, response.Content), 5);
                    return (true, response.Content);
                }
                EndWaiting();
                return (false, "db error");
            }
        }

        /// <summary>
        /// Saves routine by api call
        /// </summary>
        /// <param name="ntitle"></param>
        /// <param name="ncontent"></param>
        /// <param name="n_id"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> SaveOffDay(int id, int typ, string title, string start, string end)
        {
            RemoveFromCache("GetUserOffDays");
            RemoveFromCache("GetTeamOffDays");
            Waiting();
            var request = new RestRequest("event/save", Method.Post);
            request.RequestFormat = DataFormat.Json;
            if (typ == 1)
            {
                start = start.Split(' ')[0] + " 00:00:00";
                end = start.Split(' ')[0] + " 23:59:59";
            }

            request.AddJsonBody(new { id = id, typ = typ, title = title, start = start, end = end });

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
        }
        #endregion

        #region Scrims

        /// <summary>
        /// adds new scrim to db by api call
        /// time = timestringfrom + | + timestringto
        /// </summary>
        /// <returns></returns>
        public static async Task<(bool, string)> NewScrim(int typ, string title, string opponent_name, string time_start, string time_end, int event_type)
        {
            Waiting();
            var request = new RestRequest("scrim/new", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { typ = typ, title = title, opponent_name = opponent_name, time_start = time_start, time_end = time_end, event_type = event_type });

            var response = await client.ExecuteAsync<RestResponse>(request);
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
        public static async Task<(bool, string)> DeleteScrim(int id)
        {
            Waiting();
            var request = new RestRequest("scrim/delete", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { scrim_id = id });

            var response = await client.ExecuteAsync<RestResponse>(request);
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
        public static async Task<(bool, string)> GetTeamScrims()
        {
            Waiting();
            var request = new RestRequest("scrim/team", Method.Get);
            request.RequestFormat = DataFormat.Json;

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
        }

        /// <summary>
        /// Saves routine by api call
        /// </summary>
        /// <param name="ntitle"></param>
        /// <param name="ncontent"></param>
        /// <param name="n_id"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> SaveScrim(int id, string title, string comment, string time_start, string time_end, string opponent_name, string map_1_id, string map_2_id, string map_3_id, int typ, int event_type)
        {
            Waiting();
            var request = new RestRequest("scrim/save", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { scrim_id = id, title = title, comment = comment, time_start = time_start, time_end = time_end, opponent_name = opponent_name, map_1_id = map_1_id, map_2_id = map_2_id, map_3_id = map_3_id, typ = typ, event_type = event_type });

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
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
        public static async Task<(bool, string)> SetScrimResponse(int scrim_id, int typ)
        {
            Waiting();
            var request = new RestRequest("scrim/setresponse", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { scrim_id = scrim_id, typ = typ });

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
        }

        /// <summary>
        /// gets the scrim response:
        /// 0 default
        /// 1 accept
        /// 2 deny
        /// </summary>
        /// <returns></returns>
        public static async Task<(bool, string)> GetScrimResponse()
        {
            Waiting();
            var request = new RestRequest("scrim/getresponse", Method.Get);
            request.RequestFormat = DataFormat.Json;

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
        }

        #endregion

        #region tracker

        /// <summary>
        /// sets the scrim response:
        /// 0 default
        /// 1 accept
        /// 2 deny
        /// </summary>
        /// <param name="scrim_id"></param>
        /// <param name="typ"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> SetUbisoftID(string ubisoft_id)
        {
            RemoveFromCache("GetUbisoftID");
            Waiting();
            var request = new RestRequest("user/setubisoftid", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { ubisoft_id = ubisoft_id });

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
        }

        /// <summary>
        /// gets the scrim response:
        /// 0 default
        /// 1 accept
        /// 2 deny
        /// </summary>
        /// <returns></returns>
        public static async Task<(bool, string)> GetUbisoftID()
        {
            var CacheResponse = GetCachedResponse("");
            if (!string.IsNullOrEmpty(CacheResponse.Item2))
            {
                return CacheResponse;
            }
            else
            {
                Waiting();
                var request = new RestRequest("user/getubisoftid", Method.Get);
                request.RequestFormat = DataFormat.Json;

                var response = await client.ExecuteAsync<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    AddToCache("", (true, response.Content), 5);
                    return (true, response.Content);
                }
                EndWaiting();
                return (false, "db error");
            }
        }

        #endregion

        #region tracker

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

                var response = await client.ExecuteAsync<RestResponse>(request);
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

                var response = await client.ExecuteAsync<RestResponse>(request);
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

                var response = await client.ExecuteAsync<RestResponse>(request);
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

                var response = await client.ExecuteAsync<RestResponse>(request);
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

        #endregion

        #region license
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

                var response = await client.ExecuteAsync<RestResponse>(request);
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

            var response = await client.ExecuteAsync<RestResponse>(request);
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
        #endregion

        #region strats
        /// <summary>
        /// adds new scrim to db by api call
        /// time = timestringfrom + | + timestringto
        /// </summary>
        /// <returns></returns>
        public static async Task<(bool, string)> NewStrat(string name, int game_id, int map_id, int position_id, int version, string content)
        {
            RemoveFromCache("GetStrats");
            Waiting();
            var request = new RestRequest("strat/new", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { name = name, game_id = game_id, map_id = map_id, position_id = position_id, version = version, content = content });

            var response = await client.ExecuteAsync<RestResponse>(request);
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
        public static async Task<(bool, string)> DeleteStrat(int strat_id)
        {
            RemoveFromCache("GetStrats");
            Waiting();
            var request = new RestRequest("strat/delete", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { strat_id = strat_id });

            var response = await client.ExecuteAsync<RestResponse>(request);
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
        public static async Task<(bool, string)> GetStrats()
        {
            var CacheResponse = GetCachedResponse("GetStrats");
            if (!string.IsNullOrEmpty(CacheResponse.Item2))
            {
                return CacheResponse;
            }
            else
            {
                Waiting();
                var request = new RestRequest("strat/all", Method.Post);
                request.RequestFormat = DataFormat.Json;

                var response = await client.ExecuteAsync<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
                {
                    EndWaiting();
                    AddToCache("GetStrats", (true, response.Content), 3);
                    return (true, response.Content);
                }
                EndWaiting();
                return (false, "db error");
            }
        }

        /// <summary>
        /// Saves routine by api call
        /// </summary>
        /// <param name="ntitle"></param>
        /// <param name="ncontent"></param>
        /// <param name="n_id"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> SaveStrat(int strat_id, string name, int map_id, int position_id, int version, string content)
        {
            //compress
            content = Globals.CompressString(content);

            RemoveFromCache("GetStrats");
            Waiting();
            var request = new RestRequest("strat/save", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { strat_id = strat_id, name = name, map_id = map_id, position_id = position_id, version = version, content = content });

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
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

        private static DateTime last_cleanup = DateTime.Now;

        private static (bool, string) GetCachedResponse(string parameters, [CallerMemberName] string method = "")
        {
            CleanUpCache();
            var CacheRow = Cache.Where(x => x.Method == method && x.Parameters == parameters).FirstOrDefault();
            if (CacheRow != null && method != "") return CacheRow.Response;
            return (false, null);
        }

        public static void RemoveFromCache(string method)
        {
            Cache = Cache.Where(x => x.Method != method).ToList();
        }

        private static void CleanUpCache()
        {
            if (DateTime.Now.Subtract(last_cleanup).TotalMinutes < 3) return;
            if (Cache != null)
            {
                Cache = Cache.Where(x => x.Expiry > DateTime.Now).OrderByDescending(x => x.Expiry).ToList();
            }
            else
            {
                Cache = new List<ApiResponse>();
            }
        }

        private static void AddToCache(string parameters, (bool, string) response, int expiryMinutes, [CallerMemberName] string method = "")
        {
            if (method == "") return;
            Cache.Add(new ApiResponse(method, parameters, response, expiryMinutes));
        }

        //public static void ReadCacheFile()
        //{
        //    if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/xstrat/cache.xml")) return;
        //    try
        //    {
        //        var serializer = new XmlSerializer(typeof(List<ApiResponse>));
        //        using (var reader = XmlReader.Create(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/xstrat/cache.xml"))
        //        {
        //            Cache = (List<ApiResponse>)serializer.Deserialize(reader);
        //            CleanUpCache();
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        Logger.Log("Could not load Cache " + ex.Message);
        //        Notify.sendError("Could not load Cache " + ex.Message);
        //    }    
        //}

        //public static void SaveCacheFile()
        //{
        //    Waiting();
        //    CleanUpCache();
        //    if(Cache != null && Cache.Count > 0)
        //    {
        //        try
        //        {
        //            var serializer = new XmlSerializer(new List<ApiResponse>().GetType());
        //            using (var writer = XmlWriter.Create(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/xstrat/cache.xml"))
        //            {
        //                serializer.Serialize(writer, Cache);
        //                writer.Dispose();
        //            }
        //        }
        //        catch(Exception ex)
        //        {
        //            Logger.Log("Could not save Cache " + ex.Message);
        //            Notify.sendError("Could save load Cache " + ex.Message);
        //        }
        //    }
        //    EndWaiting();
        //}

        #endregion
    }
    public class ApiResponse
    {
        public string Method { get; set; }
        public string Parameters { get; set; }
        public DateTime Expiry { get; set; }
        public (bool, string) Response { get; set; }

        public ApiResponse(string method, string parameters, (bool, string) response, int expiryMinutes)
        {
            Method = method;
            Parameters = parameters;
            Response = response;
            Expiry = DateTime.Now.AddMinutes(expiryMinutes);
        }

        public ApiResponse()
        {
        }
    }
}
