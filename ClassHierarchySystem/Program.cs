public class User
{
    public string Name { get; set; }
    public string Email { get; set; }

    public User(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public virtual void DisplayInfo()
    {
        Console.WriteLine($"Name: {Name}, email: {Email}");
    }
}

public class Student : User
{
    public int StudentID { get; set; }
    
    public Student(string name, string email, int studentID) : base(name, email)
    {
        StudentID = studentID;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[{StudentID}] Name: {Name}, email: {Email}");
    }
}

public class Teacher : User
{
    public string Course { get; set; }

    public Teacher(string name, string email, string course) : base(name, email)
    {
        Course = course;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Name: {Name}, email: {Email}, course: {Course}");
    }
}

public class Administrator : User
{
    public string Role { get; set; }

    public Administrator(string name, string email, string role) : base(name, email)
    {
        Role = role;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Name: {Name}, email: {Email}, role: {Role}");
    }
}

public class UserRepository<T> where T : User
{
    private List<T> users = new List<T>();
    
    public void AddUser(T user)
    {
        users.Add(user);
    }

    public void ShowAllUsers()
    {
        foreach (var user in users)
            user.DisplayInfo();
    }
}

class Program
{
    static UserRepository<User> users = new UserRepository<User>();

    static void Main()
    {
        users.AddUser(new Student("Danny", "email", 0));
        users.AddUser(new Administrator("Vasya", "master", "Boss"));

        users.ShowAllUsers();
    }
}