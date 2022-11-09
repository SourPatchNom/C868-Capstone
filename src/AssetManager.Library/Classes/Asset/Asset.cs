namespace AssetManager.Library.Classes.Asset;

public abstract class Asset
{
    public int AssetId { get; private set; }
    public string Name { get; private set; }
    public string Summary { get; private set; }
    public string Author { get; private set; }
    public string Url { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public string? Doi { get; private set; }
    public DateTime Created { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsOfficial { get; private set; }
    public bool IsInternal { get; private set; }

    protected Asset(string name, string summary, string author, string url, string thumbnailUrl, string? doi, DateTime created, bool isActive, bool isOfficial, bool isInternal)
    {
        AssetId = -1;
        Name = name;
        Summary = summary;
        Author = author;
        Url = url;
        ThumbnailUrl = thumbnailUrl;
        Doi = doi;
        Created = created;
        IsActive = isActive;
        IsOfficial = isOfficial;
        IsInternal = isInternal;
    }

    protected Asset(int assetId, string name, string summary, string author, string url, string thumbnailUrl, string? doi, DateTime created, bool isActive, bool isOfficial, bool isInternal)
    {
        AssetId = assetId;
        Name = name;
        Summary = summary;
        Author = author;
        Url = url;
        ThumbnailUrl = thumbnailUrl;
        Doi = doi;
        Created = created;
        IsActive = isActive;
        IsOfficial = isOfficial;
        IsInternal = isInternal;
    }
}