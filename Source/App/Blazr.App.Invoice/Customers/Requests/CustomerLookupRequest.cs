/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Core;
using Blazr.App.Core;
using MediatR;


namespace Blazr.App.Invoice.Core;

public readonly record struct CustomerLookupRequest() : IRequest<Result<ListItemsProvider<CustomerLookUpItem>>>;
