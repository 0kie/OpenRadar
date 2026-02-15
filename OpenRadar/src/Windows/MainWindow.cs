using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Lumina.Excel.Sheets;

namespace OpenRadar.Windows;

public class MainWindow : Window
{
    public MainWindow() : base($"OpenRadar {P.GetType().Assembly.GetName().Version} ###openradar")
    {
        Flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize;
        SizeConstraints = new()
        {
            MinimumSize = new Vector2(250, 350),
            MaximumSize = new Vector2(250, 350)
        };
        P.windowSystem.AddWindow(this);
    }

    public void Dispose()
    {
        P.windowSystem.RemoveWindow(this);
    }

    public override void Draw()
    {
        var LookingForGroupDetailPos = AddonHandler.addonPosition;
        var windowPos = new Vector2(LookingForGroupDetailPos.X + AddonHandler.addonWidth, LookingForGroupDetailPos.Y);
        ImGui.SetWindowPos(windowPos);

        var extractedPlayers = Network.RecentExtractedPlayers;

        if (extractedPlayers.Count > 0)
        {
            var listing = Network.PFListings
                .FirstOrDefault(l => 
                l.hostContentId == extractedPlayers.First().content_id);
                
            if (listing != null)
            {
                ImGui.TextColoredWrapped(new Vector4(0f, 1f, 0f, 1f), listing.duty.Name.ToString());
                ImGui.Text($"{listing.hostName} - {listing.hostWorld}");
                ImGui.Separator();
                ImGui.Dummy(new Vector2(20,20));
                for (int i = 0; i < listing.slotCount; i++)
                {
                    var job = listing.jobsPresent[i];
                    if (job.RowId != 0)
                    {
                        ImGui.Text($"[{job.Abbreviation.GetText().ToString()}] -");
                        ImGui.SameLine();
                        var player = extractedPlayers[i];
                        if (!player.name.IsNullOrEmpty())
                        {
                            ImGui.Text($"{player.name} - {player.world}");
                        }
                        else
                        {
                            ImGui.Text("Player Missing");
                        }
                    }
                    else
                    {
                        ImGui.Text("Empty");
                    }
                }
            }
        }
        else
        {
            ImGui.Text("Loading...");
        }
    }
}
