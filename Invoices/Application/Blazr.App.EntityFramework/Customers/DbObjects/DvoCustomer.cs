/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.ComponentModel.DataAnnotations;

namespace Blazr.App.Infrastructure;

public sealed record DvoCustomer
{
    [Key] public Guid CustomerID { get; init; } = Guid.Empty;
    public string? CustomerName { get; set; }

    public static DmoCustomer Map(DvoCustomer item)
        => new()
        {
            Id = new(item.CustomerID),
            Name = new (item.CustomerName ?? string.Empty)
        };

    public static Bool<DmoCustomer> MapToBool(DvoCustomer item)
        => Bool<DmoCustomer>.Input(Map(item));
}
