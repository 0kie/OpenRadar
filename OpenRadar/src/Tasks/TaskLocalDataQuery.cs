namespace OpenRadar.Tasks;

public static class TaskLocalDataQuery
{
    public static void Enqueue(ulong contentId)
    {
        P.taskManager.Enqueue(() => LocalDataQuery(contentId));
    }

    private static void LocalDataQuery(ulong contentId)
    {
        if (contentId != 0)
        {
            Svc.Log.Debug($"1 - Querying Local Database: {contentId}");
            var playerInfo = Database.GetPlayerByContentId(contentId);
            if (playerInfo == null)
            {
                TaskPlayerTrackQuery.Enqueue(contentId);
            }
            else
            {
                Data.UpdatePlayerList(playerInfo);
            }
        }
    }
}