/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Core.Invoices;
using Blazr.Diode.Mediator;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Blazr.Test;

public partial class InvoiceTests
{
    [Fact]
    public async Task DeleteAnInvoiceItem()
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
        var itemToDelete = entityMutor.InvoiceEntity.InvoiceItems.First();


        // Create a Delete Item action and dispatch it through the entity mutor.
        var action = DeleteInvoiceItemAction.Create(itemToDelete);
        var deleteActionResult = entityMutor.Dispatch(action.Dispatcher);

        Assert.True(deleteActionResult.Succeeded);

        // Get the current Mutor Entity
        var updatedEntity = entityMutor.InvoiceEntity;

        // Commit the changes to the data store
        var commandResult = await mediator.DispatchAsync(InvoiceEntityCommandRequest.Create(entityMutor.InvoiceEntity, entityMutor.State));

        Assert.True(commandResult.Succeeded);

        // Get the Invoice Entity from the Data Store
        var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(Id));

        Assert.False(entityResult.HasException);

        var dbEntity = entityResult.Value!;

        // Check the stored data is the same as the edited entity
        Assert.Equivalent(updatedEntity, dbEntity);

        Assert.Equal(dbEntity.InvoiceRecord.TotalAmount.Value, dbEntity.InvoiceItems.Sum(item => item.Amount.Value));
    }
}
