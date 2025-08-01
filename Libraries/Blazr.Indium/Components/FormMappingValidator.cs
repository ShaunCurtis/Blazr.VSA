﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using global::Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;
using System.Globalization;

namespace Blazr.Indium;

/// <summary>
/// Exposes validation messages for a given <see cref="FormMappingContext"/>.
/// </summary>
internal class FormMappingValidator : ComponentBase, IDisposable
{
    private IDisposable? _subscriptions;
    private EditContext? _originalEditContext;

    [Parameter] public EditContext? CurrentEditContext { get; set; }

    [CascadingParameter] internal FormMappingContext? MappingContext { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (CurrentEditContext == null)
        {
            throw new InvalidOperationException($"{nameof(FormMappingValidator)} requires a " +
                $"parameter of type {nameof(EditContext)}.");
        }

        if (MappingContext == null)
        {
            return;
        }

        _subscriptions = CurrentEditContext.EnableFormMappingContextExtensions(MappingContext);
        _originalEditContext = CurrentEditContext;
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (MappingContext == null)
        {
            return;
        }

        if (CurrentEditContext != _originalEditContext)
        {
            // While we could support this, there's no known use case presently. Since InputBase doesn't support it,
            // it's more understandable to have the same restriction.
            throw new InvalidOperationException($"{GetType()} does not support changing the " +
                $"{nameof(EditContext)} dynamically.");
        }
    }

    void IDisposable.Dispose()
    {
        _subscriptions?.Dispose();
        _subscriptions = null;
    }
}

