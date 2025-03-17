/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.EntityFrameworkCore;

namespace Blazr.App.Infrastructure.Server;

public sealed class InMemoryInvoiceTestDbContext
    : DbContext
{
    public DbSet<DboCustomer> Customers { get; set; } = default!;

    public DbSet<CustomerLookUpItem> CustomerLookUp { get; set; } = default!;

    public DbSet<DboInvoice> Invoices { get; set; } = default!;

    public DbSet<DvoInvoice> DvoInvoices { get; set; } = default!;

    public DbSet<DboInvoiceItem> InvoiceItems { get; set; } = default!;

    public InMemoryInvoiceTestDbContext(DbContextOptions<InMemoryInvoiceTestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DboCustomer>().ToTable("Customers");
        modelBuilder.Entity<DboInvoice>().ToTable("Invoices");
        modelBuilder.Entity<DboInvoiceItem>().ToTable("InvoiceItems");
        modelBuilder.Entity<DvoInvoice>()
            .ToInMemoryQuery(()
                => from i in this.Invoices
                   join c in this.Customers! on i.CustomerID equals c.CustomerID
                   select new DvoInvoice
                   {
                       CustomerID = i.CustomerID,
                       CustomerName = c.CustomerName,
                       Date = i.Date,
                       InvoiceID = i.InvoiceID,
                       TotalAmount = i.TotalAmount,
                   }).HasKey(x => x.InvoiceID);

        modelBuilder.Entity<CustomerLookUpItem>()
            .ToInMemoryQuery(()
                => from z in this.Customers
                   select new CustomerLookUpItem
                   {
                       Id = z.CustomerID,
                       Name = z.CustomerName,
                   }).HasNoKey();
    }
}
