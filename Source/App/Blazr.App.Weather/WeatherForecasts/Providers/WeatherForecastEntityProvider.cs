/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Presentation;

namespace Blazr.App.Core;

public class WeatherForecastEntityProvider : IEntityProvider<DmoWeatherForecast, WeatherForecastId>
{
    private readonly IMediator _mediator;

    public Func<WeatherForecastId, Task<Result<DmoWeatherForecast>>> RecordRequest
        => (id) => _mediator.Send(new WeatherForecastRecordRequest(id));

    public Func<DmoWeatherForecast, CommandState, Task<Result<WeatherForecastId>>> RecordCommand
        => (record, state) => _mediator.Send(new WeatherForecastCommandRequest(record, state));

    public Func<GridState<DmoWeatherForecast>, Task<Result<ListItemsProvider<DmoWeatherForecast>>>> ListRequest
        => (state) => _mediator.Send(new WeatherForecastListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });

    public WeatherForecastEntityProvider(IMediator mediator)
    {
        _mediator = mediator;
    }

    public WeatherForecastId GetKey(object obj)
    {
        return obj switch
        {
            WeatherForecastId id => id,
            DmoWeatherForecast record => record.Id,
            Guid guid => new WeatherForecastId(guid),
            _ => WeatherForecastId.Default
        };
    }

    public bool TryGetKey(object obj, out WeatherForecastId key)
    {
        key = GetKey(obj);
        return key != WeatherForecastId.Default;
    }

    public DmoWeatherForecast NewRecord
        => DefaultRecord;

    public static DmoWeatherForecast DefaultRecord
        => new DmoWeatherForecast { Id = WeatherForecastId.Default };
}
