using CapacitiesCommandPaletteExtension.Commands;
using CapacitiesCommandPaletteExtension.Settings;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CapacitiesCommandPaletteExtension;

internal sealed partial class AppendToObjectTextPage : DynamicListPage
{
    private readonly CapacitiesSettingsManager _settingsManager;
    private readonly string _objectId;
    private readonly string _objectTitle;
    private readonly CreateObjectOpenBehavior _openBehavior;
    private string _currentSearchText = string.Empty;

    public AppendToObjectTextPage(
        CapacitiesSettingsManager settingsManager,
        string objectId,
        string objectTitle,
        CreateObjectOpenBehavior openBehavior)
    {
        _settingsManager = settingsManager;
        _objectId = objectId;
        _objectTitle = objectTitle;
        _openBehavior = openBehavior;

        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = "Append text";
        Name = "Append text";
        PlaceholderText = "Enter content here";
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        _currentSearchText = newSearch;
        RaiseItemsChanged();
    }

    public override IListItem[] GetItems()
    {
        var text = _currentSearchText.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return [
                new ListItem(new NoOpCommand())
                {
                    Title = "Enter content here",
                    Subtitle = "Press Enter to append using the chosen action.",
                }
            ];
        }

        return [
            new ListItem(new AppendToObjectAndOpenCommand(_settingsManager, _objectId, _objectTitle, text, _openBehavior))
            {
                Title = "Append now",
                Subtitle = _openBehavior switch
                {
                    CreateObjectOpenBehavior.Web => "Append and open in Web.",
                    CreateObjectOpenBehavior.None => "Append only.",
                    _ => "Append and open in App.",
                },
            }
        ];
    }
}
