/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public class CustomerEditContextValidator : AbstractValidator<CustomerEditContext>
{
    public CustomerEditContextValidator()
    {
        this.RuleFor(p => p.Name)
            .MinimumLength(3)
            .WithState(p => p);
    }
}
