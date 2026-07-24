using CapacitiesCommandPaletteExtension.Settings;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CapacitiesCommandPaletteExtension.Commands;

internal sealed partial class SaveApiTokenCommand : InvokableCommand
{
    private readonly CapacitiesSettingsManager _settingsManager;
    private readonly string _apiToken;

    public SaveApiTokenCommand(CapacitiesSettingsManager settingsManager, string apiToken)
    {
        _settingsManager = settingsManager;
        _apiToken = apiToken;
    }

    public override string Name => "Save API token";

    public override IconInfo Icon => new("\uE73E");

    public override CommandResult Invoke()
    {
        _settingsManager.SaveApiToken(_apiToken);
        return CommandResult.GoBack();
    }
}