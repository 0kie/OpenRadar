using System.Collections.Generic;
using Lumina.Excel.Sheets;

namespace OpenRadar;

public static class Data
{
    public static List<ulong> FailedPlateContentIds = new List<ulong>();
    //public static List<ulong> ExtractedContentIds = Enumerable.Repeat<ulong>(0, 8).ToList();

    public static PostInfo CurrentPost = new PostInfo(0, new List<byte>(), new List<ulong>());

    public static List<PlayerInfo?> ExtractedPlayers = Enumerable.Repeat<PlayerInfo?>(null, 8).ToList();

    public static void UpdatePlayerList(PlayerInfo? playerInfo)
    {
        if (playerInfo == null)
            return;
        int index = CurrentPost.contentIds.IndexOf(playerInfo.content_id);

        if (index >= 0 && index < ExtractedPlayers.Count)
        {
            ExtractedPlayers[index] = playerInfo;
        }
        else
        {
            Svc.Log.Debug($"ContentId {playerInfo.content_id} not found in ExtractedContentIds.");
        }
    }

    public static void ResetExtractedData()
    {
        //ExtractedContentIds = Enumerable.Repeat<ulong>(0, 8).ToList();
        CurrentPost = new PostInfo(0, new List<byte>(), new List<ulong>());
        ExtractedPlayers = Enumerable.Repeat<PlayerInfo?>(null, 8).ToList();
    }

    public record PlayerInfo(
        ulong content_id,
        string? name,
        ushort world
    );

    public record PostInfo(
        ushort dutyId,
        List<byte> jobIds,
        List<ulong> contentIds
    );

    public class ListingInformation
    {
        public ulong hostContentId { get; set; }
        public List<ClassJob> jobsPresent { get; set; } = new();
        public ContentFinderCondition duty { get; set; }
        public string? description { get; set; }
        public string? hostName { get; set; }
        public string? hostWorld { get; set; }
        public int slotCount { get; set; }
    }
}