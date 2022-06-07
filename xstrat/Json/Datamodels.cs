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

    public class DiscordData
    {
        public string webhook { get; set; }
        public int sn_created { get; set; }
        public int sn_changed { get; set; }
        public int sn_weekly { get; set; }
        public int sn_soon { get; set; }
        public int sn_delay { get; set; }

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

        public Scrim(int id, string title, string comment, string time_start, string time_end, string opponent_name, int team_id, int? map_1_id, int? map_2_id, int? map_3_id, int typ, int creator_id, string creation_date)
        {
            this.id = id;
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
        public int strat_id { get; set; }
        public string name { get; set; }
        public int team_id { get; set; }
        public int game_id { get; set; }
        public int map_id { get; set; }
        public int position_id { get; set; }
        public int version { get; set; }
        public string content { get; set; }

        public Strat(int strat_id, string name, int team_id, int game_id, int map_id, int position_id, int version, string walls, string items)
        {
            this.strat_id = strat_id;
            this.name = name;
            this.team_id = team_id;
            this.game_id = game_id;
            this.map_id = map_id;
            this.position_id = position_id;
            this.version = version;
            this.content = walls;
        }
        public Strat()
        {
            this.strat_id = 0;
            this.name = "strat 1";
            this.team_id = 0;
            this.game_id = 0;
            this.map_id = 0;
            this.position_id = 0;
            this.version = 1;
            this.content = "";
        }
    }

    public class teamInfo
    {
        public string team_name { get; set; }
        public string admin_name { get; set; }
        public string game_name { get; set; }
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

}
