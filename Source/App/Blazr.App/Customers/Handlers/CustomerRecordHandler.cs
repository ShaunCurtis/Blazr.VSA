/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure.Server;

/// <summary>
/// Mediatr Handler for executing record requests to get a Customer Entity
/// </summary>
public sealed record CustomerRecordHandler : IRequestHandler<CustomerRecordRequest, Result<DmoCustomer>>
{
    private IRecordRequestBroker _broker;

    public CustomerRecordHandler(IRecordRequestBroker broker)
    {
        _broker = broker;
    }

    public async Task<Result<DmoCustomer>> Handle(CustomerRecordRequest request, CancellationToken cancellationToken)
    {
        Expression<Func<DboCustomer, bool>> findExpression = (item) =>
            item.CustomerID == request.Id.Value;

        var query = new RecordQueryRequest<DboCustomer>(findExpression);

        var result = await _broker.ExecuteAsync<DboCustomer>(query);

        if (!result.HasSucceeded(out DboCustomer? record))
            return result.ConvertFail<DmoCustomer>();

        var returnItem = DboCustomerMap.Map(record);

        return Result<DmoCustomer>.Success(returnItem);
    }
}
