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

namespace xstrat
{
    public static class ApiHandler
    {
        #region client functions
        public static RestClient client;

        /// <summary>
        /// creates restclient instance
        /// </summary>
        public async static void Initialize()
        {
            if(client == null)
            {

                if (SettingsHandler.APIURL != null)
                {
                    client = new RestClient(SettingsHandler.APIURL);
                }
                else
                {
                    Notify.sendError("Please enter a proper url to reach the server. Use https://app.xstrat.app/api as default");
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
            var request = new RestRequest("register", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { name = _name, email = _email, password = _pw });

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
        public static async Task<(bool, string)> LoginAsync(string _email, string _pw)
        {
            Waiting();
            var request = new RestRequest("login", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { email = _email, password = _pw });

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
        /// verifies the api token by api call
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<bool> VerifyToken(string token)
        {
            Waiting();
            var request = new RestRequest("verify", Method.Post);
            client.Authenticator = new JwtAuthenticator(token);
            request.RequestFormat = DataFormat.Json;

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true);
            }
            client.Authenticator = null;
            EndWaiting();
            return (false);
        }

        public static async Task<(bool, string)> GetMaps()
        {
            Waiting();
            var request = new RestRequest("maps", Method.Get);
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
        public static async Task<(bool, string)> Games()
        {
            Waiting();
            var request = new RestRequest("games", Method.Get);
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

        public static async Task<(bool, string)> GetAdminStatus()
        {
            Waiting();
            var request = new RestRequest("team/verifyadmin", Method.Get);
            request.RequestFormat = DataFormat.Json;
            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
        }
        #endregion
        #region team
        public static async Task<(bool, string)> JoinTeam(string id, string pw)
        {
            Waiting();
            var request = new RestRequest("team/join", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { join_password = pw });
            request.AddJsonBody(new { team_id = id});

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted) //success
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

        public static async Task<(bool, string)> LeaveTeam()
        {
            Waiting();
            var request = new RestRequest("team/leave", Method.Get);
            request.RequestFormat = DataFormat.Json;

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, response.Content);
        }
        public static async Task<(bool, string)> VerifyAdmin()
        {
            Waiting();
            var request = new RestRequest("team/verifyadmin", Method.Get);
            request.RequestFormat = DataFormat.Json;

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, response.Content);
        }
        public static async Task<(bool, string)> NewTeam(string name, int igame_id)
        {
                Waiting();
                if (name == null || name == "")
                {
                    return(false, "invalid input");
                }
                var request = new RestRequest("team/new", Method.Post);
                request.RequestFormat = DataFormat.Json;
                var param = new NewParams { name = name, game_id = igame_id };
                request.AddJsonBody(param);

                var response = await client.ExecuteAsync<RestResponse>(request);
                if (response.StatusCode == System.Net.HttpStatusCode.Created) //success
                {
                    EndWaiting();
                    return (true, response.Content);
                }
                EndWaiting();
                return (false, response.Content);
        }
        public static async Task<(bool, string)> TeamJoinpassword()
        {
            Waiting();
            var request = new RestRequest("team/joinpassword", Method.Get);
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
        public static async Task<(bool, string)> UpdateNameTeam(string _newname)
        {
            Waiting();
            var request = new RestRequest("team/new", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { newname = _newname });

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, "db error");
        }
        public static async Task<(bool, string)> TeamMembers()
        {
            Waiting();
            var request = new RestRequest("team/members", Method.Get);
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
        public static async Task<(bool, string)> TeamInfo()
        {
            Waiting();
            var request = new RestRequest("team/info", Method.Get);
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
        public static async Task<(bool, string)> DeleteTeam()
        {
            Waiting();
            var request = new RestRequest("team/delete", Method.Post);
            request.RequestFormat = DataFormat.Json;

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, response.Content);
        }
        public static async Task<(bool, string)> RenameTeam(string newname)
        {
            Waiting();
            var request = new RestRequest("team/rename ", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new {newname = newname});

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, response.Content);
        }
        public static async Task<(bool, string)> GetColor()
        {
            Waiting();
            var request = new RestRequest("team/getcolor", Method.Get);
            request.RequestFormat = DataFormat.Json;

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, response.Content);
        }
        public static async Task<(bool, string)> SetColor(string color)
        {
            Waiting();
            var request = new RestRequest("team/setcolor", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new {color = color.Replace("#FF", "#")});

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, response.Content);
        }

        public static async Task<(bool, string)> GetDiscordId()
        {
            Waiting();
            var request = new RestRequest("team/getdiscord", Method.Get);
            request.RequestFormat = DataFormat.Json;

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, response.Content);
        }
        public static async Task<(bool, string)> SetDiscordId(string discord)
        {
            Waiting();
            var request = new RestRequest("team/setdiscord", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { discord = discord});

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, response.Content);
        }

        public static async Task<(bool, string)> GetDiscordData()
        {
            Waiting();
            var request = new RestRequest("team/getdiscordata", Method.Get);
            request.RequestFormat = DataFormat.Json;

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, response.Content);
        }
        public static async Task<(bool, string)> SetDiscordWebhook(string webhook, int sn_created, int sn_changed, int sn_weekly, int sn_soon, int sn_delay)
        {
            Waiting();
            var request = new RestRequest("team/setdiscorddata", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { webhook = webhook, sn_created = sn_created, sn_changed = sn_changed, sn_weekly = sn_weekly, sn_soon = sn_soon, sn_delay = sn_delay});

            var response = await client.ExecuteAsync<RestResponse>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Accepted) //success
            {
                EndWaiting();
                return (true, response.Content);
            }
            EndWaiting();
            return (false, response.Content);
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
            Waiting();
            var request = new RestRequest("routines/delete", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { routine_id = id});

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
            Waiting();
            var request = new RestRequest("routines/content", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { routine_id = id});

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
        public static async Task<(bool, string)> GetAllRoutines()
        {
            Waiting();
            var request = new RestRequest("routines/all", Method.Get);
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
        public static async Task<(bool, string)> SaveRoutine(string ntitle, string ncontent, int n_id)
        {
            Waiting();
            var request = new RestRequest("routines/save", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { title = ntitle, content = ncontent, routine_id = n_id});

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
            Waiting();
            var request = new RestRequest("event/new", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { typ = typ, title = title, start = start, end = end});

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
            Waiting();
            var request = new RestRequest("event/user", Method.Get);
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
        /// Loads all routines by api call
        /// </summary>
        /// <returns></returns>
        public static async Task<(bool, string)> GetTeamOffDays()
        {
            Waiting();
            var request = new RestRequest("event/team", Method.Get);
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
        public static async Task<(bool, string)> SaveOffDay(int id, int typ, string title, string start, string end)
        {
            Waiting();
            var request = new RestRequest("event/save", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new {id = id, typ = typ, title = title, start = start, end = end });

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
        public static async Task<(bool, string)> NewScrim(int typ, string title,string opponent_name, string time_start, string time_end)
        {
            Waiting();
            var request = new RestRequest("scrim/new", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { typ = typ, title = title, opponent_name = opponent_name, time_start = time_start, time_end = time_end });

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
        public static async Task<(bool, string)> SaveScrim(int id,string title, string comment, string time_start, string time_end, string opponent_name, int? map_1_id, int? map_2_id, int? map_3_id, int typ)
        {
            Waiting();
            var request = new RestRequest("scrim/save", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { scrim_id = id, title = title, comment = comment, time_start = time_start, time_end = time_end, opponent_name = opponent_name, map_1_id = map_1_id, map_2_id = map_2_id, map_3_id = map_3_id, typ = typ });

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
            request.AddJsonBody(new { scrim_id = scrim_id, typ = typ});

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
            Waiting();
            var request = new RestRequest("user/setubisoftid", Method.Post);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new { ubisoft_id = ubisoft_id});

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
            Waiting();
            var request = new RestRequest("user/getubisoftid", Method.Get);
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

        #region helper methodes

        /// <summary>
        /// Sets waiting cursor
        /// </summary>
        private static void Waiting()
        {
            Cursor.Current = Cursors.WaitCursor;
        }

        /// <summary>
        /// Removes waiting cursor
        /// </summary>
        private static void EndWaiting()
        {
            Cursor.Current = Cursors.Default;
        }

        #endregion
    }
}
