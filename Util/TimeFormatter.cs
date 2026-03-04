namespace Quartermaster.Util;

/// <summary>
/// Format time durations: "3h 20m remaining", "2d 5h ago", etc.
/// </summary>
public static class TimeFormatter
{
    public static string FormatRemaining(TimeSpan remaining)
    {
        if (remaining <= TimeSpan.Zero)
            return "Done";

        if (remaining.TotalDays >= 1)
            return $"{(int)remaining.TotalDays}d {remaining.Hours}h";

        if (remaining.TotalHours >= 1)
            return $"{(int)remaining.TotalHours}h {remaining.Minutes}m";

        return $"{remaining.Minutes}m {remaining.Seconds}s";
    }

    public static string FormatAgo(DateTimeOffset timestamp)
    {
        var elapsed = DateTimeOffset.UtcNow - timestamp;

        if (elapsed.TotalDays >= 1)
            return $"{(int)elapsed.TotalDays}d ago";

        if (elapsed.TotalHours >= 1)
            return $"{(int)elapsed.TotalHours}h ago";

        if (elapsed.TotalMinutes >= 1)
            return $"{(int)elapsed.TotalMinutes}m ago";

        return "just now";
    }
}
