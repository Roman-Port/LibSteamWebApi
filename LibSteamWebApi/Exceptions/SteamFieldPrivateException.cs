using System;
using System.Collections.Generic;
using System.Text;

namespace LibSteamWebApi.Exceptions
{
    public class SteamFieldPrivateException : Exception
    {
        public SteamFieldPrivateException(string fieldName) : base($"Due to this person's privacy settings, \"{fieldName}\" is unavailable.")
        {

        }
    }
}
