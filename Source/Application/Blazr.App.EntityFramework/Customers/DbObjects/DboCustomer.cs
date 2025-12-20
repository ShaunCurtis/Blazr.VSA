/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.ComponentModel.DataAnnotations;

namespace Blazr.App.Infrastructure;

public sealed record DboCustomer : ICommandEntity
{
    [Key] public Guid CustomerID { get; init; } = Guid.Empty;
    public string CustomerName { get; init; } = string.Empty;

    public static DboCustomer Map(DmoCustomer item)
        => new()
        {
            CustomerID = item.Id.Value,
            CustomerName = item.Name.Value
        };
}
