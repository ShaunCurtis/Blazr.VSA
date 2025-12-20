/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;

namespace Blazr.App.Weather.Core;

public readonly record struct IdentityId(Guid Value) : IEntityId
{
    public bool IsDefault => this == Default;
    public static IdentityId Default => new(Guid.Empty);

    public override string ToString()
    {
        return Value.ToString();
    }
}
