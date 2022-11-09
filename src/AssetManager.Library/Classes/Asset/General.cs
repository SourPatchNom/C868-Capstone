namespace AssetManager.Library.Classes.Asset;

public class General : Asset
{
    public General(string name, string summary, string author, string url, string thumbnailUrl, string doi, DateTime created, bool isActive, bool isOfficial, bool isInternal) : base(name, summary, author, url, thumbnailUrl, doi, created, isActive, isOfficial, isInternal)
    {
    }

    public General(int assetId, string name, string summary, string author, string url, string thumbnailUrl, string doi, DateTime created, bool isActive, bool isOfficial, bool isInternal) : base(assetId, name, summary, author, url, thumbnailUrl, doi, created, isActive, isOfficial, isInternal)
    {
    }
}