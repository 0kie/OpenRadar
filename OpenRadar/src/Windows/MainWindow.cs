using System.Net.Mail;
using System.Net.WebSockets;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ECommons.GameHelpers;
using ECommons.ImGuiMethods;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using Microsoft.Win32.SafeHandles;
using Openradar;

namespace OpenRadar.Windows;

public class MainWindow : Window
{
    public MainWindow() : base($"OpenRadar {P.GetType().Assembly.GetName().Version} ###openradar")
    {
        Flags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoFocusOnAppearing;
        SizeConstraints = new()
        {
            MinimumSize = new Vector2(400, 300),
            MaximumSize = new Vector2(400, 300)
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

        var extractedPlayers = Data.ExtractedPlayers;

        if (extractedPlayers.Count > 0)
        {
            var listing = Data.CurrentPost;
                
            if (listing != null)
            {
                var dutyName = Util.DutyIdToName(listing.dutyId);
                ImGuiEx.TextCentered(new Vector4(0f, 1f, 0f, 1f), dutyName);
                ImGui.Separator();
                ImGui.Dummy(new Vector2(20,10));

                if (ImGui.BeginTable("Players", 4, ImGuiTableFlags.BordersH | ImGuiTableFlags.SizingFixedFit))
                {
                    ImGui.TableSetupColumn("##job", ImGuiTableColumnFlags.None, 20f);
                    ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.None, 140f);
                    ImGui.TableSetupColumn("World", ImGuiTableColumnFlags.None, 100f);
                    ImGui.TableSetupColumn("Prog", ImGuiTableColumnFlags.None, 80f);
                    ImGui.TableHeadersRow();
                    for (int i = 0; i < listing.jobIds.Count; i++)
                    {
                        var job = listing.jobIds[i];
                        ImGui.TableNextRow();
                        ImGui.TableNextColumn();
                        if (job != 0)
                        {
                            var player = extractedPlayers[i];
                            var jobIcon = Util.GetJobIcon(job);
                            if (jobIcon != null)
                                ImGui.Image(jobIcon.Handle, new Vector2(20,20));
                            else
                                ImGui.Image(Util.GetJobIcon(45)!.Handle, new Vector2(20,20));
                            ImGui.TableNextColumn();
                            if (player != null && !player.name.IsNullOrEmpty())
                            {   
                                ImGui.Text(player.name);
                            }
                            else
                            {
                                ImGui.TextColored(new Vector4(1f, 0.7f, 0.2f, 1f), "Finding Player...");
                            }
                            ImGui.TableNextColumn();
                            if (player == null || player.world == 0)
                                ImGui.Text("");
                            else
                            {
                                ImGui.Text(Util.WorldIdToName(player.world));
                            }
                            ImGui.TableNextColumn();
                            var prog = Data.ProgPoints[i];

                            if (prog != null)
                                ImGui.Text(prog);
                            else
                            {
                                using (var font = ImRaii.PushFont(UiBuilder.IconFont))
                                {
                                    ImGui.TextColored(new Vector4(1f, 0.7f, 0.2f, 1f),FontAwesomeIcon.Spinner.ToIconString());
                                }
                            }
                        }
                        else
                        {
                            ImGui.Image(Util.GetJobIcon(45)!.Handle, new Vector2(20,20));
                            ImGui.TableNextColumn();
                            ImGui.TextColored(new Vector4(0f, 1f, 0.2f, 1f), "Empty");
                        }
                    }
                }
                ImGui.EndTable();
            }
        }
        else
        {
            ImGui.Text("Loading...");
        }
    }
}
