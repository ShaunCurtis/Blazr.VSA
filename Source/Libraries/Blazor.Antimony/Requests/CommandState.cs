/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

/// <summary>
/// Struct to represent the state of an object
/// and therefore the data store command type to apply.
/// 0 = None - no Change to an existing record
/// 1 = Add - a new record
/// 2 = Update - an existing record that has been mutated i.e. Dirty
/// -1 = Delete - the record should be deleted
/// </summary>
public readonly record struct CommandState
{
    public int Index { get; private init; } = 0;
    public string Value { get; private init; } = "None";

    public CommandState() { }

    private CommandState(int index, string value)
    {
        Index = index;
        Value = value;
    }

    public override string ToString()
    {
        return this.Value;
    }

    public CommandState AsDirty
        => this.Index == 0 ? CommandState.Update : this;

    public static CommandState None = new CommandState(0, "None");
    public static CommandState Add = new CommandState(1, "Add");
    public static CommandState Update = new CommandState(2, "Update");
    public static CommandState Delete = new CommandState(-1, "Delete");

    public static CommandState GetState(int index)
        => (index) switch
        {
            1 => CommandState.Add,
            2 => CommandState.Update,
            -1 => CommandState.Delete,
            _ => CommandState.None,
        };
}
