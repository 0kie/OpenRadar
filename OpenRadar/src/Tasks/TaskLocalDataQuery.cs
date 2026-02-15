namespace OpenRadar.Tasks;

public static class TaskLocalDataQuery
{
    public static void Enqueue(ulong contentId)
    {
        P.taskManager.Enqueue(() => LocalDataQuery(contentId));
    }

    private static void LocalDataQuery(ulong contentId)
    {
        var playerInfo = Database.GetPlayerByContentId(contentId);
        if (playerInfo == null)
        {
            Svc.Log.Information("Fetching and Parsing Plate Packet");
            TaskPlateInfoFetch.Enqueue(contentId);
        }
        else
        {
            Data.UpdatePlayerList(playerInfo);
        }
    }
}