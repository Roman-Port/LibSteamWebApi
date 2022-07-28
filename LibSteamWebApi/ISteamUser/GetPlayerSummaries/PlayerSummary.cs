using LibSteamWebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibSteamWebApi.ISteamUser.GetPlayerSummaries
{
    public class PlayerSummary
    {
        internal PlayerSummary(InternalResponse data)
        {
            this.data = data;
        }

        private InternalResponse data;

        public long SteamId => long.Parse(data.steamid);
        public bool IsProfilePublic => data.communityvisibilitystate == 3;
        public bool HasCommunityProfile => data.profilestate == 1;
        public string DisplayName => data.personaname;
        public PlayerSummaryIcon Avatar => new PlayerSummaryIcon(data);
        public DateTime LastLogoff => SteamWebClient.TimestampToDateTime(data.lastlogoff);
        public PlayerPersonaState Status => (PlayerPersonaState)data.personastate;
        public string RealName => HelperEnsureAvailable(data.realname, "RealName");
        public long PrimaryClanId => long.Parse(HelperEnsureAvailable(data.primaryclanid, "PrimaryClanId"));
        public DateTime TimeCreated => SteamWebClient.TimestampToDateTime(HelperEnsureAvailable(data.timecreated, "TimeCreated").Value);
        public PlayerSummaryLocation Location => new PlayerSummaryLocation(data);

        internal static T HelperEnsureAvailable<T>(T data, string name)
        {
            if (data == null)
                throw new SteamFieldPrivateException(name);
            return data;
        }
    }
}
