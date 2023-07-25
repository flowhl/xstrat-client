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
        [JsonProperty("accessToken")]
        public string? AccessToken { get; set; }

        [JsonProperty("expiresIn")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refreshToken")]
        public string? RefreshToken { get; set; }

        [JsonProperty("tokenType")]
        public string? TokenType { get; set; }

        [JsonProperty("user")]
        public User? User { get; set; }

        [JsonProperty("createdAt")]
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
