using System.Collections.Generic;
using Dalamud.Game.Gui.PartyFinder.Types;

namespace OpenRadar;

public static class Network
{
    public static List<PlayerInfo?> RecentExtractedPlayers = new();
    public static ulong FailedContentId = 0;

    public static void ListingHostExtract(IPartyFinderListing listing, IPartyFinderListingEventArgs args)
    {
        var playerInfo = new PlayerInfo(listing.ContentId, listing.Name.TextValue, (ushort)listing.HomeWorld.RowId);
        Database.AddPlayer(playerInfo); 
    }

    private unsafe static PlayerInfo FetchPlatePacketInfo(nint ptr)
    {
        var contentId = *((ulong*)ptr+2);
        var playerName = Util.ReadUtf8String((byte*)ptr + 421);
        ushort worldId = *((ushort*)ptr + 16);

        return new PlayerInfo(contentId, playerName, worldId);
    }
}
