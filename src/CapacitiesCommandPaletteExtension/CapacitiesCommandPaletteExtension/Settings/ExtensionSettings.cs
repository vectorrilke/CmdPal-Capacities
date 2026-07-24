namespace CapacitiesCommandPaletteExtension.Settings;

internal enum InsertMode
{
    Append,
}

internal enum OutputMode
{
    Markdown,
}

internal enum CreateObjectOpenBehavior
{
    None,
    Web,
    App,
}

internal sealed record ExtensionSettings(
    string ApiToken,
    InsertMode InsertMode,
    OutputMode OutputMode,
    CreateObjectOpenBehavior CreateObjectOpenBehavior);