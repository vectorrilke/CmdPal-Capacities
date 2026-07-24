using CapacitiesCommandPaletteExtension.Settings;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CapacitiesCommandPaletteExtension.Commands;

internal sealed partial class OpenCapacitiesObjectCommand : InvokableCommand
{
    private readonly CapacitiesSettingsManager _settingsManager;
    private readonly string _objectId;
    private readonly string _objectTitle;
    private readonly CreateObjectOpenBehavior _openBehavior;

    public OpenCapacitiesObjectCommand(
        CapacitiesSettingsManager settingsManager,
        string objectId,
        string objectTitle,
        CreateObjectOpenBehavior openBehavior)
    {
        _settingsManager = settingsManager;
        _objectId = objectId;
        _objectTitle = objectTitle;
        _openBehavior = openBehavior;
    }

    public override string Name => "Open object";

    public override IconInfo Icon => new("\uE8A7");

    public override CommandResult Invoke()
    {
        var settings = _settingsManager.Current;
        if (string.IsNullOrWhiteSpace(settings.ApiToken))
        {
            return CommandResult.ShowToast("Add your Capacities API token in the settings first.");
        }

        var openMessage = ObjectOpenHelper.OpenObject(settings.ApiToken, _objectId, _openBehavior);
        return CommandResult.ShowToast($"{_objectTitle}: {openMessage}");
    }
}
