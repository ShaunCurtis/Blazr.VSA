/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Infrastructure;

namespace Blazr.App.Infrastructure;

public sealed class DboCustomerMap : IDboEntityMap<DboCustomer, DmoCustomer>
{
    public DmoCustomer MapTo(DboCustomer item)
        => Map(item);

    public DboCustomer MapTo(DmoCustomer item)
        => Map(item);

    public static DmoCustomer Map(DboCustomer item)
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
