/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public abstract record RecordState
{
    private RecordState() { }

    public sealed record New() : RecordState;
    public sealed record Dirty() : RecordState;
    public sealed record Clean() : RecordState;
    public sealed record Deleted() : RecordState;

    public static RecordState NewState => new RecordState.New();
    public static RecordState DirtyState => new RecordState.Dirty();
    public static RecordState CleanState => new RecordState.Clean();
    public static RecordState DeletedState => new RecordState.Deleted();

}
