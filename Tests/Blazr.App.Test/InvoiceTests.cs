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
    public async Task GetAnInvoice()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();

        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        // Get the test item and it's Id from the Test Provider
        var controlItem = _testDataProvider.Invoices.Skip(Random.Shared.Next(3)).First();
        var controlRecord = this.AsDmoInvoice(controlItem);
        var controlId = controlRecord.Id;
        var _controlInvoiceItems = _testDataProvider.InvoiceItems.Where(item => item.InvoiceID == controlItem.InvoiceID);
        var controlInvoiceItems = _controlInvoiceItems.Select(item => this.AsDmoInvoiceItem(item)).ToList();

        var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(controlId));

        Assert.True(entityResult.HasValue);
        Assert.Equal(controlInvoiceItems.Count, entityResult.Value!.InvoiceItems.Count);
        Assert.Contains(entityResult.Value!.InvoiceItems.First(), controlInvoiceItems);
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(0, 20)]
    [InlineData(1, 2)]
    public async Task GetInvoiceGrid(int startIndex, int pageSize)
    {
        var provider = GetServiceProvider();

        // Get the total expected count and the first record of the page
        var testCount = _testDataProvider.Invoices.Count();
        var testPageCount = _testDataProvider.Invoices.Skip(startIndex).Take(pageSize).Count();

        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        var request = new InvoiceListRequest()
        {
            PageSize = pageSize,
            StartIndex = startIndex,
            SortColumn = null,
            SortDescending = false,
        };

        var listResult = await mediator.DispatchAsync(request);

        Assert.True(listResult.HasValue);
        Assert.Equal(testCount, listResult.Value!.TotalCount);
        Assert.Equal(testPageCount, listResult.Value!.Items.Count());
    }

    [Fact]
    public async Task UpdateAnInvoice()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;
        var mutorFactory = provider.GetRequiredService<InvoiceMutorFactory>()!;

        // Get a sample Invoice Mutor
        var entity = await this.GetASampleEntityAsync(mediator);
        var Id = entity.InvoiceRecord.Id;

        // Get the invoice mutor from the factory
        var entityMutor = await mutorFactory.GetInvoiceEntityMutorAsync(entity.InvoiceRecord.Id);
        
        // Create an Invoice Record Mutor
        var invoiceMutor = InvoiceRecordMutor.Load(entityMutor.InvoiceEntity.InvoiceRecord);

        // Update the Invoice Record Mutor
        invoiceMutor.Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-5));

        // Update the Entity Mutor by dispatching the itemMutor's Dispatcher
        var actionResult = entityMutor.Dispatch(invoiceMutor.Dispatcher);

        Assert.True(actionResult.Succeeded);

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
        var entityMutor =  await mutorFactory.CreateInvoiceEntityMutorAsync();
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
        Assert.Equivalent(updatedEntity, dbEntity);
        Assert.Equal(dbEntity.InvoiceRecord.TotalAmount.Value, dbEntity.InvoiceItems.Sum(item => item.Amount.Value));
    }

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


    [Fact]
    public async Task DeleteAnInvoice()
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
        var commandResult = await entityMutor.DeleteAsync();

        Assert.True(commandResult.Succeeded);
    }

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
        var commandResult = await mediator.DispatchAsync(InvoiceCommandRequest.Create(entityMutor.InvoiceEntity, entityMutor.State));

        Assert.True(commandResult.Succeeded);

        // Get the Invoice Entity from the Data Store
        var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(Id));

        Assert.False(entityResult.HasException);

        var dbEntity = entityResult.Value!;

        // Check the stored data is the same as the edited entity
        Assert.Equivalent(updatedEntity, dbEntity);

        Assert.Equal(dbEntity.InvoiceRecord.TotalAmount.Value, dbEntity.InvoiceItems.Sum(item => item.Amount.Value));
    }

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
        Assert.Equivalent(updatedEntity, dbEntity);
        Assert.Equal(dbEntity.InvoiceRecord.TotalAmount.Value, dbEntity.InvoiceItems.Sum(item => item.Amount.Value));
    }

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
