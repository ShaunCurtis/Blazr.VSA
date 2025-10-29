﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.EntityFramework;

namespace Blazr.App.Infrastructure;

public record CustomerFKHandler : IRequestHandler<CustomerFKRequest, Result<IEnumerable<FkoCustomer>>>
{
    private IDbContextFactory<InMemoryInvoiceTestDbContext> _factory;

    public CustomerFKHandler(IDbContextFactory<InMemoryInvoiceTestDbContext> dbContextFactory)
    {
        _factory = dbContextFactory;
    }

    public async Task<Result<IEnumerable<FkoCustomer>>> HandleAsync(CustomerFKRequest request, CancellationToken cancellationToken)
    {
        using var dbContext = await _factory.CreateDbContextAsync();

        return await dbContext
            .GetItemsAsync<FkCustomer>(ListQueryRequest<FkCustomer>
                .Create(cancellationToken))
            .ExecuteTransformAsync(provider => Result<IEnumerable<FkoCustomer>>
                .Create(provider.Items.Select(item => item.Map)));
    }
}

public record FkCustomer
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public FkoCustomer Map
        => new(new(this.Id), new(this.Name));
}

