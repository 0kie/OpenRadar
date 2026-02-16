using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace OpenRadar.Tasks;

public static class TaskFriendInfoFetch
{
    public static void Enqueue(ulong contentId)
    {
            P.taskManager.Enqueue(() => FriendInfoFetch(contentId));
    }

    private unsafe static bool FriendInfoFetch(ulong contentId)
    {
        if (!EzThrottler.Throttle("FriendInfo", 4000))
            return false; 
        Svc.Log.Debug($"4 - Fetching and Parsing Friend Packet: {contentId}");
        AgentFriendlist.Instance()->RequestFriendInfo(contentId);

        return true;
    }
}