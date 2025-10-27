/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public static class InvoiceItemListExtensions
{
    public static List<DmoInvoiceItem> RemoveItem(this List<DmoInvoiceItem> items, DmoInvoiceItem itemToRemove)
    {
        var newList = items.ToList();
        if (newList.Remove(itemToRemove))
            return newList;
        return items;
    }

    public static List<DmoInvoiceItem> AddItem(this List<DmoInvoiceItem> items, DmoInvoiceItem itemToAdd)
    {
        var newList = items.ToList();
        return newList;
    }

    public static Result<DmoInvoiceItem> GetInvoiceItem(this InvoiceMutor mutor, DmoInvoiceItem item)
        => Result<DmoInvoiceItem>.Create(mutor.CurrentEntity.InvoiceItems.SingleOrDefault(_item => _item.Id == item.Id), "The record does not exist in the Invoice Items");

    public static bool ContainsInvoiceItem(this InvoiceMutor mutor, DmoInvoiceItem item)
        => mutor.CurrentEntity.InvoiceItems.Any(_item => item.Id.Equals(_item.Id));

    public static bool ContainsItem(this List<DmoInvoiceItem> items, DmoInvoiceItem item)
        => items.Any(_item => item.Id.Equals(_item.Id));
}
