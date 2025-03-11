/*
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.InMemory
Microsoft.EntityFrameworkCore.Tools
*/

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public override string ToString()
    {
        return $"[{Id}] {Name}, {Age} y.o.";
    }
}

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }

    public override string ToString()
    {
        return $"[{Id}] '{Title}'";
    }
}

public class Enrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public double Grade { get; set; }
    public Student Student { get; set; }
    public Course Course { get; set; }

    public override string ToString()
    {
        return $"[{Id}] ({Student}) -- ({Course}): {Grade}";
    }
}

public class UniversityContext: DbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("UniversityDB");
    }
}

class Program
{
    static string? GetString(string label, bool canBeEmpty = false)
    {
        Console.Write($"Enter {label.ToLower()}: ");
        var input = Console.ReadLine();
        if (input.Length == 0)
        {
            if (!canBeEmpty)
                Console.WriteLine($"[ERROR] {label} is required");
            return null;
        }
        return input;
    }

    static int GetInt(string label, bool canBeEmpty = false)
    {
        Console.Write($"Enter {label.ToLower()}: ");
        try
        {
            var input = int.Parse(Console.ReadLine());
            if (input <= 0)
            {
                Console.WriteLine("[ERROR] Number must be positive");
                return -1;
            }
            return input;
        }
        catch
        {
            if (!canBeEmpty)
                Console.WriteLine("[ERROR] You must enter integer number");
            return -1;
        }
    }

    static void SeedDatabase(UniversityContext context)
    {
        if (!context.Students.Any() && !context.Courses.Any())
        {
            var students = new List<Student>
            {
                new Student { Name = "pEtrov", Age = 20 },
                new Student { Name = "Bobby", Age = 21 },
                new Student { Name = "Danny", Age = 23 },
                new Student { Name = "kOlbas", Age = 21 },
            };
            var courses = new List<Course>
            {
                new Course { Title = "Dodge Gaben's refraction" },
                new Course { Title = "Master of Hook accuracy" },
                new Course { Title = "Quas Wex Exort training" },
            };

            context.Students.AddRange(students);
            context.Courses.AddRange(courses);
            context.SaveChanges();

            var enrollments = new List<Enrollment>
            {
                new Enrollment {
                    StudentId = students[0].Id,
                    CourseId = courses[0].Id,
                    Grade = 4
                },
                new Enrollment {
                    StudentId = students[0].Id,
                    CourseId = courses[2].Id,
                    Grade = 2
                },
                new Enrollment {
                    StudentId = students[1].Id,
                    CourseId = courses[2].Id,
                    Grade = 5
                },
                new Enrollment {
                    StudentId = students[1].Id,
                    CourseId = courses[0].Id,
                    Grade = 5
                },
                new Enrollment {
                    StudentId = students[2].Id,
                    CourseId = courses[1].Id,
                    Grade = 2
                },
                new Enrollment {
                    StudentId = students[2].Id,
                    CourseId = courses[0].Id,
                    Grade = 3
                },
                new Enrollment {
                    StudentId = students[2].Id,
                    CourseId = courses[2].Id,
                    Grade = 5
                },
                new Enrollment {
                    StudentId = students[3].Id,
                    CourseId = courses[1].Id,
                    Grade = 3
                },
                new Enrollment {
                    StudentId = students[3].Id,
                    CourseId = courses[2].Id,
                    Grade = 4
                },
            };

            context.Enrollments.AddRange(enrollments);
            context.SaveChanges();
            Console.WriteLine($"[SUCCESS] {students.Count} students, {courses.Count} courses and {enrollments.Count} enrollments have been added to DB");
        }
    }

    static void AddStudent(UniversityContext context)
    {
        var name = GetString("Student's name");
        if (name == null) return;
        var age = GetInt("Student's age");
        if (age == -1) return;
        
        var student = new Student { Name = name, Age = age};
        context.Students.Add(student);
        context.SaveChanges();
        Console.WriteLine($"[SUCCESS] New student {student} has been added");
    }

    static void AddCourse(UniversityContext context)
    {
        var title = GetString("Course's title");
        if (title == null) return;

        var course = new Course { Title = title };
        context.Courses.Add(course);
        context.SaveChanges();
        Console.WriteLine($"[SUCCESS] New course {course} has been added");
    }

    static void ShowAllStudents(UniversityContext context)
    {
        Console.WriteLine("Students:");
        bool studentsExist = false;
        var students = context.Students.ToList();
        foreach (var student in students)
        {
            studentsExist = true;
            Console.WriteLine($"\t{student}");
        }
        if (!studentsExist)
            Console.WriteLine("\tno students");
    }

    static void ShowAllCourses(UniversityContext context)
    {
        Console.WriteLine("Courses:");
        bool coursesExist = false;
        var courses = context.Courses.ToList();
        foreach (var course in courses)
        {
            coursesExist = true;
            Console.WriteLine($"\t{course}");
        }
        if (!coursesExist)
            Console.WriteLine("\tno courses");
    }

    static void EnrollStudent(UniversityContext context)
    {
        ShowAllStudents(context);
        var studentId = GetInt("Student's id");
        if (studentId == -1) return;
        if (!context.Students.Select(s => s.Id).ToList().Contains(studentId))
        {
            Console.WriteLine($"[ERROR] Student [{studentId}] doesn't exist");
            return;
        }

        ShowAllCourses(context);
        var courseId = GetInt("Course's id");
        if (courseId == -1) return;
        if (!context.Courses.Select(c => c.Id).ToList().Contains(courseId))
        {
            Console.WriteLine($"[ERROR] Course [{courseId}] doesn't exist");
            return;
        }

        Console.Write("Enter grade: ");
        double grade;
        try
        {
            grade = double.Parse(Console.ReadLine());
        }
        catch (FormatException)
        {
            Console.WriteLine("[ERROR] Wrong format of float");
            return;
        }

        var enrollment = new Enrollment {
            StudentId = studentId,
            CourseId = courseId,
            Grade = grade,
        };
        context.Enrollments.Add(enrollment);
        context.SaveChanges();
        Console.WriteLine($"New enrollment: {enrollment}");
    }

    static void QueryStudents(UniversityContext context)
    {
        var students = context.Students.Include(s => s.Enrollments).ThenInclude(e => e.Course).ToList();
        bool studentsExist = false;
        foreach (var student in students)
        {
            studentsExist = true;
            Console.WriteLine($"[{student.Id}] {student.Name}, {student.Age} y.o.:");
            bool enrollmentsExist = false;
            foreach (var enrollment in student.Enrollments)
            {
                enrollmentsExist = true;
                Console.WriteLine($"\t{enrollment.Course.Title}: {enrollment.Grade}");
            }
            if (!enrollmentsExist)
                Console.WriteLine("\tno courses");
        }
        if (!studentsExist)
            Console.WriteLine("No students");
    }

    static void UpdateStudent(UniversityContext context)
    {
        ShowAllStudents(context);
        int studentId = GetInt("Student's id");
        if (studentId == -1) return;
        var student = context.Students.Find(studentId);
        if (student != null)
        {
            var newName = GetString("New name", canBeEmpty: true);
            int newAge = GetInt("New age", canBeEmpty: true);

            student.Name = newName == null ? student.Name : newName;
            student.Age = newAge == -1 ? student.Age : newAge;
            context.SaveChanges();
            Console.WriteLine($"[SUCCESS] Updated student: {student}");
        }
        else Console.WriteLine($"[ERROR] Student [{studentId}] doesn't exist");
    }

    static void DeleteStudent(UniversityContext context)
    {
        ShowAllStudents(context);
        int studentId = GetInt("Student's id");
        if (studentId == -1) return;
        var student = context.Students.Find(studentId);
        if (student != null)
        {
            context.Students.Remove(student);
            context.SaveChanges();
            Console.WriteLine($"[SUCCESS] Student {student} has been deleted");
        }
        else Console.WriteLine($"[ERROR] Student [{studentId}] doesn't exist");
    }

    static void FindStudentsByAge(UniversityContext context)
    {
        int age = GetInt("Student's age");
        if (age == -1) return;
        var students = context.Students.Where(s => s.Age == age).ToList();
        Console.WriteLine($"Students {age} y.o.:");
        foreach (var student in students)
            Console.WriteLine($"\t{student}");
        if (students.Count() == 0)
            Console.WriteLine($"No students {age} y.o.");
    }

    static void FindCoursesByStudent(UniversityContext context)
    {
        ShowAllStudents(context);
        int studentId = GetInt("Student's id");
        if (studentId == -1) return;
        var student = context.Students.Find(studentId);

        var courses = context.Enrollments
            .Where(e => e.StudentId == studentId)
            .Select(e => e.Course)
            .ToList();
        Console.WriteLine($"Courses of {student}:");
        foreach (var course in courses)
            Console.WriteLine($"\t{course}");
        if (courses.Count == 0)
            Console.WriteLine($"Student [{studentId}] doesn't have any courses");
    }

    static void ShowStudentsSortedByAge(UniversityContext context)
    {
        var studentsAscending = context.Students.OrderBy(s => s.Age).ToList();
        if (studentsAscending.Count == 0)
        {
            Console.WriteLine("No students");
            return;
        }

        Console.WriteLine("Students sorted by age ascending:");
        foreach (var student in studentsAscending)
            Console.WriteLine($"\tstudent");

        var studentsDescending  = context.Students.OrderByDescending(s => s.Age).ToList();
        Console.WriteLine("\nStudents sorted by age descending:");
        foreach (var student in studentsDescending)
            Console.WriteLine($"\tstudent");
    }

    static void ShowGoodStudents(UniversityContext context)
    {
        double limit = 4;
        var highAchieres = context.Students
            .Where(s => s.Enrollments.Any())
            .Select(s => new { Student = s, AvgGrade = s.Enrollments.Average(e => e.Grade) })
            .Where(s => s.AvgGrade > limit)
            .ToList();
        if (highAchieres.Count > 0)
        {
            Console.WriteLine($"Students with avg grade > {limit}:");
            foreach (var entity in highAchieres)
                Console.WriteLine($"\t{entity.Student}: avg = {entity.AvgGrade}");
            var bestStudent = highAchieres.OrderByDescending(s => s.AvgGrade).FirstOrDefault();
            Console.WriteLine($"\nBest student: {bestStudent.Student} - avg = {bestStudent.AvgGrade}");
        }
        else Console.WriteLine($"No students with avg grade > {limit}");
    }

    static void FindStudentsWithoutCourses(UniversityContext context)
    {
        var studentsWithoutCourses = context.Students
            .Where(s => !s.Enrollments.Any())
            .ToList();
        if (studentsWithoutCourses.Count > 0)
        {
            Console.WriteLine("Students without courses:");
            foreach (var student in studentsWithoutCourses)
                Console.WriteLine($"\t{student}");
            var mostPopularCourse = context.Courses
                .Select(c => new { Course = c, StudentCount = context.Enrollments.Count(e => e.CourseId == c.Id ) })
                .OrderByDescending(c => c.StudentCount)
                .FirstOrDefault();
            if (mostPopularCourse != null)
                Console.WriteLine($"\nMost popular course: {mostPopularCourse.Course} - {mostPopularCourse.StudentCount} student(s)");
            else
                Console.WriteLine("\nNo courses");
        }
        else Console.WriteLine("All students have courses");
    }

    static void PrintMenu()
    {
        Console.WriteLine("\nEnter an option:");
        Console.WriteLine("[1] Print all options");
        Console.WriteLine("[2] Student's functions");
        Console.WriteLine("[3] Course's functions");
        Console.WriteLine("[4] Enrollment's functions");
        Console.WriteLine("[5] Add test data");
        Console.WriteLine("\n[c] Clear console");
        Console.WriteLine("[q] Exit");
    }

    static void PrintAllOptions()
    {
        Console.WriteLine("\nEnter an option:");
        Console.WriteLine("[1] Print all options");
        Console.WriteLine("[2] Student's functions:");
        PrintStudentOptions();
        Console.WriteLine("[3] Course's functions:");
        PrintCourseOptions();
        Console.WriteLine("[4] Enrollment's functions:");
        PrintEnrollmentOptions();
        Console.WriteLine("[5] Add test data");
        Console.WriteLine("\n[c] Clear console");
        Console.WriteLine("[q] Exit");
    }

    static void PrintStudentOptions()
    {
        Console.WriteLine("\t[1] Add student");
        Console.WriteLine("\t[2] Show all students");
        Console.WriteLine("\t[3] Update student");
        Console.WriteLine("\t[4] Delete student");
        Console.WriteLine("\t[5] Find students by age");
        Console.WriteLine("\t[6] Show students sorted by age");
        Console.WriteLine("\t[7] Show good students and the best");
    }

    static void PrintCourseOptions()
    {
        Console.WriteLine("\t[1] Add course");
        Console.WriteLine("\t[2] Show all courses");
    }

    static void PrintEnrollmentOptions()
    {
        Console.WriteLine("\t[1] Show students with courses");
        Console.WriteLine("\t[2] Enroll student on course");
        Console.WriteLine("\t[3] Find courses by student");
        Console.WriteLine("\t[4] Find students without courses");
    }

    static string GetChoice()
    {
        PrintMenu();
        Console.Write("\n>> ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "2":
                PrintStudentOptions();
                Console.Write("\n>> ");
                return choice + Console.ReadLine();
            case "3":
                PrintCourseOptions();
                Console.Write("\n>> ");
                return choice + Console.ReadLine();
            case "4":
                PrintEnrollmentOptions();
                Console.Write("\n>> ");
                return choice + Console.ReadLine();
            default:
                return choice;
        }
    }

    static void Main()
    {
        using (var context = new UniversityContext())
        {
            while (true)
            {
                string choice = GetChoice();
                switch (choice)
                {
                    case "1":
                        PrintAllOptions();
                        break;

                    case "21":
                        AddStudent(context);
                        break;
                    case "22":
                        ShowAllStudents(context);
                        break;
                    case "23":
                        UpdateStudent(context);
                        break;
                    case "24":
                        DeleteStudent(context);
                        break;
                    case "25":
                        FindStudentsByAge(context);
                        break;
                    case "26":
                        ShowStudentsSortedByAge(context);
                        break;
                    case "27":
                        ShowGoodStudents(context);
                        break;

                    case "31":
                        AddCourse(context);
                        break;
                    case "32":
                        ShowAllCourses(context);
                        break;

                    case "41":
                        QueryStudents(context);
                        break;
                    case "42":
                        EnrollStudent(context);
                        break;
                    case "43":
                        FindCoursesByStudent(context);
                        break;
                    case "44":
                        FindStudentsWithoutCourses(context);
                        break;

                    case "5":
                        SeedDatabase(context);
                        break;

                    case "c":
                        Console.Clear();
                        break;
                    case "q":
                        Console.WriteLine("Program is over");
                        return;
                    default:
                        Console.WriteLine($"[ERROR] Unknown command: {choice}");
                        break;
                }
            }
        }
    }
}