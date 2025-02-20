#  Data Pipeline

The application data pipeline is a composite design incorporating patterns from several frameworks.

- Entity Framework is used as the **Object Request Mapper**, overlayed by a **Command/Query Separation** implementation coupled with **Mediatr**. 

- Entity Framework implements the basic repository and unit of work patterns.  CQS proides a thin layer over EF to separate commands from queries.  

- Mediatr is used to implement the CQS pattern and decouple the front and back end.


All data within the data pipeline is *READONLY*: declared as either `record` or `readonly struct`.  Data retrieved from a data source is a **copy** of the data within the data source.  You don't mutate the source data by changing the copy: you pass a mutated copy to the data store to replace the existing data.

The data pipeline performs two basic activities:

1. Querying for single items or lists of items - retrieve a *record* or a *list*.
2. Submitting conmands to change data. - *add*, *update* or *delete* a record.

## Querying

### Querying for a list of items

UI's use various types of grids and tables to display list data.  Each implement specific request and result objects to request data and get results.

They all implement:

1. Paging
1. Sorting
1. Filtering 

And return:

1. A paged data set
2. The total record count in the dataset.

The pipeline implements it's own request and result objects.

The ListPresenter implementations manage the in and out mappings between the request and result objects specific to the grid implementations and the pipeline objects.  In the solution there are implementations for FluentUI, MudBlazor and QuickGrid. 

The service definition for the WeatherForecast FluentUI record is:

```csharp
services.AddTransient<IFluentGridPresenter<DmoWeatherForecast>, FluentGridPresenter<DmoWeatherForecast>>();
```

### Querying for an item

Queries for an item from the UI use the generic `IViewPresenter<TRecord, TKey>`.

```csharp
public interface IViewPresenter<TRecord, TKey>
    where TRecord : class, new()
    where TKey : IEntityKey
{
    public IDataResult LastDataResult { get; }
    public TRecord Item { get; }

    public Task LoadAsync(TKey id);
}
```

The service definition for the WeatherForecast record is:

```csharp
services.AddTransient<IViewPresenter<DmoWeatherForecast, WeatherForecastId>, ViewPresenter<DmoWeatherForecast, WeatherForecastId>>();
```

### Editing a Record

There is no defined generic presenter.  All presenters are record specific.

```csharp
services.AddTransient<WeatherForecastEditPresenter>();
```
