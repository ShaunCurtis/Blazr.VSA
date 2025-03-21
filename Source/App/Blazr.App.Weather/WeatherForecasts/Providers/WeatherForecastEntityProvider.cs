/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.App.Weather.Core;

namespace Blazr.App.Presentation;

public class WeatherForecastEntityProvider : IEntityProvider<DmoWeatherForecast, WeatherForecastId>
{
    private readonly IMediator _mediator;
    private IdentityId _ownerId = IdentityId.Default;

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

    public void SetOwnerIdContext(IdentityId ownerId)
    {
        _ownerId = ownerId;
    }

    public DmoWeatherForecast NewRecord
        => new DmoWeatherForecast { Id = WeatherForecastId.Default, OwnerId = _ownerId };
}
