using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xstrat.Calendar;

namespace xstrat.Json
{
    public class CalendarEntry : ICalendarEvent
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Label { get; set; }

        /// <summary>
        /// 0 = scrim / blue
        /// 1 = offday / red
        /// 2 = ??? / purple
        /// </summary>
        public int typ { get; set; }

        public List<Object> args { get; set; }

        public bool visible { get; set; } = true;
        public User user { get; set; }
        public Scrim scrim { get; set; }
    }

    public class CalendarFilterType
    {
        public string name { get; set; }
        public int id { get; set; }

        public CalendarFilterType(int id, string name)
        {
            this.name = name;
            this.id = id;
        }
    }

    public class Content
    {
        public string content { get; set; }
    }

    public class Data
    {
        public int fieldCount { get; set; }
        public int affectedRows { get; set; }
        public int insertId { get; set; }
        public int serverStatus { get; set; }
        public int warningCount { get; set; }
        public string message { get; set; }
        public bool protocol41 { get; set; }
        public int changedRows { get; set; }
    }

    public class TeamSettingsData
    {
        public string webhook { get; set; }
        public int sn_created { get; set; }
        public int sn_changed { get; set; }
        public int sn_weekly { get; set; }
        public int sn_soon { get; set; }
        public int sn_delay { get; set; }
        public int use_on_days { get; set; }

    }

    public class DiscordID
    {
        public string discord { get; set; }
    }

    public class UbisoftID
    {
        public string ubisoft_id { get; set; }
    }

    public class Floor
    {
        public int id { get; set; }
        public int level { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public int map_id { get; set; }

        public Floor(int id, string name, string image, int map_id, int level)
        {
            this.id = id;
            this.name = name;
            this.image = image;
            this.map_id = map_id;
            this.level = level;
        }
    }

    public class Game
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class JColor
    {
        public string color { get; set; }
    }

    public class JoinPw
    {
        public int id { get; set; }
        public string join_password { get; set; }
    }

    public class Map
    {
        public int id { get; set; }
        public string name { get; set; }
        public int game_id { get; set; }

        public Map(int id, string name, int game_id)
        {
            this.id = id;
            this.name = name;
            this.game_id = game_id;
        }
    }

    public class xPosition
    {
        public int id { get; set; }
        public int map_id { get; set; }
        public string name { get; set; }

        public xPosition(int id, int map_id, string name)
        {
            this.id = id;
            this.map_id = map_id;
            this.name = name;
        }
    }

    public class NewParams
    {
        public string name { get; set; }
        public int game_id { get; set; }

    }

    public class OffDay
    {
        public int? Id { get; set; }
        public int? user_id { get; set; }
        public string creation_date { get; set; }

        /// <summary>
        /// types:
        /// 0 exactly
        /// 1 entire day
        /// 2 weekly
        /// 3 every second week
        /// 4 monthly
        /// </summary>
        public int typ { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }

        public OffDay(int id, int user_id, string creation_date, int typ, string title, string start, string end)
        {
            Id = id;
            this.user_id = user_id;
            this.creation_date = creation_date;
            this.typ = typ;
            this.title = title;
            this.start = start;
            this.end = end;
        }
    }

    public class OffDayType
    {
        public int id { get; set; }
        public string name { get; set; }

        /// <summary>
        /// type:
        /// 0 - exakt
        /// 1 - ganztägig
        /// 2 - wöchentlich
        /// 3 - jede 2. woche
        /// 4 - monatlich
        /// </summary>
        public OffDayType(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

    public class Position
    {
        public int id { get; set; }
        public int map_id { get; set; }
        public string name { get; set; }
        public List<Strat> strats { get; set; }

        public Position(int id, int map_id, string name, List<Strat> strats)
        {
            this.id = id;
            this.map_id = map_id;
            this.name = name;
            this.strats = strats;
        }

        public Position()
        {
        }
    }

    public class Routine
    {
        public int id { get; set; }
        public string title { get; set; }
        public int user_id { get; set; }
        public string created_date { get; set; }
        public string content { get; set; }
    }

    public class Scrim
    {
        public int id { get; set; }
        public int event_type { get; set; }
        public string title { get; set; }
        public string comment { get; set; }
        public string time_start { get; set; }
        public string time_end { get; set; }
        public string opponent_name { get; set; }
        public int team_id { get; set; }
        public int? map_1_id { get; set; }
        public int? map_2_id { get; set; }
        public int? map_3_id { get; set; }
        /// <summary>
        /// 0 - Normal
        /// 1 - 6+6
        /// </summary>
        public int typ { get; set; }
        public int creator_id { get; set; }
        public string creation_date { get; set; }
        public int? response_typ { get; set; }
        public string acc_user_list { get; set; }
        public string deny_user_list { get; set; }
        public string ign_user_list { get; set; }


        public Scrim(int id, int event_type ,string title, string comment, string time_start, string time_end, string opponent_name, int team_id, int? map_1_id, int? map_2_id, int? map_3_id, int typ, int creator_id, string creation_date, string acc_user_list, string deny_user_list, string ign_user_list)
        {
            this.id = id;
            this.event_type = event_type;
            this.title = title;
            this.comment = comment;
            this.time_start = time_start;
            this.time_end = time_end;
            this.opponent_name = opponent_name;
            this.team_id = team_id;
            this.map_1_id = map_1_id;
            this.map_2_id = map_2_id;
            this.map_3_id = map_3_id;
            this.typ = typ;
            this.creator_id = creator_id;
            this.creation_date = creation_date;
            this.acc_user_list = acc_user_list;
            this.deny_user_list = deny_user_list;
            this.ign_user_list = ign_user_list;
        }
    }

    public class EventType
    {
        public int id { get; set; }
        public string name { get; set; }

        public EventType(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

    public class ScrimMode
    {
        public int id { get; set; }
        public string name { get; set; }

        public ScrimMode(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }

    public class Strat
    {
        public int id { get; set; }
        public string name { get; set; }
        public int team_id { get; set; }
        public int game_id { get; set; }
        public int map_id { get; set; }
        public int position_id { get; set; }
        public int version { get; set; }
        public string content { get; set; }
        public string created_date { get; set; }
        public int created_by { get; set; }
        public string last_edit_time { get; set; }

        public Strat(int id,string name, int team_id, int game_id, int map_id, int position_id, int version, string content, string created_date, int created_by, string last_edit_time)
        {
            this.id = id;
            this.name = name;
            this.team_id = team_id;
            this.game_id = game_id;
            this.map_id = map_id;
            this.position_id = position_id;
            this.version = version;
            this.content = content;
            this.created_date = created_date;
            this.created_by = created_by;
            this.last_edit_time = last_edit_time;
        }

        public Strat()
        {
            this.id = 0;
            this.name = "strat 1";
            this.team_id = 0;
            this.game_id = 0;
            this.map_id = 0;
            this.position_id = 0;
            this.version = 1;
            this.content = "";
        }
    }

    public class TeamInfo
    {
        public string team_name { get; set; }
        public string admin_name { get; set; }
        public string game_name { get; set; }
        public int use_on_days { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public string ubisoft_id { get; set; }
    }

    public class XMap
    {
        public string Name { get; set; }
        public int game_id { get; set; }
        public List<Floor> floors { get; set; } = new List<Floor>();
        public List<Position> positions { get; set; } = new List<Position>();

        public XMap(string name, int game_id, List<Floor> floors, List<Position> positions)
        {
            Name = name;
            this.game_id = game_id;
            this.floors = floors;
            this.positions = positions;
        }

        public XMap()
        {
            floors.Add(new Floor(0, "basement", @"https://xstrat.app/wp-content/uploads/2022/03/DZ-consulate-basement.png", 0, 0));
            floors.Add(new Floor(1, "first floor", @"https://xstrat.app/wp-content/uploads/2022/03/DZ-consulate-groundfloor.png", 0, 1));
            var stratlist = new List<Strat>();
            stratlist.Add(new Strat());
            positions.Add(new Position(0, 0, "1", stratlist));
            positions.Add(new Position(1, 0, "2", stratlist));
            Name = "Bank";
            game_id = 0;
        }


    }

    public class Operator
    {
        public int id { get; set; }
        public string name { get; set; }
        public int game_id { get; set; }
        public int type { get; set; }

        public Operator(int id, string name, int game_id, int type)
        {
            this.id = id;
            this.name = name;
            this.game_id = game_id;
            this.type = type;
        }

        public Operator(){
            
        }
    }




    /// <summary>
    /// api response by stats call
    /// {
    //    "success": 1,
    //    "data": {
    //        "players": {
    //            "425df52e-8c3e-4013-b7bc-7120c16345b2": {
    //                "max_mmr": 3042,
    //                "skill_mean": 27.9737880743,
    //                "deaths": 154,
    //                "profile_id": "425df52e-8c3e-4013-b7bc-7120c16345b2",
    //                "next_rank_mmr": 2800,
    //                "rank": 16,
    //                "max_rank": 18,
    //                "board_id": "pvp_ranked",
    //                "skill_stdev": 5.2966506263,
    //                "kills": 140,
    //                "last_match_skill_stdev_change": -0.0397702864,
    //                "past_seasons_wins": 1024,
    //                "update_time": "2022-06-07T07:10:01.647000+00:00",
    //                "last_match_mmr_change": 61,
    //                "abandons": 0,
    //                "season": 25,
    //                "past_seasons_losses": 960,
    //                "top_rank_position": 0,
    //                "last_match_skill_mean_change": 0.6180404468,
    //                "mmr": 2797,
    //                "previous_rank_mmr": 2600,
    //                "last_match_result": 1,
    //                "past_seasons_abandons": 33,
    //                "wins": 18,
    //                "region": "emea",
    //                "losses": 18
    //            }
    //        }
    //    }
    //}
    /// </summary>
    public class StatsResponse {
        [JsonProperty(PropertyName = "players")]
        public Dictionary<string, StatsResponseDetail> StatsResponseDetails { get; set; }
    }
    public class StatsResponseDetail
    {
        public int xstrat_user_id { get; set; }
        public double? max_mmr { get; set; }
        public double? skill_mean { get; set; }
        public int? deaths { get; set; }
        public string profile_id { get; set; }
        public double? next_rank_mmr { get; set; }
        public int? rank { get; set; }
        public int? max_rank { get; set; }
        public string board_id { get; set; }
        public double? skill_stdev { get; set; }
        public int? kills { get; set; }
        public double? last_match_skill_stdev_change { get; set; }
        public int? past_seasons_wins { get; set; }
        public string update_time { get; set; }
        public double? last_match_mmr_change { get; set; }
        public int? abandons { get; set; }
        public int? season { get; set; }
        public int? past_seasons_losses { get; set; }
        public int? top_rank_position { get; set; }
        public double? last_match_skill_mean_change { get; set; }
        public double? mmr { get; set; }
        public double? previous_rank_mmr { get; set; }
        public int? last_match_result { get; set; }
        public int? past_seasons_abandons { get; set; }
        public int? wins { get; set; }
        public string region { get; set; }
        public int? losses { get; set; }
    }

    /// <summary>
    /// API response by statsbyseason
    /// {
    //    "success": 1,
    //    "data": [
    //        {
    //            "max_mmr": 3042,
    //            "skill_mean": 27.9737880743,
    //            "deaths": 154,
    //            "profile_id": "425df52e-8c3e-4013-b7bc-7120c16345b2",
    //            "next_rank_mmr": 2800,
    //            "rank": 16,
    //            "max_rank": 18,
    //            "board_id": "pvp_ranked",
    //            "skill_stdev": 5.2966506263,
    //            "kills": 140,
    //            "last_match_skill_stdev_change": -0.0397702864,
    //            "past_seasons_wins": 1024,
    //            "update_time": "2022-06-07T07:10:01.647000+00:00",
    //            "last_match_mmr_change": 61,
    //            "abandons": 0,
    //            "season": 25,
    //            "past_seasons_losses": 960,
    //            "top_rank_position": 0,
    //            "last_match_skill_mean_change": 0.6180404468,
    //            "mmr": 2797,
    //            "previous_rank_mmr": 2600,
    //            "last_match_result": 1,
    //            "past_seasons_abandons": 33,
    //            "wins": 18,
    //            "region": "emea",
    //            "losses": 18
    //        }
    //    ]
    //}
    /// </summary>
    public class StatsBySeasonDetail
    {
        public int? xstrat_user_id { get; set; }
        public double? max_mmr { get; set; }
        public double? skill_mean { get; set; }
        public int? deaths { get; set; }
        public string profile_id { get; set; }
        public double? next_rank_mmr { get; set; }
        public int? rank { get; set; }
        public int? max_rank { get; set; }
        public string board_id { get; set; }
        public double? skill_stdev { get; set; }
        public int? kills { get; set; }
        public double? last_match_skill_stdev_change { get; set; }
        public int? past_seasons_wins { get; set; }
        public string update_time { get; set; }
        public double? last_match_mmr_change { get; set; }
        public int? abandons { get; set; }
        public int? season { get; set; }
        public int? past_seasons_losses { get; set; }
        public int? top_rank_position { get; set; }
        public double? last_match_skill_mean_change { get; set; }
        public double? mmr { get; set; }
        public double? previous_rank_mmr { get; set; }
        public int? last_match_result { get; set; }
        public int? past_seasons_abandons { get; set; }
        public int? wins { get; set; }
        public string region { get; set; }
        public int? losses { get; set; }
    }


    /// <summary>
    /// API response by statsbyopartor
    /// {
//    "success": 1,
//    "data": {
//        "Defender": [
//            {
//                "type": "Generalized",
//                "statsType": "operators",
//                "statsDetail": "Alibi",
//                "matchesPlayed": 3,
//                "roundsPlayed": 3,
//                "minutesPlayed": 9,
//                "matchesWon": 1,
//                "matchesLost": 2,
//                "roundsWon": 0,
//                "roundsLost": 3,
//                "kills": 3,
//                "assists": 0,
//                "death": 3,
//                "headshots": 3,
//                "meleeKills": 0,
//                "teamKills": 0,
//                "openingKills": 0,
//                "openingDeaths": 0,
//                "trades": 0,
//                "openingKillTrades": 0,
//                "openingDeathTrades": 0,
//                "revives": 0,
//                "distanceTravelled": 563,
//                "winLossRatio": 0.5,
//                "killDeathRatio": {
//                    "value": 1,
//                    "p": 0
//                },
//                "headshotAccuracy": {
//    "value": 1,
//                    "p": 0
//                },
//                "killsPerRound": {
//    "value": 1,
//                    "p": 0
//                },
//                "roundsWithAKill": {
//    "value": 0.6667,
//                    "p": 0
//                },
//                "roundsWithMultiKill": {
//    "value": 0.3333,
//                    "p": 0
//                },
//                "roundsWithOpeningKill": {
//    "value": 0,
//                    "p": 0
//                },
//                "roundsWithOpeningDeath": {
//    "value": 0,
//                    "p": 0
//                },
//                "roundsWithKOST": {
//    "value": 0.6667,
//                    "p": 0
//                },
//                "roundsSurvived": {
//    "value": 0,
//                    "p": 0
//                },
//                "roundsWithAnAce": {
//    "value": 0,
//                    "p": 0
//                },
//                "roundsWithClutch": {
//    "value": 0,
//                    "p": 0
//                },
//                "timeAlivePerMatch": 87.6667,
//                "timeDeadPerMatch": 22,
//                "distancePerRound": 187.6667
//            },
//         ...
//        ]
//    }
//}
    /// </summary>
    public class StatsByOperator
    {
        public List<StatsByOperatorDetails> Attacker { get; set; }
        public List<StatsByOperatorDetails> Defender { get; set; }
    }
    public class StatsByOperatorDetails
    {
        public string type { get; set; }
        public string statsType { get; set; }
        public int? matchesPlayed { get; set; }
        public int? roundsPlayed { get; set; }
        public int? minutesPlayed { get; set; }
        public int? matchesWon { get; set; }
        public int? roundsWon { get; set; }
        public int? roundsLost { get; set; }
        public int? kills { get; set; }
        public int? assists { get; set; }
        public int? death { get; set; }
        public int? headshots { get; set; }
        public int? meleeKills { get; set; }
        public int? teamKills { get; set; }
        public int? openingKills { get; set; }
        public int? openingDeaths { get; set; }
        public int? trades { get; set; }
        public int? openingKillTrades { get; set; }
        public int? openingDeathTrades { get; set; }
        public int? revives { get; set; }
        public int? distanceTravelled { get; set; }
        public double? winLossRatio { get; set; }
        public KeyPair killDeathRatio { get; set; }
        public KeyPair headshotAccuracy { get; set; }
        public KeyPair killsPerRound { get; set; }
        public KeyPair roundsWithAKill { get; set; }
        public KeyPair roundsWithMultiKill { get; set; }
        public KeyPair roundsWithOpeningKill { get; set; }
        public KeyPair roundsWithOpeningDeath { get; set; }
        public KeyPair roundsWithKOST { get; set; }
        public KeyPair roundsSurvived { get; set; }
        public KeyPair roundsWithAnAce { get; set; }
        public KeyPair roundsWithClutch { get; set; }
        public double? timeAlivePerMatch { get; set; }
        public int? timeDeadPerMatch { get; set; }
        public double? distancePerRound { get; set; }
    }
    public class KeyPair{
        double? value { get; set; }
        double? p { get; set; }
    }

    public class ScrimParticipationResult
    {
        public int? user_id { get; set; }
        public int? response_id { get; set; }
        public int? response_typ { get; set; }
        public int? typ { get; set; }
        public string time_start { get; set; }
    }
    public class PlayerScrimParticipationPercentage
    {
        public int user_id { get; set; }
        public int total_scrims { get; set; }
        public double type0ratio { get; set; }
        public double type1ratio { get; set; }
        public double type2ratio { get; set; }
        public int type0count{ get; set; }
        public int type1count{ get; set; }
        public int type2count{ get; set; }

        public PlayerScrimParticipationPercentage(int user_id, int total_scrims, double type0ratio, double type1ratio, double type2ratio, int type0count, int type1count, int type2count)
        {
            this.user_id = user_id;
            this.total_scrims = total_scrims;
            this.type0ratio = type0ratio;
            this.type1ratio = type1ratio;
            this.type2ratio = type2ratio;
            this.type0count = type0count;
            this.type1count = type1count;
            this.type2count = type2count;
        }
    }


}
