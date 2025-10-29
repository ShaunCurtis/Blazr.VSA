/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public record FkoCustomer(CustomerId Id, Title Name);

public readonly record struct CustomerFKRequest() : IRequest<Result<IEnumerable<FkoCustomer>>>;
