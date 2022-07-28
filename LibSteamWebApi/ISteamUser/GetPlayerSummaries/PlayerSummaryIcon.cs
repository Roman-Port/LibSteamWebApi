using System;
using System.Collections.Generic;
using System.Text;

namespace LibSteamWebApi.ISteamUser.GetPlayerSummaries
{
    public class PlayerSummaryIcon
    {
        internal PlayerSummaryIcon(InternalResponse data)
        {
            this.data = data;
        }

        private InternalResponse data;

        public string UrlSmall => data.avatar;
        public string UrlMedium => data.avatarmedium == null ? UrlSmall : data.avatarmedium;
        public string UrlFull => data.avatarfull == null ? UrlMedium : data.avatarfull;
        public string Hash => data.avatarhash;
    }
}
