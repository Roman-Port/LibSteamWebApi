using System;
using System.Collections.Generic;
using System.Text;

namespace LibSteamWebApi.Exceptions
{
    public class SteamNotRequestedException : Exception
    {
        public SteamNotRequestedException() : base("The requested item wasn't sent with this request.")
        {

        }
    }
}
