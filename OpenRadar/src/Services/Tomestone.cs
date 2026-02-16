using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using OpenRadar;

namespace Openradar;

public static class Tomestone
{
    // Tomestone's API still is WPI and requires an auth key, thus this solves redirect and parses html. 
    // Unsure whether this is generally frowned upon. Also involves large packets.
    // Will break if anti-bot measures are implemented.
    // Later implementation should ask user for auth key and heavily rate limit parsing html directly
    // First implementation is purely just parsing html because it's easier. The API is confusing

    public static void GetProg()
    {
        //ResolveRedirect("name", "world").Wait();
    }

    private static async Task GetPartyProgAsync(List<PlayerInfo?> ExtractedPlayers)
    {
        
    }

    private static async Task<string?> ResolveRedirect(string name, string world)
    {
        var nameEncoded = Uri.EscapeDataString(name);
        var url = $"https://tomestone.gg/character-name/{world}/{nameEncoded}";

        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = false
        };

        using var client = new HttpClient(handler);

        using var request = new HttpRequestMessage(HttpMethod.Head, url);
        using var response = await client.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Found && response.Headers.Location != null)
        {
            return response.Headers.Location.AbsoluteUri;
        }
        return null;
    }

}