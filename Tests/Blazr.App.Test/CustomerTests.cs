/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.Diode;
using Blazr.Diode.Mediator;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.Test;

public partial class CustomerTests
{
    [Fact]
    public async Task GetACustomer()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        // Get the test item and it's Id from the Test Provider
        var controlItem = _testDataProvider.Customers.Skip(Random.Shared.Next(10)).First();
        var controlRecord = this.AsDmoCustomer(controlItem);
        var controlId = controlRecord.Id;

        var customerResult = await mediator.DispatchAsync(new CustomerRecordRequest(controlId));

        Assert.False(customerResult.HasException);

        // check it matches the test record
        Assert.Equivalent(controlRecord, customerResult.Value);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(0, 20)]
    [InlineData(5, 10)]
    public async Task GetCustomerGrid(int startIndex, int pageSize)
    {
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        // Get the total expected count and the first record of the page
        var testCount = _testDataProvider.Customers.Count();
        var testFirstItem = _testDataProvider.Customers.Skip(startIndex).First();
        var testFirstRecord = this.AsDmoCustomer(testFirstItem);

        var customerListResult = await mediator.DispatchAsync(new CustomerListRequest()
        {
            PageSize = pageSize,
            StartIndex = startIndex,
            SortColumn = null,
            SortDescending = false
        });

        Assert.False(customerListResult.HasException);
        Assert.Equal(testCount, customerListResult.Value!.TotalCount);
        Assert.Equal(pageSize, customerListResult.Value!.Items.Count());
        //Assert.Equal(testFirstRecord, customerListResult.Value!.Items.First());
    }

    [Fact]
    public async Task UpdateACustomer()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        // Get the test item and it's Id from the Test Provider
        var controlItem = _testDataProvider.Customers.Skip(Random.Shared.Next(2)).First();
        var controlRecord = this.AsDmoCustomer(controlItem);
        var controlId = controlRecord.Id;

        // Get the record from the data pipeline
        var customerResult = await mediator.DispatchAsync(new CustomerRecordRequest(controlId));
        Assert.True(customerResult.HasValue);

        // Load the mutor
        var mutor = CustomerRecordMutor.Load(customerResult.Value!);

        // emulate a UI Edit
        mutor.Name = $"{mutor.Name} - Update";

        //emulate the Validation process 
        var validator = new CustomerRecordMutorValidator();
        var validateResult = validator.Validate(mutor);
        var editedRecord = mutor.Record;
        Assert.True(validateResult.IsValid);

        // Emulate the save button
        var customerUpdateResult = await mediator.DispatchAsync(CustomerCommandRequest.Create(mutor.Record, mutor.State));

        // Get the new record from the data pipeine
        customerResult = await mediator.DispatchAsync(new CustomerRecordRequest(controlId));

        // check it matches the test record
        Assert.False(customerResult.HasException);
        Assert.Equivalent(editedRecord, customerResult.Value);
    }

    [Fact]
    public async Task AddACustomer()
    {
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        var newCustomer = DmoCustomer.NewCustomer() with { Name = new("Alaskan") };

        var customerAddResult = await mediator.DispatchAsync(CustomerCommandRequest.Create(newCustomer, EditState.New));

        Assert.False(customerAddResult.HasException);

        var customerResult = await mediator.DispatchAsync(new CustomerRecordRequest(newCustomer.Id));

        // check it matches the test record

        // NewCustomer has the isNew flag set in the record so we need to fix that to make a compare
        var customer = newCustomer with { Id = new(newCustomer.Id.Value) };
        Assert.False(customerResult.HasException);
        Assert.Equivalent(customer, customerResult.Value);
    }


    [Fact]
    public async Task DeleteACustomer()
    {
        var provider = GetServiceProvider();
        var mediator = provider.GetRequiredService<IMediatorBroker>()!;

        // Get the test item and it's Id from the Test Provider
        var controlItem = _testDataProvider.Customers.Skip(Random.Shared.Next(10)).First();
        var controlRecord = this.AsDmoCustomer(controlItem);
        var controlId = controlRecord.Id;

        var customerAddResult = await mediator.DispatchAsync(CustomerCommandRequest.Create(controlRecord, EditState.Deleted));

        Assert.False(customerAddResult.HasException);

        var customerResult = await mediator.DispatchAsync(new CustomerRecordRequest(controlId));

        // check it matches the test record
        Assert.True(customerResult.HasException);
    }
}
