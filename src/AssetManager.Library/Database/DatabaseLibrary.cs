using System.ComponentModel;
using System.Data;
using AssetManager.Library.Classes;
using AssetManager.Library.Classes.Asset;
using AssetManager.Library.Enums;
using Npgsql;

namespace AssetManager.Library.Database;

public sealed class DatabaseLibrary
{
    private static readonly Lazy<DatabaseLibrary> LazySingleton = new Lazy<DatabaseLibrary>(() => new DatabaseLibrary());

    public static DatabaseLibrary Instance => LazySingleton.Value;
    
    private DatabaseLibrary()
    {
        
    }

    public readonly List<Course> Courses = new ();
    public readonly List<CourseAsset> CourseAssets = new ();
    public readonly List<Asset> Assets = new ();
    public readonly List<AssetHistory> AssetEdits = new ();

    public int GetAssetCount()
    {
        return Assets.Count;
    }

    public int GetEditCount()
    {
        return AssetEdits.Count;
    }

    public int GetCourseCount()
    {
        return Courses.Count;
    }

    public void RefreshCourses(NpgsqlConnection connection)
    {
        Courses.Clear();
        var command = connection.CreateCommand();
        if(connection.State != ConnectionState.Open) connection.Open();
        command.CommandText = "SELECT * FROM course";
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            Courses.Add(new Course(reader.GetInt32(0),reader.GetString(1),reader.GetString(2)));
        }
        connection.Close();
    }

    internal void RefreshAssets(NpgsqlConnection connection)
    {
        Assets.Clear();
        if(connection.State != ConnectionState.Open) connection.Open();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM asset LEFT JOIN asset_book ab on asset.asset_id = ab.asset_id LEFT JOIN asset_cohort ac on asset.asset_id = ac.asset_id ORDER BY asset.asset_id";
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            if (!reader.IsDBNull(11)) // Book
            {
                Assets.Add(new Book(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetString(5),
                    reader.IsDBNull(6) ? "" : reader.GetString(6),
                    reader.GetDateTime(7),
                    reader.GetBoolean(8),
                    reader.GetBoolean(9),
                    reader.GetBoolean(10),
                    reader.GetString(13),
                    reader.GetString(15),
                    reader.GetDateTime(14)
                ));
                continue;
            }
            
            if (!reader.IsDBNull(16)) // Cohort
            {
                Assets.Add(new Cohort(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetString(5),
                    reader.IsDBNull(6) ? "" : reader.GetString(6),
                    reader.GetDateTime(7),
                    reader.GetBoolean(8),
                    reader.GetBoolean(9),
                    reader.GetBoolean(10),
                    reader.GetDateTime(18)
                ));
                continue;
            }
            Assets.Add(new General(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                 reader.IsDBNull(6) ? "" : reader.GetString(6),
                reader.GetDateTime(7),
                reader.GetBoolean(8),
                reader.GetBoolean(9),
                reader.GetBoolean(10)
                ));
        }
        connection.Close();
    }

    internal void RefreshAssetHistory(NpgsqlConnection connection)
    {
        AssetEdits.Clear();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM asset_history";
        if(connection.State != ConnectionState.Open) connection.Open();
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            AssetEdits.Add(new AssetHistory(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetDateTime(3)));
        }
        connection.Close();
    }
    
    internal void RefreshCourseAssets(NpgsqlConnection connection)
    {
        CourseAssets.Clear();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM course_asset";
        if(connection.State != ConnectionState.Open) connection.Open();
        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            CourseAssets.Add(new CourseAsset(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2)));
        }
        connection.Close();
    }
    
    public List<Asset> GetCourseAssets(int courseId)
    {
        var assets = Assets.Where(x => CourseAssets.Any(y => y.CourseId == courseId && y.AssetId == x.AssetId )).ToList();
        var orderList = new List<Tuple<Asset, int>>();
        assets.ForEach(x => orderList.Add(new Tuple<Asset, int>(x, CourseAssets.First(y => y.CourseId == courseId && y.AssetId == x.AssetId).Hierarchy)));
        assets.Clear();
        orderList.OrderBy(x => !x.Item1.IsOfficial).ThenBy(x => x.Item2).ToList().ForEach(x => assets.Add(x.Item1));
        return assets;
    }


}