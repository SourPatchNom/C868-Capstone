using AssetManager.Library.Enums;

namespace AssetManager.Library.Classes;

public class AssetHistory
{
    public int RecordId { get; private set; }
    public int AssetId { get; private set; }
    public string Editor { get; private set; }
    public DateTime EditDate { get; private set; }

    public AssetHistory(int assetId, string editor, DateTime editDate)
    {
        RecordId = -1;
        AssetId = assetId;
        Editor = editor;
        EditDate = editDate;
    }

    public AssetHistory(int recordId, int assetId, string editor, DateTime editDate)
    {
        RecordId = recordId;
        AssetId = assetId;
        Editor = editor;
        EditDate = editDate;
    }
}