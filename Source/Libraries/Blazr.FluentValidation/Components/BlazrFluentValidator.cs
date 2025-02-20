/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Blazr.BaseComponents;

namespace Blazr.FluentValidation;

/// <summary>
/// Component to apply validation to the cascaded EditContext
/// You need to set the Record Type and the Validator
/// It plugs in to the EditContext cascaded by the EditForm
/// And validates based on events raised by the EditContext
/// </summary>
/// <typeparam name="TRecord"></typeparam>
/// <typeparam name="TValidator"></typeparam>
public sealed class BlazrFluentValidator<TRecord, TValidator> : BlazrControlBase, IDisposable
    where TRecord : class
    where TValidator : class, IValidator<TRecord>, new()
{
    [CascadingParameter] private EditContext _editContext { get; set; } = default!;

    private ValidationMessageStore _validationMessageStore = default!;
    private TValidator _validator = default!;
    private TRecord _model = default!;
    private EditContext? _existingEditContext;

    protected override Task OnParametersSetAsync()
    {
        // validate the edit context is not null
        ArgumentNullException.ThrowIfNull(_editContext);

        // This is either initialization or the EditContext has changed
        if (_existingEditContext != _editContext)
        {
            // deregister any existing context registrations
            this.ClearExistingEditContextRegistrations();

            // Get a validation store instance
            _validationMessageStore = new ValidationMessageStore(_editContext);

            // Create an instance of the Validator
            _validator = Activator.CreateInstance<TValidator>();

            // Get a reference to the model, validate not null and cast it
            var model = _editContext.Model as TRecord;
            ArgumentNullException.ThrowIfNull(model);
            _model = model;

            // Set up the listeners on the edit context so we can run validation
            _editContext.OnValidationRequested += OnValidationRequested;
            _editContext.OnFieldChanged += OnFieldChanged;

            _existingEditContext = _editContext;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Clears the existing handlers and message store 
    /// </summary>
    private void ClearExistingEditContextRegistrations()
    {
        if (_existingEditContext != null)
        {
            _existingEditContext.OnValidationRequested -= OnValidationRequested;
            _existingEditContext.OnFieldChanged -= OnFieldChanged;
        }

        // Clear any exisitng messages from the message store
        _validationMessageStore?.Clear();
    }

    /// <summary>
    /// handles a Validation Event - Validates the whole model
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnValidationRequested(object? sender, ValidationRequestedEventArgs e)
    {
        _validationMessageStore.Clear();

        var result = _validator.Validate(_model);
        this.LogErrors(result);

        _editContext.NotifyValidationStateChanged();
    }

    /// <summary>
    /// Handles a field change Event - Validates only the specific field
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        _validationMessageStore.Clear(e.FieldIdentifier);

        var result = _validator.Validate(_model, options =>
        {
            options.IncludeProperties(e.FieldIdentifier.FieldName);
        });

        this.LogErrors(result);

        _editContext.NotifyValidationStateChanged();
    }

    /// <summary>
    /// Log the errors to the validation message store
    /// </summary>
    /// <param name="result"></param>
    private void LogErrors(ValidationResult result)
    {
        if (result.IsValid)
            return;

        foreach (var error in result.Errors)
        {
            var fi = new FieldIdentifier(error.CustomState ?? _model, error.PropertyName);
            _validationMessageStore.Add(fi, error.ErrorMessage);
        }
    }

    public void Dispose()
    {
        this.ClearExistingEditContextRegistrations();
    }
}
