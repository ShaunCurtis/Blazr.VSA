﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.Core;

public readonly record struct UserRecordRequest(IdentityId Id) : IRequest<Result<DmoUser>>;
