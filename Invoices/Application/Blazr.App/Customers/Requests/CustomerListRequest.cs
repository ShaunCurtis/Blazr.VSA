/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public record CustomerListRequest
    : BaseListRequest, IRequest<Result<ListItemsProvider<DmoCustomer>>>
{ }
