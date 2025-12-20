/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Weather.Core;

namespace Blazr.App.Weather.Infrastructure;

public sealed class UserMap
{
    public static DmoUser Map(DvoUser item)
        => new()
        {
            Id = new(item.UserID),
            Name = item.Name,
            Role = item.Role
        };
    public static DmoUser Map(DboUser item)
        => new()
        {
            Id = new(item.UserID),
            Name = item.Name,
            Role = item.Role
        };

    public static DboUser Map(DmoUser item)
        => new()
        {
            UserID = item.Id.Value,
            Name = item.Name,
            Role = item.Role
        };
}
