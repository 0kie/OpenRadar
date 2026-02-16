using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Lumina.Excel.Sheets;

namespace OpenRadar;

public static class ChatHandler
{
    public static void PlateError(XivChatType type, int timestamp, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (type == XivChatType.ErrorMessage && Util.ContainsSeString(message, "Unable to display adventurer plate"))
        {
            message = "";
        }
    }
}