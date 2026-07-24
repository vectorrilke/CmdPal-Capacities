using System;

namespace CapacitiesCommandPaletteExtension.Parsing;

internal static class CapCommandParser
{
    public static bool TryParse(string rawInput, out CapCommandInput? commandInput, out string? errorMessage)
    {
        commandInput = null;
        errorMessage = null;

        var trimmedInput = rawInput.Trim();
        if (string.IsNullOrWhiteSpace(trimmedInput))
        {
            errorMessage = "Enter text to send to Capacities.";
            return false;
        }

        var commaIndex = trimmedInput.IndexOf(',');
        if (commaIndex < 0)
        {
            commandInput = new CapCommandInput(DecodeEscapedNewlines(trimmedInput), null);
            return true;
        }

        var textPart = trimmedInput[..commaIndex].Trim();
        var urlPart = trimmedInput[(commaIndex + 1)..].Trim();

        if (string.IsNullOrWhiteSpace(textPart))
        {
            errorMessage = "The text portion cannot be empty.";
            return false;
        }

        commandInput = new CapCommandInput(
            DecodeEscapedNewlines(textPart),
            string.IsNullOrWhiteSpace(urlPart) ? null : urlPart);
        return true;
    }

    private static string DecodeEscapedNewlines(string text)
    {
        return text
            .Replace("\\r\\n", "\r\n", StringComparison.Ordinal)
            .Replace("\\n", "\n", StringComparison.Ordinal)
            .Replace("\\r", "\r", StringComparison.Ordinal);
    }
}