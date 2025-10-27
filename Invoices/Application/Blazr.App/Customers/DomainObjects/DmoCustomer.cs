﻿/// ===========================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed record DmoCustomer : ICommandEntity
{
    public CustomerId Id { get; init; }
    public Title Name { get; init; }

    public DmoCustomer WithName(string name)
        => this with { Name = new(name)};

    public static DmoCustomer CreateNewEntity()
        => new DmoCustomer() { Id = CustomerId.Create() };
}
