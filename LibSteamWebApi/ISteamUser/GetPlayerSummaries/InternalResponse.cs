using System;
using System.Collections.Generic;
using System.Text;

namespace LibSteamWebApi.ISteamUser.GetPlayerSummaries
{
    class InternalResponseContainer
    {
        public InternalResponse[] players;
    }

    class InternalResponse
    {
        public string steamid;
        public int communityvisibilitystate;
        public int profilestate;
        public string personaname;
        public int commentpermission;
        public string profileurl;
        public string avatar;
        public string avatarmedium;
        public string avatarfull;
        public string avatarhash;
        public long lastlogoff;
        public int personastate;
        public string realname;
        public string primaryclanid;
        public long? timecreated;
        public int personastateflags;
        public string loccountrycode;
        public string locstatecode;
        public int? loccityid;
    }
}
