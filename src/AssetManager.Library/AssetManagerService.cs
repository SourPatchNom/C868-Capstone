using System.ComponentModel;
using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using AssetManager.Library.Classes;
using AssetManager.Library.Classes.Asset;
using AssetManager.Library.Database;

namespace AssetManager.Library;

public class AssetManagerService
{
    private static readonly Lazy<AssetManagerService> LazySingleton = new Lazy<AssetManagerService>(() => new AssetManagerService());

    public static AssetManagerService Instance => LazySingleton.Value;
    public event PropertyChangedEventHandler? DatabaseInformationUpdated;
    public readonly DatabaseService DatabaseService = DatabaseService.Instance;
    
    public AssetManagerService()
    {
        
    }

    public async Task<bool> InitializeAsync(string mysqlString)
    {
        try
        {
            var result = await DatabaseService.Initialize(mysqlString);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    public string AddOrUpdateAsset(General getAsAsset, string user)
    {
        var result = DatabaseService.Instance.AddOrUpdateAsset(getAsAsset, user, true);
        return result > 0 ? "Save Successful" : "Save Failed! Contact an administrator!";
    }
    
    public string AddOrUpdateAsset(Book getAsAsset, string user)
    {
        var result = DatabaseService.Instance.AddOrUpdateAsset(getAsAsset, user, true);
        return result > 0 ? "Save Successful" : "Save Failed! Contact an administrator!";
    }
    
    public string AddOrUpdateAsset(Cohort getAsAsset, string user)
    {
        var result = DatabaseService.Instance.AddOrUpdateAsset(getAsAsset, user, true);
        return result > 0 ? "Save Successful" : "Save Failed! Contact an administrator!";
    }

    public string DeleteAsset(int assetId, string user)
    {
        var result = DatabaseService.Instance.RemoveAsset(assetId, user,true);
        return result > 0 ? "Delete Successful" : "Delete Failed! Contact an administrator!";
    }

    public string AddOrUpdateCourseAsset(CourseAsset courseAsset, string identityName)
    {
        var result = DatabaseService.Instance.AddOrUpdateCourseAsset(courseAsset, identityName, true);
        return result > 0 ? "Save Successful" : "Save Failed! Contact an administrator!";
    }

    public string RemoveCourseAsset(CourseAsset selectedCourseAsset, string identityName)
    {
        var result = DatabaseService.Instance.RemoveCourseAsset(selectedCourseAsset, identityName,true);
        return result > 0 ? "Delete Successful" : "Delete Failed! Contact an administrator!";
    }
}