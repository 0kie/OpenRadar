
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Textures.TextureWraps;
using Lumina.Excel.Sheets;

namespace OpenRadar;

public static class Util
{
    public static IDalamudTextureWrap? GetJobIcon(uint? jobId)
    {
        if (jobId != null)
        {
            var jobTexture = Svc.Texture.GetFromGame("ui/icon/062000/0621" + jobId + ".tex").GetWrapOrEmpty();
            return jobTexture;
        }
        return null;
    }

    public unsafe static void PrintData<T>(nint dataPtr, int totalRows, int infoPerRow) where T : unmanaged
    {
        T* ptr = (T*)dataPtr;

        for (int row = 0; row < totalRows; row++)
        {
            string packetInfoRow = "";
            for (int col = 0; col < infoPerRow; col++)
            {
                T dataPoint = *(ptr + row * infoPerRow + col);
                packetInfoRow += $"{dataPoint} ";
            }
            Svc.Log.Debug(packetInfoRow);
        }
    }

    public unsafe static string ReadUtf8String(byte* b, int maxLength = 30, bool endAtNull = true)
    {
        int len = 0;
        if (endAtNull)
            while (len < maxLength && b[len] != 0)
                len++;
        else
            len = maxLength;

        return System.Text.Encoding.UTF8.GetString(b, len);
    }

    public static bool ContainsSeString(SeString seString, string part)
    {
        var fullText = seString.TextValue;
        return fullText.Contains(part, System.StringComparison.OrdinalIgnoreCase);
    }

    public static string WorldIdToName(ushort worldId)
    {
        var world = Svc.Data.GetExcelSheet<World>().First(world => world.RowId == worldId).InternalName.ExtractText();
        return world;
    }

    public static string DutyIdToName(ushort dutyId)
    {
        var dutyName = Svc.Data.GetExcelSheet<ContentFinderCondition>().FirstOrDefault(duty => duty.RowId == dutyId).Name.ExtractText();

        if (dutyName.IsNullOrEmpty())
            return "Unknown Duty";

        return char.ToUpper(dutyName[0]) + dutyName.Substring(1);
    }
}