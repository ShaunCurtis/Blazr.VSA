﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Security.Claims;

namespace Blazr.App.Auth.Core;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetIdentityId(this ClaimsPrincipal principal)
    {
        var claim = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
        if (claim is not null && Guid.TryParse(claim.Value, out Guid id))
            return id;

        return Guid.Empty;
    }

    public static Guid GetIdentityId(this ClaimsIdentity principal)
    {
        var claim = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Sid);
        if (claim is not null && Guid.TryParse(claim.Value, out Guid id))
            return id;

        return Guid.Empty;
    }

    public static string GetIdentityName(this ClaimsPrincipal principal)
    {
        var claim = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);
        return claim is not null ? claim.Value : string.Empty;
    }
}
