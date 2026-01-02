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
    public async Task UpdateADirtyInvoice()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;
        var mutorFactory = provider.GetRequiredService<InvoiceMutorFactory>()!;

        // Get a sample Invoice Mutor
        var entity = await this.GetASampleDirtyEntityAsync(mediator);
        var Id = entity.InvoiceRecord.Id;

        // Get the invoice mutor from the factory
        var entityMutor = await mutorFactory.GetInvoiceEntityMutorAsync(entity.InvoiceRecord.Id);

        Assert.True(entityMutor.IsDirty);

        // Commit the changes to the data store
        var commandResult = await entityMutor.SaveAsync();

        Assert.True(commandResult.Succeeded);

        // Get the current Mutor Entity
        var updatedEntity = entityMutor.InvoiceEntity;

        // Get the Invoice Entity from the Data Store
        var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(Id));

        Assert.True(entityResult.HasValue);

        var dbEntity = entityResult.Value!;

        // Check the stored data is tthe same as the edited entity
        Assert.Equivalent(updatedEntity, dbEntity);
        Assert.Equal(dbEntity.InvoiceRecord.TotalAmount.Value, dbEntity.InvoiceItems.Sum(item => item.Amount.Value));
    }

}
