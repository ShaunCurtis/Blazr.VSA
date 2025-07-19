/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Cadmium.Core;

public interface IRecordEditContext<TRecord>
    where TRecord : class
{
    public TRecord BaseRecord { get; }
    public TRecord AsRecord { get; }
    public Result<TRecord> ToResult { get; }

    public bool IsDirty { get; }

    public Result Load(TRecord record);
    public void Reset();
    public void SetAsPersisted();
}
