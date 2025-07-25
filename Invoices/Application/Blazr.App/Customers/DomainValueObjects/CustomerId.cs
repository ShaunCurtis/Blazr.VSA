﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public readonly record struct CustomerId(Guid Value) : IEntityId
{
    public bool IsDefault => this == Default;
    public static CustomerId Create => new(Guid.CreateVersion7());
    public static CustomerId Default => new(Guid.Empty);

    public CustomerId ValidatedId => this.IsDefault ? Create : this;

    public override string ToString()
    {
        return this.IsDefault ? "Default" : Value.ToString();
    }
}
