using System;
using Lumina.Excel.Sheets;

namespace OpenRadar;

public static class Encounters
{
    private static ushort[][] SavageRowIds =
    {
        [103, 104, 105, 106],       // aar

        [116, 117, 118, 119],       // heavensward t1
        [147, 148, 149, 150],       // heavensward  t2
        [190, 191, 192, 193],       // heavensward   t3

        [256, 257, 258, 259],       // stormblood t1
        [292, 293, 294, 295],       // stormblood  t2
        [591, 592, 593, 594],       // stormblood   t3

        [654, 683, 685, 690],       // shadowbringers t1
        [716, 720, 727, 729],       // shadowbringers  t2
        [748, 750, 752, 759],       // shadowbringers   t3

        [809, 811, 807, 801],       // endwalker t1
        [873, 881, 877, 884],       // endwalker  t2
        [937, 939, 941, 943],       // endwalker   t3

        [986, 988, 990, 992],       // dawntrail t1
        [1020, 1022, 1024, 1026],   // dawntrail  t2
        [1069, 1071, 1073, 1075]    // dawntrail   t3
    };

    private static ushort[] Ultimates = [280, 539, 694, 788, 908, 1006];

    public record Info
    (
        string? category,
        string name,
        string expansion,
        //string? savageChild = null,
        string? savageParent = null
    );

    public static class Colour
    {
        public static Vector4 Grey   = new(0.333f, 0.333f, 0.333f, 1f);
        public static Vector4 Green  = new(0.118f, 1f, 0f, 1f);
        public static Vector4 Blue   = new(0f, 0.439f, 1f, 1f);
        public static Vector4 Purple = new(0.639f, 0.208f, 0.933f, 1f);
        public static Vector4 Orange = new(1f, 0.502f, 0f, 1f);
        public static Vector4 Pink   = new(0.887f, 0.408f, 0.659f, 1f);
    }
    public static Info? DataQuery(ushort dutyId)
    {
        var duty = Svc.Data.GetExcelSheet<ContentFinderCondition>().FirstOrDefault(duty => duty.RowId == dutyId);
        if (duty.RowId == 0)
        {
            return null;
        }

        string? contentCategory = duty.ContentUICategory.Value.Name.ToString() switch
        {
            var category when category.StartsWith("Savage") => "savage",
            var category when category.StartsWith("High-end Trials") => "trials",
            _ when duty.ContentType.RowId == 28 => "ultimate",
            _ => null
        };

        var dutyName = duty.Name.ToString();
        var dutyNameClean = char.ToUpper(dutyName[0]) + dutyName.Substring(1);
        var dutyExpansion = duty.RequiredExVersion.Value.Name.ToString();
        
        if (contentCategory == "savage")
        {
            var tier = SavageRowIds.FirstOrDefault(row => row.Contains(dutyId));
            if (tier == null)
                return null;

            var savageParent = Util.DutyIdToName(tier.Last());
            return new Info(contentCategory, dutyNameClean, dutyExpansion, savageParent);
        }

        return new Info(contentCategory, dutyNameClean, dutyExpansion);
    }

    public static Vector4 ProgToColour(string prog, ushort dutyId)
    {
        // first decypher prog
        Vector4 fail = new Vector4(1f, 1f, 1f, 1f);
        string[] progParts = prog.Split(' ');
        bool ultimateOrDoor = false;
        float? progPercent = null;
        if (progParts.Length > 1)
            ultimateOrDoor = true;

        var cleaned = progParts[0].Replace("%", "");
        if (float.TryParse(cleaned, out float parsed))
            progPercent = parsed;


        if (progPercent == null)
            return fail;

        float percent = progPercent.Value;

        if (!ultimateOrDoor)
        {
            return percent switch
            {
                > 75f => Colour.Grey,
                > 50f => Colour.Green,
                > 25f => Colour.Blue,
                > 10f => Colour.Purple,
                > 3f => Colour.Orange,
                _ => Colour.Pink
            };
        }
        else
        {
            var part = progParts[1];

            if (!int.TryParse(part.AsSpan(1), out int phase))
                return fail;

            if (part[0] == 'I')
                phase = -phase;

            if (!Ultimates.Contains(dutyId)) // door boss, some savageparents
            {
                return (percent, phase) switch
                {
                    (>50f, 1) => Colour.Grey,
                    (>20f, 1) => Colour.Green,
                    (_, 1) => Colour.Blue,
                    (>50f, 2) => Colour.Purple,
                    (>20f, 2) => Colour.Orange,
                    (_, _) => Colour.Pink
                };
            }
            return dutyId switch
            {
                1006 => phase switch // fru
                {
                    1 => Colour.Grey,
                    2 => Colour.Green,
                    3 => Colour.Blue,
                    4 => Colour.Purple,
                    _ when percent < 10f => Colour.Orange,
                    _ => Colour.Pink
                },
                908 => phase switch // top
                {
                    1 => Colour.Grey,
                    2 => Colour.Green,
                    3 => Colour.Blue,
                    4 => Colour.Purple,
                    5 => Colour.Orange,
                    _ => Colour.Pink
                },
                788 => phase switch // dsr
                {
                    <=2 => Colour.Grey,
                    3 => Colour.Green,
                    4 or -1 => Colour.Blue,
                    5 => Colour.Purple,
                    6 => Colour.Orange,
                    _ => Colour.Pink
                },
                694 => phase switch // tea
                {
                    1 or -1 => Colour.Grey,
                    2 => Colour.Green,
                    -2 => Colour.Blue,
                    3 => Colour.Purple,
                    _ when percent > 10f => Colour.Orange,
                    _ => Colour.Pink
                },
                539 => phase switch // uwu
                {
                    1 => Colour.Grey,
                    2 => Colour.Green,
                    3 => Colour.Blue,
                    _ when percent > 50f => Colour.Purple,
                    _ when percent > 10f => Colour.Orange,
                    _ => Colour.Pink
                },
                280 => phase switch // ucob
                {
                    1 => Colour.Grey,
                    2 => Colour.Green,
                    3 => Colour.Blue,
                    4 => Colour.Purple,
                    _ when percent > 10f => Colour.Orange,
                    _ => Colour.Pink
                },
                _ => fail
            };
        }
    }
}