using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Reflection.PortableExecutable;
using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }

    public override string ToString()
    {
        return $"[{Id}] {Name} AKA {Username} ({Email})";
    }
}

public class Post
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }

    public override string ToString()
    {
        return $"[{Id}] title: '{Title}'";
    }
}

public class Comment
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Body { get; set; }

    public override string ToString()
    {
        int bodyLimit = 25;
        string cuttedBody;
        if (Body.Length < bodyLimit)
            cuttedBody = Body;
        else
            cuttedBody = Body.Substring(0, bodyLimit) + "...";
        return $"[{Id}]: post [{PostId}] ({Email}): '{cuttedBody}'";
    }
}

class Program
{
    static int? GetInt(string label)
    {
        Console.Write($"Enter {label}: ");
        try
        {
            int number = int.Parse(Console.ReadLine());
            return number;
        }
        catch 
        {
            Console.WriteLine("[ERROR] Wrong format of integer number");
            return null;
        }
    }
    static async Task GetUsers()
    {
        using (HttpClient client = new HttpClient())
        {
            string url = "https://jsonplaceholder.typicode.com/users";
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<User> users = JsonConvert.DeserializeObject<List<User>>(json);
                Console.WriteLine("Users:");
                if (users.Count == 0)
                    Console.WriteLine("\tno users");
                else
                    foreach (var user in users)
                        Console.WriteLine($"\t{user}");
            }
            else Console.WriteLine("[ERROR] Request is failed");
        }
    }

    static async Task GetUserPosts()
    {
        int? userId = GetInt("user's id");
        if (userId is null) return;

        using (HttpClient client = new HttpClient())
        {
            string url = $"https://jsonplaceholder.typicode.com/posts?userId={userId}";
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<Post> posts = JsonConvert.DeserializeObject<List<Post>>(json);
                Console.WriteLine($"Post of [{userId}] user:");
                if (posts.Count == 0)
                    Console.WriteLine("\tno posts");
                else
                    foreach (var post in posts)
                        Console.WriteLine($"\t{post}");
            }
            else Console.WriteLine("[ERROR] Request is failed");
        }
    }

    static async Task CreatePost()
    {
        using (HttpClient client = new HttpClient())
        {
            string url = "https://jsonplaceholder.typicode.com/posts";
            var newPost = new Post {
                UserId = 1,
                Title = "New Post",
                Body = "Hello everyone!"
            };
            string json = JsonConvert.SerializeObject(newPost);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);
            string responseJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {responseJson}");
        }
    }

    static async Task UpdatePost()
    {
        int? postId = GetInt("post's id");
        if (postId is null) return;

        using (HttpClient client = new HttpClient())
        {
            string url = $"https://jsonplaceholder.typicode.com/posts/{postId}";
            var updatedPost = new Post {
                Id = postId ?? 1,
                UserId = 1,
                Title = "Updated title",
                Body = "Updated body"
            };
            string json = JsonConvert.SerializeObject(updatedPost);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/app");
            HttpResponseMessage response = await client.PostAsync(url, content);
            string responseJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Updated post: {responseJson}");
        }
   }

    static async Task DeletePost()
    {
        int? postId = GetInt("post's id");
        if (postId is null) return;

        using (HttpClient client = new HttpClient())
        {
            string url = $"https://jsonplaceholder.typicode.com/posts/{postId}";
            HttpResponseMessage response = await client.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Post [{postId}] has been deleted");
            }
            else Console.WriteLine("[ERROR] Request is failed");
        }
    }

    static async Task FilterUsersByEmail()
    {
        Console.Write("Enter email domain: ");
        string emailDomain = Console.ReadLine();
        if (emailDomain.Length == 0)
        {
            Console.WriteLine("[ERROR] Email domain must be longer than 0");
            return;
        }

        using (HttpClient client = new HttpClient())
        {
            string url = "https://jsonplaceholder.typicode.com/users";
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<User> users = JsonConvert.DeserializeObject<List<User>>(json);
                var filteredUsers = users
                    .Where(u => u.Email.EndsWith(emailDomain))
                    .ToList();
                Console.WriteLine($"Users with {emailDomain} email domain:");
                if (filteredUsers.Count == 0)
                    Console.WriteLine("\tno users");
                else
                    foreach (var user in filteredUsers)
                        Console.WriteLine($"\t{user}");
            }
            else Console.WriteLine("[ERROR] Request is failed");
        }
    }

    static async Task AlphabetSortUsers()
    {
        using (HttpClient client = new HttpClient())
        {
            string url = "https://jsonplaceholder.typicode.com/users";
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<User> users = JsonConvert.DeserializeObject<List<User>>(json);
                var sortedUsers = users.OrderBy(u => u.Name).ToList();

                Console.WriteLine("Alphabet sorted users:");
                if (sortedUsers.Count == 0)
                    Console.WriteLine("\tno users");
                else
                    foreach (var user in sortedUsers)
                        Console.WriteLine($"\t{user}");
            }
            else Console.WriteLine("[ERROR] Request is failed");
        }
    }

    static async Task ShowTheLongestPost()
    {
        using (HttpClient client = new HttpClient())
        {
            string url = "https://jsonplaceholder.typicode.com/posts";
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<Post> posts = JsonConvert.DeserializeObject<List<Post>>(json);
                var theLongest = posts.OrderByDescending(p => p.Body.Length).FirstOrDefault();
                Console.Write("The longest post: ");
                if (theLongest is null)
                    Console.WriteLine("no posts");
                else
                    Console.WriteLine($"{theLongest}: body length = {theLongest.Body.Length}");
            }
            else Console.WriteLine("[ERROR] Request is failed");
        }
    }

    static async Task EndsWithComments()
    {
        Console.Write("Enter email's domain: ");
        string endString = Console.ReadLine();
        if (endString.Length == 0)
        {
            Console.WriteLine("Domain must be longer than 0");
            return;
        }

        using (HttpClient client = new HttpClient())
        {
            string url = "https://jsonplaceholder.typicode.com/comments";
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                List<Comment> comments = JsonConvert.DeserializeObject<List<Comment>>(json);

                var endsWithComments = comments.Where(c => c.Email.EndsWith(endString)).ToList();
                Console.WriteLine($"Comments end with '{endString}':");
                if (endsWithComments.Count == 0)
                    Console.WriteLine("\tno comments");
                else
                    foreach (var comment in endsWithComments)
                        Console.WriteLine($"\t{comment}");
            }
            else Console.WriteLine("[ERROR] Request is failed");
        }
    }

    static void PrintMenu()
    {
        Console.WriteLine("Menu:");
        Console.WriteLine("[1] Print all options");
        Console.WriteLine("[2] User options");
        Console.WriteLine("[3] Post options");
        Console.WriteLine("[4] Comment options");
        Console.WriteLine("[c] Clear console");
        Console.WriteLine("[q] Exit");
    }

    static void PrintAllOptions()
    {
        Console.WriteLine("\nAll options:");
        Console.WriteLine("[1] Print all options");
        Console.WriteLine("[2] User options:");
        PrintUserOptions();
        Console.WriteLine("[3] Post options:");
        PrintPostOptions();
        Console.WriteLine("[4] Comment options:");
        PrintCommentOptions();
        Console.WriteLine("[c] Clear console");
        Console.WriteLine("[q] Exit");
    }

    static void PrintUserOptions()
    {
        Console.WriteLine("\t[1] Show users");
        Console.WriteLine("\t[2] Filter users by domain of email");
        Console.WriteLine("\t[3] Alphabet sort of users");
    }

    static void PrintPostOptions()
    {
        Console.WriteLine("\t[1] Show user's posts");
        Console.WriteLine("\t[2] Add new post");
        Console.WriteLine("\t[3] Update post");
        Console.WriteLine("\t[4] Delete post");
        Console.WriteLine("\t[5] Show the longest post");
    }

    static void PrintCommentOptions()
    {
        Console.WriteLine("\t[1] Show all comments with email ends with");
    }

    static string GetChoice()
    {
        Console.Write("\n>> ");
        string choice = Console.ReadLine();
        switch (choice)
        {
            case "2":
                PrintUserOptions();
                Console.Write("\n>> ");
                return choice + Console.ReadLine();
            case "3":
                PrintPostOptions();
                Console.Write("\n>> ");
                return choice + Console.ReadLine();
            case "4":
                PrintCommentOptions();
                Console.Write("\n>> ");
                return choice + Console.ReadLine();
            default:
                return choice;
        }
    }

    static async Task Main()
    {
        PrintMenu();
        while (true)
        {
            string choice = GetChoice();
            switch (choice)
            {
                case "1":
                    PrintAllOptions();
                    break;
                
                case "21":
                    await GetUsers();
                    break;
                case "22":
                    await FilterUsersByEmail();
                    break;
                case "23":
                    await AlphabetSortUsers();
                    break;
                
                case "31":
                    await GetUserPosts();
                    break;
                case "32":
                    await CreatePost();
                    break;
                case "33":
                    await UpdatePost();
                    break;
                case "34":
                    await DeletePost();
                    break;
                case "35":
                    await ShowTheLongestPost();
                    break;
                
                case "41":
                    await EndsWithComments();
                    break;
                                
                case "c":
                    Console.Clear();
                    PrintMenu();
                    break;
                case "q":
                    Console.WriteLine("Program is over");
                    return;
                default:
                    Console.WriteLine($"Unknown command: {choice}");
                    break;
            }
        }
    }
}