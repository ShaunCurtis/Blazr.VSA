/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using FluentValidation;

namespace Blazr.App.Invoice.Core;

public sealed class DmoInvoiceEditContextValidator : AbstractValidator<DmoInvoiceEditContext>
{
    public DmoInvoiceEditContextValidator()
    {
        this.RuleFor(p => p.CustomerId)
            .NotEmpty()
            .WithState(p => p);

        this.RuleFor(p => p.Date)
            .NotNull()
            .WithState(p => p);
    }
}
