using AssetManager.Library.Classes.Asset;

namespace AssetManager.Library.Tests;

public class AssetDataUnitTest
{
    [Fact]
    public void AssetIsValidGeneral()
    {
        var dateTime = DateTime.UtcNow;
        var general = new General(-1, "Test Asset", "Test Summary", "Test Author", "http://www.wgu.edu/", "/images/asset_icon.png/", "Test Doi", dateTime, true, true, true);
        Assert.IsAssignableFrom<Asset>(general);
        Assert.Equal("Test Asset", general.Name);
        Assert.Equal("Test Summary", general.Summary);
        Assert.Equal("Test Author", general.Author);
        Assert.Equal("http://www.wgu.edu/",general.Url);
        Assert.Equal("/images/asset_icon.png/",general.ThumbnailUrl);
        Assert.Equal("Test Doi",general.Doi);
        Assert.Equal(dateTime,general.Created);
        Assert.True(general.IsActive);
        Assert.True(general.IsInternal);
        Assert.True(general.IsOfficial);
    }

    [Fact]
    public void AssetIsValidBook()
    {
        var dateTime = DateTime.UtcNow;
        var book = new Book(-1, "Test Asset", "Test Summary", "Test Author", "https://www.wgu.edu/", "/images/asset_icon.png/", "Test Doi", dateTime, true, true, true,"ISBN","Publisher",dateTime);
        Assert.IsAssignableFrom<Asset>(book);
        Assert.Equal("Test Asset", book.Name);
        Assert.Equal("Test Summary", book.Summary);
        Assert.Equal("Test Author", book.Author);
        Assert.Equal("https://www.wgu.edu/",book.Url);
        Assert.Equal("/images/asset_icon.png/",book.ThumbnailUrl);
        Assert.Equal("Test Doi",book.Doi);
        Assert.Equal(dateTime,book.Created);
        Assert.True(book.IsActive);
        Assert.True(book.IsInternal);
        Assert.True(book.IsOfficial);
        Assert.Equal("ISBN",book.Isbn);
        Assert.Equal("Publisher",book.Publisher);
        Assert.Equal(dateTime,book.PublishDate);
    }
    
    [Fact]
    public void AssetIsValidCohort()
    {
        var dateTime = DateTime.UtcNow;
        var cohort = new Cohort(-1, "Test Asset", "Test Summary", "Test Author", "https://www.wgu.edu/", "/images/asset_icon.png/", "Test Doi", dateTime, true, true, true,dateTime);
        Assert.IsAssignableFrom<Asset>(cohort);
        Assert.Equal("Test Asset", cohort.Name);
        Assert.Equal("Test Summary", cohort.Summary);
        Assert.Equal("Test Author", cohort.Author);
        Assert.Equal("https://www.wgu.edu/",cohort.Url);
        Assert.Equal("/images/asset_icon.png/",cohort.ThumbnailUrl);
        Assert.Equal("Test Doi",cohort.Doi);
        Assert.Equal(dateTime,cohort.Created);
        Assert.True(cohort.IsActive);
        Assert.True(cohort.IsInternal);
        Assert.True(cohort.IsOfficial);
        Assert.Equal(dateTime,cohort.Recorded);
    }
}