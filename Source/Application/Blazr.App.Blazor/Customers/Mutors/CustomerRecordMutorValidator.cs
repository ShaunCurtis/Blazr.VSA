/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class CustomerRecordMutorValidator : AbstractValidator<CustomerRecordMutor>
{
    public CustomerRecordMutorValidator()
    {
        this.RuleFor(p => p.Name)
            .MinimumLength(3)
            .WithState(p => p);
    }
}
