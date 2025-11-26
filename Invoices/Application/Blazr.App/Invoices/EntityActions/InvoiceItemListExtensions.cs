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
        items.Add(itemToAdd);
        return items;
    }

    public static Bool<DmoInvoiceItem> GetInvoiceItem(this InvoiceMutor mutor, DmoInvoiceItem item)
        => Bool<DmoInvoiceItem>.Read( 
            value: mutor.CurrentEntity.InvoiceItems.SingleOrDefault(_item => _item.Id == item.Id), 
            errorMessage: "The record does not exist in the Invoice Items");

    public static Bool<DmoInvoiceItem> CheckInvoiceItemExists(this InvoiceMutor mutor, DmoInvoiceItem item)
        => mutor.CurrentEntity.InvoiceItems.Any(_item => item.Id.Equals(_item.Id))
            ? Bool<DmoInvoiceItem>.Success(item)
            : Bool<DmoInvoiceItem>.Failure("Invoice Item does not exist");

    public static Bool<DmoInvoiceItem> CheckInvoiceItemDoesNotExist(this InvoiceMutor mutor, DmoInvoiceItem item)
        => mutor.CurrentEntity.InvoiceItems.Any(_item => item.Id.Equals(_item.Id))
            ? Bool<DmoInvoiceItem>.Failure("Invoice Item already exists")
            : Bool<DmoInvoiceItem>.Success(item);

    public static Bool<DmoInvoiceItem> GetInvoiceItem(this InvoiceEntity entity, DmoInvoiceItem item)
        => Bool<DmoInvoiceItem>.Read(
            value: entity.InvoiceItems.SingleOrDefault(_item => _item.Id == item.Id),
            errorMessage: "The record does not exist in the Invoice Items");

    public static Bool<DmoInvoiceItem> GetInvoiceItem(this InvoiceEntity entity, InvoiceItemId id)
        => Bool<DmoInvoiceItem>.Read(
            value: entity.InvoiceItems.SingleOrDefault(_item => _item.Id == id),
            errorMessage: "The record does not exist in the Invoice Items");

    public static Bool<DmoInvoiceItem> CheckInvoiceItemExists(this InvoiceEntity entity, DmoInvoiceItem item)
        => entity.InvoiceItems.Any(_item => item.Id.Equals(_item.Id))
            ? Bool<DmoInvoiceItem>.Success(item)
            : Bool<DmoInvoiceItem>.Failure("Invoice Item does not exist");

    public static Bool<DmoInvoiceItem> CheckInvoiceItemDoesNotExist(this InvoiceEntity entity, DmoInvoiceItem item)
        => entity.InvoiceItems.Any(_item => item.Id.Equals(_item.Id))
            ? Bool<DmoInvoiceItem>.Failure("Invoice Item already exists")
            : Bool<DmoInvoiceItem>.Success(item);
}
