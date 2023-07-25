using System;

namespace xstrat.Models.Supabase
{
    public class Map
    {
        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string Name { get; set; }

        public string GameId { get; set; }

        public string Floor0SVG { get; set; }

        public string Floor1SVG { get; set; }

        public string Floor2SVG { get; set; }

        public string Floor3SVG { get; set; }
    }
}
