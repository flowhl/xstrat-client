using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DragonFruit.Data;
using DragonFruit.Data.Serializers.Newtonsoft;
using DragonFruit.Six.Api;
using DragonFruit.Six.Api.Accounts;
using DragonFruit.Six.Api.Accounts.Enums;
using DragonFruit.Six.Api.Authentication;
using DragonFruit.Six.Api.Authentication.Entities;

namespace xstrat.Core
{
    public class StatsHandler : Dragon6Client
    {
        // change this to whatever you want
        private static readonly string _tokenFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DragonFruit Network", "ubi.token");

        static StatsHandler()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_tokenFile));
        }

        /// <summary>
        /// Tells the Dragon6 Client how to get a token in the case of a restart or expiration
        /// </summary>
        protected override IUbisoftToken GetToken()
        {
            if (File.Exists(_tokenFile))
            {
                // if we have a file with some potentially valid keys, try that first
                var token = FileServices.ReadFile<UbisoftToken>(_tokenFile);

                if (token.Expiry < DateTime.Now)
                    return token;
            }

            // store logins somewhere that is NOT in the code
            var username = "xstrat.app";
            var password = "9KPUBr6HSA^@LouE";
            var newToken = this.GetUbiToken(username, password);

            // write new token to disk (non-blocking)
            _ = Task.Run(() => FileServices.WriteFile(_tokenFile, newToken));

            // return to keep going
            return newToken;
        }
    }
}
