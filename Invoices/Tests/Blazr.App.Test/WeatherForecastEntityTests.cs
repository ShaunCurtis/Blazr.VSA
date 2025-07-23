/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Presentation;
using Blazr.App.UI;
using Blazr.Cadmium;
using Blazr.Cadmium.Core;
using Blazr.Diode;
using Blazr.Manganese;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.Test;

public partial class WeatherForecastEntityTests
{

    //[Fact]
    //public async Task UpdateAForecast()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();

    //    //Gets the Providers
    //    var _entityUIProvider = provider.GetService<IUIEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
    //    var entityUIProvider = (WeatherForecastUIEntityProvider)_entityUIProvider;

    //    var _entityProvider = provider.GetService<IEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
    //    var entityProvider = (WeatherForecastEntityProvider)_entityProvider;

    //    // Get the a random item and it's Id from the Test Provider
    //    var testItem = _testDataProvider.WeatherForecasts.Skip(Random.Shared.Next(50)).First();
    //    var testId = new WeatherForecastId(testItem.WeatherForecastID);

    //    // Build a record that matches the edits we will apply to the entity
    //    DmoWeatherForecast updatedRecord = AsDmoWeatherForecast(testItem) with { Summary = "Test Edit" };

    //    //Outputs from the process that need to be tested
    //    bool result = false;

    //    // Get the UI Broker for the test item
    //    var uiBroker = await entityUIProvider.GetEntityEditUIBrokerAsync(testId);

    //    //Update the Summary as it would be done in the UI
    //    uiBroker.EditMutator.Summary = "Test Edit";

    //    // Test that the EditMutator has been updated
    //    Assert.Equal(updatedRecord, uiBroker.EditMutator.AsRecord);

    //    //Execute the Save - the method linked to the save button in the UI
    //    await uiBroker.SaveItemAsync(true);

    //    // Output the result of the save
    //    uiBroker.LastResult.ApplySideEffect(
    //        success: () => result = true,
    //        failure: (ex) => result = false);

    //    // And check the update was successful
    //    Assert.True(result);



    //    DmoWeatherForecast? dbRecord = null;

    //    // Get the record we just updated
    //    await entityProvider.RecordRequestAsync(testId)
    //        .OutputTaskAsync(
    //        success: (record) =>
    //        {
    //            dbRecord = record;
    //            result = true;
    //        },
    //        failure: (ex) => result = false);

    //    // check the query was successful
    //    Assert.True(result);
    //    // check it matches the update record
    //    Assert.Equal(updatedRecord, dbRecord);
    //}

    //[Fact]
    //public async Task DeleteAForecast()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();

    //    //Gets the Providers
    //    var _entityUIProvider = provider.GetService<IUIEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
    //    var entityUIProvider = (WeatherForecastUIEntityProvider)_entityUIProvider;

    //    var _entityProvider = provider.GetService<IEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
    //    var entityProvider = (WeatherForecastEntityProvider)_entityProvider;

    //    // Get the test item and it's Id from the Test Provider
    //    var testItem = _testDataProvider.WeatherForecasts.First();

    //    var testId = new WeatherForecastId(testItem.WeatherForecastID);

    //    // Get the current record count
    //    var CurrentItemCount = _testDataProvider.WeatherForecasts.Count();

    //    //Outputs from the process that need to be tested
    //    bool result = false;

    //    var uiBroker = await entityUIProvider.GetEntityEditUIBrokerAsync(testId);

    //    await uiBroker.DeleteItemAsync();


    //    await uiBroker.SaveItemAsync(true);

    //    result = false;
    //    uiBroker.LastResult.ApplySideEffect(
    //        success: () => result = true);

    //    // check the update was successful
    //    Assert.True(result);

    //    result = false;

    //    await entityProvider.RecordRequestAsync(testId)
    //        .OutputTaskAsync(
    //        failure: (ex) =>
    //        {
    //            result = true;
    //        });

    //    // check the query was successful
    //    Assert.True(result);

    //    result = false;
    //    ListItemsProvider<DmoWeatherForecast> listItemsProvider = default!;

    //    await Result<WeatherForecastListRequest>
    //        .Create(new()
    //        {
    //            PageSize = 1,
    //            StartIndex = 0,
    //        })
    //        .ApplyTransformOnException<ListItemsProvider<DmoWeatherForecast>>(entityProvider.ListItemsRequestAsync)
    //        .OutputTaskAsync(success: (provider) =>
    //        {
    //            listItemsProvider = provider;
    //            result = true;
    //        });

    //    // check the query was successful
    //    Assert.True(result);
    //    Assert.Equal(CurrentItemCount - 1, listItemsProvider.TotalCount);
    //}

    //[Fact]
    //public async Task AddAForecast()
    //{
    //    var provider = GetServiceProvider();

    //    //Gets the Providers
    //    var _entityProvider = provider.GetService<IEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
    //    var entityProvider = (WeatherForecastEntityProvider)_entityProvider;

    //    var _entityUIProvider = provider.GetService<IUIEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
    //    var entityUIProvider = (WeatherForecastUIEntityProvider)_entityUIProvider;

    //    // Get the current record count
    //    var CurrentItemCount = _testDataProvider.WeatherForecasts.Count();

    //    // Get the broker with a default Id - as we would in the UI
    //    // This creates an new entity 
    //    var uiBroker = await entityUIProvider.GetEntityEditUIBrokerAsync(WeatherForecastId.Default);

    //    // Update the EditMutator with the new record details as we would in the UI
    //    uiBroker.EditMutator.Date = DateTime.Now;
    //    uiBroker.EditMutator.Summary = "Test Add";
    //    uiBroker.EditMutator.Temperature = 30;

    //    bool result = false;

    //    await uiBroker.SaveItemAsync();

    //    uiBroker.LastResult.ApplySideEffect(
    //        success: () => result = true);

    //    WeatherForecastId newId = uiBroker.Id;

    //    var newRecord = new DmoWeatherForecast
    //    {
    //        Id = newId,
    //        Date = new(DateTime.Now),
    //        Summary = "Test Add",
    //        Temperature = new(30)
    //    };

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
    //    Assert.Equal(newRecord, dbRecord);

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
    //        .ApplyTransformOnException<ListItemsProvider<DmoWeatherForecast>>(entityProvider.ListItemsRequestAsync)
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
