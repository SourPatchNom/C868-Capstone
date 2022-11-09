namespace AssetManager.Library.Classes.Asset;

public class Book : Asset
{
    public string Isbn { get; private set; }
    public string Publisher { get; private set; }
    public DateTime PublishDate { get; private set; }

    public Book(string name, string summary, string author, string url, string thumbnailUrl, string doi, DateTime created, bool isActive, bool isOfficial, bool isInternal, string isbn, string publisher, DateTime publishDate) : base(name, summary, author, url, thumbnailUrl, doi, created, isActive, isOfficial, isInternal)
    {
        Isbn = isbn;
        Publisher = publisher;
        PublishDate = publishDate;
    }

    public Book(int assetId, string name, string summary, string author, string url, string thumbnailUrl, string doi, DateTime created, bool isActive, bool isOfficial, bool isInternal, string isbn, string publisher, DateTime publishDate) : base(assetId, name, summary, author, url, thumbnailUrl, doi, created, isActive, isOfficial, isInternal)
    {
        Isbn = isbn;
        Publisher = publisher;
        PublishDate = publishDate;
    }
}