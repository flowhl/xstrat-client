using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace xstrat.Dissect
{
    #region Dissect
    public class Gamemode
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }

    public class Map
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class MatchFeedback
    {
        [JsonProperty("type")]
        public Type Type { get; set; }

        [JsonProperty("time")]
        public string Time { get; set; }

        [JsonProperty("timeInSeconds")]
        public double TimeInSeconds { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("operator")]
        public Operator Operator { get; set; }

        [JsonProperty("target")]
        public string Target { get; set; }

        [JsonProperty("headshot")]
        public bool? Headshot { get; set; }
    }

    public class MatchType
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }

    public class Operator
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public object Id { get; set; }
    }

    public class Player
    {
        [JsonProperty("id")]
        public object Id { get; set; }

        [JsonProperty("profileID")]
        public string ProfileID { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("teamIndex")]
        public int TeamIndex { get; set; }

        [JsonProperty("operator")]
        public Operator Operator { get; set; }

        [JsonProperty("heroName")]
        public object HeroName { get; set; }

        [JsonProperty("alliance")]
        public int Alliance { get; set; }

        [JsonProperty("roleImage")]
        public object RoleImage { get; set; }

        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("rolePortrait")]
        public object RolePortrait { get; set; }

        [JsonProperty("spawn")]
        public string Spawn { get; set; }
    }

    public class MatchReplay
    {
        [JsonProperty("rounds")]
        public List<Round> Rounds { get; set; }

        [JsonProperty("stats")]
        public List<Stat> Stats { get; set; }
    }

    public class Round
    {
        [JsonIgnore]
        public MatchReplay Root { get; set; }

        [JsonProperty("gameVersion")]
        public string GameVersion { get; set; }

        [JsonProperty("codeVersion")]
        public int CodeVersion { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("matchType")]
        public MatchType MatchType { get; set; }

        [JsonProperty("map")]
        public Map Map { get; set; }

        [JsonProperty("site")]
        public string Site { get; set; }

        [JsonProperty("recordingPlayerID")]
        public object RecordingPlayerID { get; set; }

        [JsonProperty("recordingProfileID")]
        public string RecordingProfileID { get; set; }

        [JsonProperty("additionalTags")]
        public string AdditionalTags { get; set; }

        [JsonProperty("gamemode")]
        public Gamemode Gamemode { get; set; }

        [JsonProperty("roundsPerMatch")]
        public int RoundsPerMatch { get; set; }

        [JsonProperty("roundsPerMatchOvertime")]
        public int RoundsPerMatchOvertime { get; set; }

        [JsonProperty("roundNumber")]
        public int RoundNumber { get; set; }

        [JsonProperty("overtimeRoundNumber")]
        public int OvertimeRoundNumber { get; set; }

        [JsonProperty("teams")]
        public List<Team> Teams { get; set; }

        [JsonProperty("players")]
        public List<Player> Players { get; set; }

        [JsonProperty("gmSettings")]
        public List<object> GmSettings { get; set; }

        [JsonProperty("playlistCategory")]
        public object PlaylistCategory { get; set; }

        [JsonProperty("matchID")]
        public string MatchID { get; set; }

        [JsonProperty("matchFeedback")]
        public List<MatchFeedback> MatchFeedback { get; set; }

        [JsonProperty("stats")]
        public List<Stat> Stats { get; set; }
    }

    public class Stat
    {
        [JsonIgnore]
        public MatchReplay Root { get; set; }
        [JsonIgnore]
        public Round Round { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("kills")]
        public int Kills { get; set; }

        [JsonProperty("died")]
        public bool Died { get; set; }

        [JsonProperty("assists")]
        public int Assists { get; set; }

        [JsonProperty("headshots")]
        public int Headshots { get; set; }

        [JsonProperty("headshotPercentage")]
        public double HeadshotPercentage { get; set; }

        [JsonProperty("rounds")]
        public int? Rounds { get; set; }

        [JsonProperty("deaths")]
        public int Deaths { get; set; }
        //Custom calculated:

        public double KD
        {
            get
            {
                return Deaths > 0 ? (double)Kills / (double)Deaths : 0;
            }
        }

        public string KDString
        {
            get
            {
                return KD.ToString("F2");
            }
        }

        public string DiedString
        {
            get
            {
                return Died.ToString();

            }
        }

        public string HeadshotPercentageString
        {
            get
            {
                return HeadshotPercentage.ToString("F2") + "%";
            }
        }

        [JsonIgnore]
        public int EntryKills
        {
            get
            {
                //is general stat
                if (Round != null)
                {
                    int team = Round.Players.Where(x => x.Username == Username).FirstOrDefault().TeamIndex;
                    var killFeed = Round.MatchFeedback.Where(x => x.Type.Name == "Kill" && Round.Players.Where(x => x.TeamIndex == team).Select(x => x.Username).Contains(x.Username));
                    if (killFeed.Count() == 0) return 0;

                    if (killFeed.FirstOrDefault().Username == Username)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }

                }
                //is round specific stat
                else
                {
                    if (Root == null) return 0;
                    int entryKills = 0;
                    foreach (var round in Root.Rounds)
                    {
                        entryKills += round.Stats.Where(x => x.Username == Username).Sum(x => x.EntryKills);
                    }
                    return entryKills;
                }
            }
        }

        [JsonIgnore]
        public double KOST
        {
            get
            {
                //is general stat
                if (Round != null)
                {
                    int team = Round.Players.Where(x => x.Username == Username).FirstOrDefault().TeamIndex;
                    var teamKillFeed = Round.MatchFeedback.Where(x => Round.Players.Where(x => x.TeamIndex == team).Select(x => x.Username).Contains(x.Username));
                    //Kill
                    bool isKill = teamKillFeed.Any(x => x.Type.Name == "Kill" && x.Username == Username);
                    //Objective
                    bool isObjective = teamKillFeed.Any(x => (x.Type.Name == "DefuserPlantComplete" || x.Type.Name == "DefuserDisableComplete") && x.Username == Username);
                    //Survive
                    bool isSurvive = !Round.MatchFeedback.Any(x => x.Type.Name == "Kill" && x.Target == Username);
                    //Trade
                    bool isTrade = false;
                    var killRow = Round.MatchFeedback.FirstOrDefault(x => x.Type.Name == "Kill" && x.Target == Username);
                    if (killRow != null)
                    {
                        string killer = killRow.Username;
                        isTrade = teamKillFeed.Any(x => x.Type.Name == "Kill" && x.Target == killer && (x.TimeInSeconds - killRow.TimeInSeconds <= 10));
                    }

                    if (isKill || isObjective || isSurvive || isTrade)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                //is round specific stat
                else
                {
                    if (Root == null) return 0;
                    List<double> KOSTValues = new List<double>();
                    foreach (var round in Root.Rounds)
                    {
                        var stats = round.Stats.Where(x => x.Username == Username);
                        if (stats.Count() == 0) continue;
                        KOSTValues.Add(stats.Average(x => x.KOST));
                    }
                    if(KOSTValues.Count == 0)
                    {
                        return 0;
                    }
                    return KOSTValues.Average();
                }
            }
        }

        [JsonIgnore]
        public string KOSTPercentageString
        {
            get
            {
                return KOST.ToString("F2") + "%";
            }
        }

        [JsonIgnore]
        public double KPR
        {
            get
            { 
                if(Round != null)
                {
                    return Kills;
                }
                else
                {
                    if (Root == null) return 0;
                    return (double)Kills / (double)Root.Rounds.Count;
                }
            }
        }

        [JsonIgnore]
        public string KPRString
        {
            get
            {
                return KPR.ToString("F2");
            }
        }

    }

    public class Team
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("won")]
        public bool Won { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("winCondition")]
        public string WinCondition { get; set; }
    }

    public class Type
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }
    }


    #endregion

    #region DissectOld
    //public class Gamemode
    //{
    //    [JsonProperty("name")]
    //    public string Name { get; set; }

    //    [JsonProperty("id")]
    //    public int? Id { get; set; }
    //}

    //public class Map
    //{
    //    [JsonProperty("name")]
    //    public string Name { get; set; }

    //    [JsonProperty("id")]
    //    public string Id { get; set; }
    //}

    //public class MatchFeedback
    //{
    //    [JsonProperty("type")]
    //    public string Type { get; set; }

    //    [JsonProperty("time")]
    //    public string Time { get; set; }

    //    [JsonProperty("timeInSeconds")]
    //    public double? TimeInSeconds { get; set; }

    //    [JsonProperty("message")]
    //    public string Message { get; set; }

    //    [JsonProperty("username")]
    //    public string Username { get; set; }

    //    [JsonProperty("target")]
    //    public string Target { get; set; }

    //    [JsonProperty("headshot")]
    //    public bool? Headshot { get; set; }
    //}

    //public class MatchReplay
    //{
    //    [JsonProperty("rounds")]
    //    public List<Round> Rounds { get; set; }

    //    [JsonProperty("stats")]
    //    public List<Stat> Stats { get; set; }
    //}

    //public class MatchType
    //{
    //    [JsonProperty("name")]
    //    public string Name { get; set; }

    //    [JsonProperty("id")]
    //    public int? Id { get; set; }
    //}

    //public class Player
    //{
    //    [JsonProperty("id")]
    //    public object Id { get; set; }

    //    [JsonProperty("profileID")]
    //    public string ProfileID { get; set; }

    //    [JsonProperty("username")]
    //    public string Username { get; set; }

    //    [JsonProperty("teamIndex")]
    //    public int? TeamIndex { get; set; }

    //    [JsonProperty("heroName")]
    //    public object HeroName { get; set; }

    //    [JsonProperty("alliance")]
    //    public int? Alliance { get; set; }

    //    [JsonProperty("roleImage")]
    //    public object RoleImage { get; set; }

    //    [JsonProperty("roleName")]
    //    public string RoleName { get; set; }

    //    [JsonProperty("rolePortrait")]
    //    public object RolePortrait { get; set; }

    //    [JsonProperty("spawn")]
    //    public string Spawn { get; set; }
    //}

    //public class Root
    //{
    //    [JsonProperty("MatchReplay")]
    //    public MatchReplay MatchReplay { get; set; }
    //}

    //public class Round
    //{
    //    [JsonProperty("gameVersion")]
    //    public string GameVersion { get; set; }

    //    [JsonProperty("codeVersion")]
    //    public int? CodeVersion { get; set; }

    //    [JsonProperty("timestamp")]
    //    public DateTime? Timestamp { get; set; }

    //    [JsonProperty("matchType")]
    //    public MatchType MatchType { get; set; }

    //    [JsonProperty("map")]
    //    public Map Map { get; set; }

    //    [JsonProperty("recordingPlayerID")]
    //    public object RecordingPlayerID { get; set; }

    //    [JsonProperty("recordingProfileID")]
    //    public string RecordingProfileID { get; set; }

    //    [JsonProperty("additionalTags")]
    //    public string AdditionalTags { get; set; }

    //    [JsonProperty("gamemode")]
    //    public Gamemode Gamemode { get; set; }

    //    [JsonProperty("roundsPerMatch")]
    //    public int? RoundsPerMatch { get; set; }

    //    [JsonProperty("roundsPerMatchOvertime")]
    //    public int? RoundsPerMatchOvertime { get; set; }

    //    [JsonProperty("roundNumber")]
    //    public int? RoundNumber { get; set; }

    //    [JsonProperty("overtimeRoundNumber")]
    //    public int? OvertimeRoundNumber { get; set; }

    //    [JsonProperty("teams")]
    //    public List<Team> Teams { get; set; }

    //    [JsonProperty("players")]
    //    public List<Player> Players { get; set; }

    //    [JsonProperty("gmSettings")]
    //    public List<object> GmSettings { get; set; }

    //    [JsonProperty("playlistCategory")]
    //    public object PlaylistCategory { get; set; }

    //    [JsonProperty("matchID")]
    //    public string MatchID { get; set; }

    //    [JsonProperty("matchFeedback")]
    //    public List<MatchFeedback> MatchFeedback { get; set; }

    //    [JsonProperty("stats")]
    //    public List<Stat> Stats { get; set; }

    //    [JsonProperty("site")]
    //    public string Site { get; set; }
    //}

    //public class Stat
    //{
    //    [JsonProperty("username")]
    //    public string Username { get; set; }

    //    [JsonProperty("kills")]
    //    public int? Kills { get; set; }

    //    [JsonProperty("died")]
    //    public bool? Died { get; set; }

    //    [JsonProperty("assists")]
    //    public int? Assists { get; set; }

    //    [JsonProperty("headshots")]
    //    public int? Headshots { get; set; }

    //    [JsonProperty("headshotPercentage")]
    //    public double? HeadshotPercentage { get; set; }

    //    [JsonProperty("1vX")]
    //    public int? _1vX { get; set; }

    //    [JsonProperty("rounds")]
    //    public int? Rounds { get; set; }

    //    [JsonProperty("deaths")]
    //    public int? Deaths { get; set; }

    //    //Custom calculated:

    //    public double KD
    //    {
    //        get
    //        {
    //            return (double)Kills.GetValueOrDefault(0) / (double)Deaths.GetValueOrDefault(1);
    //        }
    //    }

    //    public string KDString
    //    {
    //        get
    //        {
    //            return KD.ToString("F2");
    //        }
    //    }

    //    public string DiedString
    //    {
    //        get
    //        {
    //            return Died.ToString();

    //        }
    //    }

    //    public string HeadshotPercentageString
    //    {
    //        get
    //        {
    //            return HeadshotPercentage.GetValueOrDefault().ToString("F2") + "%";
    //        }
    //    }
    //}

    //public class Team
    //{
    //    [JsonProperty("name")]
    //    public string Name { get; set; }

    //    [JsonProperty("score")]
    //    public int? Score { get; set; }

    //    [JsonProperty("won")]
    //    public bool? Won { get; set; }

    //    [JsonProperty("role")]
    //    public string Role { get; set; }

    //    [JsonProperty("winCondition")]
    //    public string WinCondition { get; set; }
    //}

    #endregion

}
