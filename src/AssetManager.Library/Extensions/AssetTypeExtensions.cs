using AssetManager.Library.Classes.Asset;
using AssetManager.Library.Classes;
using AssetManager.Library.Enums;

namespace AssetManager.Library.Extensions;

public static class AssetTypeExtensions
{
    public static string GetAssetTypeString(Asset asset)
    {
        return asset switch
        {
            General => "",
            Book => "Book",
            Cohort => "Cohort",
            _ => "Err!"
        };
    }

    public static AssetType GetAssetTypeEnum(Asset asset)
    {
        return asset switch
        {
            General => AssetType.Asset,
            Book => AssetType.Book,
            Cohort => AssetType.Cohort,
            _ => AssetType.Asset
        };
    }
}