using System;
using System.Linq;
using xstrat.Core;

namespace xstrat.Models.Supabase
{
    public class Team
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string AdminUserID { get; set; }

        public string GameID { get; set; }

        public int Deleted { get; set; }

        public string Webhook { get; set; }

        public int NotifyCreated { get; set; }

        public int NotifyChanged { get; set; }

        public int NotifyWeekly { get; set; }

        public int NotifySoon { get; set; }

        public int NotifyDelay { get; set; }

        public int UseOnDays { get; set; }
        
        public DateTime? CreatedAt { get; set; }

        public string Password { get; set; }

        public string GameName { get {  return DataCache.CurrentGames.Where(x => x.Id == GameID).FirstOrDefault()?.Name ?? Id; } }
        public string AdminName { get {  return DataCache.CurrentTeamMates.Where(x => x.Id == AdminUserID).FirstOrDefault()?.Name ?? Id; } }
    }
}
