/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core.Invoices;
using Blazr.Diode.Mediator;
using Blazr.Gallium;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.Presentation;

/// <summary>
/// Service to create InvoiceEntity Mutors 
/// Register as Singleton or Scoped Service
/// </summary>
public sealed class InvoiceMutorFactory
{
    private IServiceProvider _serviceProvider;

    public InvoiceMutorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<InvoiceEntityMutor> GetInvoiceEntityMutorAsync(InvoiceId id)
    {
        var mutor = ActivatorUtilities.CreateInstance<InvoiceEntityMutor>(_serviceProvider, new object[] { id });
        await mutor.LoadTask;
        return mutor;
    }

    public async Task<InvoiceEntityMutor> CreateInvoiceEntityMutorAsync()
    {
        var mutor = ActivatorUtilities.CreateInstance<InvoiceEntityMutor>(_serviceProvider, new object[] { InvoiceId.NewId });
        await mutor.LoadTask;
        return mutor;
    }
}

/// <summary>
/// This is the Mutor for an Invoice Entity
/// To create an instance use the CreateInvoiceEntityMutorAsync method on the InvoiceEntityMutorFactory DI Service
/// </summary>
public sealed class InvoiceEntityMutor
{
    private readonly IMediatorBroker _mediator;
    private readonly IMessageBus _messageBus;

    public InvoiceEntity BaseEntity { get; private set; }
    public InvoiceEntity InvoiceEntity { get; private set; }
    public Result LastResult { get; private set; } = Result.Successful();
    public bool IsNew => this.BaseEntity.InvoiceRecord.Id.IsNew;
    public Task LoadTask { get; private set; } = Task.CompletedTask;
    public bool IsDirty => !this.InvoiceEntity.Equals(BaseEntity);

    public InvoiceEntityMutor(IMediatorBroker mediator, IMessageBus messageBus, InvoiceId id)
    {
        _mediator = mediator;
        _messageBus = messageBus;
        this.BaseEntity = InvoiceEntityFactory.Create();
        this.InvoiceEntity = this.BaseEntity;
        this.LoadTask = this.LoadAsync(id);
    }

    public RecordState State => (this.IsNew, this.IsDirty) switch
    {
        (true, _) => RecordState.NewState,
        (false, false) => RecordState.CleanState,
        (false, true) => RecordState.DirtyState,
    };

    public Result Dispatch(Func<InvoiceEntity, Result<InvoiceEntity>> dispatcher)
    {
        var result = dispatcher.Invoke(InvoiceEntity)
        .Match(successAction: entity => this.InvoiceEntity = entity)
        .Match(successAction: entity => _messageBus.Publish<InvoiceEntity>(entity.InvoiceRecord.Id));

        this.LastResult = result.AsResult;

        return this.LastResult;
    }

    private async Task LoadAsync(InvoiceId id)
    {
        if (id.IsNew)
        {
            this.Set(InvoiceEntityFactory.Create());
            return;
        }

        this.LastResult = (await _mediator
            .DispatchAsync(InvoiceEntityRequest.Create(id)))
            .Match(
                successAction: this.Set,
                failureAction: message => this.SetAsNew())
            .AsResult;
    }

    public async Task<Result> SaveAsync()
    {
        var result = await _mediator.DispatchAsync(new InvoiceEntityCommandRequest(this.InvoiceEntity, RecordState.DirtyState, Guid.NewGuid()));

        this.LastResult = result.AsResult;

        return this.LastResult;
    }
    public async Task<Result> DeleteAsync()
    {
        var result = await _mediator.DispatchAsync(new InvoiceEntityCommandRequest(this.InvoiceEntity, RecordState.DeletedState, Guid.NewGuid()))
            .MatchAsync(successAction: value => this.BaseEntity = this.InvoiceEntity);

        this.LastResult = result.AsResult;

        return this.LastResult;
    }

    public InvoiceItemRecordMutor GetInvoiceItemRecordMutor(InvoiceItemId id)
        => this.InvoiceEntity.GetInvoiceItem(id)
            .Map(value => InvoiceItemRecordMutor.Load(value))
            .Write( defaultValue: InvoiceItemRecordMutor.NewMutor(this.InvoiceEntity.InvoiceRecord.Id));

    public InvoiceItemRecordMutor GetNewInvoiceItemRecordMutor()
        => InvoiceItemRecordMutor.NewMutor(this.BaseEntity.InvoiceRecord.Id);

    public Return Reset()
    {
        this.Set(this.BaseEntity);
        return Return.Success();
    }

    private void Set(InvoiceEntity entity)
    {
        this.BaseEntity = entity;
        this.InvoiceEntity = InvoiceEntityFactory.ApplyEntityRules(entity);

        // Checks the entity business rules and applies any changes which changes the state of the mutor
        this.LastResult = this.IsDirty
            ? this.LastResult = Result.Failure("The stored total cost does no match the sum of the entities.  The entity must be saved to fix the problem.")
            : Result.Successful();
    }

    private void SetAsNew()
    {
        this.InvoiceEntity = InvoiceEntityFactory.Create();
        this.BaseEntity = this.InvoiceEntity;
    }
}
