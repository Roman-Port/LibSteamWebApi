using System;
using System.Collections.Generic;
using System.Text;

namespace LibSteamWebApi.ISteamUser.GetPlayerSummaries
{
    public class PlayerSummaryLocation
    {
        internal PlayerSummaryLocation(InternalResponse data)
        {
            this.data = data;
        }

        private InternalResponse data;

        public string CountryCode => PlayerSummary.HelperEnsureAvailable(data.loccountrycode, "CountryCode");
        public string StateCode => PlayerSummary.HelperEnsureAvailable(data.locstatecode, "StateCode");
        public int LoccityId => PlayerSummary.HelperEnsureAvailable(data.loccityid, "LoccityId").Value;
    }
}
