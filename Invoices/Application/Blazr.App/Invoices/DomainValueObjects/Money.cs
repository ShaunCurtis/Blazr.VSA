/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public readonly record struct Money(decimal Amount) : IEquatable<Money>
{
    public bool IsDefault => this == Default;
    public static Money Default => new(decimal.MinValue);

    public override string ToString()
        => $"£{Amount}";
}
