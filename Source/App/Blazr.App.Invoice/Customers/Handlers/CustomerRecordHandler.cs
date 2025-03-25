/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Antimony;
using Blazr.Antimony.Infrastructure.EntityFramework;
using Blazr.App.Invoice.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Blazr.App.Invoice.Infrastructure;

/// <summary>
/// Mediatr Handler for executing record requests to get a Customer Entity
/// </summary>
public sealed class CustomerRecordHandler : IRequestHandler<CustomerRecordRequest, Result<DmoCustomer>>
{
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public CustomerRecordHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Result<DmoCustomer>> Handle(CustomerRecordRequest request, CancellationToken cancellationToken)
    {
        var dbContext = _factory.CreateDbContext();

        Expression<Func<DvoCustomer, bool>> findExpression = (item) =>
            item.CustomerID == request.Id.Value;

        var query = new RecordQueryRequest<DvoCustomer>(findExpression);

        var result = await dbContext.GetRecordAsync<DvoCustomer>(query);

        if (!result.HasSucceeded(out DvoCustomer? record))
            return result.ConvertFail<DmoCustomer>();

        var returnItem = CustomerMap.Map(record);

        return Result<DmoCustomer>.Success(returnItem);
    }
}
