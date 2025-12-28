/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Diode.Mediator;
using Blazr.Gallium;
using Microsoft.Extensions.DependencyInjection;
using System;
using Blazr.App.Core.Invoices;

namespace Blazr.App.Core;

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
    public Return LastResult { get; private set; } = Return.Success();
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

    public EditState State => (this.IsNew, this.IsDirty) switch
    {
        (true, _) => EditState.New,
        (false, false) => EditState.Clean,
        (false, true) => EditState.Dirty,
    };

    public Return Dispatch(Func<InvoiceEntity, Return<InvoiceEntity>> dispatcher)
        => dispatcher.Invoke(InvoiceEntity)
            .SetReturn(ret => LastResult = ret)
            .Notify(hasValue: value => this.InvoiceEntity = value)
            .Notify(hasValue: value => _messageBus.Publish<InvoiceEntity>(value.InvoiceRecord.Id))
            .ToReturn();

    private async Task LoadAsync(InvoiceId id)
    {
        if (id.IsNew)
        {
            this.Set(InvoiceEntityFactory.Create());
            return;
        }

        await _mediator.DispatchAsync(new InvoiceEntityRequest(id))
            .SetReturnAsync(ret => this.LastResult = ret)
            .NotifyAsync(
                hasValue: this.Set,
                hasNoValue: this.SetAsNew
            )
            .ToReturnAsync();
    }

    public async Task<Return> SaveAsync()
        => await _mediator.DispatchAsync(new InvoiceCommandRequest(this.InvoiceEntity, EditState.Dirty, Guid.NewGuid()))
            .SetReturnAsync(ret => this.LastResult = ret);

    public async Task<Return> DeleteAsync()
        => await _mediator.DispatchAsync(new InvoiceCommandRequest(this.InvoiceEntity, EditState.Deleted, Guid.NewGuid()))
            .SetReturnAsync(ret => this.LastResult = ret);

    public InvoiceItemRecordMutor GetInvoiceItemRecordMutor(InvoiceItemId id)
        => this.InvoiceEntity.GetInvoiceItem(id)
            .Write(
                hasValue: value => InvoiceItemRecordMutor.Load(value),
                hasNoValue: () => InvoiceItemRecordMutor.NewMutor(this.InvoiceEntity.InvoiceRecord.Id));

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
            ? this.LastResult = Return.Failure("The stored total cost does no match the sum of the entities.  The entity must be saved to fix the problem.")
            : Return.Success();
    }

    private void SetAsNew()
    {
        this.InvoiceEntity = InvoiceEntityFactory.Create();
        this.BaseEntity = this.InvoiceEntity;
    }
}
