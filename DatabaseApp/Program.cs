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
}

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; }
}

public class Enrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public double Grade { get; set; }
    public Student Student { get; set; }
    public Course Course { get; set; }
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
    static string? GetString(string label)
    {
        Console.Write($"Enter {label.ToLower()}: ");
        var input = Console.ReadLine();
        if (input.Length == 0)
        {
            Console.WriteLine($"[ERROR] {label} is required");
            return null;
        }
        return input;
    }

    static int GetInt(string label)
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
                    StudentId = students[1].Id,
                    CourseId = courses[2].Id,
                    Grade = 5
                },
                new Enrollment {
                    StudentId = students[2].Id,
                    CourseId = courses[1].Id,
                    Grade = 2
                },
                new Enrollment {
                    StudentId = students[3].Id,
                    CourseId = courses[1].Id,
                    Grade = 3
                },
            };

            context.Enrollments.AddRange(enrollments);
            context.SaveChanges();
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
        Console.WriteLine($"[SUCCESS] New student {name}, {age} y.o. has been added");
    }

    static void AddCourse(UniversityContext context)
    {
        var title = GetString("Course's title");
        if (title == null) return;

        var course = new Course { Title = title };
        context.Courses.Add(course);
        context.SaveChanges();
        Console.WriteLine($"[SUCCESS] New course '{title}' has been added");
    }

    static void ShowAllStudents(UniversityContext context)
    {
        Console.WriteLine("Students:");
        bool studentsExist = false;
        var students = context.Students.ToList();
        foreach (var student in students)
        {
            studentsExist = true;
            Console.WriteLine($"\t[{student.Id}] {student.Name}, {student.Age} y.o.");
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
            Console.WriteLine($"\t[{course.Id}] '{course.Title}'");
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
        double grade = double.Parse(Console.ReadLine());

        var enrollment = new Enrollment {
            StudentId = studentId,
            CourseId = courseId,
            Grade = grade,
        };
        context.Enrollments.Add(enrollment);
        context.SaveChanges();
        Console.WriteLine($"Student [{studentId}] has been enrolled on course [{courseId}]");
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
            var newName = GetString("New name");
            if (newName == null) return;
            int newAge = GetInt("New age");
            if (newAge == -1) return;

            student.Name = newName;
            student.Age = newAge;
            context.SaveChanges();
            Console.WriteLine($"[SUCCESS] Student [{studentId}] has been updated");
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
            Console.WriteLine($"[SUCCESS] Student [{studentId}] has been deleted");
        }
        else Console.WriteLine($"[ERROR] Student [{studentId}] doesn't exist");
    }

    static void FindStudentByAge(UniversityContext context)
    {
        int age = GetInt("Student's age");
        if (age == -1) return;
        var students = context.Students.Where(s => s.Age == age).ToList();
        Console.WriteLine($"Students {age} y.o.:");
        foreach (var student in students)
            Console.WriteLine($"[{student.Id}] {student.Name}");
        if (students.Count() == 0)
            Console.WriteLine($"No students {age} y.o.");
    }

    static void FindCoursesByStudent(UniversityContext context)
    {
        ShowAllStudents(context);
        int studentId = GetInt("Student's id");
        if (studentId == -1) return;
        var courses = context.Enrollments
            .Where(e => e.StudentId == studentId)
            .Select(e => e.Course)
            .ToList();
        Console.WriteLine($"Student [{studentId}]'s courses:");
        foreach (var course in courses)
            Console.WriteLine($"[{course.Id}] '{course.Title}'");
        if (courses.Count() == 0)
            Console.WriteLine($"Student [{studentId}] doesn't have any courses");
    }
    /*
    TODO
    1. Показ всех студентов, отсортированных по возрасту (убыв и возр);
    2. Студенты с grade > 4. Определить лучшего студента
    3. Найти и вывести студентов без курсов. + рекомндация записаться на курс с наибольшим колвом студентов
    
    */

    static void PrintMenu()
    {
        Console.WriteLine("\nEnter an option:");
        Console.WriteLine("[1] Add student");
        Console.WriteLine("[2] Add course");
        Console.WriteLine("[3] Show all students");
        Console.WriteLine("[4] Show all courses");
        Console.WriteLine("[5] Enroll student on course");
        Console.WriteLine("[6] Show students with courses");
        // Console.WriteLine("");
        Console.WriteLine("\n[c] Clear console");
        Console.WriteLine("[q] Exit");
    }

    static void Main()
    {
        using (var context = new UniversityContext())
        {
            SeedDatabase(context);
            PrintMenu();

            while (true)
            {
                Console.Write("\n>> ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        AddStudent(context);
                        break;
                    case "2":
                        AddCourse(context);
                        break;
                    case "3":
                        ShowAllStudents(context);
                        break;
                    case "4":
                        ShowAllCourses(context);
                        break;
                    case "5":
                        EnrollStudent(context);
                        break;
                    case "6":
                        QueryStudents(context);
                        break;

                    case "c":
                        Console.Clear();
                        PrintMenu();
                        break;
                    case "q":
                        Console.WriteLine("Program is over");
                        return;
                    default:
                        Console.WriteLine("");
                        break;
                }
            }
        }
    }
}