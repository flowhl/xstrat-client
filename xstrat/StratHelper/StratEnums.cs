using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xstrat.StratHelper
{
    public static class StratFileHelper
    {
        //map_id, floor nr, file_name
        public static List<Tuple<int, int, string>> MapFiles { get; private set; } = new List<Tuple<int, int, string>>();

        public static string GetImagePathForSpot(int map, int floor)
        {
            string res = null;
            res = MapFiles.Where(x => x.Item1 == map && x.Item2 == floor).FirstOrDefault()?.Item3;
            return res;
        }

        internal static void Initialize()
        {
            MapFiles.Add(new Tuple<int, int, string>(6, 1, "coastline_top"));
        }
    }
}
