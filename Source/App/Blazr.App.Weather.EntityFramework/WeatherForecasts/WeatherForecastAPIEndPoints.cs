/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Auth.Core;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blazr.App.Weather.EntityFramework;

public static class WeatherForecastAPIEndPoints
{
    public static void AddWeatherForecastAPIEndpoints(this WebApplication app)
    {
        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastAliveAPIUrl, (CancellationToken cancellationToken) =>
        {
            return Results.Ok("Alive");
        });

        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastListAPIUrl, async ([FromBody] WeatherForecastListRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(request, cancellationToken);
            return Results.Ok(result);
        })
            .RequireAuthorization(@AppPolicies.IsViewerPolicy);

        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastRecordAPIUrl, async ([FromBody] WeatherForecastRecordRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(request, cancellationToken);
            return Results.Ok(result);
        })
            .RequireAuthorization(@AppPolicies.IsViewerPolicy);


        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastCommandAPIUrl, async ([FromBody] WeatherForecastCommandRequest request, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(request, cancellationToken);
            return Results.Ok(result);
        })
            .RequireAuthorization(@AppPolicies.IsEditorPolicy);
    }
}