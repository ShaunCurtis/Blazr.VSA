/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode.Infrastructure.EntityFramework;

/// <summary>
/// This static class implements CQS Handlers
/// against an EF `TDbContext`
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public static class CQSEFBroker<TDbContext>
    where TDbContext : DbContext
{
    public static async Task<Result<TRecord>> ExecuteCommandAsync<TRecord>(TDbContext dbContext, CommandRequest<TRecord> request, CancellationToken cancellationToken = new())
        where TRecord : class
    {
        if ((request.Item is not ICommandEntity))
            return Result<TRecord>.Failure($"{request.Item.GetType().Name} Does not implement ICommandEntity and therefore you can't Update/Add/Delete it directly.");

        var stateRecord = request.Item;
        var result = 0;
        switch (request.State.Index)
        {
            case EditState.StateNewIndex:
                dbContext.Add<TRecord>(request.Item);
                result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

                return result == 1
                    ? Result<TRecord>.Success(request.Item)
                    : Result<TRecord>.Failure("Error adding Record");

            case EditState.StateDeletedIndex:
                dbContext.Remove<TRecord>(request.Item);
                result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

                return result == 1
                    ? Result<TRecord>.Success(request.Item)
                    : Result<TRecord>.Failure("Error deleting Record");

            case EditState.StateDirtyIndex:
                dbContext.Update<TRecord>(request.Item);
                result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.None);

                return result == 1
                    ? Result<TRecord>.Success(request.Item)
                    : Result<TRecord>.Failure("Error saving Record");

            default:
                return Result<TRecord>.Failure("Nothing executed.  Unrecognised State.");
        }
    }

    public static async Task<Result<ListItemsProvider<TRecord>>> GetItemsAsync<TRecord>(TDbContext dbContext, ListQueryRequest<TRecord> request)
        where TRecord : class
    {
        int totalRecordCount;

        // Turn off tracking.  We're only querying, no changes
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        // Get the IQueryable DbSet for TRecord
        IQueryable<TRecord> query = dbContext.Set<TRecord>();

        // If we have a filter defined, apply the predicate delegate to the IQueryable instance
        if (request.FilterExpression is not null)
            query = query.Where(request.FilterExpression).AsQueryable();

        // Get the total record count after applying the filters
        totalRecordCount = query is IAsyncEnumerable<TRecord>
            ? await query.CountAsync(request.Cancellation).ConfigureAwait(ConfigureAwaitOptions.None)
            : query.Count();

        // If we have a sorter, apply the sorter to the IQueryable instance
        if (request.SortExpression is not null)
        {
            query = request.SortDescending
                ? query.OrderByDescending(request.SortExpression)
                : query.OrderBy(request.SortExpression);
        }

        // Apply paging to the filtered and sorted IQueryable
        if (request.PageSize > 0)
            query = query
                .Skip(request.StartIndex)
                .Take(request.PageSize);

        // Finally materialize the list from the data source
        var list = query is IAsyncEnumerable<TRecord>
            ? await query.ToListAsync().ConfigureAwait(ConfigureAwaitOptions.None)
            : query.ToList();

        return Result<ListItemsProvider<TRecord>>.Success(new ListItemsProvider<TRecord>(list, totalRecordCount));
    }

    public static async Task<Result<TRecord>> GetRecordAsync<TRecord>(TDbContext dbContext, RecordQueryRequest<TRecord> request)
        where TRecord : class
    {
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        var record = await dbContext.Set<TRecord>()
            .FirstOrDefaultAsync(request.FindExpression)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (record is null)
            return Result<TRecord>.Failure($"No record retrieved with the Key provided");

        return Result<TRecord>.Success(record);
    }
}
