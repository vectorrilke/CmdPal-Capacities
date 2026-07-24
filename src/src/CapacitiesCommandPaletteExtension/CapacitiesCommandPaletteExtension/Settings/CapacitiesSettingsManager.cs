using System;
using System.IO;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CapacitiesCommandPaletteExtension.Settings;

internal sealed class CapacitiesSettingsManager
{
    private const string ApiTokenSettingId = "apiToken";
    private const string CreateObjectOpenBehaviorSettingId = "createObjectOpenBehavior";
    private const string ApiTokenFileName = "api-token.txt";
    private const string TokenDisplayPrefix = "token set: ";

    private readonly Microsoft.CommandPalette.Extensions.Toolkit.Settings _settings = new();
    private readonly TextSetting _apiTokenSetting;
    private readonly ChoiceSetSetting _createObjectOpenBehaviorSetting;
    private bool _apiTokenSavedSinceLastRead;
    public event Action? SettingsChanged;

    public CapacitiesSettingsManager()
    {
        _apiTokenSetting = new TextSetting(
            ApiTokenSettingId,
            "Capacities API Token",
            "Auth token used to write content into Capacities",
            string.Empty);
        _settings.Add(_apiTokenSetting);

        _createObjectOpenBehaviorSetting = new ChoiceSetSetting(
            CreateObjectOpenBehaviorSettingId,
            "Default action after searching for or creating a new object",
            "Choose what happens after searching for or creating a new object",
            [
                new ChoiceSetSetting.Choice("Open Capacities App", "app"),
                new ChoiceSetSetting.Choice("Open Capacities Web", "web"),
                new ChoiceSetSetting.Choice("Do nothing", "none"),
            ])
        {
            Value = "app",
        };
        _settings.Add(_createObjectOpenBehaviorSetting);

        // Migrate API token to hidden storage and keep masked value in visible settings.
        var storedApiToken = ReadApiToken();
        if (string.IsNullOrWhiteSpace(storedApiToken))
        {
            var legacyToken = _apiTokenSetting.Value ?? _settings.GetSetting<string>(ApiTokenSettingId) ?? string.Empty;
            if (LooksLikeFullApiToken(legacyToken))
            {
                var normalizedLegacyToken = NormalizeTokenInput(legacyToken);
                WriteApiToken(normalizedLegacyToken);
                _apiTokenSetting.Value = FormatTokenSettingDisplay(normalizedLegacyToken);
            }
        }
        else
        {
            _apiTokenSetting.Value = FormatTokenSettingDisplay(storedApiToken);
        }

    }

    public ICommandSettings Settings => _settings;

    public ExtensionSettings Current => new(
        ApiToken: ApiToken,
        InsertMode: InsertMode.Append,
        OutputMode: OutputMode.Markdown,
        CreateObjectOpenBehavior: ParseCreateObjectOpenBehavior(_settings.GetSetting<string>(CreateObjectOpenBehaviorSettingId)));

    public string ApiToken => ResolveAndSyncApiToken();

    public void SaveApiToken(string apiToken)
    {
        var normalizedToken = NormalizeTokenInput(apiToken);

        if (string.IsNullOrWhiteSpace(normalizedToken))
        {
            ClearApiToken();
            return;
        }

        WriteApiToken(normalizedToken);
        _apiTokenSetting.Value = FormatTokenSettingDisplay(normalizedToken);
        _apiTokenSavedSinceLastRead = true;
        SettingsChanged?.Invoke();
    }

    public void ClearApiToken()
    {
        DeleteApiToken();
        _apiTokenSetting.Value = string.Empty;
        _apiTokenSavedSinceLastRead = false;
        SettingsChanged?.Invoke();
    }

    public bool ConsumeApiTokenSavedFlag()
    {
        var wasSaved = _apiTokenSavedSinceLastRead;
        _apiTokenSavedSinceLastRead = false;
        return wasSaved;
    }

    private static string GetApiTokenPath()
    {
        return Path.Combine(GetStorageFolderPath(), ApiTokenFileName);
    }

    private static string GetStorageFolderPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CapacitiesCommandPaletteExtension");
    }

    private static string ReadApiToken()
    {
        try
        {
            var path = GetApiTokenPath();
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            return NormalizeTokenInput(File.ReadAllText(path));
        }
        catch
        {
            return string.Empty;
        }
    }

    private static void WriteApiToken(string apiToken)
    {
        try
        {
            var path = GetApiTokenPath();
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, NormalizeTokenInput(apiToken));
        }
        catch
        {
            // Best-effort hidden storage.
        }
    }

    private static void DeleteApiToken()
    {
        try
        {
            var path = GetApiTokenPath();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Best-effort hidden storage cleanup.
        }
    }

    private string ResolveAndSyncApiToken()
    {
        var hiddenToken = ReadApiToken();
        var visibleValue = _apiTokenSetting.Value ?? _settings.GetSetting<string>(ApiTokenSettingId) ?? string.Empty;
        var normalizedVisibleValue = NormalizeTokenInput(visibleValue);

        if (string.IsNullOrWhiteSpace(normalizedVisibleValue) && !string.IsNullOrWhiteSpace(hiddenToken))
        {
            DeleteApiToken();
            _apiTokenSetting.Value = string.Empty;
            return string.Empty;
        }

        if (LooksLikeFullApiToken(normalizedVisibleValue))
        {
            WriteApiToken(normalizedVisibleValue);
            _apiTokenSetting.Value = FormatTokenSettingDisplay(normalizedVisibleValue);
            return normalizedVisibleValue;
        }

        if (!string.IsNullOrWhiteSpace(hiddenToken))
        {
            _apiTokenSetting.Value = FormatTokenSettingDisplay(hiddenToken);
            return hiddenToken;
        }

        return string.Empty;
    }

    private static bool LooksLikeFullApiToken(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (value.StartsWith(TokenDisplayPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (value.EndsWith("...", StringComparison.Ordinal))
        {
            return false;
        }

        if (value.Contains("****", StringComparison.Ordinal))
        {
            return false;
        }

        return value.StartsWith("cap-api-", StringComparison.OrdinalIgnoreCase) || value.Length > 20;
    }

    private static string NormalizeTokenInput(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.StartsWith(TokenDisplayPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return trimmed[TokenDisplayPrefix.Length..].Trim();
        }

        return trimmed;
    }

    public static string FormatTokenSettingDisplay(string token)
    {
        var trimmed = NormalizeTokenInput(token);
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return string.Empty;
        }

        return MaskToken(trimmed);
    }

    public static string MaskToken(string token)
    {
        var trimmed = NormalizeTokenInput(token);
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return string.Empty;
        }

        if (trimmed.Length <= 4)
        {
            return new string('*', trimmed.Length);
        }

        const int visiblePrefixLength = 18;
        var prefixLength = Math.Min(visiblePrefixLength, trimmed.Length - 4);
        if (prefixLength <= 0)
        {
            return "****";
        }

        return trimmed[..prefixLength] + "****";
    }

    private static CreateObjectOpenBehavior ParseCreateObjectOpenBehavior(string? value)
    {
        if (string.Equals(value, "app", StringComparison.OrdinalIgnoreCase))
        {
            return CreateObjectOpenBehavior.App;
        }

        if (string.Equals(value, "none", StringComparison.OrdinalIgnoreCase))
        {
            return CreateObjectOpenBehavior.None;
        }

        return CreateObjectOpenBehavior.App;
    }
}