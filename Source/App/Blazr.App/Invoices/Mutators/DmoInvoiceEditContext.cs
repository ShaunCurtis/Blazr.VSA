/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed class DmoInvoiceEditContext : BaseRecordEditContext<DmoInvoice, InvoiceId>, IRecordEditContext<DmoInvoice>
{
    public InvoiceId Id => this.BaseRecord.Id;


    [TrackState] public string? CustomerName { get; set; }
    
    [TrackState] public Guid CustomerId { get; set; }

    // We use a DateTime here as some edit controls only like DateTime
    [TrackState] public DateTime? Date { get; set; } = DateTime.Now;

    public override DmoInvoice AsRecord => this.BaseRecord with
    {
         Date = DateOnly.FromDateTime(this.Date ?? DateTime.Now),
        CustomerId = new(this.CustomerId),
        CustomerName = this.CustomerName ?? "Not Set",
    };

    public DmoInvoiceEditContext() : base() { }

    public DmoInvoiceEditContext(DmoInvoice record) : base(record) { }

    public override IDataResult Load(DmoInvoice record)
    {
        if (!this.BaseRecord.Id.IsDefault)
            return DataResult.Failure("A record has already been loaded.  You can't overload it.");

        this.BaseRecord = record;
        this.Date = record.Date.ToDateTime(TimeOnly.MinValue);
        this.CustomerName = record.CustomerName;
        this.CustomerId = record.CustomerId.Value;
        return DataResult.Success();
    }
}
