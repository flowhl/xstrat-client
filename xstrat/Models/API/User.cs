using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xstrat.Models.API
{
    public class User
    {
        [JsonProperty("actionLink")]
        public string? ActionLink { get; set; }

        [JsonProperty("appMetadata")]
        public Dictionary<string, object> AppMetadata { get; set; } = new Dictionary<string, object>();


        [JsonProperty("aud")]
        public string? Aud { get; set; }

        [JsonProperty("confirmationSentAt")]
        public DateTime? ConfirmationSentAt { get; set; }

        [JsonProperty("confirmedAt")]
        public DateTime? ConfirmedAt { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("emailConfirmedAt")]
        public DateTime? EmailConfirmedAt { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("identities")]
        public List<UserIdentity> Identities { get; set; } = new List<UserIdentity>();

        [JsonProperty("invitedAt")]
        public DateTime? InvitedAt { get; set; }

        [JsonProperty("last_sign_inAt")]
        public DateTime? LastSignInAt { get; set; }

        [JsonProperty("phone")]
        public string? Phone { get; set; }

        [JsonProperty("phone_confirmedAt")]
        public DateTime? PhoneConfirmedAt { get; set; }

        [JsonProperty("recoverySentAt")]
        public DateTime? RecoverySentAt { get; set; }

        [JsonProperty("role")]
        public string? Role { get; set; }

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("userMetadata")]
        public Dictionary<string, object> UserMetadata { get; set; } = new Dictionary<string, object>();

    }
}
