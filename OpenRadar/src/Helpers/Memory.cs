using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Dalamud.Game.Gui.PartyFinder.Types;
using Dalamud.Hooking;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using OpenRadar.Tasks;
using SQLitePCL;





namespace OpenRadar;

public unsafe class Memory : IDisposable
{
    private delegate void RequestPlateInfoDelegate(ulong param_1, ulong contentId);
    [Signature("40 53 48 81 EC 80 0F 00 00 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 70 0F 00 00 48 8B 0D ?? ?? ?? ?? 48 8B DA E8 ?? ?? ?? ?? 45 33 C9 C7 44 24 20 B0 00 00 00 45 33 C0 48 C7 44 24 28 20 00 00 00 48 8D 54 24 20 48 89 5C 24 40 48 8B C8 C7 44 24 48 01 00 00 00")]
    private RequestPlateInfoDelegate getRequestPlateInfo = null!;

    private delegate void OnPostPacketReceiveDelegate(ulong param_1, long* param_2);
    [Signature("48 89 5C 24 18 55 48 8D AC 24 60 FC FF FF", DetourName = nameof(OnPostReceiveDetour))]
    private Hook<OnPostPacketReceiveDelegate> onPostPacketReceiveHook = null!;

    private void OnPostReceiveDetour(ulong param_1, long* packetPtr)
    {
        ushort dutyId = *((ushort*)packetPtr + 20);
        CurrentPost = new PostInfo(dutyId, new List<ISharedImmediateTexture?>(), new List<IDalamudTextureWrap?>(), new List<ulong>());
        for (int i = 0; i < 8; i++)
        {
            ulong content_id = *((ulong*)packetPtr + i + 12);
            byte jobId = *((byte*)packetPtr + i + 224);
            ulong slotAccepting = *((ulong*)packetPtr + i + 20);

            CurrentPost.contentIds.Add(content_id);
            CurrentPost.jobIcons.Add(Util.GetJobIcon(jobId));
            CurrentPost.roleIcons.Add(Util.JobFlagsToRoleTexture((JobFlags)slotAccepting));

            TaskLocalDataQuery.Enqueue(content_id);
        }
        onPostPacketReceiveHook.Original(param_1, packetPtr);
    }

    public void RequestPlateInfo(ulong contentId)
    {
        getRequestPlateInfo(0, contentId);
    }

    public Memory()
    {
        Svc.Hook.InitializeFromAttributes(this);
        onPostPacketReceiveHook.Enable();
    }

    public void Dispose()
    {
        onPostPacketReceiveHook.Dispose();
    }
}