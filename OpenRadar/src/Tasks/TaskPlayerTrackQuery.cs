namespace OpenRadar.Tasks;

public static class TaskPlayerTrackQuery
{
    public static void Enqueue(ulong contentId)
    {
        P.taskManager.Enqueue(() => QueryPlayerTrack(contentId));
    }

    private static void QueryPlayerTrack(ulong contentId)
    {
        if (C.PlayerTrackReader)
        {
            Svc.Log.Debug($"2 - Querying PlayerTrack: {contentId}");
            var playerInfo = PlayerTrackInterop.Extract(contentId);
            if (playerInfo != null)
            {
                Data.UpdatePlayerList(playerInfo);
                return;
                
            }
        }
        TaskPlateInfoFetch.Enqueue(contentId);
    }
}