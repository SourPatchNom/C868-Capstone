using AssetManager.Library.Classes.Asset;
using AssetManager.Library.Enums;

namespace AssetManager.Library.Tests;

public class AssetTypeUnitTest
{

    public static readonly object[][] AssetsAndType =
    {
        new object[] { new General(-1, "Test Asset", "Test Summary", "Test Author", "http://www.wgu.edu/", "/images/asset_icon.png/", "Test Doi", DateTime.UtcNow, true, true, true), AssetType.Asset},
        new object[] { new Book(-1, "Test Asset", "Test Summary", "Test Author", "https://www.wgu.edu/", "/images/asset_icon.png/", "Test Doi", DateTime.UtcNow, true, true, true,"ISBN","Publisher",DateTime.UtcNow), AssetType.Book },
        new object[] { new Cohort(-1, "Test Asset", "Test Summary", "Test Author", "https://www.wgu.edu/", "/images/asset_icon.png/", "Test Doi", DateTime.UtcNow, true, true, true,DateTime.UtcNow), AssetType.Cohort }
    };
    
    [Theory, MemberData(nameof(AssetsAndType))]
    public void ExtensionsAssetTypeEnumFromAssetTest(Asset asset, AssetType type)
    {
        Assert.Equal(Extensions.AssetTypeExtensions.GetAssetTypeEnum(asset), type);
    }
    
    public static readonly object[][] AssetsAndString =
    {
        new object[] { new General(-1, "Test Asset", "Test Summary", "Test Author", "http://www.wgu.edu/", "/images/asset_icon.png/", "Test Doi", DateTime.UtcNow, true, true, true), "" },
        new object[] { new Book(-1, "Test Asset", "Test Summary", "Test Author", "https://www.wgu.edu/", "/images/asset_icon.png/", "Test Doi", DateTime.UtcNow, true, true, true,"ISBN","Publisher",DateTime.UtcNow), "Book" },
        new object[] { new Cohort(-1, "Test Asset", "Test Summary", "Test Author", "https://www.wgu.edu/", "/images/asset_icon.png/", "Test Doi", DateTime.UtcNow, true, true, true,DateTime.UtcNow), "Cohort" }
    };
    
        
    [Theory, MemberData(nameof(AssetsAndString))]
    public void ExtensionsAssetTypeStringFromAssetTest(Asset asset, string assetString)
    {
        Assert.Equal(Extensions.AssetTypeExtensions.GetAssetTypeString(asset), assetString);
    }
}