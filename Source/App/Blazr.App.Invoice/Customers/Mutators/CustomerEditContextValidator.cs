﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using FluentValidation;

namespace Blazr.App.Invoice.Core;

public sealed class CustomerEditContextValidator : AbstractValidator<CustomerEditContext>
{
    public CustomerEditContextValidator()
    {
        this.RuleFor(p => p.CustomerName)
            .MinimumLength(3)
            .WithState(p => p);
    }
}
