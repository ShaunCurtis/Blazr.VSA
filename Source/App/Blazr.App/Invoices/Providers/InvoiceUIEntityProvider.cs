/// ============================================================
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.UI;

public sealed record InvoiceUIEntityProvider : IUIEntityProvider<DmoInvoice>
{
    public string SingleDisplayName { get; } = "Invoice";
    public string PluralDisplayName { get; } = "Invoices";
    public Type? EditForm { get; } = null;
    public Type? ViewForm { get; } = null;
    public string Url { get; } = "/invoice";
}
