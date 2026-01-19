/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Core.Invoices;
using Blazr.App.Presentation;
using Blazr.Diode.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Blazr.Manganese;

namespace Blazr.Test;

public partial class InvoiceTests
{
    [Fact]
    public async Task UpdateAnInvoiceItem()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;
        var mutorFactory = provider.GetRequiredService<InvoiceMutorFactory>()!;

        // Get an Invoice Id
        var entity = await this.GetASampleEntityAsync(mediator);
        var Id = entity.InvoiceRecord.Id;

        // Get the Entity Mutor
        var entityMutor = await mutorFactory.GetInvoiceEntityMutorAsync(entity.InvoiceRecord.Id);

        // Get the Item Mutor
        var itemMutor = InvoiceItemRecordMutor.Load(entityMutor.InvoiceEntity.InvoiceItems.First());

        // Simulate editing the Amount field in the UI
        itemMutor.Amount = 59;

        // Update the Entity Mutor by dispatching the itemMutor's Dispatcher
        var actionResult = entityMutor.Dispatch(itemMutor.Dispatcher);

        Assert.True(actionResult.IsSuccess);

        // Commit the changes to the data store
        var commandResult = await entityMutor.SaveAsync();

        Assert.True(commandResult.IsSuccess);

        // Get the Invoice Entity from the Data Store
        var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(Id));

        Assert.True(entityResult.HasValue);

        // Get the Mutor Entities
        var updatedEntity = entityMutor.InvoiceEntity;
        var dbEntity = entityResult.AsSuccess.Value;

        // Check the stored data is the same as the edited entity
        Assert.Equal(updatedEntity, dbEntity);
        Assert.Equal(dbEntity.InvoiceRecord.TotalAmount.Value, dbEntity.InvoiceItems.Sum(item => item.Amount.Value));
    }

}
