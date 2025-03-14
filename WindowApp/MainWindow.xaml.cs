using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// WPF template

namespace WindowApp
{
    public partial class MainWindow : Window
    {
        const string UsersFilename = "users.json";
        private UserViewModel ViewModel = new UserViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
            LoadUsersFromFile();
            UserListBox.ItemsSource = ViewModel.Users;
        }

        public void WindowClosing(object sender, CancelEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(UsersFilename))
            {
                string json = JsonConvert.SerializeObject(ViewModel.Users);
                writer.WriteLine(json);
            }
        }

        public void LoadUsersFromFile()
        {
            if (!File.Exists(UsersFilename))
                return;

            using (StreamReader reader = new StreamReader(UsersFilename))
            {
                string json = reader.ReadToEnd();
                var users = JsonConvert.DeserializeObject<ObservableCollection<User>>(json);
                if (users != null)
                    ViewModel.Users = users;
            }
        }

        public void AddUser_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UserNameInput.Text) &&
                UserNameInput.Text != "Enter name" &&
                int.TryParse(UserAgeInput.Text, out int age) &&
                age > 0)
            {
                ViewModel.Users.Add(new User { Name = UserNameInput.Text, Age = age });
                UserNameInput.Text = "Enter name";
                UserAgeInput.Text = "Enter age";
                UserNameInput.Foreground = Brushes.Gray;
                UserAgeInput.Foreground = Brushes.Gray;
            }
        }

        public void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedUser != null)
            {
                ViewModel.Users.Remove(ViewModel.SelectedUser);
            }
        }

        private void AgeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox.Text == "Enter age")
            {
                textbox.Text = "";
                textbox.Foreground = Brushes.Black;
            }
        }

        private void AgeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textbox.Text))
            {
                textbox.Text = "Enter age";
                textbox.Foreground = Brushes.Gray;
            }
        }

        private void UserListBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedUser = (User)UserListBox.SelectedItem;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox.Text == "Enter name")
            {
                textbox.Text = "";
                textbox.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textbox.Text))
            {
                textbox.Text = "Enter name";
                textbox.Foreground = Brushes.Gray;
            }
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (textbox.Text == "Search..")
            {
                textbox.Text = "";
                textbox.Foreground = Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            if (string.IsNullOrWhiteSpace(textbox.Text))
            {
                textbox.Text = "Search..";
                textbox.Foreground = Brushes.Gray;
            }
        }

        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            string query = SearchBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(query) || query == "Search..")
                UserListBox.ItemsSource = ViewModel.Users;
            else
                UserListBox.ItemsSource = ViewModel.Users
                    .Where(u => u.Name.ToLower().Contains(query))
                    .ToList();
        }
    }
}
