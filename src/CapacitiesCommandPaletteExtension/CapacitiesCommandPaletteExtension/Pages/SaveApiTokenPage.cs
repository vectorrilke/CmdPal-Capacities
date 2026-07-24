using System.Collections.Generic;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using CapacitiesCommandPaletteExtension.Commands;
using CapacitiesCommandPaletteExtension.Settings;

namespace CapacitiesCommandPaletteExtension;

internal sealed partial class SaveApiTokenPage : DynamicListPage
{
    private static readonly IconInfo ApiTokenIcon = new("\uE73E");
    private static readonly IconInfo ApiTokenWarningIcon = new("\uE814");

    private readonly CapacitiesSettingsManager _settingsManager;
    private string _currentSearchText = string.Empty;

    public SaveApiTokenPage(CapacitiesSettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = "Save API token";
        Name = "Save API token";
        PlaceholderText = "Enter API token here";
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        _currentSearchText = newSearch;
        RaiseItemsChanged();
    }

    public override IListItem[] GetItems()
    {
        var token = _currentSearchText.Trim();
        var currentToken = _settingsManager.Current.ApiToken;

        var items = new List<IListItem>();

        if (string.IsNullOrWhiteSpace(token))
        {
            items.Add(new ListItem(new NoOpCommand())
            {
                Title = string.IsNullOrWhiteSpace(currentToken)
                    ? "not set"
                    : CapacitiesSettingsManager.FormatTokenSettingDisplay(currentToken),
                Subtitle = "Paste API token and press Enter",
                Icon = string.IsNullOrWhiteSpace(currentToken) ? ApiTokenWarningIcon : ApiTokenIcon,
            });

            if (!string.IsNullOrWhiteSpace(currentToken))
            {
                items.Add(new ListItem(new ClearApiTokenCommand(_settingsManager))
                {
                    Title = "Clear saved token",
                    Subtitle = "Remove the stored API token",
                });
            }

            return [..items];
        }

        items.Add(new ListItem(new SaveApiTokenCommand(_settingsManager, token))
        {
            Title = "Save API token",
            Subtitle = CapacitiesSettingsManager.MaskToken(token),
            Icon = ApiTokenIcon,
        });

        return [..items];
    }
}