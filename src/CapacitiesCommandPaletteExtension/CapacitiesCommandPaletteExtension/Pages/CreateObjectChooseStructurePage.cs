using System.Collections.Generic;
using CapacitiesCommandPaletteExtension.Capacities;
using CapacitiesCommandPaletteExtension.Commands;
using CapacitiesCommandPaletteExtension.Settings;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CapacitiesCommandPaletteExtension;

internal sealed partial class CreateObjectChooseStructurePage : DynamicListPage
{
    private static readonly IconInfo ApiTokenWarningIcon = new("\uE814");
    private static readonly IconInfo StructureIcon = new("\uE838");

    private readonly CapacitiesSettingsManager _settingsManager;
    private string _currentSearchText = string.Empty;

    public CreateObjectChooseStructurePage(CapacitiesSettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = "Choose structure";
        Name = "Choose structure";
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        _currentSearchText = newSearch;
        RaiseItemsChanged();
    }

    public override IListItem[] GetItems()
    {
        var settings = _settingsManager.Current;
        if (string.IsNullOrWhiteSpace(settings.ApiToken))
        {
            return [
                new ListItem(new NoOpCommand())
                {
                    Title = "Add API token first",
                    Subtitle = "Open API token page and save your Capacities token.",
                    Icon = ApiTokenWarningIcon,
                }
            ];
        }

        var query = _currentSearchText.Trim();
        var searchResult = CapacitiesClient.SearchStructures(settings.ApiToken, query, 20);
        if (!string.IsNullOrWhiteSpace(searchResult.Error))
        {
            return [
                new ListItem(new NoOpCommand())
                {
                    Title = "Could not load structures",
                    Subtitle = searchResult.Error,
                }
            ];
        }

        if (searchResult.Structures.Count == 0)
        {
            return [
                new ListItem(new NoOpCommand())
                {
                    Title = string.IsNullOrWhiteSpace(query) ? "Type to search structures" : "No matching structures",
                    Subtitle = string.IsNullOrWhiteSpace(query)
                        ? "Start typing structure name or id to choose one."
                        : "Try a broader search query.",
                }
            ];
        }

        var items = new List<IListItem>();
        foreach (var structure in searchResult.Structures)
        {
            items.Add(new ListItem(new CreateObjectEnterNamePage(_settingsManager, structure.Id, structure.Title))
            {
                Title = structure.Title,
                Subtitle = structure.Description,
                Icon = StructureIcon,
            });
        }

        return [..items];
    }
}
