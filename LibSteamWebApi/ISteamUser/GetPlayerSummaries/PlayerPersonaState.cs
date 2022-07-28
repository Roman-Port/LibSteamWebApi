using System;
using System.Collections.Generic;
using System.Text;

namespace LibSteamWebApi.ISteamUser.GetPlayerSummaries
{
    public enum PlayerPersonaState
    {
        Offline = 0,
        Online = 1,
        Busy = 2,
        Away = 3,
        Snooze = 4,
        LookingToTrade = 5,
        LookingToPlay = 6
    }
}
