
using System;

namespace xstrat.Models.Supabase
{
    public class Position
    {
        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string Name { get; set; }

        public string MapId { get; set; }
    }
}
