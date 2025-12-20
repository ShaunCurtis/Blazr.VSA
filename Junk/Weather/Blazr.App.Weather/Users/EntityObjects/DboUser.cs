/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.ComponentModel.DataAnnotations;

namespace Blazr.App.Weather.Infrastructure;

public sealed record DboUser : ICommandEntity
{
    [Key] public Guid UserID { get; init; } = Guid.Empty;
    public string Name { get; init; } = "[Not Set]";
    public string Role { get; init; } = "VisitorRole";
}
