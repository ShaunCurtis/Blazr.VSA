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
    public const int StateNone = 0;
    public const int StateAdd = 1;
    public const int StateUpdate = 2;
    public const int StateDelete = -1;

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

    public static CommandState None = new CommandState(StateNone, "None");
    public static CommandState Add = new CommandState(StateAdd, "Add");
    public static CommandState Update = new CommandState(StateUpdate, "Update");
    public static CommandState Delete = new CommandState(StateDelete, "Delete");

    public static CommandState GetState(int index)
        => (index) switch
        {
            StateAdd => CommandState.Add,
            StateUpdate => CommandState.Update,
            StateDelete => CommandState.Delete,
            _ => CommandState.None,
        };
}
