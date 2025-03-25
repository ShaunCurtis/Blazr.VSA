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

namespace Blazr.App.Invoice.Infrastructure.Server;

/// <summary>
/// Mediator Handler to get an Invoice Entity - A DmoInvoice object
/// </summary>
public sealed class InvoiceRecordHandler : IRequestHandler<InvoiceRequests.InvoiceRecordRequest, Result<DmoInvoice>>
{
    private readonly IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public InvoiceRecordHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<Result<DmoInvoice>> Handle(InvoiceRequests.InvoiceRecordRequest request, CancellationToken cancellationToken)
    {
        var dbContext = _factory.CreateDbContext();

        Expression<Func<DvoInvoice, bool>> findExpression = (item) =>
            item.InvoiceID == request.Id.Value;

        var query = new RecordQueryRequest<DvoInvoice>(findExpression);

        var result = await dbContext.GetRecordAsync<DvoInvoice>(query);

        if (!result.HasSucceeded(out DvoInvoice? record))
            return result.ConvertFail<DmoInvoice>();

        var returnItem = InvoiceMap.Map(record);

        return Result<DmoInvoice>.Success(returnItem);
    }
}
