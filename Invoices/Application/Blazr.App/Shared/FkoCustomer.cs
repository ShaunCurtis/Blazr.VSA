/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public record FkoCustomer(CustomerId Id, Title Name)
{
    public static FkoCustomer Default = new(CustomerId.Default, Title.Default);
}

public readonly record struct CustomerFKRequest() : IRequest<Bool<IEnumerable<FkoCustomer>>>;
