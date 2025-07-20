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
}
