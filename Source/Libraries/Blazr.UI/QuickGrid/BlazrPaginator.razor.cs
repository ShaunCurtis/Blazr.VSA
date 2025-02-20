/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// 
/// Modified from the orignal. 
/// 
/// Licensed to the .NET Foundation under one or more agreements.
/// The .NET Foundation licenses this file to you under the MIT license.
/// 
/// ============================================================
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.UI;

public partial class BlazrPaginator : ComponentBase
{
    [Parameter, EditorRequired] public PaginationState State { get; set; } = default!;

    private Task GoFirstAsync() => GoToPageAsync(0);
    private Task GoPreviousAsync() => GoToPageAsync(State.CurrentPageIndex - 1);
    private Task GoNextAsync() => GoToPageAsync(State.CurrentPageIndex + 1);
    private Task GoLastAsync() => GoToPageAsync(State.LastPageIndex.GetValueOrDefault(0));

    private bool CanGoBack => State.CurrentPageIndex > 0;
    private bool CanGoForwards => State.CurrentPageIndex < State.LastPageIndex;

    private int _totalCount;
    private int _currentPage;

    private Task GoToPageAsync(int pageIndex)
    {
        _currentPage = pageIndex + 1;
        return State.SetCurrentPageIndexAsync(pageIndex);
    }

    /// <summary>
    /// Setter for the page input.  Validates the value against the current Pagination State
    /// and ensures the used value is within the valid range
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private Task SetPageAsync(int value)
    {
        _currentPage = value;

        if (value < 1)
            _currentPage = 1;

        if (value > State.LastPageIndex)
            _currentPage = State.LastPageIndex + 1 ?? 1;

        return State.SetCurrentPageIndexAsync(_currentPage - 1);
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        base.SetParametersAsync(parameters);

        _currentPage = State.CurrentPageIndex + 1;
        _totalCount = this.State.TotalItemCount ?? 0;

        return base.SetParametersAsync(ParameterView.Empty);
    }

    protected override void OnInitialized()
    {
        this.State.TotalItemCountChanged += this.OnTotalCountChanged;
    }

    private void OnTotalCountChanged(object? sender, int? totalCount)
    {
        _totalCount = totalCount ?? _totalCount;
        this.StateHasChanged();
    }

    public void Dispose()
    {
        this.State.TotalItemCountChanged -= this.OnTotalCountChanged;
    }
}
