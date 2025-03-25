using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazr.App.Weather.API;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddWeatherAppAPIServices(builder.HostEnvironment.BaseAddress);

await builder.Build().RunAsync();
