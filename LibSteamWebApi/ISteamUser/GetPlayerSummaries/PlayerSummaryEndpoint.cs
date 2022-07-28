using LibSteamWebApi.Exceptions;
using LibSteamWebApi.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSteamWebApi.ISteamUser.GetPlayerSummaries
{
    public class PlayerSummaryEndpoint : BaseRequestEndpoint<long, PlayerSummary>
    {
        internal PlayerSummaryEndpoint(SteamWebClient client)
        {
            this.client = client;
        }

        private readonly SteamWebClient client;

        private const string INTERFACE_NAME = "ISteamUser";
        private const string METHOD_NAME = "GetPlayerSummaries";
        private const int METHOD_VERSION = 2;
        private const int MAX_BLOCK_SIZE = 99;

        public override BlockRequest<long, PlayerSummary> CreateRequestBuilder()
        {
            return new PlayerSummaryRequest(this);
        }

        private async Task<IList<PlayerSummary>> InternalFetchBlocks(long[] steamId64s, int offset, int count)
        {
            List<PlayerSummary> result = new List<PlayerSummary>();
            int internalOffset = 0;
            while (internalOffset < count)
            {
                //Determine how much is readable
                int readable = Math.Min(MAX_BLOCK_SIZE, count - internalOffset);

                //Read this block
                result.AddRange(await InternalFetchSingleBlock(steamId64s, offset + internalOffset, readable));

                //Advance
                internalOffset += readable;
            }
            return result;
        }

        private async Task<IList<PlayerSummary>> InternalFetchSingleBlock(long[] steamId64s, int offset, int count)
        {
            //Build request
            string request = "";
            for (int i = 0; i < count; i++)
            {
                if (i != 0)
                    request += ",";
                request += steamId64s[offset + i].ToString();
            }

            //Do initial web request
            InternalResponseContainer responseContainer = await client.SteamRequestGet<InternalResponseContainer>(INTERFACE_NAME, METHOD_NAME, METHOD_VERSION,
                new Dictionary<string, string>() {
                    { "steamids", request }
                }
            );

            //Wrap all
            PlayerSummary[] summaries = new PlayerSummary[responseContainer.players.Length];
            for (int i = 0; i < summaries.Length; i++)
                summaries[i] = new PlayerSummary(responseContainer.players[i]);

            return summaries;
        }

        public override long GetKeyFromValue(PlayerSummary value)
        {
            return value.SteamId;
        }

        class PlayerSummaryRequest : BlockRequest<long, PlayerSummary>
        {
            internal PlayerSummaryRequest(PlayerSummaryEndpoint endpoint)
            {
                this.endpoint = endpoint;
            }

            private readonly PlayerSummaryEndpoint endpoint;

            protected override long GetKeyFromValue(PlayerSummary value)
            {
                return value.SteamId;
            }

            protected override Task<IList<PlayerSummary>> FetchResults(ICollection<long> value)
            {
                return endpoint.InternalFetchBlocks(value.ToArray(), 0, value.Count);
            }
        }
    }
}
