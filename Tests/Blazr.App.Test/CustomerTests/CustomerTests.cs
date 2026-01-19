/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Presentation;
using Blazr.Diode.Mediator;
using Blazr.Manganese;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.Test;

public partial class CustomerTests
{
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
        var mutor = CustomerRecordMutor.Load(customerResult.AsSuccess.Value);

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
        Assert.Equivalent(editedRecord, customerResult.AsSuccess.Value);
    }
}
