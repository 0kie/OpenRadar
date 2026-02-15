
using Dalamud.Interface.Textures.TextureWraps;

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

    public unsafe static string ReadUtf8String(byte* b, int maxLength = 30)
    {
        int len = 0;
        while (len < maxLength && b[len] != 0)
            len++;

        return System.Text.Encoding.UTF8.GetString(b, len);
    }
}