/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

/// <summary>
/// Struct to represent the state of an object
/// and therefore the data store command type to apply.
/// 0 = None - no Change to an existing record
/// 1 = Add - a new record
/// 2 = Update - an existing record that has been mutated i.e. Dirty
/// -1 = Delete - the record should be deleted
/// </summary>
public readonly record struct EditState
{
    public const int StateCleanIndex = 0;
    public const int StateNewIndex = 1;
    public const int StateDirtyIndex = 2;
    public const int StateDeletedIndex = -1;

    public int Index { get; private init; } = 0;
    public string Value { get; private init; } = "None";

    public EditState() { }

    private EditState(int index, string value)
    {
        Index = index;
        Value = value;
    }

    public EditState AsDirty => this.Index == StateCleanIndex ? EditState.Dirty : this; 

    public override string ToString()
    {
        return this.Value;
    }

    public static EditState Clean = new EditState(StateCleanIndex, "Clean");
    public static EditState New = new EditState(StateNewIndex, "New");
    public static EditState Dirty = new EditState(StateDirtyIndex, "Dirty");
    public static EditState Deleted = new EditState(StateDeletedIndex, "Deleted");

    public static EditState GetState(int index)
        => (index) switch
        {
            StateNewIndex => EditState.New,
            StateDirtyIndex => EditState.Dirty,
            StateDeletedIndex => EditState.Deleted,
            _ => EditState.Clean,
        };
}
