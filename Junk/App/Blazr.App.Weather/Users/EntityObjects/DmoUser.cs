/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.Core;

public sealed record DmoUser
{
    public IdentityId Id { get; init; } = IdentityId.Default;
    public string Name { get; init; } = "[Not Set]";
    public string Role { get; init; } = "VisitorRole";
}
