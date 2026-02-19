using System.Collections.Generic;
using Dalamud.Game.Agent;
using Dalamud.Game.Agent.AgentArgTypes;
using Dalamud.Game.Gui.PartyFinder.Types;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using ECommons.DalamudServices.Legacy;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Arrays.Common;
using OpenRadar.Tasks;

namespace OpenRadar;

public static class Network
{
    public static List<PlayerInfo?> RecentExtractedPlayers = new();
    public static ulong FailedContentId = 0;

    public unsafe static void PFExtract(nint dataPtr, ushort opCode, uint sourceActorId, uint targetActorId, NetworkMessageDirection direction)
    {        
        if (direction == NetworkMessageDirection.ZoneDown)
        {
            if (opCode == 689)
            {
                // PlateInfo found
                var playerInfo = FetchPlatePacketInfo(dataPtr);  
                if (playerInfo!=null)
                {
                    Database.AddPlayer(playerInfo);           
                    Data.UpdatePlayerList(playerInfo);
                }
            }
            if (opCode == 589)
            {
                // Plate Fail
                TaskFriendInfoFetch.Enqueue(FailedContentId);
            }
            if (opCode == 282)
            {   
                // Friend Packet
                ulong contentId = *((ulong*)dataPtr+1);
                string name = Util.ReadUtf8String((byte*)dataPtr+22);
                ushort worldId = *((ushort*)dataPtr + 8);
                PlayerInfo playerInfo = new PlayerInfo(contentId, name, worldId);

                Database.AddPlayer(playerInfo);
                Data.UpdatePlayerList(playerInfo);
            }
        }
    }

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
