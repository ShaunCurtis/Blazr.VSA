/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public readonly record struct Date
{
    public DateOnly Value { get; init; }

    public DateTime ToDateTime => this.Value.ToDateTime(TimeOnly.MinValue);

    public Date() 
    {
        this.Value = DateOnly.MinValue;
    }

    public Date(DateOnly date)
    {
        this.Value = date;
    }

    public Date(DateTime date)
    {
        this.Value = DateOnly.FromDateTime(date);
    }

    public Date(DateTimeOffset date)
    {
        this.Value = DateOnly.FromDateTime(date.DateTime);
    }

    public override string ToString()
    {
        return this.Value.ToString("dd-MMM-yy");
    }
}
