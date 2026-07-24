using System;
using CapacitiesCommandPaletteExtension.Capacities;
using CapacitiesCommandPaletteExtension.Settings;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CapacitiesCommandPaletteExtension.Commands;

internal sealed partial class CreateObjectCommand : InvokableCommand
{
    private readonly CapacitiesSettingsManager _settingsManager;
    private readonly string _structureId;
    private readonly string _objectName;
    private readonly string _objectContent;

    public CreateObjectCommand(
        CapacitiesSettingsManager settingsManager,
        string structureId,
        string objectName,
        string objectContent)
    {
        _settingsManager = settingsManager;
        _structureId = structureId;
        _objectName = objectName;
        _objectContent = objectContent;
    }

    public override string Name => "Create object";

    public override IconInfo Icon => new("\uE70F");

    public override CommandResult Invoke()
    {
        var settings = _settingsManager.Current;
        var content = DecodeEscapedNewlines(_objectContent);

        var createResult = CapacitiesClient.CreateObject(settings.ApiToken, _structureId, _objectName, content);
        if (!createResult.Succeeded)
        {
            return CommandResult.ShowToast(createResult.Message);
        }

        var openResultMessage = HandlePostCreateOpen(settings.ApiToken, settings.CreateObjectOpenBehavior, createResult.ObjectId);
        return CommandResult.ShowToast(openResultMessage);
    }

    private static string HandlePostCreateOpen(string apiToken, CreateObjectOpenBehavior behavior, string objectId)
    {
        if (behavior == CreateObjectOpenBehavior.None)
        {
            return "Object created.";
        }

        var openMessage = ObjectOpenHelper.OpenObject(apiToken, objectId, behavior);
        return $"Object created. {openMessage}";
    }

    private static string DecodeEscapedNewlines(string text)
    {
        return text
            .Replace("\\r\\n", "\r\n", StringComparison.Ordinal)
            .Replace("\\n", "\n", StringComparison.Ordinal)
            .Replace("\\r", "\r", StringComparison.Ordinal);
    }
}
