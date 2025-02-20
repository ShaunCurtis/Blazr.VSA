/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Infrastructure.Server;

/// <summary>
/// This class implements the "standard" Server Command Handler
/// against an EF `TDbContext`
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public sealed class CommandServerBroker<TDbContext>
    : ICommandBroker
    where TDbContext : DbContext
{
    private readonly IDbContextFactory<TDbContext> _factory;

    public CommandServerBroker(IDbContextFactory<TDbContext> factory)
    {
        _factory = factory;
    }

    public async ValueTask<Result<TRecord>> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class
    {
        return await this.ExecuteCommandAsync<TRecord>(request);
    }

    private async ValueTask<Result<TRecord>> ExecuteCommandAsync<TRecord>(CommandRequest<TRecord> request)
    where TRecord : class
    {
        using var dbContext = _factory.CreateDbContext();

        if ((request.Item is not ICommandEntity))
            return Result<TRecord>.Fail(new CommandException($"{request.Item.GetType().Name} Does not implement ICommandEntity and therefore you can't Update/Add/Delete it directly."));

        var stateRecord = request.Item;

        // First check if it's new.
        if (request.State == CommandState.Add)
        {
            dbContext.Add<TRecord>(request.Item);
            var result = await dbContext.SaveChangesAsync(request.Cancellation).ConfigureAwait(ConfigureAwaitOptions.None);

            return result == 1
                ? Result<TRecord>.Success(request.Item)
                : Result<TRecord>.Fail( new CommandException("Error adding Record"));
        }

        // Check if we should delete it
        if (request.State == CommandState.Delete)
        {
            dbContext.Remove<TRecord>(request.Item);
            var result = await dbContext.SaveChangesAsync(request.Cancellation).ConfigureAwait(ConfigureAwaitOptions.None);
            
            return result == 1
                ? Result<TRecord>.Success(request.Item)
                : Result<TRecord>.Fail(new CommandException( "Error deleting Record"));
        }

        // Finally it changed
        if (request.State == CommandState.Update)
        {
            dbContext.Update<TRecord>(request.Item);
            var result = await dbContext.SaveChangesAsync(request.Cancellation).ConfigureAwait(ConfigureAwaitOptions.None);

            return result == 1
                ? Result<TRecord>.Success(request.Item)
                : Result<TRecord>.Fail(new CommandException("Error saving Record"));
        }

        return Result<TRecord>.Fail(new CommandException("Nothing executed.  Unrecognised State."));
    }
}
