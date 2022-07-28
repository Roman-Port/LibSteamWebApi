using System;
using System.Collections.Generic;
using System.Text;

namespace LibSteamWebApi.Exceptions
{
    public class SteamRequestErrorException : Exception
    {
        public SteamRequestErrorException(Exception innerException) : base("Couldn't download data from Steam API.", innerException)
        {

        }
    }
}
