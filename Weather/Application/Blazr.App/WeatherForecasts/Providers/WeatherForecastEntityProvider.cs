/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.Cadmium;
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode.Mediator;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.App.Presentation;

public class WeatherForecastEntityProvider
   : EntityProvider<DmoWeatherForecast>,
    IEntityProvider<DmoWeatherForecast, WeatherForecastId>
{
    public WeatherForecastEntityProvider(IMediatorBroker mediator, IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _serviceProvider = serviceProvider;
    }

    //public Func<GridState<DmoWeatherForecast>, Task<Result<GridItemsProviderResult<DmoWeatherForecast>>>> GetItemsAsync1
    //    => (state) => _mediator.Send(new WeatherForecastListRequest()
    //        {
    //            PageSize = state.PageSize,
    //            StartIndex = state.StartIndex,
    //            SortColumn = state.SortField,
    //            SortDescending = state.SortDescending
    //        })
    //    .MapTaskAsync<GridItemsProviderResult<DmoWeatherForecast>>(FromListItemsProvider)
    //        ;

    public async Task<Result<GridItemsProviderResult<DmoWeatherForecast>>> GetItemsAsync(GridState<DmoWeatherForecast> state)
    {
        var asyncResult = await _mediator.Send(new WeatherForecastListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });

        return asyncResult.ApplyTransform<GridItemsProviderResult<DmoWeatherForecast>>(FromListItemsProvider);
    }

    public Func<WeatherForecastId, Task<Result<WeatherForecastEntity>>> EntityRequestAsync
        => (id) => id.IsDefault ? NewEntityRequestAsync(id) : ExistingEntityRequestAsync(id);

    public Func<WeatherForecastEntity, Task<Result<WeatherForecastId>>> EntityCommandAsync
        => (record) => _mediator.Send(new WeatherForecastEntityCommandRequest(record));

    public Func<WeatherForecastId, Task<Result<DmoWeatherForecast>>> RecordRequestAsync
        => (id) => id.IsDefault ? NewRecordRequestAsync(id) : ExistingRecordRequestAsync(id);

    public Func<StateRecord<DmoWeatherForecast>, Task<Result<WeatherForecastId>>> RecordCommandAsync
        => (record) => _mediator.Send(new WeatherForecastCommandRequest(record));

    public Func<GridState<DmoWeatherForecast>, Task<Result<ListItemsProvider<DmoWeatherForecast>>>> GridItemsRequestAsync
        => (state) => _mediator.Send(new WeatherForecastListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });

    public Func<WeatherForecastListRequest, Task<Result<ListItemsProvider<DmoWeatherForecast>>>> ListItemsRequestAsync
        => (request) => _mediator.Send(request);

    public async ValueTask<Result<WeatherForecastEntity>> GetEntityAsync(WeatherForecastId id)
    {
        var result = (await _mediator.Send(new WeatherForecastRecordRequest(id)))
            .ApplyTransform<WeatherForecastEntity>((record) =>
            {
                WeatherForecastEntity.Load(record);
                return Result<WeatherForecastEntity>.Failure($"No entity exists for Id{id}.  Created default entity.");
            });

        return result;
    }

    public Result<WeatherForecastId> GetKey(object? obj)
        => obj switch
        {
            WeatherForecastId id => Result<WeatherForecastId>.Create(id),
            DmoWeatherForecast record => Result<WeatherForecastId>.Create(record.Id),
            Guid guid => Result<WeatherForecastId>.Create(new(guid)),
            _ => Result<WeatherForecastId>.Failure($"Could not convert the provided key - {obj?.ToString()}")
        };

    public DmoWeatherForecast NewRecord
        => new DmoWeatherForecast { Id = WeatherForecastId.Default };

    private readonly IMediatorBroker _mediator;
    private readonly IServiceProvider _serviceProvider;

    private Func<WeatherForecastId, Task<Result<DmoWeatherForecast>>> ExistingRecordRequestAsync
        => (id) => _mediator.Send(new WeatherForecastRecordRequest(id));

    private Func<WeatherForecastId, Task<Result<DmoWeatherForecast>>> NewRecordRequestAsync
        => (id) => Task.FromResult(Result<DmoWeatherForecast>.Create(new DmoWeatherForecast { Id = WeatherForecastId.Default }));

    private Func<WeatherForecastId, Task<Result<WeatherForecastEntity>>> ExistingEntityRequestAsync
        => (id) => _mediator.Send(new WeatherForecastEntityRequest(id));

    private Func<WeatherForecastId, Task<Result<WeatherForecastEntity>>> NewEntityRequestAsync
        => (id) => Task.FromResult(Result<WeatherForecastEntity>.Create(WeatherForecastEntity.Create()));
}
