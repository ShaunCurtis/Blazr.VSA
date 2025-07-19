# API Implementation

There are two projects that implement the necessary API functionality.

1. **Blazr.App.API** defines the server side API Endpoints.
2. **Blazr.App.Infratructure.API** defines the client side *Mediatr* Handlers.

The WebAssembly deployment project references the *Blazr.App.Infratructure* and *Blazr.App.Infratructure.API* projects, and configures Mediatr to scan the Infratructure.API project to register the defined *HttpClient* based handlers.

```csharp
var assemblies = new[] { 
    typeof(DmoWeatherForecast).Assembly,
    typeof(DboWeatherForecast).Assembly,
    typeof(ApplicationApiInfrastructureServices).Assembly 
};

services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(assemblies)
);
``` 

Here's the `WeatherForecastAPIItemRequestHandler`.  It uses the `IHttpClientFactory` to create an `HttpClient` to make the API call.  The Mediatr request, `WeatherForecastItemRequest`, is passed to the API.

```csharp
public class WeatherForecastAPIItemRequestHandler : IRequestHandler<WeatherForecastItemRequest, Result<DmoWeatherForecast>>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherForecastAPIItemRequestHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Result<DmoWeatherForecast>> Handle(WeatherForecastItemRequest request, CancellationToken cancellationToken)
    {
        using var http = _httpClientFactory.CreateClient(AppDictionary.Common.WeatherHttpClient);

        var httpResult = await http.PostAsJsonAsync<WeatherForecastItemRequest>(AppDictionary.WeatherForecast.WeatherForecastItemAPIUrl, request, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!httpResult.IsSuccessStatusCode)
            return Result<DmoWeatherForecast>.Fail( new ItemQueryException($"The server returned a status code of : {httpResult.StatusCode}"));

        var listResult = await httpResult.Content.ReadFromJsonAsync<Result<DmoWeatherForecast>>()
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return listResult;
    }
}
```

