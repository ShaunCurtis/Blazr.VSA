/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;

namespace Blazr.Cadmium.Presentation;

public interface ILookupUIBrokerFactory
{
    public ValueTask<ILookUpUIBroker<TLookupRecord>> GetAsync<TLookupRecord, TPresenter>()
            where TLookupRecord : class, ILookupItem, new()
            where TPresenter : class, ILookUpUIBroker<TLookupRecord>
;
}