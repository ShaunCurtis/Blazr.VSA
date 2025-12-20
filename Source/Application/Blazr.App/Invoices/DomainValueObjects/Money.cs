/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Globalization;

namespace Blazr.App.Core;

public readonly record struct Money
{
    public decimal Value { get; private init; }
    public bool HasValue { get; private init; }

    public bool IsZero => Value != 0;

    public static Money Default => new(0);

    public Money(decimal value)
    {
        if (value >= 0)
        {
            Value = value;
            HasValue = true;
            return;
        }
        Value = 0;
        HasValue = false;
    }

    public override string ToString()
    {
        if (this.HasValue)
            return Value.ToString("C", CultureInfo.CreateSpecificCulture("en-GB"));

        return "Not Set";
    }

    public static Money operator +(Money one, Money two)
        => new Money(one.Value + two.Value);

    public static Money operator -(Money one, Money two)
        => new Money(one.Value - two.Value);

    public static Money operator *(Money one, Money two)
        => new Money(one.Value * two.Value);

    public static Money operator /(Money one, Money two)
        => new Money(Decimal.Round(one.Value / two.Value, 2, MidpointRounding.AwayFromZero));
}
