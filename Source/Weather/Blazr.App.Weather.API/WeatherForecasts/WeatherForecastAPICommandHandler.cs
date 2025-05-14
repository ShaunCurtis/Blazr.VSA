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
/// Mediatr Handler for executing commands against a WeatherForecast Entity in an Entity Framework Context
/// </summary>
public sealed record WeatherForecastAPICommandHandler : IRequestHandler<WeatherForecastCommandRequest, Result<WeatherForecastId>>
{
    private readonly IMessageBus _messageBus;
    private readonly IHttpClientFactory _factory;
    private readonly AuthenticationStateProvider _authStateProvider;

    public WeatherForecastAPICommandHandler(IHttpClientFactory clientFactory, IMessageBus messageBus, AuthenticationStateProvider authenticationStateProvider)
    {
        _messageBus = messageBus;
        _factory = clientFactory;
        _authStateProvider = authenticationStateProvider;
    }

    public async Task<Result<WeatherForecastId>> HandleAsync(WeatherForecastCommandRequest request, CancellationToken cancellationToken)
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        using var http = _factory.CreateClient(AppDictionary.Common.WeatherHttpClient);
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.GetIdentityId().ToString());

        var httpResult = await http.PostAsJsonAsync<WeatherForecastCommandRequest>(AppDictionary.WeatherForecast.WeatherForecastCommandAPIUrl, request, cancellationToken);

        if (!httpResult.IsSuccessStatusCode)
            return Result<WeatherForecastId>.Fail(new DataPipelineException($"The server returned a status code of : {httpResult.StatusCode}"));

        var result = await httpResult.Content.ReadFromJsonAsync<Result<WeatherForecastId>>()
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return result ?? Result<WeatherForecastId>.Fail(new DataPipelineException($"No data was returned"));
    }
}
