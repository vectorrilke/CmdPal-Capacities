using System;
using System.Diagnostics;
using CapacitiesCommandPaletteExtension.Capacities;
using CapacitiesCommandPaletteExtension.Settings;
using Microsoft.Win32;

namespace CapacitiesCommandPaletteExtension.Commands;

internal static class ObjectOpenHelper
{
    public static string OpenObject(string apiToken, string objectId, CreateObjectOpenBehavior behavior)
    {
        if (behavior == CreateObjectOpenBehavior.None)
        {
            return "No open action selected.";
        }

        var spaceResult = CapacitiesClient.GetSpaceId(apiToken);
        if (!spaceResult.Succeeded)
        {
            return "Could not load space ID for opening.";
        }

        try
        {
            if (behavior == CreateObjectOpenBehavior.App)
            {
                if (!IsCapacitiesProtocolRegistered())
                {
                    return "Capacities app protocol not found.";
                }

                Process.Start(new ProcessStartInfo($"capacities://{spaceResult.SpaceId}/{objectId}")
                {
                    UseShellExecute = true,
                });
                return "Opened Capacities app on the object.";
            }

            var webUrl = CapacitiesClient.BuildObjectWebUrl(spaceResult.SpaceId, objectId);
            Process.Start(new ProcessStartInfo(webUrl)
            {
                UseShellExecute = true,
            });
            return "Opened Capacities web.";
        }
        catch
        {
            return "Could not open configured destination.";
        }
    }

    private static bool IsCapacitiesProtocolRegistered()
    {
        using var protocolKey = Registry.ClassesRoot.OpenSubKey("capacities");
        return protocolKey is not null;
    }
}