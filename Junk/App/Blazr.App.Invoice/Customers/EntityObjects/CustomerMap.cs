/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Invoice.Core;

namespace Blazr.App.Invoice.Infrastructure;

public sealed class CustomerMap
{
    public static DmoCustomer Map(DvoCustomer item)
        => new()
        {
            Id = new(item.CustomerID),
            CustomerName = new(item.CustomerName),
        };

    public static DboCustomer Map(DmoCustomer item)
        => new()
        {
            CustomerID = item.Id.Value,
            CustomerName = item.CustomerName
        };
}
