/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App;
using Blazr.App.Core;
using Blazr.App.UI;
using Blazr.Diode;
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
        var mutorFactory = provider.GetRequiredService<InvoiceEntityMutorFactory>()!;

        // Get an Invoice Mutor
        var entity = await this.GetASampleEntityAsync(mediator);
        var Id = entity.InvoiceRecord.Id;

        var mutor = await mutorFactory.CreateInvoiceEntityMutorAsync(entity.InvoiceRecord.Id);

        // Update the Mutor
        var updatedInvoice = mutor.InvoiceEntity.InvoiceRecord with { Date = new(DateTime.Now.AddDays(-5)) };
        var action = UpdateInvoiceAction.Create(updatedInvoice);
        var entityUpdateResult = mutor.Dispatch(action.Dispatcher);

        // Update the Data store from the Mutor
        var updateResult = await mutor.SaveAsync();

        Assert.False(updateResult.Failed);

        // Get the current Mutor Entity
        var updatedEntity = mutor.InvoiceEntity;

        // Get the Invoice Entity from the Data Store
        var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(Id));

        Assert.False(entityResult.HasException);

        var dbEntity = entityResult.Value!;

        // Check the stored data is tthe same as the edited entity
        Assert.Equivalent(updatedEntity, dbEntity);
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

    //[Fact]
    //public async Task UpdateAnInvoiceItem()
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

    //    // Get the item to Update
    //    var itemToUpdate = entity.InvoiceItems.First() with { Amount = new(59) };

    //    // Update the Mutor
    //    var action = UpdateInvoiceItemAction.Create(itemToUpdate);

    //    var actionResult = mutor.Dispatch(action.Dispatcher);

    //    // Commit the changes to the data store
    //    var commandResult = await mediator.DispatchAsync(InvoiceCommandRequest.Create(mutor.InvoiceEntity, mutor.State));

    //    Assert.False(commandResult.Failed);

    //    // Get the Invoice Entity from the Data Store
    //    var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(Id));

    //    Assert.False(entityResult.HasException);

    //    // Get the Mutor Entities
    //    var updatedEntity = mutor.InvoiceEntity;
    //    var dbEntity = entityResult.Value!;

    //    // Check the stored data is the same as the edited entity
    //    Assert.Equivalent(updatedEntity, dbEntity);

    //    var rulesCheckResult = InvoiceEntity.CheckEntityRules(dbEntity);

    //    Assert.False(rulesCheckResult.HasException);
    //}

    //[Fact]
    //public async Task AddAnInvoiceItem()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();
    //    var mediator = provider.GetRequiredService<IMediatorBroker>()!;
    //    var mutor = provider.GetRequiredService<InvoiceEntityMutor>()!;

    //    // Get an Invoice Mutor
    //    var entity = await this.GetASampleEntityAsync(mediator);

    //    var mutorResult = await mutor.LoadAsync(entity.InvoiceRecord.Id);
    //    Assert.False(mutorResult.Failed);

    //    // create the item to add
    //    var itemToAdd = new DmoInvoiceItem()
    //    {
    //        Description = new("Added Plane"),
    //        Amount = new(59),
    //        Id = InvoiceItemId.Create(),
    //        InvoiceId = entity.InvoiceRecord.Id,
    //    };

    //    // Update the Mutor
    //    var action = AddInvoiceItemAction.Create(itemToAdd);

    //    var actionResult = mutor.Dispatch(action.Dispatcher);

    //    Assert.False(actionResult.Failed);

    //    // Get the current Mutor Entity
    //    var updatedEntity = mutor.InvoiceEntity;

    //    // Commit the changes to the data store
    //    var commandResult = await mediator.DispatchAsync(InvoiceCommandRequest.Create(mutor.InvoiceEntity, mutor.State));

    //    Assert.False(commandResult.Failed);

    //    // Get the Invoice Entity from the Data Store
    //    var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(mutor.InvoiceEntity.InvoiceRecord.Id));

    //    Assert.False(entityResult.HasException);

    //    var dbEntity = entityResult.Value!;

    //    // Check the stored data is the same as the edited entity
    //    Assert.Equivalent(updatedEntity, dbEntity);

    //    var rulesCheckResult = InvoiceEntity.CheckEntityRules(dbEntity);

    //    Assert.False(rulesCheckResult.HasException);
    //}
}
