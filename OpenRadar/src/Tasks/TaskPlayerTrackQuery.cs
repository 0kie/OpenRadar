namespace OpenRadar.Tasks;

public static class TaskPlayerTrackQuery
{
    public static void Enqueue(ulong contentId)
    {
        P.taskManager.Enqueue(() => QueryPlayerTrack(contentId));
    }

    private static void QueryPlayerTrack(ulong contentId)
    {
        Svc.Log.Debug($"2 - Querying PlayerTrack: {contentId}");
        var playerInfo = PlayerTrackInterop.Extract(contentId);
        if (playerInfo == null)
        {
            TaskPlateInfoFetch.Enqueue(contentId);
        }
        else
        {
            Data.UpdatePlayerList(playerInfo);
        }
    }
}