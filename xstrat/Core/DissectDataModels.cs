using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace xstrat.Dissect
{

    public class ActivityFeed
    {
        [JsonProperty("type")]
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonProperty("username")]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonProperty("time")]
        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonProperty("timeInSeconds")]
        [JsonPropertyName("timeInSeconds")]
        public int? TimeInSeconds { get; set; }

        [JsonProperty("target")]
        [JsonPropertyName("target")]
        public string Target { get; set; }

        [JsonProperty("headshot")]
        [JsonPropertyName("headshot")]
        public bool? Headshot { get; set; }
    }

    public class Gamemode
    {
        [JsonProperty("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public int? Id { get; set; }
    }

    public class Header
    {
        [JsonProperty("gameVersion")]
        [JsonPropertyName("gameVersion")]
        public string GameVersion { get; set; }

        [JsonProperty("codeVersion")]
        [JsonPropertyName("codeVersion")]
        public int? CodeVersion { get; set; }

        [JsonProperty("timestamp")]
        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }

        [JsonProperty("matchType")]
        [JsonPropertyName("matchType")]
        public MatchType MatchType { get; set; }

        [JsonProperty("map")]
        [JsonPropertyName("map")]
        public Map Map { get; set; }

        [JsonProperty("recordingPlayerID")]
        [JsonPropertyName("recordingPlayerID")]
        public object RecordingPlayerID { get; set; }

        [JsonProperty("recordingProfileID")]
        [JsonPropertyName("recordingProfileID")]
        public string RecordingProfileID { get; set; }

        [JsonProperty("additionalTags")]
        [JsonPropertyName("additionalTags")]
        public string AdditionalTags { get; set; }

        [JsonProperty("gamemode")]
        [JsonPropertyName("gamemode")]
        public Gamemode Gamemode { get; set; }

        [JsonProperty("roundsPerMatch")]
        [JsonPropertyName("roundsPerMatch")]
        public int? RoundsPerMatch { get; set; }

        [JsonProperty("roundsPerMatchOvertime")]
        [JsonPropertyName("roundsPerMatchOvertime")]
        public int? RoundsPerMatchOvertime { get; set; }

        [JsonProperty("roundNumber")]
        [JsonPropertyName("roundNumber")]
        public int? RoundNumber { get; set; }

        [JsonProperty("overtimeRoundNumber")]
        [JsonPropertyName("overtimeRoundNumber")]
        public int? OvertimeRoundNumber { get; set; }

        [JsonProperty("teams")]
        [JsonPropertyName("teams")]
        public List<Team> Teams { get; set; }

        [JsonProperty("players")]
        [JsonPropertyName("players")]
        public List<Player> Players { get; set; }

        [JsonProperty("gmSettings")]
        [JsonPropertyName("gmSettings")]
        public List<object> GmSettings { get; set; }

        [JsonProperty("playlistCategory")]
        [JsonPropertyName("playlistCategory")]
        public object PlaylistCategory { get; set; }

        [JsonProperty("matchID")]
        [JsonPropertyName("matchID")]
        public string MatchID { get; set; }
    }

    public class Map
    {
        [JsonProperty("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public int? Id { get; set; }
    }

    public class Match
    {
        [JsonProperty("rounds")]
        [JsonPropertyName("rounds")]
        public List<Round> Rounds { get; set; }

        [JsonProperty("playerStats")]
        [JsonPropertyName("playerStats")]
        public List<PlayerStat> PlayerStats { get; set; }
    }

    public class MatchType
    {
        [JsonProperty("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public int? Id { get; set; }
    }

    public class Player
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public object Id { get; set; }

        [JsonProperty("profileID")]
        [JsonPropertyName("profileID")]
        public string ProfileID { get; set; }

        [JsonProperty("username")]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonProperty("teamIndex")]
        [JsonPropertyName("teamIndex")]
        public int? TeamIndex { get; set; }

        [JsonProperty("heroName")]
        [JsonPropertyName("heroName")]
        public object HeroName { get; set; }

        [JsonProperty("alliance")]
        [JsonPropertyName("alliance")]
        public int? Alliance { get; set; }

        [JsonProperty("roleImage")]
        [JsonPropertyName("roleImage")]
        public object RoleImage { get; set; }

        [JsonProperty("roleName")]
        [JsonPropertyName("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("rolePortrait")]
        [JsonPropertyName("rolePortrait")]
        public object RolePortrait { get; set; }
    }

    public class PlayerStat
    {
        [JsonProperty("username")]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonProperty("teamIndex")]
        [JsonPropertyName("teamIndex")]
        public int? TeamIndex { get; set; }

        [JsonProperty("operator")]
        [JsonPropertyName("operator")]
        public string Operator { get; set; }

        [JsonProperty("kills")]
        [JsonPropertyName("kills")]
        public int? Kills { get; set; }

        [JsonProperty("died")]
        [JsonPropertyName("died")]
        public bool? Died { get; set; }

        [JsonProperty("assists")]
        [JsonPropertyName("assists")]
        public int? Assists { get; set; }

        [JsonProperty("headshots")]
        [JsonPropertyName("headshots")]
        public int? Headshots { get; set; }

        [JsonProperty("headshotPercentage")]
        [JsonPropertyName("headshotPercentage")]
        public double? HeadshotPercentage { get; set; }

        [JsonProperty("rounds")]
        [JsonPropertyName("rounds")]
        public int? Rounds { get; set; }

        [JsonProperty("deaths")]
        [JsonPropertyName("deaths")]
        public int? Deaths { get; set; }
    }

    public class Root
    {
        [JsonProperty("Match")]
        [JsonPropertyName("Match")]
        public Match Match { get; set; }
    }

    public class Round
    {
        [JsonProperty("header")]
        [JsonPropertyName("header")]
        public Header Header { get; set; }

        [JsonProperty("activityFeed")]
        [JsonPropertyName("activityFeed")]
        public List<ActivityFeed> ActivityFeed { get; set; }

        [JsonProperty("playerStats")]
        [JsonPropertyName("playerStats")]
        public List<PlayerStat> PlayerStats { get; set; }
    }

    public class Team
    {
        [JsonProperty("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonProperty("score")]
        [JsonPropertyName("score")]
        public int? Score { get; set; }
    }


}
