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


    #region Dissect
    public class Gamemode
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("id")]
        public int? Id;
    }

    public class Map
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("id")]
        public string Id;
    }

    public class MatchFeedback
    {
        [JsonProperty("type")]
        public string Type;

        [JsonProperty("time")]
        public string Time;

        [JsonProperty("timeInSeconds")]
        public double? TimeInSeconds;

        [JsonProperty("message")]
        public string Message;

        [JsonProperty("username")]
        public string Username;

        [JsonProperty("target")]
        public string Target;

        [JsonProperty("headshot")]
        public bool? Headshot;
    }

    public class MatchReplay
    {
        [JsonProperty("rounds")]
        public List<Round> Rounds;

        [JsonProperty("stats")]
        public List<Stat> Stats;
    }

    public class MatchType
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("id")]
        public int? Id;
    }

    public class Player
    {
        [JsonProperty("id")]
        public object Id;

        [JsonProperty("profileID")]
        public string ProfileID;

        [JsonProperty("username")]
        public string Username;

        [JsonProperty("teamIndex")]
        public int? TeamIndex;

        [JsonProperty("heroName")]
        public object HeroName;

        [JsonProperty("alliance")]
        public int? Alliance;

        [JsonProperty("roleImage")]
        public object RoleImage;

        [JsonProperty("roleName")]
        public string RoleName;

        [JsonProperty("rolePortrait")]
        public object RolePortrait;

        [JsonProperty("spawn")]
        public string Spawn;
    }

    public class Root
    {
        [JsonProperty("MatchReplay")]
        public MatchReplay MatchReplay;
    }

    public class Round
    {
        [JsonProperty("gameVersion")]
        public string GameVersion;

        [JsonProperty("codeVersion")]
        public int? CodeVersion;

        [JsonProperty("timestamp")]
        public DateTime? Timestamp;

        [JsonProperty("matchType")]
        public MatchType MatchType;

        [JsonProperty("map")]
        public Map Map;

        [JsonProperty("recordingPlayerID")]
        public object RecordingPlayerID;

        [JsonProperty("recordingProfileID")]
        public string RecordingProfileID;

        [JsonProperty("additionalTags")]
        public string AdditionalTags;

        [JsonProperty("gamemode")]
        public Gamemode Gamemode;

        [JsonProperty("roundsPerMatch")]
        public int? RoundsPerMatch;

        [JsonProperty("roundsPerMatchOvertime")]
        public int? RoundsPerMatchOvertime;

        [JsonProperty("roundNumber")]
        public int? RoundNumber;

        [JsonProperty("overtimeRoundNumber")]
        public int? OvertimeRoundNumber;

        [JsonProperty("teams")]
        public List<Team> Teams;

        [JsonProperty("players")]
        public List<Player> Players;

        [JsonProperty("gmSettings")]
        public List<object> GmSettings;

        [JsonProperty("playlistCategory")]
        public object PlaylistCategory;

        [JsonProperty("matchID")]
        public string MatchID;

        [JsonProperty("matchFeedback")]
        public List<MatchFeedback> MatchFeedback;

        [JsonProperty("stats")]
        public List<Stat> Stats;

        [JsonProperty("site")]
        public string Site;
    }

    public class Stat
    {
        [JsonProperty("username")]
        public string Username;

        [JsonProperty("kills")]
        public int? Kills;

        [JsonProperty("died")]
        public bool? Died;

        [JsonProperty("assists")]
        public int? Assists;

        [JsonProperty("headshots")]
        public int? Headshots;

        [JsonProperty("headshotPercentage")]
        public double? HeadshotPercentage;

        [JsonProperty("1vX")]
        public int? _1vX;

        [JsonProperty("rounds")]
        public int? Rounds;

        [JsonProperty("deaths")]
        public int? Deaths;
    }

    public class Team
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("score")]
        public int? Score;

        [JsonProperty("won")]
        public bool? Won;

        [JsonProperty("role")]
        public string Role;

        [JsonProperty("winCondition")]
        public string WinCondition;
    }

    #endregion

}
