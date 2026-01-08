/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public readonly record struct Title
{
    public string Value { get; private init; }

    public static readonly string DefaultValue = "[NO TITLE SET]";

    public bool IsDefault => this.Value.Equals(DefaultValue);

    public Title(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            this.Value = DefaultValue;
            return;
        }

        if (name.Length > 256)
        {
            this.Value = string.Concat(name.Substring(254), "..") ;
            return;
        }

        this.Value = name.Trim();
    }

    public static Title Default => new(DefaultValue);

    public override string ToString()
        => Value.ToString();
}
