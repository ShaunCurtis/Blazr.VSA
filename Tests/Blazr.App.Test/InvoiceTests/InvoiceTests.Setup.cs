/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.EntityFramework;
using Blazr.App.Infrastructure;
using Blazr.Diode.Mediator;
using Blazr.Gallium;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
namespace Blazr.Test;

using Blazr.Manganese;

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
        services.AddAppBlazorServerServices();
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
        var controlId = InvoiceId.Load(_testDataProvider.Invoices.Skip(Random.Shared.Next(3)).First().InvoiceID);

        // Get the Invoice Entity
        var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(controlId));

        Assert.False(entityResult.HasException);

        return entityResult.AsSuccess.Value;
    }

    private async Task<InvoiceEntity> GetASampleDirtyEntityAsync(IMediatorBroker mediator)
    {

        var controlId = InvoiceId.Load(_testDataProvider.Invoices.Skip(3).First().InvoiceID);

        // Get the Invoice Entity
        var entityResult = await mediator.DispatchAsync(new InvoiceEntityRequest(controlId));

        Assert.False(entityResult.HasException);

        return entityResult.AsSuccess.Value;
    }

    private DmoInvoice AsDmoInvoice(DboInvoice invoice)
    {
        var customer = _testDataProvider.Customers.First(item => item.CustomerID == invoice.CustomerID);
        return new DmoInvoice
        {
            Id = InvoiceId.Load(invoice.InvoiceID),
            Customer = AsCustomer(customer),
            Date = new Date(invoice.Date),
            TotalAmount = new Money(invoice.TotalAmount),
        };
    }

    private DmoInvoiceItem AsDmoInvoiceItem(DboInvoiceItem invoiceItem)
        => new DmoInvoiceItem
        {
            Id = InvoiceItemId.Load(invoiceItem.InvoiceItemID),
            InvoiceId = InvoiceId.Load(invoiceItem.InvoiceID),
            Description = new(invoiceItem.Description),
            Amount = new Money(invoiceItem.Amount),
        };

    private FkoCustomer AsCustomer(DboCustomer customer)
        => new FkoCustomer
        (
             CustomerId.Load(customer.CustomerID),
             new(customer.CustomerName)
        );
}
