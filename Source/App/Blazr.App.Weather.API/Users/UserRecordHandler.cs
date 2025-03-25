using System.Net.Http;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Weather.API;

/// <summary>
/// Mediatr Handler for executing record requests to get a WeatherForecast Entity
/// </summary>
public sealed class UserRecordAPIHandler : IRequestHandler<UserRecordRequest, Result<DmoUser>>
{
    private IHttpClientFactory _factory;

    public UserRecordAPIHandler(IHttpClientFactory clientFactory)
    {
        _factory = clientFactory;
    }

    public async Task<Result<DmoUser>> Handle(UserRecordRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }
}
