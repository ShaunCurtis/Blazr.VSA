﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================


namespace Blazr.Uranium;

public class UIButtonGroup : HtmlElementBase
{
    protected override CSSBuilder CssBuilder => base.CssBuilder
        .AddClass("btn-group me-1");

    protected override string HtmlTag => "div";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
            builder.OpenElement(0, this.HtmlTag);
            builder.AddAttribute(1, "class", this.CssClass);
            builder.AddAttribute(2, "role", "group");
            builder.AddContent(4, ChildContent);
            builder.CloseElement();
    }
}

