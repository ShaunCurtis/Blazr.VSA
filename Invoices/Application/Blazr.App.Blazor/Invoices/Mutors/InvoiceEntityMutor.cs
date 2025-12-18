/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode.Mediator;
using Blazr.Gallium;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.UI;

/// <summary>
/// Service to create InvoiceEntity Mutors 
/// Register as Singleton or Scoped Service
/// </summary>
public sealed class InvoiceEntityMutorFactory
{
    private IServiceProvider _serviceProvider;

    public InvoiceEntityMutorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<InvoiceEntityMutor> CreateInvoiceEntityMutorAsync(InvoiceId id)
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

    public bool IsDirty => !this.InvoiceEntity.Equals(BaseEntity);
    public bool IsNew { get; private set; } = true;
    public Task LoadTask = Task.CompletedTask;

    public EditState State => this.IsNew
        ? EditState.New
        : IsDirty
            ? EditState.Dirty
            : EditState.Clean;

    public InvoiceEntityMutor(IMediatorBroker mediator, IMessageBus messageBus, InvoiceId id)
    {
        _mediator = mediator;
        _messageBus = messageBus;
        this.BaseEntity = InvoiceEntity.CreateNewEntity();
        this.InvoiceEntity = this.BaseEntity;
        this.LoadTask  = this.LoadAsync(id);
    }

    public Return Dispatch(Func<InvoiceEntity, Return<InvoiceEntity>> dispatcher)
    {
        var result = dispatcher.Invoke(InvoiceEntity);

        LastResult = result.ToReturn();
        InvoiceEntity = result.Write(this.InvoiceEntity);

        _messageBus.Publish<InvoiceEntity>(this.InvoiceEntity.InvoiceRecord.Id);

        return this.LastResult;
    }

    private async Task LoadAsync(InvoiceId id)
    {
        var result = await _mediator.DispatchAsync(new InvoiceEntityRequest(id));

        this.InvoiceEntity = result.Write(InvoiceEntity.CreateNewEntity());
        this.BaseEntity = this.InvoiceEntity;
        this.LastResult = result.ToReturn();
        this.IsNew = !this.LastResult.Write();
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
