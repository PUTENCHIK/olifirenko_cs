public enum GradeType
{
    bad = 2,            // неуд
    passably = 3,       // уд
    good = 4,           // хор
    great = 5           // отл
}

public struct Student
{
    public string Name;
    public Dictionary<string, List<GradeType>> Grades;

    public Student(string name)
    {
        Name = name;
        Grades = new Dictionary<string, List<GradeType>>();
    }

    public void AddGrade(string course, GradeType grade)
    {
        if (!Grades.ContainsKey(course))
            Grades[course] = new List<GradeType>();

        Grades[course].Add(grade);
    }

    public float GetAverageGrade()
    {
        var addGrades = Grades.Values.SelectMany(g => g).Select(g => (int)g).ToList();
        return addGrades.Count > 0 ? (float)addGrades.Average() : 0;
    }
}

public class GradeSystem
{
    private List<Student> students = new List<Student>();
    public event Action<string, string, GradeType> GradeAdded;

    public void AddStudent(string name)
    {
        students.Add(new Student(name));
        Console.WriteLine($"Student [{students.Count-1}] {name} has been added");
    }

    public Student GetStudent(string studentName)
    {
        return students.FirstOrDefault(s => s.Name == studentName);
    }

    public void AddGrade(string studentName, string course, GradeType grade)
    {
        var student = GetStudent(studentName);
        if (student.Name == null)
        {
            Console.WriteLine($"Student {studentName} doesn't exist");
            return;
        }

        student.AddGrade(course, grade);
        GradeAdded?.Invoke(studentName, course, grade);
    }

    public void ShowStudentGrades(string studentName)
    {
        var student = GetStudent(studentName);
        if (student.Name == null)
        {
            Console.WriteLine($"Student {studentName} doesn't exist");
            return;
        }

        Console.WriteLine($"{student.Name}'s grades:");
        foreach (var course in student.Grades)
            Console.WriteLine($"\t{course.Key}: " + String.Join(", ", course.Value));
    }

    public void ShowTopStudents()
    {
        const int top = 3;
        var topStudents = students.OrderByDescending(s => s.GetAverageGrade()).Take(top);

        Console.WriteLine($"Top {top} students:");
        int counter = 1;
        foreach (var student in topStudents)
        {
            Console.WriteLine($"{counter}) {student.Name}: {student.GetAverageGrade()}");
            counter++;
        }
    }
}

public class NotificationSystem
{
    public void onGradeAdded(string studentName, string course, GradeType grade)
    {
        Console.WriteLine($"[EVENT] {studentName} got grade on {course}: {grade}");
    }
}

class Program
{
    static GradeSystem gs;
    static NotificationSystem ns;

    static void PrintMenu()
    {
        Console.WriteLine("Menu:");
        Console.WriteLine("1.  Add student");
        Console.WriteLine("2.  Add grade to student");
        Console.WriteLine("3.  Show student's grades");
        Console.WriteLine("4.  Show students' top");
        Console.WriteLine("99. Exit");
    }

    static void AddStudent()
    {
        Console.Write("Enter student's name: ");
        string name = Console.ReadLine();
        if (name.Length == 0)
        {
            Console.WriteLine("Name must ne longer than 0");
            return;
        }
        gs.AddStudent(name);
    }

    static void AddGrade()
    {
        Console.Write("Enter student's name: ");
        string name = Console.ReadLine();
        if (name.Length == 0)
        {
            Console.WriteLine("Name must ne longer than 0");
            return;
        }

        Console.Write("Enter course: ");
        string course = Console.ReadLine();
        if (course.Length == 0)
        {
            Console.WriteLine("Course must ne longer than 0");
            return;
        }

        int grade;
        Console.Write("Enter grade [2-5]: ");
        try
        {
            grade = int.Parse(Console.ReadLine());
        }
        catch (FormatException)
        {
            Console.WriteLine("Grade must be integer between 2 and 5");
            return;
        }

        GradeType grade_t;
        switch (grade)
        {
            case 2:
                grade_t = GradeType.bad;
                break;
            case 3:
                grade_t = GradeType.passably;
                break;
            case 4:
                grade_t = GradeType.good;
                break;
            case 5:
                grade_t = GradeType.great;
                break;
            default:
                Console.WriteLine("Grade must be integer between 2 and 5");
                return;
        }

        gs.AddGrade(name, course, grade_t);
    }

    static void Main()
    {
        gs = new GradeSystem();
        ns = new NotificationSystem();
        gs.GradeAdded += ns.onGradeAdded;

        // gs.AddStudent("Danny");
        // gs.AddStudent("Maxy");
        // gs.AddStudent("Glebby");
        // gs.AddGrade("Danny", "IOT", GradeType.bad);
        // gs.AddGrade("Danny", "IOT", GradeType.bad);
        // gs.ShowStudentGrades("Danny");
        // gs.ShowStudentGrades("Negr");

        PrintMenu();
        while (true)
        {
            Console.Write(">> ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddStudent();
                    break;
                case "2":
                    AddGrade();
                    break;
                case "3":
                    break;
                case "4":
                    break;
                case "99":
                case "q":
                case "exit":
                case "quit":
                    Console.WriteLine("Program is over");
                    return;
                default:
                    Console.WriteLine($"Unknown command: {choice}");
                    break;
            }
        }
    }
}