/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Infrastructure.Server;

public static class DbContextExtensions
{
    public static async ValueTask<Result<TRecord>> ExecuteCommandAsync<TRecord>(this DbContext dbContext, CommandRequest<TRecord> request, CancellationToken cancellationToken = new())
    where TRecord : class
    {
        return await DbBroker<DbContext>.ExecuteCommandAsync(dbContext, request, cancellationToken);
    }

    public static async ValueTask<Result<ListItemsProvider<TRecord>>> GetItemsAsync<TRecord>(this DbContext dbContext, ListQueryRequest<TRecord> request)
    where TRecord : class
    {
        return await DbBroker<DbContext>.GetItemsAsync(dbContext, request);
    }

    public static async ValueTask<Result<TRecord>> GetRecordAsync<TRecord>(this DbContext dbContext, RecordQueryRequest<TRecord> request)
        where TRecord : class
    {
        return await DbBroker<DbContext>.GetRecordAsync(dbContext, request);
    }

}

