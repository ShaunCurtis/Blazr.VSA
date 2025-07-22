/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Infrastructure;
using Blazr.App.Presentation;
using Blazr.App.UI;
using Blazr.Cadmium;
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode;
using Blazr.Manganese;
using Microsoft.AspNetCore.Components.QuickGrid;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blazr.Test;

public partial class CustomerTests
{

    [Fact]
    public async Task GetACustomer()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();

        //Injects the data broker
        var uIEntityProvider = provider.GetRequiredService<IUIEntityProvider<DmoCustomer, CustomerId>>()!;

        // Get the test item and it's Id from the Test Provider
        var controlItem = _testDataProvider.Customers.Skip(Random.Shared.Next(10)).First();
        var controlRecord = this.AsDmoCustomer(controlItem);
        var controlId = controlRecord.Id;

        //Outputs from the process that need to be tested
        bool result = false;

        var uiBroker = await uIEntityProvider.GetReadUIBrokerAsync(controlId);

        uiBroker.LastResult.Output(
            success: () => result = true,
            failure: (ex) => result = false);

        // check the query was successful
        Assert.True(result);
        // check it matches the test record
        Assert.Equal(controlRecord, uiBroker.Item);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(0, 20)]
    [InlineData(5, 10)]
    public async Task GetCustomerGrid(int startIndex, int pageSize)
    {
        var provider = GetServiceProvider();

        //Injects the data broker
        var uIEntityProvider = provider.GetRequiredService<IUIEntityProvider<DmoCustomer, CustomerId>>()!;
        //var _entityProvider = provider.GetService<IEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
        //var entityProvider = (WeatherForecastEntityProvider)_entityProvider;

        // Get the total expected count and the first record of the page
        var testCount = _testDataProvider.Customers.Count();
        var testFirstItem = _testDataProvider.Customers.Skip(startIndex).First();
        var testFirstRecord = this.AsDmoCustomer(testFirstItem);

        var uiBroker = await uIEntityProvider.GetGridUIBrokerAsync();

        // Create a GridItemsProviderRequest with the test parameters
        var gridRequest = new GridItemsProviderRequest<DmoCustomer>
        {
            Count = pageSize,
            StartIndex = startIndex,
            SortByAscending = false,
            SortByColumn = null
        };

        // dispatch the request to the UI Broker as done in the UI page using a QuickGrid component
        var mutationAction = UpdateGridRequest<DmoCustomer>.Create(gridRequest);
        var mutationResult = uiBroker.DispatchGridStateChange(mutationAction);

        var providerResult = await uiBroker.GetItemsAsync();

        //var x = gridState.
        //uiBroker.


        //var listRequest = await Result<CustomerListRequest>
        //    .Create(new CustomerListRequest { PageSize = pageSize, StartIndex = startIndex })
        //    .MapToResultAsync<ListItemsProvider<DmoCustomer>>(_entityProvider..ListItemsRequestAsync)
        //    .TaskSideEffectAsync(
        //        success: (provider) => listItemsProvider = provider,
        //        failure: (ex) => result = false);

        //Assert.True(result);
        //Assert.Equal(testCount, listItemsProvider.TotalCount);
        //Assert.Equal(pageSize, listItemsProvider.Items.Count());
        //Assert.Equal(testFirstRecord, listItemsProvider.Items.First());
    }

    //[Fact]
    //public async Task UpdateAForecast()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();

    //    //VGet the providers
    //    var _entityProvider = provider.GetService<IEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
    //    var entityProvider = (WeatherForecastEntityProvider)_entityProvider;
    //    var _entityUIProvider = provider.GetService<IUIEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
    //    var entityUIProvider = (WeatherForecastUIEntityProvider)_entityUIProvider;


    //    // Get the test item and it's Id from the Test Provider
    //    var testItem = _testDataProvider.WeatherForecasts.First();

    //    DmoWeatherForecast controlRecord = new()
    //    {
    //        Id = new(testItem.WeatherForecastID),
    //        Date = new(testItem.Date),
    //        Temperature = new(testItem.Temperature),
    //        Summary = "Test Edit"
    //    };

    //    var testId = controlRecord.Id;

    //    //Set up the outputs from the process that need testing
    //    bool result = false;

    //    // Get a UI Broker instance
    //    var uiBroker = await entityUIProvider.GetEditUIBrokerAsync<WeatherForecastEditContext>(testId);

    //    // Edit the summary as would happen in the UI
    //    uiBroker.EditMutator.Summary = controlRecord.Summary;

    //    // check the Mutator generated record against the control
    //    Assert.Equal(controlRecord, uiBroker.EditMutator.AsRecord);

    //    await uiBroker.SaveAsync();

    //    uiBroker.LastResult.Output(
    //        success: () => result = true,
    //        failure: (ex) => result = false
    //        );

    //    Assert.True(result);

    //    DmoWeatherForecast dbRecord = default!;

    //    // Get the record from the data store
    //    var recordResult = await entityProvider.RecordRequestAsync(testId)
    //        .TaskSideEffectAsync(
    //            success: (item) =>
    //            {
    //                dbRecord = item;
    //                result = true;
    //            },
    //            failure: (ex) => result = false
    //        );

    //    // check the query was successful
    //    Assert.True(result);
    //    // check the updated record matches the control record
    //    Assert.Equal(controlRecord, dbRecord);
    //}

    //[Fact]
    //public async Task DeleteAForecast()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();

    //    //Injects the data broker
    //    var _entityProvider = provider.GetService<IEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
    //    var entityProvider = (WeatherForecastEntityProvider)_entityProvider;

    //    // Get the test item and it's Id from the Test Provider
    //    var testItem = _testDataProvider.WeatherForecasts.First();

    //    var testId = new WeatherForecastId(testItem.WeatherForecastID);

    //    //Outputs from the process that need to be tested
    //    bool result = false;
    //    WeatherForecastEntity entity = default!;
    //    WeatherForecastId updatedId = default!;

    //    var recordResult = await entityProvider.EntityRequestAsync(testId)
    //        .TaskSideEffectAsync(
    //        success: (item) =>
    //        {
    //            entity = item;
    //            result = true;
    //        });

    //    // check the query was successful
    //    Assert.True(result);

    //    DmoWeatherForecast testRecord = entity.WeatherForecast;


    //    await WeatherForecastEntity.DeleteWeatherForecastAction
    //        .CreateAction()
    //        .AddSender(this)
    //        .ExecuteAction(entity)
    //        .MapToResultAsync(entityProvider.EntityCommandAsync)
    //        .OutputTaskAsync(success: (id) =>
    //        {
    //            result = true;
    //            updatedId = id;
    //        });

    //    // check the update was successful
    //    Assert.True(result);


    //    result = false;
    //    Exception? exception = null;

    //    await entityProvider.RecordRequestAsync(updatedId)
    //        .OutputTaskAsync(
    //        failure: (ex) =>
    //        {
    //            exception = ex;
    //            result = true;
    //        });

    //    // check the query was successful
    //    Assert.True(result);
    //    // check it matches the update record
    //    Assert.NotNull(exception);
    //}

    //[Fact]
    //public async Task AddAForecast()
    //{
    //    var provider = GetServiceProvider();

    //    //Get the Entity Provider
    //    var _entityProvider = provider.GetService<IEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
    //    var entityProvider = (WeatherForecastEntityProvider)_entityProvider;

    //    // Get the current record count
    //    var CurrentItemCount = _testDataProvider.WeatherForecasts.Count();

    //    // Create a new WeatherForecastEntity with a new DmoWeatherForecast
    //    var entity = WeatherForecastEntity.Load(new DmoWeatherForecast
    //        {
    //            Id = new(Guid.CreateVersion7()),
    //            Date = new(DateTime.Now),
    //            Summary = "Test Add",
    //            Temperature = new(30)
    //        },
    //        isNew: true);

    //    bool result = false;
    //    WeatherForecastId newId = default!;

    //    // Execute the entity command to add the new record
    //    var commandResult = await entityProvider.EntityCommandAsync.Invoke(entity)
    //        .TaskSideEffectAsync(success: (id) =>
    //            {
    //                result = true;
    //                newId = id;
    //            });

    //    // check the update was successful
    //    Assert.True(result);

    //    result = false;
    //    DmoWeatherForecast? dbRecord = null;

    //    // Now we try to get the record we just added
    //    await entityProvider.RecordRequestAsync(newId)
    //        .OutputTaskAsync(
    //        success: (record) =>
    //        {
    //            dbRecord = record;
    //            result = true;
    //        });

    //    // check the query was successful
    //    Assert.True(result);

    //    result = false;
    //    ListItemsProvider<DmoWeatherForecast> listItemsProvider = default!;

    //    // We create a Result from a new WeatherForecastListRequest defining our test parameters
    //    // and then map it to the WeatherListRequest method of the entity provider
    //    // This will execute the request and return a ListItemsProvider<DmoWeatherForecast> Result
    //    // which we then match to get the items provider.

    //    await Result<WeatherForecastListRequest>
    //        .Create(new()
    //        {
    //            PageSize = 1,
    //            StartIndex = 0,
    //        })
    //        .MapToResultAsync<ListItemsProvider<DmoWeatherForecast>>(entityProvider.ListItemsRequestAsync)
    //        .OutputTaskAsync(success: (provider) =>
    //        {
    //            listItemsProvider = provider;
    //            result = true;
    //        });

    //    // check the query was successful
    //    Assert.True(result);
    //    Assert.Equal(CurrentItemCount + 1, listItemsProvider.TotalCount);
    //}
}
