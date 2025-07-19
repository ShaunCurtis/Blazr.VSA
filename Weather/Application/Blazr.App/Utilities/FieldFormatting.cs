/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Globalization;
using System.Text.RegularExpressions;

namespace Blazr.App.Core;

/// <summary>
/// FieldFormatting provides a set of extension methods to format fields for display
/// </summary>
public static class FieldFormatting
{
    public static string AsGlobalDateFormat(this Date Date)
    {
        return Date.Value.ToString("dd-MMM-yyyy");
    }

    public static string AsGlobalDateFormat(this DateTime Date)
    {
        return Date.ToString("dd-MMM-yyyy");
    }

    public static string AsGlobalDateFormat(this DateOnly Date)
    {
        return Date.ToString("dd-MMM-yyyy");
    }

    public static string AsTemperature(this decimal value)
    {
        return string.Format("{0}", value.ToString("#0"));
    }

    public static string AsPercentage(this decimal value)
    {
        return string.Format("{0}%", value.ToString("#0.##"));
    }

    public static string AsMoney(this decimal value)
    {
        return string.Format("£{0}", value.ToString("#0.00"));
    }

    public static string AsWeight(this decimal value)
    {
        return value.ToString("#0.000", CultureInfo.CurrentCulture);
    }

    public static string AsYesNo(this bool value)
    {
        return value ? "Yes" : "No";
    }

    public static string AsSizedString(this string value, bool dotting, int size = 50)
    {
        if (value != null)
        {
            if (value.Length > size - 3 && dotting) return string.Concat(value.Substring(0, size - 3), "...");
            else if (value.Length > size) return value.Substring(0, size);
            else return value;
        }
        return string.Empty;
    }

    public static string AsSeparatedString(this string value)
    {
        return Regex.Replace(value, @"\B[A-Z]", " $0");
    }

    public static string TextToHtmlNewLines(this string text)
    {
        return text.Replace(System.Environment.NewLine, "<br />");
    }

    public static string ToDisplayId(this Guid value)
    {
        // There are 4 hyphens in ToString!
        return value.ToString().Substring(28, 8);
    }
}
