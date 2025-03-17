/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

/// <summary>
/// Abstract template class for building Record Edit Contexts
/// </summary>
/// <typeparam name="TRecord"></typeparam>
/// <typeparam name="TKey"></typeparam>
public abstract class BaseRecordEditContext<TRecord, TKey>
    where TRecord : class, new()
{
    public TRecord BaseRecord { get; protected set; } 
        = new();

    public abstract TRecord AsRecord { get; }

    public BaseRecordEditContext()
    {
        this.Load(this.BaseRecord);
    }

    public BaseRecordEditContext(TRecord record)
    {
        this.Load(record);
    }

    public bool IsDirty 
        => this.BaseRecord != this.AsRecord;

    public abstract IDataResult Load(TRecord record);

    public void Reset()
    {
        var record = this.BaseRecord;
        this.BaseRecord = new();
        this.Load(record);
    }

    public void SetAsPersisted()
    {
        var record = this.AsRecord;
        this.BaseRecord = new();
        this.Load(record);
    }
}
