//string[] students = { "Maxy", "Danny", "Glebby", "Pavel", "Alex" };
//string[] courses = { "IOT", "MD", "CV" };

string[] students = new string[10];
string[] courses = new string[10];

bool[,] enrollments = new bool[10, 10];


Console.WriteLine("Menu:");
Console.WriteLine("1. Add student");
Console.WriteLine("2. Show all students");
Console.WriteLine("3. Remove student");
Console.WriteLine("4. Add course");
Console.WriteLine("5. Show all courses");
Console.WriteLine("6. Remove course");
Console.WriteLine("7. Add student to course");
Console.WriteLine("8. Show course's students");
Console.WriteLine("9. Show all course-student");
Console.WriteLine("10. Remove student from course");
Console.WriteLine("11. Find student by name");
Console.WriteLine("99. Exit");

bool isRunning = true;

while (isRunning)
{
    Console.Write(">> ");
    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.Write("Enter student's name: ");
            string studentNameToAdd = Console.ReadLine();
            if (studentNameToAdd.Length == 0)
            {
                Console.WriteLine("Name must be longer than 0");
            }
            else
            {
                for (int i = 0; i < students.Length; i++)
                {
                    if (students[i] == null)
                    {
                        students[i] = studentNameToAdd;
                        Console.WriteLine($"Student [{i}] {studentNameToAdd} added");
                        break;
                    }
                }
            }
            break;
        case "2":
            Console.WriteLine("Students list:");
            for (int i = 0; i < students.Length; i++)
            {
                if (students[i] != null)
                {
                    Console.WriteLine($"\t[{i}] {students[i]};");
                }
            }
            break;
        case "3":
            Console.Write("Enter student's id: ");
            int studentIdtoRemove = int.Parse(Console.ReadLine());
            if (students[studentIdtoRemove] == null)
            {
                Console.WriteLine($"Student [{studentIdtoRemove}] doesn't exist");
            }
            else
            {
                Console.WriteLine($"Student [{studentIdtoRemove}] {students[studentIdtoRemove]} removed");
                students[studentIdtoRemove] = null;
            }
            break;
        case "4":
            Console.Write("Enter courses's name: ");
            string courseNameToAdd = Console.ReadLine();
            if (courseNameToAdd.Length == 0)
            {
                Console.WriteLine("Name must be longer than 0");
            }
            else
            {
                for (int i = 0; i < courses.Length; i++)
                {
                    if (courses[i] == null)
                    {
                        courses[i] = courseNameToAdd;
                        Console.WriteLine($"Course [{i}] {courseNameToAdd} added");
                        break;
                    }
                }
            }
            break;
        case "5":
            Console.WriteLine("Courses list:");
            for (int i = 0; i < courses.Length; i++)
            {
                if (courses[i] != null)
                {
                    Console.WriteLine($"\t[{i}] {courses[i]};");
                }
            }
            break;
        case "6":
            Console.Write("Enter course's id: ");
            int courseIdtoRemove = int.Parse(Console.ReadLine());
            if (courses[courseIdtoRemove] == null)
            {
                Console.WriteLine($"Course [{courseIdtoRemove}] doesn't exist");
            }
            else
            {
                Console.WriteLine($"Course [{courseIdtoRemove}] '{courses[courseIdtoRemove]}' removed");
                courses[courseIdtoRemove] = null;
            }
            break;
        case "7":
            Console.Write("Enter student's id: ");
            int studentIdForAddToCourse = int.Parse(Console.ReadLine());
            if (students[studentIdForAddToCourse] == null)
            {
                Console.WriteLine($"Student [{studentIdForAddToCourse}] doesn't exist");
                break;
            }

            Console.Write("Enter course's id: ");
            int courseIdForAddToCourse = int.Parse(Console.ReadLine());
            if (courses[courseIdForAddToCourse] == null)
            {
                Console.WriteLine($"Course [{courseIdForAddToCourse}] doesn't exist");
                break;
            }

            if (enrollments[courseIdForAddToCourse, studentIdForAddToCourse])
            {
                Console.WriteLine($"Student [{studentIdForAddToCourse}] is already recorded to course [{courseIdForAddToCourse}]");
            }
            else
            {
                enrollments[courseIdForAddToCourse, studentIdForAddToCourse] = true;
                Console.WriteLine($"Student [{studentIdForAddToCourse}] {students[studentIdForAddToCourse]} " +
                    $"has been recorded to course [{courseIdForAddToCourse}] '{courses[courseIdForAddToCourse]}'");
            }
            break;
        case "8":
            Console.Write("Enter course's id: ");
            int courseIdToShow = int.Parse(Console.ReadLine());
            if (courses[courseIdToShow] == null)
            {
                Console.WriteLine($"Course [{courseIdToShow}] doesn't exist");
                break;
            }
            Console.WriteLine($"Students on course [{courseIdToShow}] '{courses[courseIdToShow]}':");
            for (int i = 0; i < students.Length; i++)
            {
                if (enrollments[courseIdToShow, i])
                {
                    Console.WriteLine($"\t[{i}] {students[i]};");
                }
            }
            break;
        case "9":
            for (int idCourse = 0; idCourse < courses.Length; idCourse++)
            {
                if (courses[idCourse] != null)
                {
                    Console.WriteLine($"Course [{idCourse}] '{courses[idCourse]}':");
                    for (int idStudent = 0; idStudent < students.Length; idStudent++)
                    {
                        if (students[idStudent] != null && enrollments[idCourse, idStudent])
                        {
                            Console.WriteLine($"\t[{idStudent}] {students[idStudent]};");
                        }
                    }
                }
            }
            break;
        case "10":
            Console.Write("Enter student's id: ");
            int studentIdToUnrecord = int.Parse(Console.ReadLine());
            if (students[studentIdToUnrecord] == null)
            {
                Console.WriteLine($"Student [{studentIdToUnrecord}] doesn't exist");
                break;
            }

            Console.Write("Enter course's id: ");
            int courseIdToUnrecord = int.Parse(Console.ReadLine());
            if (courses[courseIdToUnrecord] == null)
            {
                Console.WriteLine($"Course [{courseIdToUnrecord}] doesn't exist");
                break;
            }

            bool isStudentRecored = enrollments[courseIdToUnrecord, studentIdToUnrecord];
            if (isStudentRecored == null || !isStudentRecored)
            {
                Console.WriteLine($"Student [{studentIdToUnrecord}] {students[studentIdToUnrecord]} " +
                    $"isn't recorded to course [{courseIdToUnrecord}] {courses[courseIdToUnrecord]}");
            }
            else
            {
                enrollments[courseIdToUnrecord, studentIdToUnrecord] = false;
                Console.WriteLine($"Student [{studentIdToUnrecord}] {students[studentIdToUnrecord]} " +
                    $"was unrecorded to course [{courseIdToUnrecord}] {courses[courseIdToUnrecord]}");
            }
            break;
        case "11":
            Console.Write("Enter student's name: ");
            string studentNameToFind = Console.ReadLine().ToLower();
            if (studentNameToFind.Length == 0)
            {
                Console.WriteLine("Enter at least 1 char");
                break;
            }
            Console.WriteLine("Search result:");
            for (int studentId = 0; studentId < students.Length; studentId++)
            {
                if (students[studentId] != null)
                {
                    string studentNameToCompare = students[studentId].ToLower();
                    if (studentNameToCompare.Contains(studentNameToFind))
                    {
                        Console.WriteLine($"\t[{studentId}] {students[studentId]}");
                    }
                }
            }
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