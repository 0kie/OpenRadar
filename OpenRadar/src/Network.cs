using System.Collections.Generic;
using Dalamud.Game.Gui.PartyFinder.Types;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices.Legacy;
using ECommons.GameHelpers;

namespace OpenRadar;

public static class Network
{
    public static List<ListingInformation> PFListings = new();
    public static List<PlayerInfo> RecentExtractedPlayers = new();
    private static bool IsReceivingPage = false;

    public unsafe static void PFExtract(
        nint dataPtr,
        ushort opCode,
        uint sourceActorId,
        uint targetActorId,
        NetworkMessageDirection direction)
    {        
        if (opCode == 760 && direction == NetworkMessageDirection.ZoneDown)
        {
            Svc.Log.Debug("PF Post Content ID Extraction");
            RecentExtractedPlayers.Clear();
            for (int i = 12; i < 20; i++)
            {
                // content_ids stored as ulongs in packet
                ulong content_id = *((ulong*)dataPtr + i);
                var playerInfo = PlayerTrackInterop.Extract(content_id);
                if (playerInfo != null)
                    RecentExtractedPlayers.Add(playerInfo);
            }
        }
        else if (opCode == 621)
        {
            // pf post packets, containing host, type of duty, dutyid etc
        }
        else if (opCode == 179)
        {
            // end packet after pf post packets delivered, probably total number of posts
            Svc.Log.Debug("PF Page Complete");
            IsReceivingPage = false;
        }
    }
    public static void ListingExtract(IPartyFinderListing listing, IPartyFinderListingEventArgs args)
    {
        if (!IsReceivingPage)
        {
            PFListings.Clear();
            IsReceivingPage = true;
        }

        Svc.Log.Debug("PF Listing Extraction");
        if (args.Visible == true)
        {
            var extractedListing = new ListingInformation();
            extractedListing.hostContentId = listing.ContentId;
            var jobsPresent = listing.JobsPresent;
            foreach (var job in jobsPresent)
            {
                extractedListing.jobsPresent.Add(job.Value);
            }
            extractedListing.duty = listing.Duty.Value;
            var desc = listing.Description.TextValue;
            if (!desc.IsNullOrEmpty())
                extractedListing.description = desc.ToString();
            else
                extractedListing.description = "None";
            extractedListing.hostName = listing.Name.TextValue;
            extractedListing.hostWorld = listing.HomeWorld.Value.InternalName.ExtractText();
            extractedListing.slotCount = listing.Slots.Count;
            PFListings.Add(extractedListing);
        }
    }
}
