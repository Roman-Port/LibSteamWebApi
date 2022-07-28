using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;
using LibSteamWebApi.ISteamUser.GetPlayerSummaries;
using LibSteamWebApi.Exceptions;

namespace LibSteamWebApi
{
    public class SteamWebClient
    {
        public SteamWebClient(string apiKey)
        {
            this.apiKey = apiKey;
        }

        private readonly string apiKey;
        private readonly HttpClient client = new HttpClient();

        private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public PlayerSummaryEndpoint EndpointPlayerSummary => new PlayerSummaryEndpoint(this);

        public bool UseSSL { get; set; } = true;
        public string ApiRoot { get; set; } = "api.steampowered.com";
        private string Protocol => UseSSL ? "https" : "http";

        internal async Task<T> SteamRequestGet<T>(string steamInterface, string steamMethod, int apiVersion, Dictionary<string, string> parameters)
        {
            //Build URL
            string url = $"{Protocol}://{ApiRoot}/{steamInterface}/{steamMethod}/v{apiVersion.ToString("D4")}/?key={HttpUtility.UrlEncode(apiKey)}";
            foreach (var p in parameters)
                url += $"&{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(p.Value)}";

            //Send
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            //Get as string and decode as JSON
            string responseString = await response.Content.ReadAsStringAsync();
            SteamResponse<T> responseData = JsonConvert.DeserializeObject<SteamResponse<T>>(responseString);

            return responseData.response;
        }

        internal static DateTime TimestampToDateTime(long timestamp)
        {
            return EPOCH.AddSeconds(timestamp);
        }

        public static bool TrySteamIdToSteamId64(string id, out ulong result)
        {
            //Sanity check
            if (!id.StartsWith("STEAM_"))
            {
                result = 0;
                return false;
            }

            //Split
            string[] parts = id.Substring("STEAM_".Length).Split(':');

            //Parse components
            long z;
            long y;
            long x;
            if (parts.Length != 3 || !long.TryParse(parts[0], out x) || !long.TryParse(parts[1], out y) || !long.TryParse(parts[2], out z))
                throw new SteamIdMalformedException();

            //Create
            result = (ulong)(76561197960265728 + (z * 2) + y);
            return true;
        }
        
        public static long SteamIdToSteamId64(string id)
        {
            if (TrySteamIdToSteamId64(id, out ulong result))
                return (long)result;
            throw new SteamIdMalformedException();
        }

        class SteamResponse<T>
        {
            public T response;
        }
    }
}
