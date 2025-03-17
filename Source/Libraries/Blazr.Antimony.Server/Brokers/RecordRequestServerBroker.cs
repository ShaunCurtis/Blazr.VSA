/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Infrastructure.Server;

public sealed class RecordRequestServerBroker<TDbContext>
    : IRecordRequestBroker
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public RecordRequestServerBroker(IDbContextFactory<TDbContext> factory)
    {
        _factory = factory;
    }

    public async ValueTask<Result<TRecord>> ExecuteAsync<TRecord>(RecordQueryRequest<TRecord> request)
        where TRecord : class
    {
        return await this.GetItemAsync<TRecord>(request);
    }

    private async ValueTask<Result<TRecord>> GetItemAsync<TRecord>(RecordQueryRequest<TRecord> request)
        where TRecord : class
    {
        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        var record = await dbContext.Set<TRecord>()
            .FirstOrDefaultAsync(request.FindExpression)
            .ConfigureAwait(false);

        if (record is null)
            return Result<TRecord>.Fail(new RecordQueryException($"No record retrieved with the Key provided"));

        return Result<TRecord>.Success(record);
    }
}
