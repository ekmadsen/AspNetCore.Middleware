using System;
using System.Collections.Generic;
using JetBrains.Annotations;


namespace ErikTheCoder.AspNetCore.Middleware
{
    public class AuthenticationIdentity
    {
        public string Token { get; [UsedImplicitly] set; }
        public string Username { get; [UsedImplicitly] set; }
        [UsedImplicitly] public List<string> Roles { get; [UsedImplicitly] set; }
        [UsedImplicitly] public Dictionary<string, HashSet<string>> Claims { get; [UsedImplicitly] set; }


        public AuthenticationIdentity()
        {
            Roles = new List<string>();
            Claims = new Dictionary<string, HashSet<string>>(StringComparer.CurrentCultureIgnoreCase);
        }
    }
}
