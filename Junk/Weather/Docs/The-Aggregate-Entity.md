# The Aggregate Entity

Updating a simple object, where the consequences of the change are limited to the object itself, is simple.  Updating an object that is an intricate part of a business process, and has consequences on other objects, is complex.  The aggregate pattern is designed to address that problem.

> An aggregate is a group of objects bound by one or more application rules.  The purpose of the aggregate is to ensure those rules are applied, and cannot be broken.  
 
Aggregates are black boxes.  Changes are submitted to the black box, not the individual objects within it.  The black box applies the changes and runs any logic to ensure consistency of the entities within the box.

Be aware, an aggregate only has purpose in a mutation context: you don't need aggregates to list or display data.  

## Data Objects

All data objects are immutable records.  Data is retrieved from the data store by submitting record query request to the the Mediator service.

```csharp
public readonly record struct WeatherForecastRecordRequest(WeatherForecastId Id)
    : IRequest<Result<DmoWeatherForecast>>;
```

We pass in a `WeatherForecastId` and get back a `DmoWeatherForecast` - a *Dmo* object [Data Model Object].

 