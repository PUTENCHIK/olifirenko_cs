/*
string[] students = { "Maxy", "Danny", "Glebby", "Pavel", "Alex" };
string[] courses = { "IOT", "MD", "CV" };

string[] keys = {"IOT", "IOT", "MD", "CV", "MD"};
string[] values = { "Maxy", "Danny", "Glebby", "Pavel", "Alex" };
*/

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
                        Console.WriteLine($"Student {studentNameToAdd} added");
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
                students[studentIdtoRemove] = null;
                Console.WriteLine($"Student [{studentIdtoRemove}] removed");
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
                        Console.WriteLine($"Course {courseNameToAdd} added");
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
                courses[courseIdtoRemove] = null;
                Console.WriteLine($"Student [{courseIdtoRemove}] removed");
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

            if (enrollments[courseIdForAddToCourse, studentIdForAddToCourse] == true)
            {
                Console.WriteLine($"Student [{studentIdForAddToCourse}] is already recorded to course [{courseIdForAddToCourse}]");
            }
            else
            {
                enrollments[courseIdForAddToCourse, studentIdForAddToCourse] = true;
                Console.WriteLine($"Student [{studentIdForAddToCourse}] has been recorded to course [{courseIdForAddToCourse}]");
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
            Console.WriteLine($"Students on course [{courseIdToShow}]:");
            for (int i = 0; i < students.Length; i++)
            {
                if (enrollments[courseIdToShow, i])
                {
                    Console.WriteLine($"\t[{i}] {students[i]};");
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

