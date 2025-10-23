/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.EntityFramework;
using Blazr.App.Infrastructure;
using Blazr.Cadmium.Presentation;
using Blazr.Diode.Mediator;
using Blazr.Gallium;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.Logging;
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
                typeof(Blazr.App.EntityFramework.AppServerServices).Assembly
        });

        // Add the Gallium Message Bus Server services
        services.AddScoped<IMessageBus, MessageBus>();

        // InMemory Scoped State Store 
        services.AddScoped<ScopedStateProvider>();

        // Presenter Factories
        services.AddScoped<ILookupUIBrokerFactory, LookupUIBrokerFactory>();

        services.AddAppEFServices();
        services.AddLogging(builder => builder.AddDebug());

        var provider = services.BuildServiceProvider();

        // get the DbContext factory and add the test data
        var factory = provider.GetService<IDbContextFactory<InMemoryInvoiceTestDbContext>>();
        if (factory is not null)
            InvoiceTestDataProvider.Instance().LoadDbContext<InMemoryInvoiceTestDbContext>(factory);

        return provider!;
    }

    private DmoInvoice AsDmoInvoice(DboInvoice invoice)
    {
        var customer = _testDataProvider.Customers.First(item => item.CustomerID == invoice.CustomerID);
        return new DmoInvoice
           {
               Id = new InvoiceId(invoice.InvoiceID),
               Customer = AsInvoiceCustomer(customer),
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

    private InvoiceCustomer AsInvoiceCustomer(DboCustomer customer)
        => new InvoiceCustomer
        (
             new(customer.CustomerID),
             new(customer.CustomerName)
        );
}
