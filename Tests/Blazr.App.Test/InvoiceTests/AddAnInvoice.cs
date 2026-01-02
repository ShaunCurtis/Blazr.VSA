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
    public async Task AddAnInvoice()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;
        var mutorFactory = provider.GetRequiredService<InvoiceMutorFactory>()!;

        // Get a sample Invoice Mutor and it's customer
        var testEntity = await this.GetASampleEntityAsync(mediator);
        var customer = testEntity.InvoiceRecord.Customer;

        // Get the invoice mutor from the factory
        var entityMutor = await mutorFactory.CreateInvoiceEntityMutorAsync();
        var entityId = entityMutor.InvoiceEntity.InvoiceRecord.Id;
        // Create an Invoice Record Mutor
        var invoiceMutor = InvoiceRecordMutor.Load(entityMutor.InvoiceEntity.InvoiceRecord);

        // Update the Invoice Record Mutor
        invoiceMutor.Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-5));
        invoiceMutor.Customer = customer;

        // Update the Entity Mutor by dispatching the itemMutor's Dispatcher
        var actionResult = entityMutor.Dispatch(invoiceMutor.Dispatcher);


        // Create an invoice item mutor in the correct context
        var invoiceItemMutor = entityMutor.GetNewInvoiceItemRecordMutor();

        invoiceItemMutor.Description = new("B787");
        invoiceItemMutor.Amount = new(27);

        // Update the Entity Mutor by dispatching the itemMutor's Dispatcher
        actionResult = entityMutor.Dispatch(invoiceItemMutor.Dispatcher);

        Assert.True(actionResult.Succeeded);

        // Create an invoice item mutor in the correct context
        invoiceItemMutor = entityMutor.GetNewInvoiceItemRecordMutor();

        invoiceItemMutor.Description = new("B707");
        invoiceItemMutor.Amount = new(2);

        // Update the Entity Mutor by dispatching the itemMutor's Dispatcher
        actionResult = entityMutor.Dispatch(invoiceItemMutor.Dispatcher);

        Assert.True(actionResult.Succeeded);

        // Commit the changes to the data store
        var commandResult = await entityMutor.SaveAsync();

        Assert.True(commandResult.Succeeded);

        // Get the current Mutor Entity
        var updatedEntity = entityMutor.InvoiceEntity;

        // Get the Invoice Entity from the Data Store
        var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(entityId));

        Assert.True(entityResult.HasValue);

        var dbEntity = entityResult.Value!;

        // Check the stored data is tthe same as the edited entity
        Assert.Equal(updatedEntity, dbEntity);
        Assert.Equal(dbEntity.InvoiceRecord.TotalAmount.Value, dbEntity.InvoiceItems.Sum(item => item.Amount.Value));
    }

}
