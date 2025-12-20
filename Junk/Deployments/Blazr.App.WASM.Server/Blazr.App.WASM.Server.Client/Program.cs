using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazr.App.Weather.API;
using Blazr.App.WASM.Server.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAppServices(builder.HostEnvironment.BaseAddress);

await builder.Build().RunAsync();
