using System;

namespace xstrat.Models.Supabase
{
    public class Operator
    {
        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string Name { get; set; }

        public string GameId { get; set; }

        public int Typ { get; set; }
    }
}
