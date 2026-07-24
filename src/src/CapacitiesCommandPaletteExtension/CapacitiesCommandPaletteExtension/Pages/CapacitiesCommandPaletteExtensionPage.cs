// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Timers;
using CapacitiesCommandPaletteExtension.Capacities;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using CapacitiesCommandPaletteExtension.Commands;
using CapacitiesCommandPaletteExtension.Settings;

namespace CapacitiesCommandPaletteExtension;

internal sealed partial class CapacitiesCommandPaletteExtensionPage : DynamicListPage
{
    private const int MinQueryLength = 3;
    private static readonly IconInfo CapacitiesIcon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
    private static readonly IconInfo SearchIcon = new("\uE721");
    private static readonly IconInfo StructureIcon = new("\uE8B7");
    private static readonly IconInfo ObjectIcon = new("\uE8A5");
    private static readonly IconInfo CreateObjectIcon = new("\uE70F");
    private static readonly IconInfo ApiTokenIcon = new("\uE73E");
    private static readonly IconInfo ApiTokenWarningIcon = new("\uE814");

    private readonly CapacitiesSettingsManager _settingsManager;
    private readonly Timer _searchDebounceTimer;
    private string _currentSearchText = string.Empty;
    private string _lastQuery = string.Empty;
    private string _lastToken = string.Empty;
    private bool _searchReady = true;
    private CapacitiesObjectSearchResult _cachedResult = CapacitiesObjectSearchResult.Success([]);

    public CapacitiesCommandPaletteExtensionPage(CapacitiesSettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        _settingsManager.SettingsChanged += OnSettingsChanged;
        Icon = CapacitiesIcon;
        Title = "Capacities";
        Name = "Open";
        PlaceholderText = "Search objects in Capacities";

        _searchDebounceTimer = new Timer(300)
        {
            AutoReset = false,
        };
        _searchDebounceTimer.Elapsed += (_, _) =>
        {
            _searchReady = true;
            RaiseItemsChanged();
        };
    }

    private void OnSettingsChanged()
    {
        RaiseItemsChanged();
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        _currentSearchText = newSearch;

        if (string.IsNullOrWhiteSpace(newSearch))
        {
            _searchDebounceTimer.Stop();
            _searchReady = false;
            RaiseItemsChanged();
            return;
        }

        _searchReady = false;
        _searchDebounceTimer.Stop();
        _searchDebounceTimer.Start();
    }

    public override IListItem[] GetItems()
    {
        var commandText = _currentSearchText.Trim();
        var settings = _settingsManager.Current;
        var tokenWasJustSaved = _settingsManager.ConsumeApiTokenSavedFlag();

        if (string.IsNullOrWhiteSpace(commandText))
        {
            return BuildIdleItems(tokenWasJustSaved, settings);
        }

        if (commandText.Length < MinQueryLength)
        {
            var shortItems = new List<IListItem>
            {
                new ListItem(new NoOpCommand())
                {
                    Title = "Keep typing",
                    Subtitle = $"Type at least {MinQueryLength} characters to search.",
                },
            };

            shortItems.Add(new ListItem(new CreateObjectChooseStructurePage(_settingsManager))
            {
                Title = "Create object",
                Subtitle = DescribeDefaultAction(settings.CreateObjectOpenBehavior),
                Icon = CreateObjectIcon,
            });

            shortItems.Add(CreateTokenCommandItem(tokenWasJustSaved, settings));

            return [..shortItems];
        }

        if (!string.Equals(commandText, _lastQuery, StringComparison.Ordinal) && !_searchReady)
        {
            return [
                new ListItem(new NoOpCommand())
                {
                    Title = "Searching...",
                    Subtitle = "Wait briefly to reduce API calls.",
                }
            ];
        }

        if (string.IsNullOrWhiteSpace(settings.ApiToken))
        {
            return [
                new ListItem(new NoOpCommand())
                {
                    Title = "Add API token first",
                    Subtitle = "Open API token page and save your Capacities token.",
                    Icon = ApiTokenWarningIcon,
                },
                new ListItem(new SaveApiTokenPage(_settingsManager))
                {
                    Title = "Set API token",
                    Subtitle = BuildApiTokenStatus(tokenWasJustSaved, settings),
                    Icon = ApiTokenWarningIcon,
                },
            ];
        }

        var searchResult = GetOrSearch(settings.ApiToken, commandText);
        if (!string.IsNullOrWhiteSpace(searchResult.Error))
        {
            return [
                new ListItem(new NoOpCommand())
                {
                    Title = "Search failed",
                    Subtitle = searchResult.Error,
                },
            ];
        }

        if (searchResult.Objects.Count == 0)
        {
            return [
                new ListItem(new NoOpCommand())
                {
                    Title = "No matching objects",
                    Subtitle = "Try a broader title search term.",
                },
            ];
        }

        var structureMetadata = GetStructureMetadata(settings.ApiToken);
        var groups = GroupByStructure(searchResult.Objects, structureMetadata);
        var items = new List<IListItem>();
        foreach (var group in groups)
        {
            items.Add(new ListItem(new InfoOnlyCommand())
            {
                Title = group.StructureTitle,
                Subtitle = string.IsNullOrWhiteSpace(group.StructureDescription)
                    ? "no description"
                    : group.StructureDescription,
                Icon = StructureIcon,
            });

            foreach (var match in group.Objects)
            {
                items.Add(new ListItem(new ObjectActionsPage(_settingsManager, match.Id, match.Title))
                {
                    Title = $"  {match.Title}",
                    Subtitle = string.IsNullOrWhiteSpace(match.Description)
                        ? "Open or append using the default action when available."
                        : match.Description,
                    Icon = ObjectIcon,
                });
            }
        }

        return [..items];
    }

    private IListItem[] BuildIdleItems(bool tokenWasJustSaved, ExtensionSettings settings)
    {
        return
        [
            new ListItem(new NoOpCommand())
            {
                Title = "Type object name to search",
                Subtitle = "Search objects, then open or append.",
                Icon = SearchIcon,
            },
            new ListItem(new CreateObjectChooseStructurePage(_settingsManager))
            {
                Title = "Create object",
                Subtitle = DescribeDefaultAction(settings.CreateObjectOpenBehavior),
                Icon = CreateObjectIcon,
            },
            CreateTokenCommandItem(tokenWasJustSaved, settings),
        ];
    }

    private static string DescribeDefaultAction(CreateObjectOpenBehavior openBehavior)
    {
        return openBehavior switch
        {
            CreateObjectOpenBehavior.Web => "Default action: open Capacities Web",
            CreateObjectOpenBehavior.App => "Default action: open Capacities App",
            CreateObjectOpenBehavior.None => "Default action: do nothing",
            _ => "Default action: open Capacities App",
        };
    }

    private static string BuildApiTokenStatus(bool tokenWasJustSaved, ExtensionSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.ApiToken))
        {
            return tokenWasJustSaved ? "API token saved" : "No API token saved";
        }

        return tokenWasJustSaved
            ? "Saved"
            : "Change or clear saved token";
    }

    private ListItem CreateTokenCommandItem(bool tokenWasJustSaved, ExtensionSettings settings)
    {
        var tokenIsSet = !string.IsNullOrWhiteSpace(settings.ApiToken);
        return new ListItem(new SaveApiTokenPage(_settingsManager))
        {
            Title = tokenIsSet ? "API token" : "Set API token",
            Subtitle = BuildApiTokenStatus(tokenWasJustSaved, settings),
            Icon = tokenIsSet ? ApiTokenIcon : ApiTokenWarningIcon,
        };
    }

    private CapacitiesObjectSearchResult GetOrSearch(string apiToken, string query)
    {
        if (string.Equals(query, _lastQuery, StringComparison.Ordinal) &&
            string.Equals(apiToken, _lastToken, StringComparison.Ordinal))
        {
            return _cachedResult;
        }

        _searchReady = false;
        _lastQuery = query;
        _lastToken = apiToken;
        _cachedResult = CapacitiesClient.SearchObjects(apiToken, query, 20);
        return _cachedResult;
    }

    private static Dictionary<string, StructureMetadata> GetStructureMetadata(string apiToken)
    {
        var metadata = new Dictionary<string, StructureMetadata>(StringComparer.Ordinal);
        var structuresResult = CapacitiesClient.SearchStructures(apiToken, string.Empty, 100);

        if (!string.IsNullOrWhiteSpace(structuresResult.Error))
        {
            return metadata;
        }

        foreach (var structure in structuresResult.Structures)
        {
            metadata[structure.Id] = new StructureMetadata(structure.Title, structure.Description);
        }

        return metadata;
    }

    private static IReadOnlyList<ObjectStructureGroup> GroupByStructure(
        IReadOnlyList<CapacitiesObjectMatch> objects,
        IReadOnlyDictionary<string, StructureMetadata> structureMetadata)
    {
        var groups = new List<ObjectStructureGroup>();
        var groupByKey = new Dictionary<string, ObjectStructureGroup>(StringComparer.Ordinal);

        foreach (var match in objects)
        {
            var structureId = string.IsNullOrWhiteSpace(match.StructureId) ? "unknown" : match.StructureId;
            var objectReportedTitle = string.IsNullOrWhiteSpace(match.StructureTitle) ? structureId : match.StructureTitle;

            if (!groupByKey.TryGetValue(structureId, out var group))
            {
                var structureTitle = objectReportedTitle;
                var structureDescription = string.Empty;
                if (structureMetadata.TryGetValue(structureId, out var metadata))
                {
                    if (!string.IsNullOrWhiteSpace(metadata.Title))
                    {
                        structureTitle = metadata.Title;
                    }

                    structureDescription = metadata.Description;
                }

                group = new ObjectStructureGroup(structureId, structureTitle, structureDescription, new List<CapacitiesObjectMatch>());
                groups.Add(group);
                groupByKey[structureId] = group;
            }

            group.Objects.Add(match);
        }

        return groups;
    }

    private sealed class ObjectStructureGroup
    {
        public string StructureId { get; }
        public string StructureTitle { get; }
        public string StructureDescription { get; }
        public List<CapacitiesObjectMatch> Objects { get; }

        public ObjectStructureGroup(string structureId, string structureTitle, string structureDescription, List<CapacitiesObjectMatch> objects)
        {
            StructureId = structureId;
            StructureTitle = structureTitle;
            StructureDescription = structureDescription;
            Objects = objects;
        }
    }

    private sealed class StructureMetadata
    {
        public string Title { get; }
        public string Description { get; }

        public StructureMetadata(string title, string description)
        {
            Title = title;
            Description = description;
        }
    }

    private sealed partial class InfoOnlyCommand : InvokableCommand
    {
        public override string Name => "Info";

        public override CommandResult Invoke()
        {
            return CommandResult.KeepOpen();
        }
    }
}
