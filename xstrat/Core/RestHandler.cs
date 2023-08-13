using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using xstrat.Models.API;

namespace xstrat.Core
{
    public static class RestHandler
    {
        public static RestClient Client;
        public static Timer Timer;
        public static Session CurrentSession;

        public static void Initialize()
        {
            Client = new RestClient(SettingsHandler.Settings.APIURL ?? SettingsHandler.Settings.APIURL);
            Timer = new Timer(Callback, null, 0, 2 * 60 * 1000);
        }

        public static RestRequest GetRequest(string Endpoint, Method method)
        {
            var request = new RestRequest(Endpoint, method);
            if (CurrentSession != null && CurrentSession.AccessToken.IsNotNullOrEmpty())
            {
                request.Authenticator = new JwtAuthenticator(CurrentSession.AccessToken);
            }
            request.RequestFormat = DataFormat.Json;

            return request;
        }

        private static void Callback(object state)
        {
            RenewSession();
        }

        private async static void RenewSession()
        {
            if (CurrentSession != null && (CurrentSession.ExpiresAt() - DateTime.Now).TotalSeconds <= 600)
            {
                var newSession = await ApiHandler.RenewSessionAsync(CurrentSession.AccessToken, CurrentSession.RefreshToken);
                if (newSession != null)
                {
                    CurrentSession = newSession;
                }
                else
                {
                    Notify.sendError("Could not renew session token");
                }
            }
        }
    }


}
