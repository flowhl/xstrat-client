using System;

namespace xstrat.Models.Supabase
{
    class Queue
    {
        public string VideoID { get; set; }

        public int Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Result { get; set; }

        public string? UserID { get; set; }
    }
}
