/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed partial class InvoiceEntity
{
    private readonly EntityState<DmoInvoice> _invoice;
    private List<EntityState<DmoInvoiceItem>> _invoiceItems;

    public InvoiceId Id => _invoice.Record.Id;
    public DmoInvoice Invoice =>_invoice.Record;
    public StateRecord<DmoInvoice> StateRecord => _invoice.AsStateRecord;
    public bool IsDirty => _invoice.IsDirty || _invoiceItems.Any(item => item.IsDirty);

    public IEnumerable<Dmo> ActiveMixFeeds => _mixFeeds.Where(item => item.State != EditState.Deleted).Select(item => item.Record);
    public IEnumerable<EntityState<DmoMixFeed>> AllMixFeedStateRecords => _mixFeeds.AsEnumerable();
    public IEnumerable<EntityState<DmoMixFeed>> ActiveMixFeedStateRecords => _mixFeeds.Where(item => item.State != EditState.Deleted);

    public event EventHandler<InvoiceId>? StateHasChanged;

    public decimal MixFeedTotal => this.ActiveMixFeeds.Sum(item => item.Weight.Value);

    public MixEntity(DmoMix mix, IEnumerable<DmoMixFeed> mixFeeds, bool isNew = false)
    {
        _mix = new EntityState<DmoMix>(mix, isNew);
        _mixFeeds = mixFeeds.Select(item => new EntityState<DmoMixFeed>(item)).ToList();
    }

    public Result<MixEntity> AsResult
        => Result<MixEntity>.Create(this);

    public static MixEntity Create()
            => new MixEntity(new DmoMix() { Id = MixId.Default }, Enumerable.Empty<DmoMixFeed>(), true);

    public static MixEntity Load(DmoMix mix, IEnumerable<DmoMixFeed> mixFeeds)
            => new MixEntity(mix, mixFeeds);

    public void RaiseStateHasChanged(object? sender, MixId id)
        => this.StateHasChanged?.Invoke(sender, id);
}
