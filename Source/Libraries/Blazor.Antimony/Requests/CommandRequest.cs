/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public readonly record struct CommandRequest<TRecord>(
    TRecord Item, 
    CommandState State, 
    CancellationToken Cancellation
    );

public readonly record struct CommandAPIRequest<TRecord>
{
    public TRecord? Item { get; init; }
    public int CommandIndex { get; init; }

    public CommandAPIRequest() { }

    public static CommandAPIRequest<TRecord> FromRequest(CommandRequest<TRecord> command)
        => new()
        {
            Item = command.Item,
            CommandIndex = command.State.Index
        };

    public CommandRequest<TRecord> ToRequest(CancellationToken? cancellation = null)
        => new()
        {
            Item = this.Item ?? default!,
            State = CommandState.GetState(this.CommandIndex),
            Cancellation = cancellation ?? CancellationToken.None
        };
}

