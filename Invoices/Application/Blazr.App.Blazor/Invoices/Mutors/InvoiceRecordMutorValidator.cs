/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.UI;

public class InvoiceRecordMutorValidator : AbstractValidator<InvoiceRecordMutor>
{
    public InvoiceRecordMutorValidator()
    {
        this.RuleFor(p => p.Customer.Name.Value)
            .MinimumLength(3)
            .WithState(p => p);
    }
}
