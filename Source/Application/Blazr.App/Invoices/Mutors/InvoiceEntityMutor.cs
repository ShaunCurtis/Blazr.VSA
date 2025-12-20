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
        => _serviceProvider = serviceProvider;

    public async Task<InvoiceEntityMutor> GetInvoiceEntityMutorAsync(InvoiceId id)
    {
        var mutor = ActivatorUtilities.CreateInstance<InvoiceEntityMutor>(_serviceProvider, new object[] { id });
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
    public bool IsNew { get; private set; } = true;
    public Task LoadTask { get; private set; } = Task.CompletedTask;

    public InvoiceEntityMutor(IMediatorBroker mediator, IMessageBus messageBus, InvoiceId id)
    {
        _mediator = mediator;
        _messageBus = messageBus;
        this.BaseEntity = InvoiceEntityFactory.Create();
        this.InvoiceEntity = this.BaseEntity;
        this.LoadTask = this.LoadAsync(id);
    }

    public bool IsDirty => !this.InvoiceEntity.Equals(BaseEntity);

    public EditState State => (this.IsNew, this.IsDirty) switch
    {
        (true, _) => EditState.New,
        (false, false) => EditState.Clean,
        (false, true) => EditState.Dirty,
    };

    public Return Dispatch(Func<InvoiceEntity, Return<InvoiceEntity>> dispatcher)
        => dispatcher.Invoke(InvoiceEntity)
            .WriteReturn(ret => LastResult = ret)
            .Notify(hasValue: value => this.InvoiceEntity = value)
            .Notify(hasValue: value => _messageBus.Publish<InvoiceEntity>(value.InvoiceRecord.Id))
            .ToReturn();

    private async Task LoadAsync(InvoiceId id)
        => await _mediator.DispatchAsync(new InvoiceEntityRequest(id))
            .WriteReturnAsync(ret => this.LastResult = ret)
            .WriteAsync( 
                success: value => { 
                    this.BaseEntity = value;
                    this.InvoiceEntity = value;
                },
                failure: () => { 
                    this.IsNew = true;
                    this.InvoiceEntity = InvoiceEntityFactory.Create();
                }
            );

    public async Task<Return> SaveAsync()
        => await _mediator.DispatchAsync(new InvoiceCommandRequest(this.InvoiceEntity, EditState.Dirty, Guid.NewGuid()))
            .WriteReturnAsync(ret => this.LastResult = ret);

    public InvoiceItemRecordMutor GetInvoiceItemRecordMutor(InvoiceItemId id)
        => this.InvoiceEntity.GetInvoiceItem(id)
            .Write(
                hasValue: value => InvoiceItemRecordMutor.Read(value),
                hasNoValue: () => InvoiceItemRecordMutor.Create(this.InvoiceEntity.InvoiceRecord.Id));

    public Return Reset()
    {
        this.InvoiceEntity = this.BaseEntity;
        this.LastResult = Return.Success();
        return this.LastResult;
    }
}
