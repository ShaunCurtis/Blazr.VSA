/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Invoice.Core;

public sealed record DmoCustomer
{
    public CustomerId Id { get; init; } = CustomerId.Default;
    public string CustomerName { get; init; } = string.Empty;
}
