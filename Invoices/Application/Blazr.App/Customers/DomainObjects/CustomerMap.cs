/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;

namespace Blazr.App.Infrastructure;

public sealed class CustomerMap
{
    public static DmoCustomer Map(DvoCustomer item)
        => new()
        {
            Id = new(item.CustomerID),
            Name = new(item.CustomerName ?? string.Empty)
        };

    public static DboCustomer Map(DmoCustomer item)
        => new()
        {
            CustomerID = item.Id.ValidatedId.Value,
            CustomerName = item.Name.Value
        };

    public static Result<DmoCustomer> ApplyTransform(DvoCustomer item)
        => Result<DmoCustomer>.Create(Map(item));

    public static Result<DboCustomer> MapResult(DmoCustomer item)
        => Result<DboCustomer>.Create(Map(item));
}
