using System;
using System.Collections.Generic;
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
        private UserViewModel ViewModel = new UserViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
            UserListBox.ItemsSource = ViewModel.Users;
        }

        public void AddUser_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UserNameInput.Text) && UserNameInput.Text != "Enter name")
            {
                ViewModel.Users.Add(new User { Name = UserNameInput.Text });
                UserNameInput.Text = "Enter name";
                UserNameInput.Foreground = Brushes.Gray;
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
    }
}
