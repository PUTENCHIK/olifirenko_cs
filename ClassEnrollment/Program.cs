string[] students = { "Maxy", "Danny", "Glebby", "Pavel", "Alex" };
string[] courses = { "IOT", "MD", "CV" };

string[] keys = {"IOT", "IOT", "MD", "CV", "MD"};
string[] values = { "Maxy", "Danny", "Glebby", "Pavel", "Alex" };

/*
string[] students = new string[10];
string[] courses = new string[10];

string[] keys = new string[20];
string[] values = new string[20];
*/

string enterValue, enterValue2;

Console.WriteLine("Menu:");
Console.WriteLine("1. Add student");
Console.WriteLine("2. Show all students");
Console.WriteLine("3. Add course");
Console.WriteLine("4. Add student to course");
Console.WriteLine("5. Show course's students");
Console.WriteLine("6. Exit");

bool isRunning = true;

while (isRunning)
{
    Console.Write(">> ");
    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.Write("Enter student's name: ");
            enterValue = Console.ReadLine();
            if (enterValue.Length == 0)
            {
                Console.WriteLine("Name must be longer than 0");
            }
            else
            {
                for (int i = 0; i < students.Length; i++)
                {
                    if (students[i] == null)
                    {
                        students[i] = enterValue;
                        Console.WriteLine($"Student {enterValue} added");
                        break;
                    }
                }
            }
            break;
        case "2":
            Console.WriteLine("Students list:");
            int j;
            for (j = 0; j < students.Length; j++)
            {
                if (students[j] == null)
                {
                    if (j == 0) Console.WriteLine("\tempty");
                    break;
                }
                Console.WriteLine($"\t{j+1}) {students[j]};");
            }
            break;
        case "3":
            Console.Write("Enter courses's name: ");
            enterValue = Console.ReadLine();
            if (enterValue.Length == 0)
            {
                Console.WriteLine("Name must be longer than 0");
            }
            else
            {
                for (int i = 0; i < courses.Length; i++)
                {
                    if (courses[i] == null)
                    {
                        courses[i] = enterValue;
                        Console.WriteLine($"Course {enterValue} added");
                        break;
                    }
                }
            }
            break;
        case "4":
            Console.Write("Enter student's name: ");
            enterValue = Console.ReadLine();
            if (enterValue.Length == 0)
            {
                Console.WriteLine("Student's name must be longer than 0");
                break;
            }
            bool flag = false;
            for (int i = 0; i < students.Length && students[i] != null; i++)
            {
                if (students[i] == enterValue)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                Console.WriteLine($"Student {enterValue} doesn't exist");
                break;
            }

            Console.Write("Enter course's name: ");
            enterValue2 = Console.ReadLine();
            if (enterValue2.Length == 0)
            {
                Console.WriteLine("Course's name must be longer than 0");
                break;
            }
            flag = false;
            for (int i = 0; i < courses.Length && courses[i] != null; i++)
            {
                if (courses[i] == enterValue2)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                Console.WriteLine($"Course {enterValue2} doesn't exist");
                break;
            }

            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] == null)
                {
                    keys[i] = enterValue2;
                    break;
                }
            }
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == null)
                {
                    values[i] = enterValue;
                    break;
                }
            }
            Console.WriteLine($"Student {enterValue} added to course '{enterValue2}'");
            break;
        case "5":
            Console.Write("Enter course's name: ");
            enterValue = Console.ReadLine();
            if (enterValue.Length == 0)
            {
                Console.WriteLine("Course's name must be longer than 0");
                break;
            }
            int counter = 1;
            Console.WriteLine($"'{enterValue}'s students:");
            for (int i = 0; i < keys.Length && keys[i] != null; i++)
            {
                if (keys[i] == enterValue)
                {
                    Console.WriteLine($"\t{counter}) {values[i]};");
                    counter++;                }
                if (keys[i] == null)
                {
                    break;
                }
            }
            break;
        case "6":
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

