namespace OpenRadar.Tasks;

public static class TaskPlayerTrackQuery
{
    public static void Enqueue(ulong contentId)
    {
        P.taskManager.Enqueue(() => QueryPlayerTrack(contentId));
    }

    private static void QueryPlayerTrack(ulong contentId)
    {
        if (contentId != 0)
        {
            var playerInfo = PlayerTrackInterop.Extract(contentId);
            if (playerInfo == null)
            {
                Svc.Log.Information("Querying Local Database");
                TaskLocalDataQuery.Enqueue(contentId);
            }
            else
            {
                Data.UpdatePlayerList(playerInfo);
            }
        }
    }
}