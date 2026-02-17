using Lumina.Excel.Sheets;

namespace OpenRadar;

public static class Encounters
{
    // progress?encounterExpansion=endwalker
    public record Info
    (
        string? category,
        string name,
        string expansion
    );

    public static Info? DataQuery(ushort dutyId)
    {
        var duty = Svc.Data.GetExcelSheet<ContentFinderCondition>().FirstOrDefault(duty => duty.RowId == dutyId);
        if (duty.RowId == 0)
        {
            return null;
        }
        var contentUICategory = duty.ContentUICategory.Value.Name.ToString();
        string? contentCategory = null;
        if (contentUICategory.StartsWith("Savage"))
            contentCategory = "savage";
        else if (contentUICategory.StartsWith("High-end Trials"))
            contentCategory = "extreme";
        else if (duty.ContentType.RowId == 28)
            contentCategory = "ultimate";        

        var dutyName = duty.Name.ToString();
        
        return new Info(contentCategory, char.ToUpper(dutyName[0]) + dutyName.Substring(1), duty.RequiredExVersion.Value.Name.ToString());
    }
}