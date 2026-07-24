using System.Collections.Generic;
using CapacitiesCommandPaletteExtension.Commands;
using CapacitiesCommandPaletteExtension.Settings;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CapacitiesCommandPaletteExtension;

internal sealed partial class ObjectActionsPage : DynamicListPage
{
    private readonly CapacitiesSettingsManager _settingsManager;
    private readonly string _objectId;
    private readonly string _objectTitle;
    private string _currentSearchText = string.Empty;

    public ObjectActionsPage(CapacitiesSettingsManager settingsManager, string objectId, string objectTitle)
    {
        _settingsManager = settingsManager;
        _objectId = objectId;
        _objectTitle = objectTitle;

        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = objectTitle;
        Name = "Object actions";
        PlaceholderText = $"Enter to open or start typing to add content to {_objectTitle}";
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        _currentSearchText = newSearch;
        RaiseItemsChanged();
    }

    public override IListItem[] GetItems()
    {
        var settings = _settingsManager.Current;
        var typedText = _currentSearchText.Trim();

        if (string.IsNullOrWhiteSpace(typedText))
        {
            return BuildOpenActionItems(settings.CreateObjectOpenBehavior);
        }

        return [
            new ListItem(new AppendToObjectAndOpenCommand(_settingsManager, _objectId, _objectTitle, typedText, settings.CreateObjectOpenBehavior))
            {
                Title = DescribeAppendAction(settings.CreateObjectOpenBehavior, true),
                Subtitle = "Press Enter to append using the default action.",
            },
            new ListItem(new AppendToObjectAndOpenCommand(_settingsManager, _objectId, _objectTitle, typedText, CreateObjectOpenBehavior.Web))
            {
                Title = DescribeAppendAction(CreateObjectOpenBehavior.Web, settings.CreateObjectOpenBehavior == CreateObjectOpenBehavior.Web),
                Subtitle = "Press Enter to append and open in Web.",
            },
            new ListItem(new AppendToObjectAndOpenCommand(_settingsManager, _objectId, _objectTitle, typedText, CreateObjectOpenBehavior.None))
            {
                Title = DescribeAppendAction(CreateObjectOpenBehavior.None, settings.CreateObjectOpenBehavior == CreateObjectOpenBehavior.None),
                Subtitle = "Press Enter to append without opening.",
            },
        ];
    }

    private IListItem[] BuildOpenActionItems(CreateObjectOpenBehavior defaultOpenBehavior)
    {
        if (defaultOpenBehavior == CreateObjectOpenBehavior.Web)
        {
            return [
                new ListItem(new OpenCapacitiesObjectCommand(_settingsManager, _objectId, _objectTitle, CreateObjectOpenBehavior.Web))
                {
                    Title = "Open in Web (default)",
                    Subtitle = "Press Enter to open this object.",
                },
                new ListItem(new OpenCapacitiesObjectCommand(_settingsManager, _objectId, _objectTitle, CreateObjectOpenBehavior.App))
                {
                    Title = "Open in App",
                    Subtitle = "Press Enter to open this object.",
                },
            ];
        }

        return [
            new ListItem(new OpenCapacitiesObjectCommand(_settingsManager, _objectId, _objectTitle, CreateObjectOpenBehavior.App))
            {
                Title = defaultOpenBehavior == CreateObjectOpenBehavior.App ? "Open in App (default)" : "Open in App",
                Subtitle = "Press Enter to open this object.",
            },
            new ListItem(new OpenCapacitiesObjectCommand(_settingsManager, _objectId, _objectTitle, CreateObjectOpenBehavior.Web))
            {
                Title = "Open in Web",
                Subtitle = "Press Enter to open this object.",
            },
        ];
    }

    private static string DescribeAppendAction(CreateObjectOpenBehavior behavior, bool isDefault)
    {
        var suffix = isDefault ? " (default)" : string.Empty;

        return behavior switch
        {
            CreateObjectOpenBehavior.App => $"Append then open App{suffix}",
            CreateObjectOpenBehavior.Web => $"Append then open Web{suffix}",
            CreateObjectOpenBehavior.None => $"Append only{suffix}",
            _ => $"Append then open App{suffix}",
        };
    }
}
