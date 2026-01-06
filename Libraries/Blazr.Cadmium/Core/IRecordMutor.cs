/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode;

namespace Blazr.Cadmium.Core;

public interface IRecordMutor<TRecord>
    where TRecord : class
{
    public TRecord BaseRecord { get; }
    public bool IsDirty { get; }
    public bool IsNew { get; }
    public TRecord Record { get; }
    public void Reset();
    public RecordState State { get; }
}

public abstract class RecordMutor<TRecord>
    where TRecord : class
{
    public TRecord BaseRecord { get; protected set; } = default!;
    public bool IsDirty => !this.Record.Equals(BaseRecord);
    public virtual bool IsNew { get; }
    public virtual TRecord Record { get; } = default!;

    public RecordState State => (this.IsNew, this.IsDirty) switch
    {
        (true, _) => RecordState.NewState,
        (false, false) =>RecordState.CleanState,
        (false, true) => RecordState.DirtyState,
    };
}
