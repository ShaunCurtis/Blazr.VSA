using Blazr.App.Infrastructure.Server;
using Blazr.App.Invoice.Infrastructure;
using Blazr.App.Invoice.Infrastructure.Server;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

//builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAppServices();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// get the DbContext factory and add the test data
var factory = app.Services.GetService<IDbContextFactory<InMemoryInvoiceTestDbContext>>();
if (factory is not null)
    InvoiceTestDataProvider.Instance().LoadDbContext<InMemoryInvoiceTestDbContext>(factory);

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<Blazr.App.Server.Components.ServerApp>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(
    typeof(Blazr.App.Invoice.UI.InvoiceDashboard).Assembly 
    //,typeof(Blazr.App.Weather.Core.Date).Assembly
    );

app.Run();
