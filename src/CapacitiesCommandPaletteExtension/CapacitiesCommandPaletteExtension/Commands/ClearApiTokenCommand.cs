using CapacitiesCommandPaletteExtension.Settings;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CapacitiesCommandPaletteExtension.Commands;

internal sealed partial class ClearApiTokenCommand : InvokableCommand
{
    private readonly CapacitiesSettingsManager _settingsManager;

    public ClearApiTokenCommand(CapacitiesSettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public override string Name => "Clear API token";

    public override IconInfo Icon => new("\uE74D");

    public override CommandResult Invoke()
    {
        _settingsManager.ClearApiToken();
        return CommandResult.GoBack();
    }
}