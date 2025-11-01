/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.Diode;
using Blazr.Diode.Mediator;
using Blazr.Gallium;
using Blazr.Manganese;

namespace Blazr.App.UI;

public sealed class InvoiceEntityMutor
{
    private readonly IMediatorBroker _mediator;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBus _messageBus;

    public InvoiceEntity BaseEntity { get; private set; } = InvoiceEntity.CreateNewEntity();
    public InvoiceEntity InvoiceEntity { get; private set; } = InvoiceEntity.CreateNewEntity();
    public Result LastResult { get; private set; } = Result.Success();

    public bool IsDirty => !this.InvoiceEntity.Equals(BaseEntity);
    public bool IsNew { get; private set; } = true;

    public EditState State => this.IsNew
        ? EditState.New
        : IsDirty
            ? EditState.Dirty
            : EditState.Clean;

    public InvoiceEntityMutor(IMediatorBroker mediator, IServiceProvider serviceProvider, IMessageBus messageBus)
    {
        _mediator = mediator;
        _serviceProvider = serviceProvider;
        _messageBus = messageBus;
    }

    public Result Dispatch(Func<InvoiceEntity, Result<InvoiceEntity>> dispatcher)
    {
        var result = dispatcher.Invoke(InvoiceEntity);

        LastResult = result.ToResult();
        InvoiceEntity = result.OutputValue(this.InvoiceEntity);

        _messageBus.Publish<InvoiceEntity>(this.InvoiceEntity.InvoiceRecord.Id);

        return this.LastResult;
    }

    public async Task<Result> LoadAsync(InvoiceId id)
    {
        var result = await _mediator.DispatchAsync(new InvoiceRecordRequest(id));

        this.InvoiceEntity = result.OutputValue(InvoiceEntity.CreateNewEntity);
        this.BaseEntity = this.InvoiceEntity;
        this.LastResult = result.ToResult();
        this.IsNew = !this.LastResult.OutputValue();

        return this.LastResult;
    }

    public async Task<Result> SaveAsync()
    {
        var result = await _mediator.DispatchAsync(new InvoiceCommandRequest(this.InvoiceEntity, EditState.Dirty, Guid.NewGuid()));

        this.LastResult = result;

        return this.LastResult;
    }

    public Result Reset()
    {
        this.InvoiceEntity = this.BaseEntity;
        this.LastResult = Result.Success();
        return this.LastResult;
    }
}
