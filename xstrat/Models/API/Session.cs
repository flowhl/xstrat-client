using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xstrat.Models.API
{
    public class Session
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        [JsonProperty("user")]
        public User? User { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public DateTime ExpiresAt()
        {
            return new DateTime(CreatedAt.Ticks).AddSeconds(ExpiresIn);
        }

        //
        // Summary:
        //     Returns true if the session has expired
        public bool Expired()
        {
            return ExpiresAt() < DateTime.Now;
        }
    }
}
