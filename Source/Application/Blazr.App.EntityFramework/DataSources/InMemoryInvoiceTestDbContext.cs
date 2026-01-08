/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.EntityFramework;

public sealed class InMemoryInvoiceTestDbContext
    : DbContext
{
    internal DbSet<DboCustomer> Customers { get; set; } = default!;

    internal DbSet<DvoCustomer> DvoCustomers { get; set; } = default!;

    public DbSet<FkCustomer> FkCustomers { get; set; } = default!;

    public DbSet<DboInvoice> Invoices { get; set; } = default!;

    public DbSet<DvoInvoice> DvoInvoices { get; set; } = default!;

    public DbSet<DboInvoiceItem> InvoiceItems { get; set; } = default!;
    public DbSet<DvoInvoiceItem> DvoInvoiceItems { get; set; } = default!;

    public InMemoryInvoiceTestDbContext(DbContextOptions<InMemoryInvoiceTestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DboCustomer>().ToTable("Customers");
        modelBuilder.Entity<DvoCustomer>()
            .ToInMemoryQuery(()
                => from c in this.Customers
                   select new DvoCustomer
                   {
                       CustomerID = c.CustomerID,
                       CustomerName = c.CustomerName,
                   }).HasKey(x => x.CustomerID);

        modelBuilder.Entity<FkCustomer>()
            .ToInMemoryQuery(()
                => from z in this.Customers
                   select new FkCustomer
                   {
                       Id = z.CustomerID,
                       Name = z.CustomerName,
                   }).HasNoKey();

        modelBuilder.Entity<DboInvoice>().ToTable("Invoices");

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

        modelBuilder.Entity<DboInvoiceItem>().ToTable("InvoiceItems");

        modelBuilder.Entity<DvoInvoiceItem>()
            .ToInMemoryQuery(()
                => from i in this.InvoiceItems
                   select new DvoInvoiceItem
                   {
                       Amount = i.Amount,
                       Description = i.Description,
                       InvoiceItemID = i.InvoiceItemID,
                       InvoiceID = i.InvoiceID,
                   }).HasKey(x => x.InvoiceItemID);
    }
}
