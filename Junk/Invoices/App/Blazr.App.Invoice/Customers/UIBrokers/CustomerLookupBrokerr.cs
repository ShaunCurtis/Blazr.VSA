/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using Blazr.App.Invoice.Core;
using Blazr.App.Presentation;
using Blazr.Antimony.Mediator;

namespace Blazr.App.Invoice.Presentation;

/// <summary>
/// This object should not be used in DI.
/// Create an instance through the Factory
/// </summary>
public sealed class CustomerLookupBrokerr : LookUpUIBroker<CustomerLookUpItem>
{
    public CustomerLookupBrokerr(IMediatorBroker dataBroker)
        : base(dataBroker) { }

    public async override ValueTask<Result> LoadAsync()
    {
        var result = await this.Mediator.DispatchAsync(new CustomerLookupRequest());
        if (result.HasSucceeded(out ListItemsProvider<CustomerLookUpItem>? listResult))
            this.Items = listResult.Items;

        return result.ToResult;
    }
}