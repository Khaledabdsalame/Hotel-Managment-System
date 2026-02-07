
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

using System.Text.RegularExpressions;

using System.Windows;
using System.Windows.Controls;

using System.Windows.Input;

using Microsoft.Data.Sqlite;

namespace Hotel_Managment_System.User_Control
{
    /// <summary>
    /// Interaction logic for UserControlSettings.xaml
    /// </summary>
    public partial class UserControlSettings : UserControl
    {
        ObservableCollection<User> usersList = new();

        public class User
        {
            public int ID { get; set; }
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Job { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }

            public string Password { get; set; }
        }

        bool UserOrEmailExists(string username, string email)
        {
            try
            {
                using var con = new SqliteConnection("Data Source=DatabaseHotel.db");
                con.Open();
                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                    @"SELECT COUNT(*) 
              FROM Users 
              WHERE username = @u OR email = @e";

                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@e", email);

                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Avoid crashing in designer or when DB is missing; log and treat as "not exists"
                Debug.WriteLine("UserOrEmailExists error: " + ex.Message);
                return false;
            }
        }

        private void LoadUsers()
        {
            usersList.Clear();

            try
            {
                using var con = new SqliteConnection("Data Source=DatabaseHotel.db");
                con.Open();

                var cmd = con.CreateCommand();
                cmd.CommandText = @"
    SELECT * FROM Users
    WHERE username LIKE @username
      AND first_name LIKE @first_name
      AND last_name LIKE @last_name
      AND email LIKE @email
      AND phone LIKE @phone
    ";

                cmd.Parameters.AddWithValue("@username", $"%{Usernamesh.Text}%");
                cmd.Parameters.AddWithValue("@first_name", $"%{firstnamesh.Text}%");
                cmd.Parameters.AddWithValue("@last_name", $"%{lastnamesh.Text}%");
                cmd.Parameters.AddWithValue("@email", $"%{emailsh.Text}%");
                cmd.Parameters.AddWithValue("@phone", $"%{phonesh.Text}%");

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    usersList.Add(new User
                    {
                        ID = reader.GetInt32(0),
                        Username = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        FirstName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                        LastName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        Job = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                        Email = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                        Phone = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                        Password = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                    });
                }

                UsersGrid.ItemsSource = usersList;
            }
            catch (Exception ex)
            {
                // Log and bail out — prevents design-time exceptions or crash if DB/table missing
                Debug.WriteLine("LoadUsers error: " + ex.Message);
            }
        }

        public UserControlSettings()
        {
            InitializeComponent();

            // Prevent runtime DB access while in Visual Studio designer (avoids XDG0003 during design)
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            // Ensure database and Users table exist before any queries
            try
            {
                EnsureDatabaseAndTables();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EnsureDatabaseAndTables error: " + ex.Message);
            }

            LoadUsers();
        }

        private void EnsureDatabaseAndTables()
        {
            using var con = new SqliteConnection("Data Source=DatabaseHotel.db");
            con.Open();

            using var cmd = con.CreateCommand();
            // Create Users table if it does not exist. Column names match usages throughout the control.
            cmd.CommandText = @"
    CREATE TABLE IF NOT EXISTS Users (
        user_id INTEGER PRIMARY KEY AUTOINCREMENT,
        username TEXT NOT NULL,
        first_name TEXT,
        last_name TEXT,
        job_description TEXT,
        email TEXT,
        phone TEXT,
        user_password TEXT
    );
    ";
            cmd.ExecuteNonQuery();
        }

        public void AddUsersInforamtion(string username, string password , string Phone , string email , string firstname , string lastname,string jobdes)
        {
            try
            {
                using (var connection = new SqliteConnection("Data Source=DatabaseHotel.db"))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"INSERT INTO Users (username, first_name, last_name, job_description, email, phone, user_password) VALUES (@u, @fn, @ln, @jb, @em, @ph,@ps)";
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@fn", firstname);
                    cmd.Parameters.AddWithValue("@ln", lastname);
                    cmd.Parameters.AddWithValue("@jb", jobdes);
                    cmd.Parameters.AddWithValue("@em", email);
                    cmd.Parameters.AddWithValue("@ph", Phone);
                    cmd.Parameters.AddWithValue("@ps", password);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("AddUsersInforamtion error: " + ex.Message);
                MessageBox.Show("Failed to add user: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Firstname.Text==string.Empty || lastname.Text == string.Empty || jobdes.Text == string.Empty || password.Text == string.Empty || phone.Text == string.Empty || email.Text == string.Empty )
            {
                MessageBox.Show("Please Fill All The Fields", "Required Field", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else if (!IsValidEmail(email.Text.Trim()))
            {
                MessageBox.Show("Please Enter A Valid Email Address", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (phone.Text.Trim().Length != 10)
            {
                MessageBox.Show("Invalid Phone Number.", "Invalid Phone Number", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (string.IsNullOrEmpty(username.Text))
            {
                MessageBox.Show("Username cannot be empty.", "Invalid Username", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            else if(UserOrEmailExists(username.Text.Trim(), email.Text.Trim().ToLower()))
            {
                MessageBox.Show("Username or Email already exists. Please choose a different one.", "Duplicate Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                AddUsersInforamtion(username.Text.Trim(), password.Text.Trim(), phone.Text.Trim(), email.Text.Trim().ToLower(), Firstname.Text.Trim(), lastname.Text.Trim(), jobdes.Text.Trim());
                MessageBox.Show("User Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                username.Text = "";
                password.Text = "";
                phone.Text = "";
                email.Text = "";
                Firstname.Text = "";
                lastname.Text = "";
                jobdes.Text = "";
            }

            LoadUsers();
        }

        

        
        private void Username_OnlyValid(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[a-zA-Z0-9]+$");
            e.Handled = e.Text.Contains(" ");
        }

        private void Phone_OnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]+$");
        }


        bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private void Username_BlockSpace(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        
        private void UsersGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit)
                return;

            User user = (User)e.Row.Item;
            try
            {
                using var con = new SqliteConnection("Data Source=DatabaseHotel.db");
                con.Open();

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                    @"UPDATE Users 
              SET username=@u, email=@e, phone=@p ,first_name=@fn, last_name=@ln, job_description=@jb ,user_password=@ps 
              WHERE user_id=@id";

                    cmd.Parameters.AddWithValue("@u", user.Username);
                    cmd.Parameters.AddWithValue("@e", user.Email);
                    cmd.Parameters.AddWithValue("@p", user.Phone);
                    cmd.Parameters.AddWithValue("@id", user.ID);
                    cmd.Parameters.AddWithValue("@fn", user.FirstName);
                    cmd.Parameters.AddWithValue("@ln", user.LastName);
                    cmd.Parameters.AddWithValue("@jb", user.Job);   
                    cmd.Parameters.AddWithValue("@ps", user.Password);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("UsersGrid_RowEditEnding error: " + ex.Message);
                MessageBox.Show("Failed to update user: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            User user = (User)((Button)sender).DataContext;

            if (MessageBox.Show($"Delete {user.Username}?",
                "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            try
            {
                using (var connection = new SqliteConnection("Data Source = DatabaseHotel.db"))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "DELETE FROM Users WHERE user_id=@id";
                    cmd.Parameters.AddWithValue("@id", user.ID);
                    cmd.ExecuteNonQuery();
                }

                usersList.Remove(user);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DeleteUser_Click error: " + ex.Message);
                MessageBox.Show("Failed to delete user: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Usernamesh_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadUsers();
        }

        private void emailsh_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadUsers();
                
        }

        private void phonesh_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadUsers();
        }

        private void firstnamesh_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadUsers();

        }

        private void lastnamesh_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadUsers();
        }
    }



    }
