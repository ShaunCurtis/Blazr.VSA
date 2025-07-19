using Blazr.App.Presentation;
using Blazr.App.UI;
using Blazr.Cadmium.Presentation;
using Blazr.Gallium;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Blazr.Diode.Mediator;
using Blazr.Weather.Server;
using Blazr.App.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var services = builder.Services;


builder.Services.AddAppServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Services.AddWeatherTestData();

//// get the DbContext factory and add the test data
//var factory = app.Services.GetService<IDbContextFactory<InMemoryWeatherTestDbContext>>();
//if (factory is not null)
//    WeatherTestDataProvider.Instance().LoadDbContext<InMemoryWeatherTestDbContext>(factory);

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<Blazr.Weather.Server.Components.App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Blazr.App.WeatherForecastServices).Assembly);

app.Run();
