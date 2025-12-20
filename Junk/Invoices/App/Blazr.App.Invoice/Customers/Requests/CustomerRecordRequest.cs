/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using Blazr.App.Core;
using Blazr.Antimony.Mediator;


namespace Blazr.App.Invoice.Core;

public readonly record struct CustomerRecordRequest(CustomerId Id) : IRequest<Result<DmoCustomer>>;
