/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

/// <summary>
/// This object should not be used in DI.
/// Create an instance through the Factory
/// </summary>
public sealed class CustomerLookupBrokerr : LookUpUIBroker<CustomerLookUpItem>
{
    public CustomerLookupBrokerr(IMediator dataBroker)
        : base(dataBroker) { }

    public async override ValueTask<Result> LoadAsync()
    {
        var result = await this.Mediator.Send(new CustomerLookupRequest());
        if (result.HasSucceeded(out ListResult<CustomerLookUpItem> listResult))
            this.Items = listResult.Items;

        return result.ToResult;
    }
}