/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Shared;
using Blazr.Auth.Core;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace Blazr.App.Weather.API;

/// <summary>
/// Mediatr Handler for executing list requests against a WeatherForecast Entity in a Entity Framework Context
/// </summary>
public sealed class WeatherForecastAPIListHandler : IRequestHandler<WeatherForecastListRequest, Result<ListItemsProvider<DmoWeatherForecast>>>
{
    private readonly IHttpClientFactory _factory;
    private readonly AuthenticationStateProvider _authStateProvider;

    public WeatherForecastAPIListHandler(IHttpClientFactory factory, AuthenticationStateProvider authenticationStateProvider)
    {
        _factory = factory;
        _authStateProvider = authenticationStateProvider;
    }

    public async Task<Result<ListItemsProvider<DmoWeatherForecast>>> Handle(WeatherForecastListRequest request, CancellationToken cancellationToken)
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        using var http = _factory.CreateClient(AppDictionary.Common.WeatherHttpClient);
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.GetIdentityId().ToString());

        var httpResult = await http.PostAsJsonAsync<WeatherForecastListRequest>(AppDictionary.WeatherForecast.WeatherForecastListAPIUrl, request, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!httpResult.IsSuccessStatusCode)
            return Result<ListItemsProvider<DmoWeatherForecast>>.Fail(new DataPipelineException($"The server returned a status code of : {httpResult.StatusCode}"));

        var listResult = await httpResult.Content.ReadFromJsonAsync<Result<ListItemsProvider<DmoWeatherForecast>>>()
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return listResult ?? Result<ListItemsProvider<DmoWeatherForecast>>.Fail(new DataPipelineException($"No data was returned"));
    }
}
