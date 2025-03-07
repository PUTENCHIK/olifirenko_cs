using System;
using System.Diagnostics;
using System.IO;

public enum GradeType
{
    bad = 2,            // неуд
    passably = 3,       // уд
    good = 4,           // хор
    great = 5           // отл
}

interface IGrading
{
    event Action<Student, string, GradeType> GradeAdded;
    event Action<string> StudentDeleted;

    Student AddStudent(string name);
    Student GetStudent(string name);
    void DeleteStudent(string name);
    void AddGrade(Student student, string course, GradeType grade);
    void ShowStudentGrades(string studentName);
    void ShowTopStudents();
    void FindStudentsByGrade(GradeType grade);
    void ChangeStudentName(Student student, string newName);
    void DeleteCourse(string course);
    void ShowBestAndWorstStudents();
    int AllGradesCount();
    float AverageGrade();
    void ShowGradesCount();
    void ShowMostPopularRareGrade();
}

interface IFileSystem
{
    void SaveGradesToFile(string filePath = "grades_data.txt");
    void LoadGradesFromFile(string filePath = "grades_data.txt");
    void SaveStudentsToFile(string filePath = "students_data.txt");
    void LoadStudentsFromFile(string filePath = "students_data.txt");
    void SaveReportToFile(string filePath = "report.txt");
    void AppendStudentToFile(string name, string filePath = "students_data.txt");
    void AppendGradeToFile(string studentName, string course, string grade, string filePath = "grades_data.txt");
    void ClearDataFile(string filePath);
    void BackupDataFile(string filePath);
    void FindStudentInFile(string studentName, string filePathStud = "students_data.txt", string filePathGrad = "grades_data.txt");
}

public class Student
{
    public string Name { get; private set; }
    public Dictionary<string, List<GradeType>> Grades { get; private set; }

    public Student(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Student's name must be longer than 0");
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

    public void ChangeName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Student's name must be longer than 0");
        Name = newName;
    }
}

public class GradeSystem : IGrading, IFileSystem
{
    private Dictionary<string, Student> students = new Dictionary<string, Student>();
    public event Action<Student, string, GradeType> GradeAdded;
    public event Action<string> StudentDeleted;

    public Student AddStudent(string name)
    {
        if (students.ContainsKey(name))
        {
            Console.WriteLine($"[ERROR] Student {name} is already exist");
            return null;
        }
        students[name] = new Student(name);
        Console.WriteLine($"Student {name} has been added");
        return students[name];
    }

    public Student GetStudent(string studentName)
    {
        if (!students.ContainsKey(studentName))
            return null;
        return students[studentName];
    }

    public void DeleteStudent(string name)
    {
        if (!students.ContainsKey(name))
        {
            Console.WriteLine($"Student {name} doesn't exist");
            return;
        }
        students.Remove(name);
        StudentDeleted?.Invoke(name);
    }

    public void AddGrade(Student student, string course, GradeType grade)
    {
        student.AddGrade(course, grade);
        GradeAdded?.Invoke(student, course, grade);
    }

    public void ShowStudentGrades(string studentName)
    {
        if (!students.ContainsKey(studentName))
        {
            Console.WriteLine($"Student {studentName} doesn't exist");
            return;
        }

        var student = students[studentName];
        Console.WriteLine($"{student.Name}'s grades:");
        foreach (var course in student.Grades)
            Console.WriteLine($"\t{course.Key}: " + String.Join(", ", course.Value));
    }

    public void ShowTopStudents()
    {
        const int top = 3;
        var topStudents = students.Values.OrderByDescending(s => s.GetAverageGrade()).Take(top);

        Console.WriteLine($"Top {top} students:");
        int counter = 1;
        foreach (var student in topStudents)
        {
            Console.WriteLine($"{counter}) {student.Name}: {student.GetAverageGrade()}");
            counter++;
        }

        if (counter == 1)
        {
            Console.WriteLine("\tno students");
        }
    }

    public void FindStudentsByGrade(GradeType grade)
    {
        foreach (var student in students)
        {
            int count = 0;
            foreach (var course in student.Value.Grades)
                count += course.Value.Where(g => g == grade).Count();
            if (count != 0)
                Console.WriteLine($"{student.Key} has {count} '{grade}'");
        }
    }

    public void ChangeStudentName(Student student, string newName)
    {
        if (!students.ContainsKey(newName))
        {
            students.Remove(student.Name);
            students[newName] = student;
            Console.WriteLine($"Student {student.Name} has changed name to {newName}");
            student.ChangeName(newName);
        }
        else Console.WriteLine($"Student {newName} already exists");
    }

    public void DeleteCourse(string course)
    {
        bool flag = true;
        foreach (var student in students)
        {
            if (student.Value.Grades.ContainsKey(course))
            {
                flag = false;
                int gradesCount = student.Value.Grades[course].Count();
                student.Value.Grades.Remove(course);
                Console.WriteLine($"Course '{course}' with {gradesCount} grade has been deleted from {student.Value.Name}");
            }
        }
        if (flag)
            Console.WriteLine($"Course {course} doesn't exist");
    }

    public void ShowBestAndWorstStudents()
    {
        var sortedStudents = students.Values.OrderByDescending(s => s.GetAverageGrade());
        if (sortedStudents.Count() == 0)
        {
            Console.WriteLine("No students in system");
            return;
        }
        var bestStudent = sortedStudents.First();
        var worstStudent = sortedStudents.Last();
        if (bestStudent.Name == worstStudent.Name)
            Console.WriteLine($"Only 1 student exists: {bestStudent.Name} - {bestStudent.GetAverageGrade()}");
        else
        {
            Console.WriteLine($"Best student:  {bestStudent.Name} - {bestStudent.GetAverageGrade()}");
            Console.WriteLine($"Worst student: {worstStudent.Name} - {worstStudent.GetAverageGrade()}");
        }
    }

    public int AllGradesCount()
    {
        int count = 0;
        foreach (var student in students)
            foreach (var course in student.Value.Grades)
                count += course.Value.Count();
        return count;
    }

    public float AverageGrade()
    {
        return students.Values.Select(s => s.GetAverageGrade()).Average();
    }

    public void ShowGradesCount()
    {
        Console.WriteLine($"Grades count:");
        Dictionary<GradeType, int> counts = new Dictionary<GradeType, int>();
        foreach (var student in students)
            foreach (var course in student.Value.Grades)
                foreach (var grade in course.Value)
                    counts[grade] = counts.ContainsKey(grade) ? counts[grade]+1 : 1;
        
        foreach (var grade in counts)
            Console.WriteLine($"\t'{grade.Key}': {grade.Value}");
    }

    public void ShowMostPopularRareGrade()
    {

    }

    public void SaveGradesToFile(string filePath = "grades_data.txt")
    {
        int counter = 0;
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (Student student in students.Values)
            {
                foreach (var course in student.Grades)
                {
                    foreach (GradeType grade in course.Value)
                    {
                        writer.WriteLine($"{student.Name}, {course.Key}, {grade}");
                        counter++;
                    }
                }
            }
        }
        Console.WriteLine($"[SUCCESS] Saved {counter} rows into {filePath}");
    }

    public void LoadGradesFromFile(string filePath = "grades_data.txt")
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"[ERROR] File {filePath} doesn't exist");
            return;
        }
        
        int gradesCounter = 0, studentsCounter = 0;
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 3 && Enum.TryParse(parts[2], out GradeType grade))
                {
                    gradesCounter++;
                    string studentName = parts[0].Trim();
                    Student student;
                    if ((student = GetStudent(studentName)) == null)
                    {
                        student = AddStudent(studentName);
                        studentsCounter++;
                    }
                    AddGrade(student, parts[1].Trim(), grade);
                }
                else
                    Console.WriteLine($"[WARNING] Impossible to parse line: {line}");
            }
        }
        Console.WriteLine($"[SUCCESS] Loaded {gradesCounter} grades, added {studentsCounter} students");
    }

    public void SaveStudentsToFile(string filePath = "students_data.txt")
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (Student student in students.Values)
                writer.WriteLine($"{student.Name}");
        }
    }
    
    public void LoadStudentsFromFile(string filePath = "students_data.txt")
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"[ERROR] File {filePath} doesn't exist");
            return;
        }
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine().Trim()) != null)
                AddStudent(line);
        }
    }

    public void SaveReportToFile(string filePath = "report.txt")
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Grades report:");
            bool studentsExist = false;
            foreach (Student student in students.Values)
            {
                studentsExist = true;
                writer.WriteLine($"\n\tStudent {student.Name}:");
                bool hasCourses = false;
                foreach (var course in student.Grades)
                {
                    hasCourses = true;
                    writer.WriteLine($"\t\t{course.Key}: {string.Join(", ", course.Value)}");
                }
                if (!hasCourses)
                    writer.WriteLine("\t\tNo courses");
                writer.WriteLine($"\t\tAvg grade: {student.GetAverageGrade()}");
            }
            if (!studentsExist)
                writer.WriteLine("\tNo students");
        }
        Console.WriteLine($"[SUCCESS] Grades report saves into {filePath}");
    }

    public void AppendStudentToFile(string name, string filePath = "students_data.txt")
    {

    }

    public void AppendGradeToFile(string studentName, string course, string grade, string filePath = "grades_data.txt")
    {

    }

    public void ClearDataFile(string filePath)
    {

    }

    public void BackupDataFile(string filePath)
    {

    }

    public void FindStudentInFile(string studentName, string filePathStud = "students_data.txt", string filePathGrad = "grades_data.txt")
    {

    }
}

public class NotificationSystem
{
    public void onGradeAdded(Student student, string course, GradeType grade)
    {
        Console.WriteLine($"[EVENT] {student.Name} got grade on {course}: {grade}");
    }

    public void onStudentDeleted(string name)
    {
        Console.WriteLine($"[EVENT] Student {name} has been deleted from system");
    }
}

class Program
{
    static GradeSystem gs;
    static NotificationSystem ns;

    static void PrintMenu()
    {
        Console.WriteLine("Work with files:");
        Console.WriteLine("[sg] Save grades to file");
        Console.WriteLine("[lg] Load grades from file");
        Console.WriteLine("[ss] Save students to file");
        Console.WriteLine("[ls] Load students from file");
        Console.WriteLine("[sr] Save report to file");
        Console.WriteLine("[as] Append student to file");
        Console.WriteLine("[ag] Append grade to file");
        Console.WriteLine("[cd] Clear data file");
        Console.WriteLine("[bd] Backup data file");
        Console.WriteLine("[fs] Find studentIn file");
        Console.WriteLine("\nMenu:");
        Console.WriteLine("[0]  Add prepared data");
        Console.WriteLine("[1]  Add student");
        Console.WriteLine("[2]  Delete student");
        Console.WriteLine("[3]  Add grade to student");
        Console.WriteLine("[4]  Show student's grades");
        Console.WriteLine("[5]  Show top students");
        Console.WriteLine("[6]  Find students by grade");
        Console.WriteLine("[7]  Change student's name");
        Console.WriteLine("[8]  Delete course");
        Console.WriteLine("[9]  Show the best and the worst students");
        Console.WriteLine("[10] Show full grades statistic");
        Console.WriteLine("\n[99] Exit + save report");
    }

    static string GetFilePath(string label)
    {
        Console.Write($"Enter file name {label} (empty to save into default file): ");
        return Console.ReadLine();
    }
    
    static void SaveGradesToFile()
    {
        string filePath = GetFilePath("to save grades");
        if (filePath.Length == 0)
            gs.SaveGradesToFile();
        else
            gs.SaveGradesToFile(filePath);
    }

    static void LoadGradesFromFile()
    {
        string filePath = GetFilePath("to load grades");
        if (filePath.Length == 0)
            gs.LoadGradesFromFile();
        else
            gs.LoadGradesFromFile(filePath);
    }

    static void SaveStudentsToFile()
    {

    }

    static void LoadStudentsFromFile()
    {

    }

    static void SaveReportToFile()
    {

    }


    static void AddPreparedData()
    {
        var stDanny = gs.AddStudent("Danny");
        var stMxy = gs.AddStudent("Maxy");
        var stGlebby = gs.AddStudent("Glebby");
        gs.AddGrade(stDanny, "IOT", GradeType.bad);
        gs.AddGrade(stDanny, "IOT", GradeType.bad);
        gs.AddGrade(stDanny, "IOT", GradeType.passably);
        gs.AddGrade(stMxy, "IOT", GradeType.bad);
        gs.AddGrade(stMxy, "IOT", GradeType.passably);
        gs.AddGrade(stMxy, "IOT", GradeType.great);
        gs.AddGrade(stGlebby, "IOT", GradeType.great);
        gs.AddGrade(stGlebby, "IOT", GradeType.great);
        gs.AddGrade(stGlebby, "IOT", GradeType.good);
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

    static void DeleteStudent()
    {
        Console.Write("Enter student's name: ");
        string name = Console.ReadLine();
        if (name.Length == 0)
        {
            Console.WriteLine("Name must ne longer than 0");
            return;
        }
        gs.DeleteStudent(name);
    }

    static GradeType? GetGrade()
    {
        int grade;
        Console.Write("Enter grade [2-5]: ");
        try
        {
            grade = int.Parse(Console.ReadLine());
        }
        catch (FormatException)
        {
            Console.WriteLine("Grade must be integer between 2 and 5");
            return null;
        }
        if (grade < 2 || grade > 5)
        {
            Console.WriteLine("Grade must be integer between 2 and 5");
            return null;
        }
        return (GradeType)grade;
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
        var student = gs.GetStudent(name);
        if (student == null)
        {
            Console.WriteLine($"Student {name} doesn't exist");
            return;
        }

        Console.Write("Enter course: ");
        string course = Console.ReadLine();
        if (course.Length == 0)
        {
            Console.WriteLine("Course must ne longer than 0");
            return;
        }

        GradeType? grade = GetGrade();
        if (grade == null)
            return;

        gs.AddGrade(student, course, grade ?? GradeType.passably);
    }

    static void ShowStudentGrades()
    {
        Console.Write("Enter student's name: ");
        string name = Console.ReadLine();
        if (name.Length == 0)
        {
            Console.WriteLine("Name must ne longer than 0");
            return;
        }
        gs.ShowStudentGrades(name);
    }

    static void ShowTopStudents()
    {
        gs.ShowTopStudents();
    }

    static void FindStudentsByGrade()
    {
        GradeType? grade = GetGrade();
        if (grade == null)
            return;
        
        gs.FindStudentsByGrade(grade ?? GradeType.passably);
    }

    static void ChangeStudentName()
    {
        Console.Write("Enter student's name: ");
        string name = Console.ReadLine();
        if (name.Length == 0)
        {
            Console.WriteLine("Name must ne longer than 0");
            return;
        }
        var student = gs.GetStudent(name);
        if (student == null)
        {
            Console.WriteLine($"Student {name} doesn't exist");
            return;
        }

        Console.Write("Enter student's new name: ");
        string newName = Console.ReadLine();
        if (newName.Length == 0)
        {
            Console.WriteLine("Name must ne longer than 0");
            return;
        }
        
        gs.ChangeStudentName(student, newName);
    }

    static void DeleteCourse()
    {
        Console.Write("Enter course's name: ");
        string course = Console.ReadLine();
        if (course.Length == 0)
        {
            Console.WriteLine("Course must ne longer than 0");
            return;
        }

        gs.DeleteCourse(course);
    }

    static void ShowBestAndWorstStudents()
    {
        gs.ShowBestAndWorstStudents();
    }

    static void ShowFullGradeStatistics()
    {
        int allGradesCount = gs.AllGradesCount();
        Console.WriteLine($"All grades count: {allGradesCount}");
        float averageGrade = gs.AverageGrade();
        Console.WriteLine($"Average grade: {averageGrade}");
        gs.ShowGradesCount();
        gs.ShowMostPopularRareGrade();
    }

    static void Main()
    {
        gs = new GradeSystem();
        ns = new NotificationSystem();
        gs.GradeAdded += ns.onGradeAdded;
        gs.StudentDeleted += ns.onStudentDeleted;

        PrintMenu();
        while (true)
        {
            Console.Write("\n>> ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "sg":
                    SaveGradesToFile();
                    break;
                case "lg":
                    LoadGradesFromFile();
                    break;
                case "ss":
                    SaveStudentsToFile();
                    break;
                case "ls":
                    LoadStudentsFromFile();
                    break;
                case "sr":
                    SaveReportToFile();
                    break;

                case "0":
                    AddPreparedData();
                    break;
                case "1":
                    AddStudent();
                    break;
                case "2":
                    DeleteStudent();
                    break;
                case "3":
                    AddGrade();
                    break;
                case "4":
                    ShowStudentGrades();
                    break;
                case "5":
                    ShowTopStudents();
                    break;
                case "6":
                    FindStudentsByGrade();
                    break;
                case "7":
                    ChangeStudentName();
                    break;
                case "8":
                    DeleteCourse();
                    break;
                case "9":
                    ShowBestAndWorstStudents();
                    break;
                case "10":
                    ShowFullGradeStatistics();
                    break;
                
                case "99": case "q": case "exit": case "quit":
                    gs.SaveReportToFile();
                    Console.WriteLine("Program is over");
                    return;
                default:
                    Console.WriteLine($"Unknown command: {choice}");
                    break;
            }
        }
    }
}