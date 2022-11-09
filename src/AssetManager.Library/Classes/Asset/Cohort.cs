namespace AssetManager.Library.Classes.Asset;

public class Cohort : Asset
{
    public DateTime Recorded { get; private set; }

    public Cohort(string name, string summary, string author, string url, string thumbnailUrl, string doi, DateTime created, bool isActive, bool isOfficial, bool isInternal, DateTime recorded) : base(name, summary, author, url, thumbnailUrl, doi, created, isActive, isOfficial, isInternal)
    {
        Recorded = recorded;
    }

    public Cohort(int assetId, string name, string summary, string author, string url, string thumbnailUrl, string doi, DateTime created, bool isActive, bool isOfficial, bool isInternal, DateTime recorded) : base(assetId, name, summary, author, url, thumbnailUrl, doi, created, isActive, isOfficial, isInternal)
    {
        Recorded = recorded;
    }
}