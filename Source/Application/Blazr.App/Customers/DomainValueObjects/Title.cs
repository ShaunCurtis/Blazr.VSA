/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public readonly record struct Title
{
    public string Value { get; private init; }

    public static readonly string DefaultValue = "No Value Set";

    public bool IsDefault => this.Value.Equals(DefaultValue);

    public Title(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            this.Value = string.Empty;
            return;
        }

        if (name.Length > 100)
        {
            this.Value = string.Empty;
            return;
        }

        this.Value = name.Trim();
    }

    public static Title Default => new() { Value = DefaultValue};

    public override string ToString()
    {
        return Value.ToString();
    }
}
