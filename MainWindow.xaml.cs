
using System.IO;
using System.Windows;

using System.Windows.Input;

using Microsoft.Data.Sqlite;
namespace Hotel_Managment_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        bool isNewDb = !File.Exists("DatabaseHotel.db");

        private static void create_databse()
        {
            string dbFile = "DatabaseHotel.db";
            string connectionString = $"Data Source={dbFile}";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();


                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText =
                @"CREATE TABLE IF NOT EXISTS Users (
    user_id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT NOT NULL,
    first_name TEXT,
    last_name TEXT,
    job_description TEXT,
    email TEXT UNIQUE,
    phone TEXT,
    user_password TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS Customers (
    customer_id     INTEGER PRIMARY KEY AUTOINCREMENT,
    first_name      TEXT NOT NULL,
    last_name       TEXT NOT NULL,
    phone           TEXT UNIQUE,
    address         TEXT,
    national_id     TEXT
);
CREATE TABLE IF NOT EXISTS Rooms (
    RoomID INTEGER PRIMARY KEY AUTOINCREMENT,
    RoomNumber TEXT NOT NULL UNIQUE,
    RoomType TEXT,
    PricePerNight REAL NOT NULL,
    Status TEXT DEFAULT 'Available'
);
CREATE TABLE IF NOT EXISTS Reservations (
    ReservationID INTEGER PRIMARY KEY AUTOINCREMENT,
    customer_id INTEGER NOT NULL,
    RoomID INTEGER NOT NULL,
    FirstName TEXT NOT NULL,
    LastName TEXT NOT NULL,
    CheckInDate TEXT NOT NULL,
    CheckOutDate TEXT NOT NULL,
    National_id     TEXT,
    Phone           TEXT,
    Status TEXT DEFAULT 'Booked',
    TotalPrice REAL,
    FOREIGN KEY (customer_id) REFERENCES Customers(customer_id),
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID)
);

INSERT INTO Users (username, first_name, last_name, job_description, email, phone, user_password) 
VALUES ('admin', 'John', 'Doe', 'System Administrator', 'admin@example.com', '+1234567890', 'admin123');";
                createTableCmd.ExecuteNonQuery();

            }

        }
        public MainWindow()
        {
            InitializeComponent();
            if (isNewDb)
            {
                create_databse();
            }


        }

        public bool IsValidUsernamePassword(string username, string password)
        {
            using (var connection = new SqliteConnection("Data Source = DatabaseHotel.db"))
            {
                connection.Open();

                

                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE username=@username AND user_password=@password";
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count > 0)
                    return true;
                else
                    return false;
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            
                passwordf.Visibility = Visibility.Hidden;
                passwordf_Show.Visibility = Visibility.Visible;
                passwordf_Show.Text = passwordf.Password;


        }

        private void ToggleEye_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordf_Show.Visibility = Visibility.Hidden;
            passwordf.Visibility = Visibility.Visible;
            passwordf.Password = passwordf_Show.Text;


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool check =IsValidUsernamePassword(UsrnameFiled.Text.Trim(), (passwordf.Visibility == Visibility.Visible) ? passwordf.Password.Trim() : passwordf_Show.Text.Trim() );
            if (UsrnameFiled.Text.Trim()== string.Empty || (passwordf.Visibility == Visibility.Visible) ? passwordf.Password.Trim() == string.Empty : passwordf_Show.Text.Trim()==string.Empty)
            {
                MessageBox.Show("Please Fill All The Fields","Required Field",MessageBoxButton.OK,MessageBoxImage.Information);
            }
            else {           
                if (check)
                {
                    Home home = new Home(UsrnameFiled.Text.Trim());
                    home.Show();
                    this.Close();

                }
                else
                {
                    MessageBox.Show("Invalid Username Or Password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
    }
}