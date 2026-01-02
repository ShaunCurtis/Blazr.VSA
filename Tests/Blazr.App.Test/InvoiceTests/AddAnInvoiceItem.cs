/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Core.Invoices;
using Blazr.Diode.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.Test;

public partial class InvoiceTests
{
    [Fact]
    public async Task AddAnInvoiceItem()
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
        var itemMutor = entityMutor.GetInvoiceItemRecordMutor(InvoiceItemId.Default);

        itemMutor.Description = "Added Plane";
        itemMutor.Amount = 77;

        // Update the Entity Mutor by dispatching the itemMutor's Dispatcher
        var actionResult = entityMutor.Dispatch(itemMutor.Dispatcher);

        Assert.True(actionResult.Succeeded);

        // Commit the changes to the data store
        var commandResult = await entityMutor.SaveAsync();

        Assert.True(commandResult.Succeeded);

        // Get the Invoice Entity from the Data Store
        var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(Id));

        Assert.True(entityResult.HasValue);

        // Get the Mutor Entities
        var updatedEntity = entityMutor.InvoiceEntity;
        var dbEntity = entityResult.Value!;

        // Check the stored data is the same as the edited entity
        Assert.Equal(updatedEntity, dbEntity);
        Assert.Equal(dbEntity.InvoiceRecord.TotalAmount.Value, dbEntity.InvoiceItems.Sum(item => item.Amount.Value));
    }
}
