namespace Quartermaster.Util;

/// <summary>
/// Format numbers: 1200 → "1.2k", 1500000 → "1.5m".
/// </summary>
public static class GilFormatter
{
    public static string Format(decimal amount)
    {
        return amount switch
        {
            >= 1_000_000 => $"{amount / 1_000_000:F1}m",
            >= 1_000 => $"{amount / 1_000:F1}k",
            _ => amount.ToString("N0"),
        };
    }

    public static string FormatWithSign(decimal amount)
    {
        var prefix = amount >= 0 ? "+" : "";
        return prefix + Format(amount);
    }
}
