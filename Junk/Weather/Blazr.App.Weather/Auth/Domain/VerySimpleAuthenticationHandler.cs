/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Blazr.App.Auth.Core;

public class VerySimpleAuthenticationHandler : AuthenticationHandler<VerySimpleAuthSchemeOptions>
{
    public VerySimpleAuthenticationHandler(IOptionsMonitor<VerySimpleAuthSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check we have an Authorization Header
        if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
            return Task.FromResult(AuthenticateResult.Fail("Header not found."));

        Guid token = Guid.NewGuid();

        // Try and get the Guid Bearer token from the Header
        try
        {
            var tokenString = Request.Headers[HeaderNames.Authorization]
                .ToString()
                .Split(" ")[1];
            if (!Guid.TryParse(tokenString, out token))
                return Task.FromResult(AuthenticateResult.Fail("Token could not be deserialized."));
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Error deserializing Token."));
        }

        // Try and get the user associated with the Guid
        if (!TestIdentities.TryGetIdentity(token, out ClaimsIdentity claimsIdentity))
            return Task.FromResult(AuthenticateResult.Fail("No Identity found for the token provided."));

        // Create and return a AuthenticationTicket
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var ticket = new AuthenticationTicket(claimsPrincipal, this.Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
