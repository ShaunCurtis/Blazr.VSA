/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.EntityFrameworkCore;

namespace Blazr.App.Infrastructure.Server;

public sealed class InvoiceTestDataProvider
{
    public IEnumerable<DboCustomer> Customers => _customers.AsEnumerable();
    public IEnumerable<DboInvoice> Invoices => _invoices.AsEnumerable();
    public IEnumerable<DboInvoiceItem> InvoiceItems => _invoiceItems.AsEnumerable();

    private List<DboCustomer> _customers = new List<DboCustomer>();
    private List<DboInvoice> _invoices = new List<DboInvoice>();
    private List<DboInvoiceItem> _invoiceItems = new List<DboInvoiceItem>();

    public InvoiceTestDataProvider()
    {
        this.Load();
    }

    private void Load()
    {
        _customers = new();

        var id = Guid.CreateVersion7();
        DboCustomer customer = new()
        {
            CustomerID = id,
            CustomerName = "EasyJet"
        };
        _customers.Add(customer);

        {
            var _id = Guid.CreateVersion7();
            _invoices.Add(new()
            {
                InvoiceID = _id,
                CustomerID = id,
                Date = DateTime.Now.AddDays(-3),
                TotalAmount = 50
            });
            _invoiceItems.Add(new()
            {
                InvoiceItemID = Guid.CreateVersion7(),
                InvoiceID = _id,
                Description = "Airbus A321",
                Amount = 15
            });
            _invoiceItems.Add(new()
            {
                InvoiceItemID = Guid.CreateVersion7(),
                InvoiceID = _id,
                Description = "Airbus A350",
                Amount = 35
            });
        }

        id = Guid.CreateVersion7();
        customer = new()
        {
            CustomerID = id,
            CustomerName = "RyanAir"
        };
        _customers.Add(customer);

        {
            var _id = Guid.CreateVersion7();
            _invoices.Add(new()
            {
                InvoiceID = _id,
                CustomerID = id,
                Date = DateTime.Now.AddDays(-2),
                TotalAmount = 27
            });
            _invoiceItems.Add(new()
            {
                InvoiceItemID = Guid.CreateVersion7(),
                InvoiceID = _id,
                Description = "Airbus A319",
                Amount = 12
            });
            _invoiceItems.Add(new()
            {
                InvoiceItemID = Guid.CreateVersion7(),
                InvoiceID = _id,
                Description = "Airbus A321",
                Amount = 15
            });
        }

        id = Guid.CreateVersion7();
        customer = new()
        {
            CustomerID = id,
            CustomerName = "Air France"
        };
        _customers.Add(customer);

        {
            var _id = Guid.CreateVersion7();
            _invoices.Add(new()
            {
                InvoiceID = _id,
                CustomerID = id,
                Date = DateTime.Now.AddDays(-1),
                TotalAmount = 60
            });
            _invoiceItems.Add(new()
            {
                InvoiceItemID = Guid.CreateVersion7(),
                InvoiceID = _id,
                Description = "Airbus A330",
                Amount = 25
            });
            _invoiceItems.Add(new()
            {
                InvoiceItemID = Guid.CreateVersion7(),
                InvoiceID = _id,
                Description = "Airbus A350",
                Amount = 35
            });
        }
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "TAP" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "American Airlines" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "Quantas" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "Virgin" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "Lufhansa" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "SAS" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "Emirates" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "Etihad" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "Cathay Pacific" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "Singapore Airlines" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "United Airlines" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "Alaskan" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "Logan Air" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "Qatar Airlines" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "Air Egypt" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "Iberia" });
        _customers.Add(new() { CustomerID = Guid.CreateVersion7(), CustomerName = "Alitalia" });

    }

    public void LoadDbContext<TDbContext>(IDbContextFactory<TDbContext> factory) where TDbContext : DbContext
    {
        using var dbContext = factory.CreateDbContext();

        var dboCustomers = dbContext.Set<DboCustomer>();
        var dboInvoices = dbContext.Set<DboInvoice>();
        var dboInvoiceItems = dbContext.Set<DboInvoiceItem>();

        // Check if we already have a full data set
        // If not clear down any existing data and start again
        if (dboCustomers.Count() == 0)
            dbContext.AddRange(_customers);

        if (dboInvoices.Count() == 0)
            dbContext.AddRange(_invoices);

        if (dboInvoiceItems.Count() == 0)
            dbContext.AddRange(_invoiceItems);

        dbContext.SaveChanges();
    }

    private static InvoiceTestDataProvider? _provider;

    public static InvoiceTestDataProvider Instance()
    {
        if (_provider is null)
            _provider = new InvoiceTestDataProvider();

        return _provider;
    }
}