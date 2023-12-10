using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xstrat.Models.Supabase
{
    public class StatsHelper
    {
    }
    public enum Gamemode
    {
        Bomb,
        Hostage,
        SecureArea,
        Unknown
    }
    public enum Gametype
    {
        Ranked,
        Unranked,
        Causal,
        Arcade,
        Custom,
        Unknown
    }
    public enum ActivityType
    {
        Kill,
        FriendlyFireOff,
        FriendlyFireOn,
        LocateObjective,
        OperatorSwap,
        ReverseFriendlyFireOff,
        ReverseFriendlyFireOn,
        SurrenderAccepted,
        SurrenderDenied,
        DefuserPlantStart,
        DefuserPlantComplete,
        DefuserDisableStart,
        DefuserDisableComplete,
    }
}
