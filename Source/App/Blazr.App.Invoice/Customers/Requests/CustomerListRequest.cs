/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Core;
using Blazr.App.Core;
using MediatR;

namespace Blazr.App.Invoice.Core;

public record CustomerListRequest
    : BaseListRequest, IRequest<Result<ListItemsProvider<DmoCustomer>>>
{ }
