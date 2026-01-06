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
