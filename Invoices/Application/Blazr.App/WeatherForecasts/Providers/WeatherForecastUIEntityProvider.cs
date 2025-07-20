/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.Cadmium;
using Blazr.Cadmium.Core;
using Blazr.Cadmium.Presentation;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.App.UI;

public sealed record WeatherForecastUIEntityProvider : IUIEntityProvider<DmoWeatherForecast, WeatherForecastId>
{
    private readonly IServiceProvider _serviceProvider;

    public string SingleDisplayName { get; } = "Weather Forecast";
    public string PluralDisplayName { get; } = "Weather Forecasts";
    public Type? EditForm { get; } = typeof(WeatherForecastEditForm);
    public Type? ViewForm { get; } = typeof(WeatherForecastViewForm);
    public string Url { get; } = "/weatherForecast";

    public WeatherForecastUIEntityProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<IReadUIBroker<DmoWeatherForecast, WeatherForecastId>> GetReadUIBrokerAsync(WeatherForecastId id)
    {
        var presenter = ActivatorUtilities.CreateInstance<ReadUIBroker<DmoWeatherForecast, WeatherForecastId>>(_serviceProvider);
        await presenter.LoadAsync(id);

        return presenter;
    }

    public ValueTask<IGridUIBroker<DmoWeatherForecast>> GetGridUIBrokerAsync()
    {
        var presenter = ActivatorUtilities.CreateInstance<GridUIBroker<DmoWeatherForecast, WeatherForecastId>>(_serviceProvider);

        return ValueTask.FromResult<IGridUIBroker<DmoWeatherForecast>>(presenter);
    }

    public async ValueTask<WeatherForecastEntityEditUIBroker> GetEntityEditUIBrokerAsync(WeatherForecastId id)
    {
        var presenter = ActivatorUtilities.CreateInstance<WeatherForecastEntityEditUIBroker>(_serviceProvider);
        await presenter.LoadAsync(id);
        return presenter;
    }

    public async ValueTask<IEditUIBroker<TEditContext, WeatherForecastId>> GetEditUIBrokerAsync<TEditContext>(WeatherForecastId id)
        where TEditContext : IRecordEditContext<DmoWeatherForecast>, new()
    {
        var presenter = ActivatorUtilities.CreateInstance<EditUIBroker<DmoWeatherForecast, TEditContext, WeatherForecastId>>(_serviceProvider);
        await presenter.LoadAsync(id);
        return presenter;
    }
}
