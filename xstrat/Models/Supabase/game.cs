using System;

namespace xstrat.Models.Supabase
{
    public class Game 
    {
        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string Name { get; set; }
    }
}
