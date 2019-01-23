using System.Collections.ObjectModel;


namespace ErikTheCoder.AspNetCore.Middleware
{
    public class AuthenticationIdentities : KeyedCollection<string, AuthenticationIdentity>
    {
        protected override string GetKeyForItem(AuthenticationIdentity Item) => Item.Token;
    }
}
