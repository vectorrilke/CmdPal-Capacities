using System.Collections.Generic;
using CapacitiesCommandPaletteExtension.Commands;
using CapacitiesCommandPaletteExtension.Settings;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CapacitiesCommandPaletteExtension;

internal sealed partial class CreateObjectEnterContentPage : DynamicListPage
{
    private static readonly IconInfo ObjectIcon = new("\uE8A5");

    private readonly CapacitiesSettingsManager _settingsManager;
    private readonly string _structureId;
    private readonly string _objectName;
    private string _currentSearchText = string.Empty;

    public CreateObjectEnterContentPage(CapacitiesSettingsManager settingsManager, string structureId, string objectName)
    {
        _settingsManager = settingsManager;
        _structureId = structureId;
        _objectName = objectName;
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = "Enter object content";
        Name = "Enter object content";
        PlaceholderText = "Enter content here";
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        _currentSearchText = newSearch;
        RaiseItemsChanged();
    }

    public override IListItem[] GetItems()
    {
        var content = _currentSearchText.Trim();
        return [
            new ListItem(new CreateObjectCommand(_settingsManager, _structureId, _objectName, content))
            {
                Title = string.IsNullOrWhiteSpace(content) ? "Create blank object" : "Create object",
                Subtitle = string.IsNullOrWhiteSpace(content)
                    ? "Press Enter to create and open it without content."
                    : $"Name: {_objectName}",
                Icon = ObjectIcon,
            }
        ];
    }
}
