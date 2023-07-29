using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xstrat.Calendar;
using xstrat.Models.Supabase;

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
        public int Typ { get; set; }

        public List<Object> Args { get; set; }

        public bool Visible { get; set; } = true;
        public UserData User { get; set; }
        public Models.Supabase.CalendarEvent Scrim { get; set; }
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
        /// 5 - daily
        /// </summary>
        public OffDayType(int id, string name)
        {
            this.id = id;
            this.name = name;
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

    public class XMap
    {
        public string Name { get; set; }
        public int game_id { get; set; }
        public List<Floor> floors { get; set; } = new List<Floor>();
        public List<Models.Supabase.Position> positions { get; set; } = new List<Models.Supabase.Position>();

        public XMap(string name, int game_id, List<Floor> floors, List<Models.Supabase.Position> positions)
        {
            Name = name;
            this.game_id = game_id;
            this.floors = floors;
            this.positions = positions;
        }

        //TODO: Fix XMap
        public XMap()
        {
            //floors.Add(new Floor(0, "basement", @"https://xstrat.app/wp-content/uploads/2022/03/DZ-consulate-basement.png", 0, 0));
            //floors.Add(new Floor(1, "first floor", @"https://xstrat.app/wp-content/uploads/2022/03/DZ-consulate-groundfloor.png", 0, 1));
            //var stratlist = new List<Strat>();
            //stratlist.Add(new Strat());
            //positions.Add(new Models.Supabase.Position(0, 0, "1", stratlist));
            //positions.Add(new Models.Supabase.Position(1, 0, "2", stratlist));
            //Name = "Bank";
            //game_id = 0;
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
        public string xstrat_user_id { get; set; }
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
        public string xstrat_user_id { get; set; }
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
        public string user_id { get; set; }
        public string response_id { get; set; }
        public int? response_typ { get; set; }
        public int? typ { get; set; }
        public string time_start { get; set; }
    }
    public class PlayerScrimParticipationPercentage
    {
        public string user_id { get; set; }
        public int total_scrims { get; set; }
        public double type0ratio { get; set; }
        public double type1ratio { get; set; }
        public double type2ratio { get; set; }
        public int type0count{ get; set; }
        public int type1count{ get; set; }
        public int type2count{ get; set; }

        public PlayerScrimParticipationPercentage(string user_id, int total_scrims, double type0ratio, double type1ratio, double type2ratio, int type0count, int type1count, int type2count)
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
