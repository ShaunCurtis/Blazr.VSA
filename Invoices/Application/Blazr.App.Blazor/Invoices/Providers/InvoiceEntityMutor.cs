/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode.Mediator;
using Blazr.Gallium;

namespace Blazr.App.UI;

public sealed class InvoiceEntityMutor
{
    private readonly IMediatorBroker _mediator;
    private readonly IMessageBus _messageBus;

    public InvoiceEntity BaseEntity { get; private set; }
    public InvoiceEntity InvoiceEntity { get; private set; }
    public Return LastResult { get; private set; } = Return.Success();

    public bool IsDirty => !this.InvoiceEntity.Equals(BaseEntity);
    public bool IsNew { get; private set; } = true;

    public EditState State => this.IsNew
        ? EditState.New
        : IsDirty
            ? EditState.Dirty
            : EditState.Clean;

    public InvoiceEntityMutor(IMediatorBroker mediator, IMessageBus messageBus)
    {
        _mediator = mediator;
        _messageBus = messageBus;
        this.BaseEntity = InvoiceEntity.CreateNewEntity();
        this.InvoiceEntity = this.BaseEntity;
    }

    public Return Dispatch(Func<InvoiceEntity, Return<InvoiceEntity>> dispatcher)
    {
        var result = dispatcher.Invoke(InvoiceEntity);

        LastResult = result.ToReturn();
        InvoiceEntity = result.Write(this.InvoiceEntity);

        _messageBus.Publish<InvoiceEntity>(this.InvoiceEntity.InvoiceRecord.Id);

        return this.LastResult;
    }

    public Return Mutate(DmoInvoice invoice)
    {
        var result = InvoiceEntity.CreateWithEntityRulesApplied(invoice, this.InvoiceEntity.InvoiceItems);
        LastResult = result.ToReturn();

        this.InvoiceEntity = result.Write(this.InvoiceEntity);

        return this.LastResult;
    }

    public Return Mutate(IEnumerable<DmoInvoiceItem> invoiceItems)
    {
        var result = InvoiceEntity.CreateWithEntityRulesApplied(this.InvoiceEntity.InvoiceRecord, invoiceItems);
        LastResult = result.ToReturn();

        this.InvoiceEntity = result.Write(this.InvoiceEntity);

        return this.LastResult;
    }

    public Return Mutate(DmoInvoice invoice, IEnumerable<DmoInvoiceItem> invoiceItems)
    {
        var result = InvoiceEntity.CreateWithEntityRulesApplied(invoice, invoiceItems);
        LastResult = result.ToReturn();

        this.InvoiceEntity = result.Write(this.InvoiceEntity);

        return this.LastResult;
    }

    public async Task<Return> LoadAsync(InvoiceId id)
    {
        var result = await _mediator.DispatchAsync(new InvoiceRecordRequest(id));

        this.InvoiceEntity = result.Write(InvoiceEntity.CreateNewEntity());
        this.BaseEntity = this.InvoiceEntity;
        this.LastResult = result.ToReturn();
        this.IsNew = !this.LastResult.Write();

        return this.LastResult;
    }

    public async Task<Return> SaveAsync()
    {
        var result = await _mediator.DispatchAsync(new InvoiceCommandRequest(this.InvoiceEntity, EditState.Dirty, Guid.NewGuid()));

        this.LastResult = result;

        return this.LastResult;
    }

    public Return Reset()
    {
        this.InvoiceEntity = this.BaseEntity;
        this.LastResult = Return.Success();
        return this.LastResult;
    }
}
