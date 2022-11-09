using AssetManager.Library.Classes;
using AssetManager.Library.Classes.Asset;
using Npgsql;

namespace AssetManager.Library.Database.Migration;

internal static class DatabaseTools
{
    public static bool TestingFullResetDatabase(NpgsqlConnection connection)
    {
        Console.WriteLine("Reset of MySql Database requested!");
        var npgsqlCommand = connection.CreateCommand();
        npgsqlCommand.CommandText = @"
DROP TABLE if EXISTS course CASCADE;

CREATE TABLE if NOT EXISTS course(
    course_id SERIAL NOT NULL UNIQUE,
    number VARCHAR(255),
    name VARCHAR(255)
); 

TRUNCATE TABLE course;

DROP TABLE if EXISTS asset CASCADE;
CREATE TABLE if NOT EXISTS asset(
    asset_id SERIAL PRIMARY KEY,
    name VARCHAR(255),
    summary TEXT,
    author VARCHAR(255),
    url VARCHAR(255),
    thumb_url VARCHAR(255),
    doi_num VARCHAR(255),
    date_created TIMESTAMP,
    is_active BOOL,
    is_official BOOL,
    is_internal BOOL
); TRUNCATE TABLE asset;

DROP TABLE if EXISTS course_asset;

CREATE TABLE if NOT EXISTS course_asset(
    course_id INT NOT NULL,
    asset_id INT NOT NULL,
    display_hierarchy INT,
    PRIMARY KEY (course_id,asset_id),
    CONSTRAINT fk_course FOREIGN KEY(course_id) REFERENCES course(course_id), 
    CONSTRAINT fk_asset FOREIGN KEY(asset_id) REFERENCES asset(asset_id)
);
CREATE UNIQUE INDEX course_asset_index on course_asset (course_id, asset_id);
TRUNCATE TABLE course_asset;

DROP TABLE if EXISTS asset_history;
CREATE TABLE if NOT EXISTS asset_history(
    record_id SERIAL PRIMARY KEY,
    asset_id INT,
    editor VARCHAR(255) NOT NULL,
    edit_date TIMESTAMP
); TRUNCATE TABLE asset_history;

DROP TABLE if EXISTS asset_book;
CREATE TABLE if NOT EXISTS asset_book(
    record_id SERIAL PRIMARY KEY,
    asset_id INT,
    publisher VARCHAR(255),
    publish_date TIMESTAMP,
    isbn VARCHAR(255)
); TRUNCATE TABLE asset_book;

DROP TABLE if EXISTS asset_cohort;
CREATE TABLE if NOT EXISTS asset_cohort(
    record_id SERIAL PRIMARY KEY,
    asset_id INT,
    recorded_date TIMESTAMP
); TRUNCATE TABLE asset_cohort;
";
        return npgsqlCommand.ExecuteNonQuery() != -1;
    }

    internal static async void TestingAddEvalDataToDatabase()
    {
         Console.WriteLine("Add bulk data to MySql Database requested!");
         string sampleData = " This is procedurally generated data for evaluation of features. This is NOT official information.";
        var courseList = new List<(string, string)>
        {
            ("C101", "Basics of Evaluation"),
            ("C202", "Advanced Evaluation"),
            ("C868", "Capstone Complete Goals")
        };

        
        
        for (int i = 0; i < courseList.Count; i++)
        {
            var generatedText = sampleData + " Associated generated course:" + i+1 +" Randomized numeral to verify unique data in views:";
            var courseId = DatabaseService.Instance.AddOrUpdateCourse(new Course(courseList[i].Item1,courseList[i].Item2),false);
            var assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " Instructors Guide", "The official course guide!" + generatedText + GetRandomizedNumeral(i) + ".", "CI Group", "https://www.wgu.edu/", "/images/asset_icon.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), true, true, true),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 1),"Admin #" + Random.Shared.Next(0,100),false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new Cohort("Dr. Doe "+courseList[i].Item1 + " Cohort", "A previously recorded cohort all about this class!" + generatedText + GetRandomizedNumeral(i) + ".", "Dr Sample", "https://www.wgu.edu/", "/images/asset_icon_cohort.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), true, true, true, DateTime.UtcNow.AddYears(-2)),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 2),"Admin #" + Random.Shared.Next(0,100),false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " " + Random.Shared.Next(0,19).ToString() + " Week Study Plan" , "A study plan to pass the class." + generatedText + GetRandomizedNumeral(i) + ".", "CI Group", "https://www.wgu.edu/", "/images/asset_icon.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), true, true, true),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 3),"Admin #" + Random.Shared.Next(0,100),false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new Book("A Textbook About " + courseList[i].Item1, "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum." + generatedText + GetRandomizedNumeral(i) + ".", "Dr John Doe & Dr Jane Doe", "https://www.wgu.edu/", "/images/asset_icon_book.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), true, false, true, Random.Shared.Next(1000000000,int.MaxValue).ToString(),"Big Money Press", DateTime.Now.AddDays(Random.Shared.Next(100,2000))),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 4),"Admin #" + Random.Shared.Next(0,100),false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " Video Resource 1", "A big playlist of computer science related crash course episodes!" + generatedText + GetRandomizedNumeral(i) + ".", "Admin #" + Random.Shared.Next(0,100), "https://www.wgu.edu/", "/images/asset_icon.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), true, false, false),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 5),"Admin #" + Random.Shared.Next(0,100),false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " Video Resource 2", "A big playlist of computer science related crash course episodes!" + generatedText + GetRandomizedNumeral(i) + ".", "Admin #" + Random.Shared.Next(0,100), "https://www.wgu.edu/", "/images/asset_icon.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), true, false, false),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 6),"Admin #" + Random.Shared.Next(0,100),false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " Old Course Materials", "Old Description!" + generatedText + GetRandomizedNumeral(i) + ".", "Admin #" + Random.Shared.Next(0,100), "https://www.wgu.edu/", "/images/asset_icon.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), false, false, false),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 7),"Admin #" + Random.Shared.Next(0,100),false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new Book("An Old Textbook About " + courseList[i].Item1, "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum." + generatedText + GetRandomizedNumeral(i) + ".", "Dr John Doe & Dr Jane Doe", "https://www.wgu.edu/", "/images/asset_icon_book.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), true, false, true, Random.Shared.Next(1000000000,int.MaxValue).ToString(),"Big Money Press", DateTime.Now.AddDays(Random.Shared.Next(100,2000))),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 8),"Admin #" + Random.Shared.Next(0,100),false);
        }
    }
    
    internal static async void TestingAddDevDataToDatabase()
    {
         Console.WriteLine("Add bulk data to MySql Database requested!");
         string sampleData = " This is procedurally generated data for testing, development, and evaluation. This is NOT official information.";
        var courseList = new List<(string, string)>
        {
            ("C182", "Introduction to IT"),
            ("C173", "Scripting and Programming - Foundations"),
            ("C464", "Introduction to Communication"),
            ("C779", "Web Development Foundations"),
            ("C455", "English Composition 1"),
            ("C172", "Network and Security - Foundations"),
            ("C955", "Applied Probability and Statistics"),
            ("C777", "Web Development Foundations"),
            ("C168", "Critical Thinking and Logic"),
            ("C175", "Data Management - Foundations"),
            ("C170", "Data Management - Applications"),
            ("C867", "Scripting and Programming - Applications"),
            ("C165", "Integrated Physical Sciences"),
            ("C176", "Business of IT - Project Management"),
            ("C957", "Applied Algebra"),
            ("C100", "Introduction to Humanities"),
            ("C949", "Data Structures and Algorithms"),
            ("C846", "Business of IT - Applications"),
            ("C255", "Introduction to Geography"),
            ("C768", "Technical Communications"),
            ("C963", "American Politics and The Constitution"),
            ("C393", "IT Foundations"),
            ("C188", "Software Engineering"),
            ("C961", "Ethics in Technology"),
            ("C484", "Organizational Behavior and Leadership"),
            ("C394", "IT Applications"),
            ("C968", "Software I - C#"),
            ("C773", "User Interface Design"),
            ("C857", "Software Quality Assurance"),
            ("C969", "Software II - Advanced C#"),
            ("C856", "User Experience Design"),
            ("C971", "Mobile Application Development Using C#"),
            ("C191", "Advanced Data Management"),
            ("C868", "Software Development Capstone")
        };

        
        
        for (int i = 0; i < courseList.Count; i++)
        {
            var generatedText = sampleData + " Associated generated course:" + i+1 +" Randomized numeral to verify unique data in views:";
            var courseId = DatabaseService.Instance.AddOrUpdateCourse(new Course(courseList[i].Item1,courseList[i].Item2),false);
            var assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " Instructors Guide", "The official course guide!" + generatedText + GetRandomizedNumeral(i) + ".", "CI Group", "https://www.wgu.edu/", "/images/asset_icon.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), true, true, true),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 1),"Admin #" + Random.Shared.Next(0,100),false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new Cohort("Dr. Doe "+courseList[i].Item1 + " Cohort", "A previously recorded cohort all about this class!" + generatedText + GetRandomizedNumeral(i) + ".", "Dr Sample", "https://www.wgu.edu/", "/images/asset_icon_cohort.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), true, true, true, DateTime.UtcNow.AddYears(-2)),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 2),"Admin #" + Random.Shared.Next(0,100),false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " " + Random.Shared.Next(0,19).ToString() + " Week Study Plan" , "A study plan to pass the class." + generatedText + GetRandomizedNumeral(i) + ".", "CI Group", "https://www.wgu.edu/", "/images/asset_icon.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), true, true, true),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 3),"Admin #" + Random.Shared.Next(0,100),false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new Book("A Textbook About " + courseList[i].Item1, "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum." + generatedText + GetRandomizedNumeral(i) + ".", "Dr John Doe & Dr Jane Doe", "https://www.wgu.edu/", "/images/asset_icon_book.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), true, false, true, Random.Shared.Next(1000000000,int.MaxValue).ToString(),"Big Money Press", DateTime.Now.AddDays(Random.Shared.Next(100,2000))),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 4),"Admin #" + Random.Shared.Next(0,100),false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " Video Resource 1", "A big playlist of computer science related crash course episodes!" + generatedText + GetRandomizedNumeral(i) + ".", "Admin #" + Random.Shared.Next(0,100), "https://www.wgu.edu/", "/images/asset_icon.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), true, false, false),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 5),"Admin #" + Random.Shared.Next(0,100),false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " Video Resource 2", "A big playlist of computer science related crash course episodes!" + generatedText + GetRandomizedNumeral(i) + ".", "Admin #" + Random.Shared.Next(0,100), "https://www.wgu.edu/", "/images/asset_icon.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), true, false, false),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 6),"Admin #" + Random.Shared.Next(0,100),false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " Old Course Materials", "Old Description!" + generatedText + GetRandomizedNumeral(i) + ".", "Admin #" + Random.Shared.Next(0,100), "https://www.wgu.edu/", "/images/asset_icon.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), false, false, false),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 7),"Admin #" + Random.Shared.Next(0,100),false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new Book("An Old Textbook About " + courseList[i].Item1, "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum." + generatedText + GetRandomizedNumeral(i) + ".", "Dr John Doe & Dr Jane Doe", "https://www.wgu.edu/", "/images/asset_icon_book.png", "Admin #" + Random.Shared.Next(0,100), DateTime.UtcNow.AddYears(-2), true, false, true, Random.Shared.Next(1000000000,int.MaxValue).ToString(),"Big Money Press", DateTime.Now.AddDays(Random.Shared.Next(100,2000))),"Admin #" + Random.Shared.Next(0,100),false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 8),"Admin #" + Random.Shared.Next(0,100),false);
        }
    }

    internal static async void TestingAddBulkDataToDatabase()
    {
         Console.WriteLine("Add bulk data to MySql Database requested!");
         string sampleData = " This is procedurally generated data for testing, development, and evaluation. This is NOT official information.";
         var courseList = new List<(string, string)>();

        for (int i = 0; i < 1000; i++)
        {
            courseList.Add(("Course " + i,"Course Name " + i));
        }

        for (int i = 0; i < courseList.Count; i++)
        {
            var generatedText = sampleData + " Associated generated course:" + i+1 +" Randomized numeral to verify unique data in views:";
            var courseId = DatabaseService.Instance.AddOrUpdateCourse(new Course(courseList[i].Item1,courseList[i].Item2),false);
            var assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " Instructors Guide", "The official course guide!" + generatedText + GetRandomizedNumeral(i) + ".", "CI Group", "https://www.wgu.edu/", "/images/asset_icon.png", "Admin", DateTime.UtcNow.AddYears(-2), true, true, true),"Admin",false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 1),"Admin",false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new Cohort("Dr. Doe "+courseList[i].Item1 + " Cohort", "A previously recorded cohort all about this class!" + generatedText + GetRandomizedNumeral(i) + ".", "Dr Sample", "https://www.wgu.edu/", "/images/asset_icon_cohort.png", "Admin", DateTime.UtcNow.AddYears(-2), true, true, true, DateTime.UtcNow.AddYears(-2)),"Admin",false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 2),"Admin",false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " " + Random.Shared.Next(0,19).ToString() + " Week Study Plan" , "A study plan to pass the class." + generatedText + GetRandomizedNumeral(i) + ".", "CI Group", "https://www.wgu.edu/", "/images/asset_icon.png", "Admin", DateTime.UtcNow.AddYears(-2), true, true, true),"Admin",false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 3),"Admin",false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new Book("A Textbook About " + courseList[i].Item1, "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum." + generatedText + GetRandomizedNumeral(i) + ".", "Dr John Doe & Dr Jane Doe", "https://www.wgu.edu/", "/images/asset_icon_book.png", "Admin", DateTime.UtcNow.AddYears(-2), true, false, true, Random.Shared.Next(1000000000,int.MaxValue).ToString(),"Big Money Press", DateTime.Now.AddDays(Random.Shared.Next(100,2000))),"Admin",false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 4),"Admin",false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " Video Resource 1", "A big playlist of computer science related crash course episodes!" + generatedText + GetRandomizedNumeral(i) + ".", "Admin", "https://www.wgu.edu/", "/images/asset_icon.png", "Admin", DateTime.UtcNow.AddYears(-2), true, false, false),"Admin",false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 5),"Admin",false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " Video Resource 2", "A big playlist of computer science related crash course episodes!" + generatedText + GetRandomizedNumeral(i) + ".", "Admin", "https://www.wgu.edu/", "/images/asset_icon.png", "Admin", DateTime.UtcNow.AddYears(-2), true, false, false),"Admin",false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 6),"Admin",false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new General(courseList[i].Item1 + " Old Course Materials", "Old Description!" + generatedText + GetRandomizedNumeral(i) + ".", "Admin", "https://www.wgu.edu/", "/images/asset_icon.png", "Admin", DateTime.UtcNow.AddYears(-2), false, false, false),"Admin",false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 7),"Admin",false);
            assetId = DatabaseService.Instance.AddOrUpdateAsset(new Book("An Old Textbook About " + courseList[i].Item1, "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum." + generatedText + GetRandomizedNumeral(i) + ".", "Dr John Doe & Dr Jane Doe", "https://www.wgu.edu/", "/images/asset_icon_book.png", "Admin", DateTime.UtcNow.AddYears(-2), true, false, true, Random.Shared.Next(1000000000,int.MaxValue).ToString(),"Big Money Press", DateTime.Now.AddDays(Random.Shared.Next(100,2000))),"Admin",false);
            DatabaseService.Instance.AddOrUpdateCourseAsset(new CourseAsset(courseId, assetId, 8),"Admin",false);
            
        }
    }
    
    private static int GetRandomizedNumeral(int input)
    {
        return ((100 - input) * Random.Shared.Next(100)) * Random.Shared.Next(100);
    }
}