using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace ProjectName.Shared.Utilities;
public static class ImageExtensions
{
    public static byte[]? ConvertBase64ToByteArray(this string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64)) return null;
        if (!base64.StartsWith("data:")) return null;
        base64 = base64.Replace("data:image/webp;base64,", "")
            .Replace("data:image/png;base64,", "");
        return Convert.FromBase64String(base64);
    }

    public static string ReplaceUrlToImageTags(this string input)
    {
        string pattern = @"https?://[^\s]+(\.jpg|\.jpeg|\.png|\.gif|\.bmp|\.svg|\.webp)";
        string output = Regex.Replace(input, pattern, match =>
        {
            return $"<img src=\"{match.Value}\" />";
        });
        return output;
    }

    public static string UpdateImage(this string html) => $"<img src=\"{html}\" />";

    public static MarkupString ShowImages(this string input)
    {
        string pattern = @"data:image[^;]+;base64,([^\""]+)";
        string output = Regex.Replace(input, pattern, match =>
        {
            return $"<img src=\"{match.Value}\" />";
        });
        output = output.Replace(Environment.NewLine, "<br/>");
        return (MarkupString)output;
    }    
}
