using System.Collections.Generic;
using CapacitiesCommandPaletteExtension.Settings;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CapacitiesCommandPaletteExtension;

internal sealed partial class CreateObjectEnterNamePage : DynamicListPage
{
    private static readonly IconInfo ObjectIcon = new("\uE8A5");

    private readonly CapacitiesSettingsManager _settingsManager;
    private readonly string _structureId;
    private readonly string _structureTitle;
    private string _currentSearchText = string.Empty;

    public CreateObjectEnterNamePage(CapacitiesSettingsManager settingsManager, string structureId, string structureTitle)
    {
        _settingsManager = settingsManager;
        _structureId = structureId;
        _structureTitle = structureTitle;
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = "Enter object name";
        Name = "Enter object name";
        PlaceholderText = "Enter title here";
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        _currentSearchText = newSearch;
        RaiseItemsChanged();
    }

    public override IListItem[] GetItems()
    {
        var objectName = _currentSearchText.Trim();
        if (string.IsNullOrWhiteSpace(objectName))
        {
            return [
                new ListItem(new NoOpCommand())
                {
                    Title = "Type object name",
                    Subtitle = $"Structure: {_structureTitle}",
                    Icon = ObjectIcon,
                }
            ];
        }

        return [
            new ListItem(new CreateObjectEnterContentPage(_settingsManager, _structureId, objectName))
            {
                Title = $"Name: {objectName}",
                Subtitle = "Press Enter to continue.",
                Icon = ObjectIcon,
            }
        ];
    }
}
