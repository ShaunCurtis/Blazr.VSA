/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.UI.Editor;

public static class ComponentHelper
{
    public static bool TryGetFieldIdentifier(Expression<Func<string>>? expression, [NotNullWhen(true)] out FieldIdentifier fi)
    {
        if (expression is null)
        {
            fi = new FieldIdentifier();
            return false;
        }

        fi = FieldIdentifier.Create(expression);
        return true;
    }
}
