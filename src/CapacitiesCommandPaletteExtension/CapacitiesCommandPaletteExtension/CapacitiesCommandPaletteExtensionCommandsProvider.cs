// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using CapacitiesCommandPaletteExtension.Settings;

namespace CapacitiesCommandPaletteExtension;

public partial class CapacitiesCommandPaletteExtensionCommandsProvider : CommandProvider
{
    private readonly CapacitiesSettingsManager _settingsManager = new();

    public CapacitiesCommandPaletteExtensionCommandsProvider()
    {
        DisplayName = "Capacities";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Settings = _settingsManager.Settings;
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return [
            new CommandItem(new CapacitiesCommandPaletteExtensionPage(_settingsManager))
            {
                Title = DisplayName,
                Subtitle = "Send notes, links, and markdown to your chosen Capacities object.",
            },
        ];
    }

}
