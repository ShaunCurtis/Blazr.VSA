﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode.Mediator;

namespace Blazr.App.Core;

public readonly record struct CustomerCommandRequest(
        StateRecord<DmoCustomer> Item)
    : IRequest<Result<CustomerId>>;
