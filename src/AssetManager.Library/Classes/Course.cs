namespace AssetManager.Library.Classes;

public class Course
{
    public int CourseId { get; private set; }
    public string Number { get; private set; }
    public string Name { get; private set; }

    public Course(string number, string name)
    {
        CourseId = -1;
        Number = number;
        Name = name;
    }

    public Course(int courseId, string number, string name)
    {
        CourseId = courseId;
        Number = number;
        Name = name;
    }
}