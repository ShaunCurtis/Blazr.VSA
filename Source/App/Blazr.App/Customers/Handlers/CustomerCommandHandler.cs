/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Gallium;

namespace Blazr.App.Infrastructure.Server;

/// <summary>
/// Mediatr Handler for executing commands against a Customer Entity
/// </summary>
public sealed record CustomerCommandHandler : IRequestHandler<CustomerCommandRequest, Result<CustomerId>>
{
    private ICommandBroker _broker;
    private IMessageBus _messageBus;

    public CustomerCommandHandler(ICommandBroker broker, IMessageBus messageBus)
    {
        _messageBus = messageBus;
        _broker = broker;
    }

    public async Task<Result<CustomerId>> Handle(CustomerCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await _broker.ExecuteAsync<DboCustomer>(new CommandRequest<DboCustomer>(
            Item: DboCustomerMap.Map(request.Item),
            State: request.State,
            Cancellation: cancellationToken
        ));

        if (!result.HasSucceeded(out DboCustomer? record))
            return result.ConvertFail<CustomerId>();

        _messageBus.Publish<DmoCustomer>(DboCustomerMap.Map(record));

        return Result<CustomerId>.Success(new CustomerId(record.CustomerID));
    }
}
