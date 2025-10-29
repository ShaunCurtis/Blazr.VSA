/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App;
using Blazr.App.Core;
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

        var entityResult = await mediator.DispatchAsync(new InvoiceRecordRequest(controlId));

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

        // Get a sample Invoice Mutor
        var mutor = await this.GetASampleMutorAsync(mediator);

        // Update the Mutor
        var updatedInvoice = mutor.CurrentEntity.InvoiceRecord with { Date = new(DateTime.Now.AddDays(-5)) };
        var entityUpdateResult = mutor.Mutate(updatedInvoice);
        mutor = entityUpdateResult.Value!;

        // Update the Data store from the Mutor
        var updateResult = await mediator.DispatchAsync(new InvoiceCommandRequest(mutor.CurrentEntity, EditState.Dirty, Guid.NewGuid()));

        Assert.False(updateResult.HasException);

        // Get the current Mutor Entity
        var updatedEntity = mutor.CurrentEntity;

        // Get the Invoice Entity from the Data Store
        var entityResult = await mediator.DispatchAsync(new InvoiceRecordRequest(mutor.Id));

        Assert.False(entityResult.HasException);

        var dbEntity = entityResult.Value!;

        // Check the stored data is tthe same as the edited entity
        Assert.Equivalent(updatedEntity, dbEntity);
    }


    [Fact]
    public async Task AddAnInvoice()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        // Get a sample Invoice Mutor
        var baseMutor = await this.GetASampleMutorAsync(mediator);

        // Get a sample Invoice Mutor
        var mutor = InvoiceMutor.CreateNew();

        // Update the Mutor
        var newInvoice = mutor.CurrentEntity.InvoiceRecord with
        {
            Customer = baseMutor.CurrentEntity.InvoiceRecord.Customer,
            Date = new(DateTime.Now.AddDays(-1)),
        };

        var newItems = new List<DmoInvoiceItem>()
        {
            new DmoInvoiceItem
            {
                Id = InvoiceItemId.Create(),
                InvoiceId = mutor.Id,
                 Description = new("B787"),
                  Amount = new(27)
            },
            new DmoInvoiceItem
            {
                Id = InvoiceItemId.Create(),
                InvoiceId = mutor.Id,
                 Description = new("B707"),
                  Amount = new(1)
            },
        };

        var entityUpdateResult = mutor.Mutate(newInvoice, newItems);
        mutor = entityUpdateResult.Value!;

        // Update the Data store from the Mutor
        var updateResult = await mediator.DispatchAsync(new InvoiceCommandRequest(mutor.CurrentEntity, EditState.Dirty, Guid.NewGuid()));

        Assert.False(updateResult.HasException);

        // Get the current Mutor Entity
        var updatedEntity = mutor.CurrentEntity;

        // Get the Invoice Entity from the Data Store
        var entityResult = await mediator.DispatchAsync(new InvoiceRecordRequest(mutor.Id));

        Assert.False(entityResult.HasException);

        var dbEntity = entityResult.Value!;

        // Check the stored data is tthe same as the edited entity
        Assert.Equivalent(updatedEntity, dbEntity);
    }

    [Fact]
    public async Task DeleteAnInvoice()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        // Get a sample Invoice Mutor
        var mutor = await this.GetASampleMutorAsync(mediator);

        // Delete from the Data store 
        var deleteResult = await mediator.DispatchAsync(new InvoiceCommandRequest(mutor.CurrentEntity, EditState.Deleted, Guid.NewGuid()));

        Assert.False(deleteResult.HasException);

        // Get the Invoice Entity from the Data Store
        var entityResult = await mediator.DispatchAsync(new InvoiceRecordRequest(mutor.Id));

        // Check the invoice doesn't exist
        Assert.True(entityResult.HasException);
    }

    [Fact]
    public async Task DeleteAnInvoiceItem()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        // Get a sample Invoice Mutor
        var mutor = await this.GetASampleMutorAsync(mediator);

        // Get the item to delete
        var itemToDelete = mutor.CurrentEntity.InvoiceItems.First();

        // Update the Mutor
        var deleteActionResult = DeleteInvoiceItemAction
            .Create(itemToDelete)
            .Dispatch(mutor);

        Assert.False(deleteActionResult.HasException);

        // Get the current Mutor Entity
        var updatedEntity = deleteActionResult.Value!.CurrentEntity;

        // Commit the changes to the data store
        var commandResult = await deleteActionResult
            .ExecuteTransformAsync(async _mutor => await mediator.DispatchAsync(InvoiceCommandRequest.Create(_mutor)));

        Assert.False(commandResult.HasException);

        // Get the Invoice Entity from the Data Store
        var entityResult = await mediator.DispatchAsync(new InvoiceRecordRequest(mutor.Id));

        Assert.False(entityResult.HasException);

        var dbEntity = entityResult.Value!;

        // Check the stored data is the same as the edited entity
        Assert.Equivalent(updatedEntity, dbEntity);

        var rulesCheckResult = InvoiceEntity.CheckEntityRules(dbEntity);

        Assert.False(rulesCheckResult.HasException);
    }

    [Fact]
    public async Task UpdateAnInvoiceItem()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        // Get a sample Invoice Mutor
        var mutor = await this.GetASampleMutorAsync(mediator);

        // Get the item to Update
        var itemToUpdate = mutor.CurrentEntity.InvoiceItems.First() with { Amount = new(59) };

        // Update the Mutor
        var actionResult = UpdateInvoiceItemAction
            .Create(itemToUpdate)
            .Dispatch(mutor);

        Assert.False(actionResult.HasException);

        // Commit the changes to the data store
        var commandResult = await actionResult
            .ExecuteTransformAsync(async _mutor => await mediator.DispatchAsync(InvoiceCommandRequest.Create(_mutor)));

        Assert.False(commandResult.HasException);

        // Get the Invoice Entity from the Data Store
        var entityResult = await mediator.DispatchAsync(new InvoiceRecordRequest(mutor.Id));

        Assert.False(entityResult.HasException);

        // Get the Mutor Entities
        var updatedEntity = actionResult.Value!.CurrentEntity;
        var dbEntity = entityResult.Value!;

        // Check the stored data is the same as the edited entity
        Assert.Equivalent(updatedEntity, dbEntity);

        var rulesCheckResult = InvoiceEntity.CheckEntityRules(dbEntity);

        Assert.False(rulesCheckResult.HasException);
    }

    [Fact]
    public async Task AddAnInvoiceItem()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        // Get a sample Invoice Mutor
        var mutor = await this.GetASampleMutorAsync(mediator);

        // Get the item to delete
        var itemToAdd = new DmoInvoiceItem()
        {
            Description = new("Added Plane"),
            Amount = new(59),
            Id = InvoiceItemId.Create(),
            InvoiceId = mutor.Id,
        };

        // Update the Mutor
        var actionResult = AddInvoiceItemAction
            .Create(itemToAdd)
            .Dispatch(mutor);

        Assert.False(actionResult.HasException);

        // Get the current Mutor Entity
        var updatedEntity = actionResult.Value!.CurrentEntity;

        // Commit the changes to the data store
        var commandResult = await actionResult
            .ExecuteTransformAsync(async _mutor => await mediator.DispatchAsync(InvoiceCommandRequest.Create(_mutor)));

        Assert.False(commandResult.HasException);

        // Get the Invoice Entity from the Data Store
        var entityResult = await mediator.DispatchAsync(new InvoiceRecordRequest(mutor.Id));

        Assert.False(entityResult.HasException);

        var dbEntity = entityResult.Value!;

        // Check the stored data is the same as the edited entity
        Assert.Equivalent(updatedEntity, dbEntity);

        var rulesCheckResult = InvoiceEntity.CheckEntityRules(dbEntity);

        Assert.False(rulesCheckResult.HasException);
    }
}
