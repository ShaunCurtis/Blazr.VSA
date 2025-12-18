/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App;
using Blazr.App.Core;
using Blazr.App.EntityFramework;
using Blazr.App.Infrastructure;
using Blazr.Cadmium.Presentation;
using Blazr.Diode.Mediator;
using Blazr.Gallium;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
namespace Blazr.Test;

public partial class InvoiceTests
{
    private InvoiceTestDataProvider _testDataProvider;

    public InvoiceTests()
        => _testDataProvider = InvoiceTestDataProvider.Instance();

    private ServiceProvider GetServiceProvider()
    {
        var services = new ServiceCollection();

        // Add Blazor Mediator Service
        services.AddMediator(new Assembly[] {
                typeof(Blazr.App.EntityFramework.AppEFServerServices).Assembly
        });

        // Add the Gallium Message Bus Server services
        services.AddScoped<IMessageBus, MessageBus>();

        // InMemory Scoped State Store 
        services.AddScoped<ScopedStateProvider>();

        services.AddAppEFServices();
        services.AddLogging(builder => builder.AddDebug());

        var provider = services.BuildServiceProvider();

        // get the DbContext factory and add the test data
        var factory = provider.GetService<IDbContextFactory<InMemoryInvoiceTestDbContext>>();
        if (factory is not null)
            InvoiceTestDataProvider.Instance().LoadDbContext<InMemoryInvoiceTestDbContext>(factory);

        return provider!;
    }

    private async Task<InvoiceEntity> GetASampleEntityAsync(IMediatorBroker mediator)
    {

        // Get a test item and it's Id from the Test Provider
        var controlId = new InvoiceId(_testDataProvider.Invoices.Skip(Random.Shared.Next(3)).First().InvoiceID);

        // Get the Invoice Entity
        var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(controlId));

        Assert.False(entityResult.HasException);

        return entityResult.Value!;
    }

    private DmoInvoice AsDmoInvoice(DboInvoice invoice)
    {
        var customer = _testDataProvider.Customers.First(item => item.CustomerID == invoice.CustomerID);
        return new DmoInvoice
           {
               Id = new InvoiceId(invoice.InvoiceID),
               Customer = AsCustomer(customer),
               Date = new Date(invoice.Date),
               TotalAmount = new Money(invoice.TotalAmount),
           };
    }

    private DmoInvoiceItem AsDmoInvoiceItem(DboInvoiceItem invoiceItem)
        => new DmoInvoiceItem
        {
            Id = new InvoiceItemId(invoiceItem.InvoiceItemID),
            InvoiceId = new InvoiceId(invoiceItem.InvoiceID),
            Description = new(invoiceItem.Description),
            Amount = new Money(invoiceItem.Amount),
        };

    private FkoCustomer AsCustomer(DboCustomer customer)
        => new FkoCustomer
        (
             new(customer.CustomerID),
             new(customer.CustomerName)
        );
}
