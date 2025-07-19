/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public interface ICommandBroker
{
    public ValueTask<Result<TRecord>> ExecuteAsync<TRecord>(CommandRequest<TRecord> request, CancellationToken cancellationToken)
        where TRecord : class;
}

public interface ICommandBroker<TRecord>
{
    public ValueTask<Result<TRecord>> ExecuteAsync(CommandRequest<TRecord> request, CancellationToken cancellationToken);
}
