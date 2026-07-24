using CapacitiesCommandPaletteExtension.Capacities;
using CapacitiesCommandPaletteExtension.Parsing;
using CapacitiesCommandPaletteExtension.Settings;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CapacitiesCommandPaletteExtension.Commands;

internal sealed partial class AppendToObjectAndOpenCommand : InvokableCommand
{
    private readonly CapacitiesSettingsManager _settingsManager;
    private readonly string _objectId;
    private readonly string _objectTitle;
    private readonly string _rawInput;
    private readonly CreateObjectOpenBehavior _openBehavior;

    public AppendToObjectAndOpenCommand(
        CapacitiesSettingsManager settingsManager,
        string objectId,
        string objectTitle,
        string rawInput,
        CreateObjectOpenBehavior openBehavior)
    {
        _settingsManager = settingsManager;
        _objectId = objectId;
        _objectTitle = objectTitle;
        _rawInput = rawInput;
        _openBehavior = openBehavior;
    }

    public override string Name => "Append to object";

    public override IconInfo Icon => new("\uE8D2");

    public override CommandResult Invoke()
    {
        var settings = _settingsManager.Current;

        if (string.IsNullOrWhiteSpace(settings.ApiToken))
        {
            return CommandResult.ShowToast("Add your Capacities API token in the settings first.");
        }

        if (!CapCommandParser.TryParse(_rawInput, out var parsedInput, out var errorMessage) || parsedInput is null)
        {
            return CommandResult.ShowToast(errorMessage ?? "Could not parse the input.");
        }

        var appendResult = CapacitiesClient.AppendToObject(settings.ApiToken, _objectId, parsedInput);
        if (!appendResult.Succeeded)
        {
            return CommandResult.ShowToast(appendResult.Message);
        }

        var openMessage = ObjectOpenHelper.OpenObject(settings.ApiToken, _objectId, _openBehavior);
        return _openBehavior == CreateObjectOpenBehavior.None
            ? CommandResult.ShowToast(appendResult.Message)
            : CommandResult.ShowToast($"{appendResult.Message} {_objectTitle}: {openMessage}");
    }
}
