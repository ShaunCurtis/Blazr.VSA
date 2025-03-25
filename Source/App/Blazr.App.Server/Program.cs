/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Infrastructure.Server;
using Blazr.App.Invoice.Infrastructure.Server;
using Blazr.App.Weather.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

//builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAppServices();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapDefaultEndpoints();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Services.AddInvoiceTestData();
app.Services.AddWeatherTestData();
//// get the DbContext factory and add the test data
//var factory = app.Services.GetService<IDbContextFactory<InMemoryInvoiceTestDbContext>>();
//if (factory is not null)
//    InvoiceTestDataProvider.Instance().LoadDbContext<InMemoryInvoiceTestDbContext>(factory);

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<Blazr.App.Server.Components.ServerApp>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Blazr.App.Invoice.UI.InvoiceDashboard).Assembly, typeof(Blazr.App.Weather.Core.Date).Assembly);

app.Run();
