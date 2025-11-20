/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode.Infrastructure.EntityFramework;

public static class DbContextExtensions
{
    public static async Task<Bool<TRecord>> ExecuteCommandAsync<TRecord>(this DbContext dbContext, CommandRequest<TRecord> request, CancellationToken cancellationToken = new())
        where TRecord : class
            => await CQSEFBroker<DbContext>.ExecuteCommandAsync(dbContext, request, cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

    public static async Task<Bool<ListItemsProvider<TRecord>>> GetItemsAsync<TRecord>(this DbContext dbContext, ListQueryRequest<TRecord> request)
        where TRecord : class
            => await CQSEFBroker<DbContext>.GetItemsAsync(dbContext, request).ConfigureAwait(ConfigureAwaitOptions.None);

    public static async Task<Bool<TRecord>> GetRecordAsync<TRecord>(this DbContext dbContext, RecordQueryRequest<TRecord> request)
        where TRecord : class
            => await CQSEFBroker<DbContext>.GetRecordAsync(dbContext, request).ConfigureAwait(ConfigureAwaitOptions.None);
}

