namespace AssetManager.Library.Classes;

public class CourseAsset
{
    public int CourseId { get; private set; }
    public int AssetId { get; private set; }
    public int Hierarchy { get; set; }

    public CourseAsset(int courseId, int assetId, int hierarchy)
    {
        CourseId = courseId;
        AssetId = assetId;
        Hierarchy = hierarchy;
    }
}