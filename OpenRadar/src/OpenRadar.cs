using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using OpenRadar.Windows;
using ECommons.Configuration;
using ECommons.DalamudServices.Legacy;
using ECommons.Opcodes;
using Dalamud.Game.Addon.Lifecycle;

namespace OpenRadar;

public sealed class OpenRadar : IDalamudPlugin
{
    public static string Name => "OpenRadar";

    internal static OpenRadar P = null!;
    private Configuration config = null!;
    public static Configuration C => P.config;


    internal WindowSystem windowSystem = null!;
    internal MainWindow mainWindow = null!;


    public OpenRadar(IDalamudPluginInterface pi)
    {
        P = this;
        ECommonsMain.Init(pi, P, Module.DalamudReflector, Module.ObjectFunctions);
        new ECommons.Schedulers.TickScheduler(Load);
    }
    public void Load()
    {
        EzConfig.Migrate<Configuration>();
        config = EzConfig.Init<Configuration>();
        windowSystem = new();
        mainWindow = new();
        Svc.PluginInterface.UiBuilder.Draw += windowSystem.Draw;
        Svc.GameNetwork.NetworkMessage += Network.PFExtract;
        Svc.PfGui.ReceiveListing += Network.ListingExtract;

        Svc.AddonLifecycle.RegisterListener(AddonEvent.PostDraw, "LookingForGroupDetail", AddonHandler.LookingForGroupDetail);

        Svc.PluginInterface.UiBuilder.OpenMainUi += () =>
        {
            mainWindow.IsOpen = true;
        };
        Svc.PluginInterface.UiBuilder.OpenConfigUi += () =>
        {
            mainWindow.IsOpen = true;
        };
        EzCmd.Add("/openradar", OnCommand);
    }

    public void Dispose()
    {
        GenericHelpers.Safe(() => Svc.PluginInterface.UiBuilder.Draw -= windowSystem.Draw);
        GenericHelpers.Safe(() => Svc.GameNetwork.NetworkMessage -= Network.PFExtract);
        GenericHelpers.Safe(() => Svc.PfGui.ReceiveListing -= Network.ListingExtract);
        GenericHelpers.Safe(() =>Svc.AddonLifecycle.UnregisterListener(AddonEvent.PostDraw, "LookingForGroupDetail", AddonHandler.LookingForGroupDetail));
        ECommonsMain.Dispose();
    }

    private void OnCommand(string command, string args)
    {
        var subcommands = args.Split(' ');

        if (subcommands.Length == 0 || args == "")
        {
            mainWindow.IsOpen = !mainWindow.IsOpen;
            return;
        }
    }
}
