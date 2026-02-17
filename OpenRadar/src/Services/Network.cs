using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Dalamud.Game.Gui.PartyFinder.Types;
using ECommons.ChatMethods;
using ECommons.DalamudServices.Legacy;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Lumina.Excel.Sheets;
using Microsoft.VisualBasic;
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

            if (opCode == 760)
            {
                // content_ids from pf post
                Data.ResetExtractedData();
                Util.PrintData<byte>(dataPtr, 20, 20);
                ushort dutyId = *((ushort*)dataPtr + 20);
                CurrentPost = new PostInfo(dutyId, new List<byte>(), new List<ulong>());
                for (int i = 0; i < 8; i++)
                {
                    ulong content_id = *((ulong*)dataPtr + i + 12);
                    byte jobId = *((byte*)dataPtr + i + 224);

                    CurrentPost.contentIds.Add(content_id);
                    CurrentPost.jobIds.Add(jobId);

                    TaskLocalDataQuery.Enqueue(content_id);
                }
            }
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

        // This will be awkward to implement and its low priority
        // very much doubt it gets available roles from pf post, rather gets it from the listings and stores locally
        /*
        Svc.Log.Debug($"{listing.Name}: ");
        var text = "";
        foreach (var job in listing.Slots)
        {
            text += $"{job.Accepting.First().ToString()} ";
        }
        Svc.Log.Debug(text);*/

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
