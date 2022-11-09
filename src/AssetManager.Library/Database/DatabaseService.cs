using System.ComponentModel;
using System.Data;
using AssetManager.Library.Classes;
using AssetManager.Library.Classes.Asset;
using AssetManager.Library.Database.Migration;
using AssetManager.Library.Enums;
using Npgsql;

namespace AssetManager.Library.Database;

public sealed class DatabaseService
{
    private static readonly Lazy<DatabaseService> LazySingleton = new Lazy<DatabaseService>(() => new DatabaseService());

    public static DatabaseService Instance => LazySingleton.Value;
    public event PropertyChangedEventHandler? DatabaseInformationUpdated;
    public readonly DatabaseLibrary Library = DatabaseLibrary.Instance;
    private bool _statusLastConnection = false;
    
    private DatabaseService()
    {
            DatabaseInformationUpdated += OnDatabaseInformationUpdated;
    }

    private NpgsqlConnection _sqlConnection = new NpgsqlConnection();
    private NpgsqlCommand _sqlCommand = new NpgsqlCommand();
    private string _connectionString = "";
    
    public async Task<bool> Initialize(string mysqlString)
    {
        _connectionString = mysqlString;
        _sqlConnection.ConnectionString = _connectionString;
        var result = CheckOrOpenConnection();
        CloseConnectionIfOpen();
        CheckForEmptyDb();
        return result;
    }

    private void CheckForEmptyDb()
    {
        CheckOrOpenConnection();
        var command = _sqlConnection.CreateCommand();
        command.CommandText = "SELECT * FROM course;";
        var needsTables = false;
        try
        {
            var first = command.ExecuteScalar();
            needsTables = first == null;
        }
        catch (Exception e)
        {
            needsTables = true;
        }
        if (needsTables)
        {
             Console.WriteLine("Empty database detected, attempting to create tables! ");
             DatabaseTools.TestingFullResetDatabase(_sqlConnection);
             CloseConnectionIfOpen();
             return;
        }
        Console.WriteLine("MySql database has tables, attempting to populate data! ");
        PopulateData();
        CloseConnectionIfOpen();
    }



    private bool CheckOrOpenConnection()
    {
        try
        {
            //Is the connection open already?
            if (_sqlConnection.State == ConnectionState.Open)
            {
                _statusLastConnection = true;
                return true;
            }
        
            //Open a connection
            _sqlConnection.Open();
        
            //Make sure it worked!
            if (_sqlConnection.State == ConnectionState.Open)
            {
                _statusLastConnection = true;
                return true;
            }

            _statusLastConnection = false;
            //It didn't work somethings wrong.
            throw new Exception("Mysql Open Failed!");
        }
        catch (NpgsqlException e)
        {
            throw new Exception("Mysql Open Failed!",e);
        }
    }

    private void CloseConnectionIfOpen()
    {
        try
        {
            if (_sqlConnection.State == ConnectionState.Open)
            {
                _sqlConnection.Close();
            }
        }
        catch (NpgsqlException e)
        {
            throw new Exception("Mysql Close Failed!",e);
        }
    }

    private void OnDatabaseInformationUpdated(object? sender, PropertyChangedEventArgs e)
    {
        PopulateData();
    }

    private async void PopulateData()
    {
        DatabaseLibrary.Instance.RefreshCourses(_sqlConnection);
        DatabaseLibrary.Instance.RefreshAssets(_sqlConnection);
        DatabaseLibrary.Instance.RefreshCourseAssets(_sqlConnection);
        DatabaseLibrary.Instance.RefreshAssetHistory(_sqlConnection);
        CloseConnectionIfOpen();
    }
    
    public void TestingResetDatabase()
    {
        CheckOrOpenConnection();
        DatabaseTools.TestingFullResetDatabase(_sqlConnection);
        CloseConnectionIfOpen();
        DatabaseInformationUpdated?.Invoke(this,new PropertyChangedEventArgs("Database Reset"));
    }

    public void TestingAddEvalDataToDatabase()
    {
        TestingResetDatabase();
        CheckOrOpenConnection();
        DatabaseTools.TestingAddEvalDataToDatabase();
        CloseConnectionIfOpen();
        DatabaseInformationUpdated?.Invoke(this,new PropertyChangedEventArgs("Eval Data Added"));
    }
    
    public void TestingAddDevDataToDatabase()
    {
        TestingResetDatabase();
        CheckOrOpenConnection();
        DatabaseTools.TestingAddDevDataToDatabase();
        CloseConnectionIfOpen();
        DatabaseInformationUpdated?.Invoke(this,new PropertyChangedEventArgs("Dev Data Added"));
    }
    
    public void TestingAddBulkDataToDatabase()
    {
        TestingResetDatabase();
        CheckOrOpenConnection();
        DatabaseTools.TestingAddBulkDataToDatabase();
        CloseConnectionIfOpen();
        DatabaseInformationUpdated?.Invoke(this,new PropertyChangedEventArgs("Bulk Data Added"));
    }
    
    public bool GetStatus() => _statusLastConnection;

    /// <summary>
    /// Inserts or updates a course in the DB. Pass with a record id of -1 to insert, else update a record.
    /// </summary>
    /// <param name="course"></param>
    /// <returns></returns>
    internal int AddOrUpdateCourse(Course course, bool notifyEvent)
    {
        try
        {
            CheckOrOpenConnection();
            NpgsqlCommand? command;
            int result;
            if (course.CourseId == -1)
            {
                command = new NpgsqlCommand("INSERT INTO course (number, name) VALUES (($1),($2)) returning course_id", _sqlConnection)
                {
                    Parameters =
                    {
                        new() { Value = course.Number },
                        new() { Value = course.Name }
                    }
                };
                result = (int) (command.ExecuteScalar() ?? -1);
                if (notifyEvent) DatabaseInformationUpdated?.Invoke(this, new PropertyChangedEventArgs("Course Insert"));
                CloseConnectionIfOpen();
                return result;
            }

            command = new NpgsqlCommand("UPDATE course SET number = ($2), name = ($3) WHERE course_id = ($1)", _sqlConnection)
            {
                Parameters =
                {
                    new() { Value = course.CourseId },
                    new() { Value = course.Number },
                    new() { Value = course.Name }
                }
            };
            result = (int) (command.ExecuteScalar() ?? -1);
            if (notifyEvent) DatabaseInformationUpdated?.Invoke(this, new PropertyChangedEventArgs("Course Update"));
            CloseConnectionIfOpen();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1;
        }
    }
    
    internal int AddOrUpdateAsset(General general, string editor, bool notifyEvent)
    {
        try
        {
            CheckOrOpenConnection();
            NpgsqlCommand? command;
            int result;

            if (general.AssetId == -1)
            {
                command = new NpgsqlCommand("INSERT INTO asset (name,summary,author,url,thumb_url,date_created,is_active,is_official,is_internal) " +
                                            "VALUES(($1),($2),($3),($4),($5),($6),($7),($8),($9)) RETURNING asset_id", _sqlConnection)
                {
                    Parameters =
                    {
                        new() { Value = general.Name },
                        new() { Value = general.Summary },
                        new() { Value = general.Author },
                        new() { Value = general.Url },
                        new() { Value = general.ThumbnailUrl },
                        new() { Value = general.Created },
                        new() { Value = general.IsActive },
                        new() { Value = general.IsOfficial },
                        new() { Value = general.IsInternal }
                    }
                };
                result = (int) (command.ExecuteScalar() ?? -1);
                if (notifyEvent) DatabaseInformationUpdated?.Invoke(this, new PropertyChangedEventArgs("General Insert"));
                CloseConnectionIfOpen();
                AddOrUpdateAssetHistory(new AssetHistory(result,  editor, DateTime.UtcNow),notifyEvent);
                return result;
            }

            command = new NpgsqlCommand("UPDATE asset SET name = ($2)," +
                                        "summary = ($3),author = ($4),url = ($5),thumb_url = ($6),date_created = ($7)," +
                                        "is_active = ($8),is_official = ($9),is_internal = ($10) WHERE asset_id = ($1)", _sqlConnection)
            {
                Parameters =
                {
                    new() { Value = general.AssetId },
                    new() { Value = general.Name },
                    new() { Value = general.Summary },
                    new() { Value = general.Author },
                    new() { Value = general.Url },
                    new() { Value = general.ThumbnailUrl },
                    new() { Value = general.Created },
                    new() { Value = general.IsActive },
                    new() { Value = general.IsOfficial },
                    new() { Value = general.IsInternal }
                }
            };
            result = command.ExecuteNonQuery();
            if (notifyEvent) DatabaseInformationUpdated?.Invoke(this, new PropertyChangedEventArgs("General Update"));
            CloseConnectionIfOpen();
            AddOrUpdateAssetHistory(new AssetHistory(result,  editor, DateTime.UtcNow), notifyEvent);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1;
        }
    }
    
    internal int AddOrUpdateAsset(Book asset, string editor, bool notifyEvent)
    { 
        try
        {
            CheckOrOpenConnection();
            NpgsqlCommand? commandAsset;
            NpgsqlCommand? commandBook;
            int result;
            int resultBook;
            if (asset.AssetId == -1)
            {
                commandAsset = new NpgsqlCommand("INSERT INTO asset (name,summary,author,url,thumb_url,date_created,is_active,is_official,is_internal) " +
                                            "VALUES(($1),($2),($3),($4),($5),($6),($7),($8),($9)) RETURNING asset_id", _sqlConnection)
                {
                    Parameters =
                    {
                        new() { Value = asset.Name },
                        new() { Value = asset.Summary },
                        new() { Value = asset.Author },
                        new() { Value = asset.Url },
                        new() { Value = asset.ThumbnailUrl },
                        new() { Value = asset.Created },
                        new() { Value = asset.IsActive },
                        new() { Value = asset.IsOfficial },
                        new() { Value = asset.IsInternal }
                    }
                };
                result = (int) (commandAsset.ExecuteScalar() ?? -1);
                if (result == -1) return result;
                commandBook = new NpgsqlCommand("INSERT INTO asset_book (asset_id, publisher, publish_date, isbn) " +
                                                "VALUES(($1),($2),($3),($4)) RETURNING record_id", _sqlConnection)
                {
                    Parameters =
                    {
                        new() { Value = result },
                        new() { Value = asset.Publisher },
                        new() { Value = asset.PublishDate },
                        new() { Value = asset.Isbn }
                    }
                };
                resultBook = (int) (commandBook.ExecuteScalar() ?? -1);
                if (notifyEvent) DatabaseInformationUpdated?.Invoke(this, new PropertyChangedEventArgs("Book Insert"));
                CloseConnectionIfOpen();
                AddOrUpdateAssetHistory(new AssetHistory(result,  editor, DateTime.UtcNow), notifyEvent);
                return resultBook != -1 ? result: -1; //Returns fail if book insert fails
            }

            commandAsset = new NpgsqlCommand("UPDATE asset SET name = ($2)," +
                                        "summary = ($3),author = ($4),url = ($5),thumb_url = ($6),date_created = ($7)," +
                                        "is_active = ($8),is_official = ($9),is_internal = ($10) WHERE asset_id = ($1)", _sqlConnection)
            {
                Parameters =
                {
                    new() { Value = asset.AssetId },
                    new() { Value = asset.Name },
                    new() { Value = asset.Summary },
                    new() { Value = asset.Author },
                    new() { Value = asset.Url },
                    new() { Value = asset.ThumbnailUrl },
                    new() { Value = asset.Created },
                    new() { Value = asset.IsActive },
                    new() { Value = asset.IsOfficial },
                    new() { Value = asset.IsInternal }
                }
            };
            result = commandAsset.ExecuteNonQuery();
            if (result == -1) return result;
            commandBook = new NpgsqlCommand("UPDATE asset_book SET publisher = ($2), publish_date = ($3), isbn= ($4) " +
                                            "WHERE asset_id = ($1)", _sqlConnection)
            {
                Parameters =
                {
                    new() { Value = asset.AssetId },
                    new() { Value = asset.Publisher },
                    new() { Value = asset.PublishDate },
                    new() { Value = asset.Isbn }
                }
            };
            resultBook = commandBook.ExecuteNonQuery();
            if (notifyEvent) DatabaseInformationUpdated?.Invoke(this, new PropertyChangedEventArgs("Book Update"));
            CloseConnectionIfOpen();
            AddOrUpdateAssetHistory(new AssetHistory(result, editor, DateTime.UtcNow), notifyEvent);
            return resultBook != -1 ? result: -1; //Returns fail if book insert fails
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1;
        }
    }

    internal int AddOrUpdateAsset(Cohort asset, string editor, bool notifyEvent)
    {
        try
        {
            CheckOrOpenConnection();
            NpgsqlCommand? commandAsset;
            NpgsqlCommand? commandCohort;
            int result;
            int resultBook;
            if (asset.AssetId == -1)
            {
                commandAsset = new NpgsqlCommand("INSERT INTO asset (name,summary,author,url,thumb_url,date_created,is_active,is_official,is_internal) " +
                                            "VALUES(($1),($2),($3),($4),($5),($6),($7),($8),($9)) RETURNING asset_id", _sqlConnection)
                {
                    Parameters =
                    {
                        new() { Value = asset.Name },
                        new() { Value = asset.Summary },
                        new() { Value = asset.Author },
                        new() { Value = asset.Url },
                        new() { Value = asset.ThumbnailUrl },
                        new() { Value = asset.Created },
                        new() { Value = asset.IsActive },
                        new() { Value = asset.IsOfficial },
                        new() { Value = asset.IsInternal }
                    }
                };
                result = (int) (commandAsset.ExecuteScalar() ?? -1);
                if (result == -1) return result;
                commandCohort = new NpgsqlCommand("INSERT INTO asset_cohort (asset_id, recorded_date) " +
                                                "VALUES(($1),($2)) RETURNING record_id", _sqlConnection)
                {
                    Parameters =
                    {
                        new() { Value = result },
                        new() { Value = asset.Recorded }
                    }
                };
                resultBook = (int) (commandCohort.ExecuteScalar() ?? -1);
                if (notifyEvent) DatabaseInformationUpdated?.Invoke(this, new PropertyChangedEventArgs("Cohort Insert"));
                CloseConnectionIfOpen();
                AddOrUpdateAssetHistory(new AssetHistory(result, editor, DateTime.UtcNow), notifyEvent);
                return resultBook != -1 ? result: -1; //Returns fail if book insert fails
            }

            commandAsset = new NpgsqlCommand("UPDATE asset SET name = ($2)," +
                                        "summary = ($3),author = ($4),url = ($5),thumb_url = ($6),date_created = ($7)," +
                                        "is_active = ($8),is_official = ($9),is_internal = ($10) WHERE asset_id = ($1)", _sqlConnection)
            {
                Parameters =
                {
                    new() { Value = asset.AssetId },
                    new() { Value = asset.Name },
                    new() { Value = asset.Summary },
                    new() { Value = asset.Author },
                    new() { Value = asset.Url },
                    new() { Value = asset.ThumbnailUrl },
                    new() { Value = asset.Created },
                    new() { Value = asset.IsActive },
                    new() { Value = asset.IsOfficial },
                    new() { Value = asset.IsInternal }
                }
            };
            result = commandAsset.ExecuteNonQuery();
            if (result == -1) return result;
            commandCohort = new NpgsqlCommand("UPDATE asset_cohort SET recorded_date = ($2) " +
                                            "WHERE asset_id =($1) ", _sqlConnection)
            {
                Parameters =
                {
                    new() { Value = asset.AssetId },
                    new() { Value = asset.Recorded }
                }
            };
            resultBook = commandCohort.ExecuteNonQuery();
            if (notifyEvent) DatabaseInformationUpdated?.Invoke(this, new PropertyChangedEventArgs("Cohort Update"));
            CloseConnectionIfOpen();
            AddOrUpdateAssetHistory(new AssetHistory(result, editor, DateTime.UtcNow), notifyEvent);
            return resultBook != -1 ? result : -1; //Returns fail if book insert fails
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1;
        }
    }

    internal int RemoveAsset(int assetId, string user, bool notifyEvent)
    {
        try
        {
            if (assetId == -1) throw new ArgumentException("Asset ID out of range. Asset ID provided is -1.");
            CheckOrOpenConnection();
            var commandDeleteAsset = new NpgsqlCommand("DELETE FROM asset WHERE asset_id = ($1)", _sqlConnection)
            {
                Parameters =
                {
                    new() { Value = assetId }
                }
            };
            int result = commandDeleteAsset.ExecuteNonQuery();
            CheckOrOpenConnection();
            commandDeleteAsset = new NpgsqlCommand("DELETE FROM asset_book WHERE asset_id = ($1)", _sqlConnection)
            {
                Parameters =
                {
                    new() { Value = assetId }
                }
            };
            result += commandDeleteAsset.ExecuteNonQuery();
            CheckOrOpenConnection();
            commandDeleteAsset = new NpgsqlCommand("DELETE FROM asset_cohort WHERE asset_id = ($1)", _sqlConnection)
            {
                Parameters =
                {
                    new() { Value = assetId }
                }
            };
            result += commandDeleteAsset.ExecuteNonQuery();
            if (notifyEvent) DatabaseInformationUpdated?.Invoke(this, new PropertyChangedEventArgs("Cohort Update"));
            AddOrUpdateAssetHistory(new AssetHistory(assetId, user, DateTime.UtcNow), notifyEvent);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1;
        }
    }
    
    internal int AddOrUpdateAssetHistory(AssetHistory assetHistory, bool notifyEvent)
    {
        try
        {
            CheckOrOpenConnection();
            NpgsqlCommand? command;
            int result;
            if (assetHistory.RecordId == -1)
            {
                command = new NpgsqlCommand("INSERT INTO asset_history (asset_id, editor, edit_date) VALUES(($1),($2),($3)) returning record_id", _sqlConnection)
                {
                    Parameters =
                    {
                        new() { Value = assetHistory.AssetId },
                        new() { Value = assetHistory.Editor },
                        new() { Value = assetHistory.EditDate }
                    }
                };
                result = (int) (command.ExecuteScalar() ?? -1);
                if (notifyEvent) DatabaseInformationUpdated?.Invoke(this, new PropertyChangedEventArgs("General History Insert"));
                CloseConnectionIfOpen();
                return result;
            }
        
            command = new NpgsqlCommand("UPDATE asset_history SET asset_id = ($2), editor = ($3), edit_date = ($4) WHERE record_id = ($1)", _sqlConnection)
            {
                Parameters =
                {
                    new() { Value = assetHistory.RecordId },
                    new() { Value = assetHistory.AssetId },
                    new() { Value = assetHistory.Editor },
                    new() { Value = assetHistory.EditDate }
                }
            };
            result = command.ExecuteNonQuery();
            if (notifyEvent) DatabaseInformationUpdated?.Invoke(this, new PropertyChangedEventArgs("General History Update"));
            CloseConnectionIfOpen();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1;
        }
    }

    internal int AddOrUpdateCourseAsset(CourseAsset courseAsset, string identityName, bool notifyEvent)
    {
        try
        {
            CheckOrOpenConnection();
            NpgsqlCommand? command;
            int result = 0;
            command = new NpgsqlCommand("INSERT INTO course_asset (course_id, asset_id, display_hierarchy) VALUES(($1),($2),($3)) ON CONFLICT(course_id, asset_id) DO UPDATE SET display_hierarchy = ($3)", _sqlConnection)
            {
                Parameters =
                {
                    new() { Value = courseAsset.CourseId },
                    new() { Value = courseAsset.AssetId },
                    new() { Value = courseAsset.Hierarchy }
                }
            };
            result = command.ExecuteNonQuery();
            if (notifyEvent) DatabaseInformationUpdated?.Invoke(this, new PropertyChangedEventArgs("Update Course Asset"));
            CloseConnectionIfOpen();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1;
        }
    }
    
    public int RemoveCourseAsset(CourseAsset courseAsset, string identityName, bool notifyEvent)
    {
        try
        {
            CheckOrOpenConnection();
            NpgsqlCommand? command;
            int result = 0;
            command = new NpgsqlCommand("DELETE FROM course_asset WHERE course_id = ($1) AND asset_id = ($2)", _sqlConnection)
            {
                Parameters =
                {
                    new() { Value = courseAsset.CourseId },
                    new() { Value = courseAsset.AssetId }
                }
            };
            result = command.ExecuteNonQuery();
            if (notifyEvent) DatabaseInformationUpdated?.Invoke(this, new PropertyChangedEventArgs("Delete Course Assset"));
            CloseConnectionIfOpen();
            return  result > 0 ? result : -1;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return -1;
        }
    }
}