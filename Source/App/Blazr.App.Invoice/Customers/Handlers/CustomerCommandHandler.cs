/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony.Core;
using Blazr.Antimony.Infrastructure.Server;
using Blazr.App.Invoice.Core;
using Blazr.Gallium;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Blazr.App.Invoice.Infrastructure;

/// <summary>
/// Mediatr Handler for executing commands against a Customer Entity
/// </summary>
public sealed record CustomerCommandHandler : IRequestHandler<CustomerCommandRequest, Result<CustomerId>>
{
    private readonly IMessageBus _messageBus;
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public CustomerCommandHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory, IMessageBus messageBus)
    {
        _factory = factory;
        _messageBus = messageBus;
    }

    public async Task<Result<CustomerId>> Handle(CustomerCommandRequest request, CancellationToken cancellationToken)
    {
        var dbContext = _factory.CreateDbContext();

        var result = await dbContext.ExecuteCommandAsync<DboCustomer>(new CommandRequest<DboCustomer>(
            Item: DboCustomerMap.Map(request.Item),
            State: request.State
        ), cancellationToken);

        if (!result.HasSucceeded(out DboCustomer? record))
            return result.ConvertFail<CustomerId>();

        _messageBus.Publish<DmoCustomer>(DboCustomerMap.Map(record));

        return Result<CustomerId>.Success(new CustomerId(record.CustomerID));
    }
}
