/// ===========================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

/// <summary>
/// Customer Core Domain Object
/// </summary>
public sealed record DmoCustomer : ICommandEntity
{
    public CustomerId Id { get; init; }
    public Title Name { get; init; }

    public static DmoCustomer NewCustomer()
        => new DmoCustomer() { Id = CustomerId.NewId() };
}
