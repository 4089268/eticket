using System;

namespace eticket.Utils;

public class BytesSizeCast
{
    public static string ToHumanReadableSize(long bytes)
    {
        if (bytes < 0) throw new ArgumentException("Bytes should not be negative", nameof(bytes));

        string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

}
