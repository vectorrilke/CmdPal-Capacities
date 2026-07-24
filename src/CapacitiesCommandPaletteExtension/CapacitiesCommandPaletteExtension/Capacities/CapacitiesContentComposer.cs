using CapacitiesCommandPaletteExtension.Parsing;
using CapacitiesCommandPaletteExtension.Settings;

namespace CapacitiesCommandPaletteExtension.Capacities;

internal static class CapacitiesContentComposer
{
    public static string Compose(CapCommandInput input, OutputMode outputMode)
    {
        return outputMode switch
        {
            OutputMode.Markdown => ComposeMarkdown(input),
            _ => ComposePlainText(input),
        };
    }

    public static string ComposePlainText(CapCommandInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Url))
        {
            return input.Text;
        }

        return $"{input.Text}\n{input.Url}";
    }

    public static string ComposeMarkdown(CapCommandInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Url))
        {
            return input.Text;
        }

        return $"- [{EscapeMarkdown(input.Text)}]({input.Url})";
    }

    private static string EscapeMarkdown(string text)
    {
        return text
            .Replace("\\", "\\\\")
            .Replace("[", "\\[")
            .Replace("]", "\\]")
            .Replace("(", "\\(")
            .Replace(")", "\\)");
    }
}