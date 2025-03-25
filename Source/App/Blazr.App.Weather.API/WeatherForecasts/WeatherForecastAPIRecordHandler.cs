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
/// Mediatr Handler for executing record requests to get a WeatherForecast Entity in an Entity Framework Context
/// </summary>
public sealed class WeatherForecastAPIRecordHandler : IRequestHandler<WeatherForecastRecordRequest, Result<DmoWeatherForecast>>
{
    private readonly IHttpClientFactory _factory;
    private readonly AuthenticationStateProvider _authStateProvider;

    public WeatherForecastAPIRecordHandler(IHttpClientFactory dbContextFactory, AuthenticationStateProvider authenticationStateProvider)
    {
        _factory = dbContextFactory;
        _authStateProvider = authenticationStateProvider;
    }

    public async Task<Result<DmoWeatherForecast>> Handle(WeatherForecastRecordRequest request, CancellationToken cancellationToken)
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        using var http = _factory.CreateClient(AppDictionary.Common.WeatherHttpClient);
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.GetIdentityId().ToString());

        var httpResult = await http.PostAsJsonAsync<WeatherForecastRecordRequest>(AppDictionary.WeatherForecast.WeatherForecastRecordAPIUrl, request, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!httpResult.IsSuccessStatusCode)
            return Result<DmoWeatherForecast>.Fail(new DataPipelineException($"The server returned a status code of : {httpResult.StatusCode}"));

        var result = await httpResult.Content.ReadFromJsonAsync<Result<DmoWeatherForecast>>()
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return result ?? Result<DmoWeatherForecast>.Fail(new DataPipelineException($"No data was returned"));
    }
}
