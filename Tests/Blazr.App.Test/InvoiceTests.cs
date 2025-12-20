/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.Diode.Mediator;
using Microsoft.Extensions.DependencyInjection;

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

        // Get an Invoice Mutor
        var entity = await this.GetASampleEntityAsync(mediator);
        var Id = entity.InvoiceRecord.Id;

        var entityMutor = await mutorFactory.GetInvoiceEntityMutorAsync(entity.InvoiceRecord.Id);
        var invoiceMutor = InvoiceRecordMutor.Load(entityMutor.InvoiceEntity.InvoiceRecord);

        // Update the Mutor
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

    //[Fact]
    //public async Task AddAnInvoice()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();
    //    var mediator = provider.GetRequiredService<IMediatorBroker>()!;
    //    var mutor = provider.GetRequiredService<InvoiceEntityMutor>()!;

    //    // Get an Invoice Mutor
    //    var entity = await this.GetASampleEntityAsync(mediator);

    //    var Id = mutor.InvoiceEntity.InvoiceRecord.Id;
    //    // Create a new Invoice
    //    var newInvoice = entity.InvoiceRecord with
    //    {
    //        Customer = entity.InvoiceRecord.Customer,
    //        Date = new(DateTime.Now.AddDays(-1)),
    //    };

    //    var newItems = new List<DmoInvoiceItem>()
    //    {
    //        new DmoInvoiceItem
    //        {
    //            Id = InvoiceItemId.Create(),
    //            InvoiceId = Id,
    //             Description = new("B787"),
    //              Amount = new(27)
    //        },
    //        new DmoInvoiceItem
    //        {
    //            Id = InvoiceItemId.Create(),
    //            InvoiceId = Id,
    //             Description = new("B707"),
    //              Amount = new(1)
    //        },
    //    };

    //    var entityUpdateResult = mutor.Mutate(newInvoice, newItems);

    //    // Update the Data store from the Mutor
    //    var updateResult = await mediator.DispatchAsync(new InvoiceCommandRequest(mutor.InvoiceEntity, EditState.Dirty, Guid.NewGuid()));

    //    Assert.False(updateResult.Failed);

    //    // Get the current Mutor Entity
    //    var updatedEntity = mutor.InvoiceEntity;

    //    // Get the Invoice Entity from the Data Store
    //    var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(Id));

    //    Assert.False(entityResult.HasException);

    //    var dbEntity = entityResult.Value!;

    //    // Check the stored data is tthe same as the edited entity
    //    Assert.Equivalent(updatedEntity, dbEntity);
    //}

    //[Fact]
    //public async Task DeleteAnInvoice()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();
    //    var mediator = provider.GetRequiredService<IMediatorBroker>()!;
    //    var mutor = provider.GetRequiredService<InvoiceEntityMutor>()!;

    //    // Get an Invoice Mutor
    //    var entity = await this.GetASampleEntityAsync(mediator);
    //    var Id = entity.InvoiceRecord.Id;

    //    var mutorResult = await mutor.LoadAsync(entity.InvoiceRecord.Id);
    //    Assert.False(mutorResult.Failed);

    //    // Delete from the Data store 
    //    var deleteResult = await mediator.DispatchAsync(new InvoiceCommandRequest(mutor.InvoiceEntity, EditState.Deleted, Guid.NewGuid()));

    //    Assert.False(deleteResult.Failed);

    //    // Get the Invoice Entity from the Data Store
    //    var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(Id));

    //    // Check the invoice doesn't exist
    //    Assert.True(entityResult.HasException);
    //}

    //[Fact]
    //public async Task DeleteAnInvoiceItem()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();
    //    var mediator = provider.GetRequiredService<IMediatorBroker>()!;
    //    var mutor = provider.GetRequiredService<InvoiceEntityMutor>()!;

    //    // Get an Invoice Mutor
    //    var entity = await this.GetASampleEntityAsync(mediator);
    //    var Id = entity.InvoiceRecord.Id;

    //    var mutorResult = await mutor.LoadAsync(entity.InvoiceRecord.Id);
    //    Assert.False(mutorResult.Failed);

    //    // Get the item to delete
    //    var itemToDelete = entity.InvoiceItems.First();

    //    // Update the Mutor
    //    var action = DeleteInvoiceItemAction.Create(itemToDelete);
    //    var deleteActionResult = mutor.Dispatch(action.Dispatcher);

    //    Assert.False(deleteActionResult.Failed);

    //    // Get the current Mutor Entity
    //    var updatedEntity = mutor.InvoiceEntity;

    //    // Commit the changes to the data store
    //    var commandResult = await mediator.DispatchAsync(InvoiceCommandRequest.Create(mutor.InvoiceEntity, mutor.State));

    //    Assert.False(commandResult.Failed);

    //    // Get the Invoice Entity from the Data Store
    //    var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(Id));

    //    Assert.False(entityResult.HasException);

    //    var dbEntity = entityResult.Value!;

    //    // Check the stored data is the same as the edited entity
    //    Assert.Equivalent(updatedEntity, dbEntity);

    //    var rulesCheckResult = InvoiceEntity.CheckEntityRules(dbEntity);

    //    Assert.False(rulesCheckResult.HasException);
    //}

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
        Assert.Equivalent(updatedEntity, dbEntity);
        Assert.Equal(dbEntity.InvoiceRecord.TotalAmount.Value, dbEntity.InvoiceItems.Sum(item => item.Amount.Value));
    }
}
