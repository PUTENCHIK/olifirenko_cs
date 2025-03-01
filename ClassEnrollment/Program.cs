internal class Program
{
    //string[] students = { "Maxy", "Danny", "Glebby", "Pavel", "Alex" };
    //string[] courses = { "IOT", "MD", "CV" };

    static string[] students = new string[10];
    static string[] courses = new string[10];
    static bool[,] enrollments = new bool[10, 10];

    static void StartPrint()
    {
        Console.WriteLine("Menu:");
        Console.WriteLine("1.                   Add student");
        Console.WriteLine("2.                   Show all students");
        Console.WriteLine("3.                   Remove student");
        Console.WriteLine("4.                   Add course");
        Console.WriteLine("5.                   Show all courses");
        Console.WriteLine("6.                   Remove course");
        Console.WriteLine("7.                   Add student to course");
        Console.WriteLine("8.                   Show course's students");
        Console.WriteLine("9.                   Show all course-student");
        Console.WriteLine("10.                  Remove student from course");
        Console.WriteLine("11.                  Find student by name");
        Console.WriteLine("(99, exit, q, quit). Exit");
    }

    static int GetValidId(string[] array, string entityName)
    {
        Console.Write($"Enter {entityName.ToLower()}'s id: ");
        try
        {
            int id = int.Parse(Console.ReadLine());
            if (array[id] == null)
            {
                Console.WriteLine($"{entityName} [{id}] doesn't exist");
                return -1;
            }
            return id;
        }
        catch (Exception ex)
        {
            if (ex is FormatException || ex is IndexOutOfRangeException)
            {
                Console.WriteLine($"{entityName}'s id must be positive integer");
                return -1;
            }
            else throw;
        }
    }

    static string? GetValidName(string entityName)
    {
        Console.Write($"Enter {entityName.ToLower()}'s name: ");
        string input = Console.ReadLine();
        if (input.Length == 0)
        {
            Console.WriteLine($"{entityName}'s name must be longer than 0");
            return null;
        }
        return input;
    }

    static void AddEntity(string[] array, string entityName)
    {
        string? name = GetValidName(entityName);
        if (name == null) return;
        int i;
        for (i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
            {
                array[i] = name;
                Console.WriteLine($"{entityName} [{i}] '{name}' added");
                return;
            }
        }
        Console.WriteLine($"Array of {entityName.ToLower()}s is full");
    }

    static void ShowList(string[] array, string entityName)
    {
        Console.WriteLine($"{entityName}'s list:");
        bool isEmpty = true;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
            {
                Console.WriteLine($"\t[{i}] {array[i]};");
                isEmpty = false;
            }
        }
        if (isEmpty)
        {
            Console.WriteLine("\tempty");
        }
    }

    static void RemoveEntity(string[] array, string entityName)
    {
        int id = GetValidId(array, entityName);
        if (id != -1)
        {
            Console.WriteLine($"{entityName} [{id}] {array[id]} removed");
            array[id] = null;
        }
    }

    static void ShowCourseStudents(int courseId)
    {
        bool noStudents = true;
        Console.WriteLine($"Students on course [{courseId}] '{courses[courseId]}':");
        for (int i = 0; i < students.Length; i++)
        {
            if (enrollments[courseId, i])
            {
                Console.WriteLine($"\t[{i}] {students[i]};");
                noStudents = false;
            }
        }
        if (noStudents)
        {
            Console.WriteLine("\tno students");
        }
    }

    static void AddStudent()
    {
        AddEntity(students, "Student");
    }

    static void ShowAllStudents()
    {
        ShowList(students, "Student");
    }

    static void RemoveStudent()
    {
        RemoveEntity(students, "Student");
    }

    static void AddCourse()
    {
        AddEntity(courses, "Course");
    }

    static void ShowAllCourses()
    {
        ShowList(courses, "Course");
    }

    static void RemoveCourse()
    {
        RemoveEntity(courses, "Course");
    }

    static void AddStudentToCourse()
    {
        int studentId = GetValidId(students, "Student");
        if (studentId == -1) return;
        int courseId = GetValidId(courses, "Course");
        if (courseId == -1) return;

        if (enrollments[courseId, studentId])
        {
            Console.WriteLine($"Student [{studentId}] {students[studentId]} " +
                $"is already recorded to course [{courseId}] {courses[courseId]}");
        }
        else
        {
            enrollments[courseId, studentId] = true;
            Console.WriteLine($"Student [{studentId}] {students[studentId]} " +
                $"has been recorded to course [{courseId}] '{courses[courseId]}'");
        }
    }

    static void ShowStudentsOnCourse()
    {
        int id = GetValidId(courses, "Course");
        if (id == -1) return;
        ShowCourseStudents(id);
    }

    static void ShowAllStudentsAndCourses()
    {
        int counter = 0;
        for (int idCourse = 0; idCourse < courses.Length; idCourse++)
        {
            if (courses[idCourse] != null)
            {
                ShowCourseStudents(idCourse);
                counter++;
            }
        }
        if (counter == 0)
        {
            Console.WriteLine("No courses");
        }
    }

    static void RemoveStudentFromCourse()
    {
        int studentId = GetValidId(students, "Student");
        if (studentId == -1) return;
        int courseId = GetValidId(courses, "Course");
        if (courseId == -1) return;

        bool isStudentRecored = enrollments[courseId, studentId];
        if (isStudentRecored == null || !isStudentRecored)
        {
            Console.WriteLine($"Student [{studentId}] {students[studentId]} " +
                $"isn't recorded to course [{courseId}] {courses[courseId]}");
        }
        else
        {
            enrollments[courseId, studentId] = false;
            Console.WriteLine($"Student [{studentId}] {students[studentId]} " +
                $"was unrecorded to course [{courseId}] {courses[courseId]}");
        }
    }

    static void FindStudentByName()
    {
        string? name = GetValidName("Student");
        if (name == null) return;
        name = name.ToLower();

        bool noResult = true;
        Console.WriteLine("Search result:");
        for (int studentId = 0; studentId < students.Length; studentId++)
        {
            if (students[studentId] != null)
            {
                string studentNameToCompare = students[studentId].ToLower();
                if (studentNameToCompare.Contains(name))
                {
                    Console.WriteLine($"\t[{studentId}] {students[studentId]}");
                    noResult = false;
                }
            }
        }
        if (noResult)
        {
            Console.WriteLine("\tno result");
        }
    }

    static void Main()
    {
        StartPrint();
        bool isRunning = true;

        while (isRunning)
        {
            Console.Write(">> ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddStudent();
                    break;
                case "2":
                    ShowAllStudents();
                    break;
                case "3":
                    RemoveStudent();
                    break;
                case "4":
                    AddCourse();
                    break;
                case "5":
                    ShowAllCourses();
                    break;
                case "6":
                    RemoveCourse();
                    break;
                case "7":
                    AddStudentToCourse();
                    break;
                case "8":
                    ShowStudentsOnCourse();
                    break;
                case "9":
                    ShowAllStudentsAndCourses();
                    break;
                case "10":
                    RemoveStudentFromCourse();
                    break;
                case "11":
                    FindStudentByName();
                    break;
                case "99":
                case "quit":
                case "q":
                case "exit":
                    Console.WriteLine("Program over");
                    isRunning = false;
                    break;

                default:
                    Console.WriteLine("Wrong option. Try again");
                    break;
            }
        }
    }
}