using System;
using System.Collections.Generic;
using System.Text;

namespace LibSteamWebApi.Exceptions
{
    public class SteamNotFoundException : Exception
    {
        public SteamNotFoundException() : base($"Steam returned no results.")
        {
        }
    }
}
