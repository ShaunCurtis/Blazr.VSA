/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public readonly record struct CustomerName
{
    public string Value { get; private init; }
    public bool IsValid { get; private init; }

    public CustomerName( string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            this.Value = string.Empty;
            this.IsValid = false;
            return;
        }

        if (name.length > 100)
        {
            this.Value = string.Empty;
            this.IsValid = false;
            return;
        }

        this.Value = name.Trim();
            this.IsValid = true;
    }

    public override string ToString()
    {
        return this.IsValid ? Value.ToString() : "No Valid Name";
    }
}
