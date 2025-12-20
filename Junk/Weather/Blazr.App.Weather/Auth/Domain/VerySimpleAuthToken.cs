/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Auth.Domain;

public record VerySimpleAuthToken
{
    public Guid Key { get; init; } = Guid.Empty;
}
