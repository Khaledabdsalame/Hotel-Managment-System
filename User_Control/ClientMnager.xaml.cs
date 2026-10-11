using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Microsoft.Data.Sqlite;
using static Hotel_Managment_System.User_Control.UserControlSettings;

namespace Hotel_Managment_System.User_Control
{
    /// <summary>
    /// Interaction logic for ClientMnager.xaml
    /// </summary>
    public partial class ClientMnager : UserControl
    {

        ObservableCollection<Client> ClientList = new();

        public class Client
        {
            public int ID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public string Adress { get; set; }
            public string NationalId { get; set; }
            
        }

        private void LoadClients()
        {
            ClientList.Clear();

            try
            {
                using var con = new SqliteConnection("Data Source=DatabaseHotel.db");
                con.Open();

                var cmd = con.CreateCommand();
                cmd.CommandText = @"
    SELECT * FROM Customers
    WHERE first_name LIKE @fn
      AND last_name LIKE @ln
      AND phone LIKE @pn
      AND national_id LIKE @natid
    ";

                cmd.Parameters.AddWithValue("@fn", $"%{Firstname.Text}%");
                cmd.Parameters.AddWithValue("@ln", $"%{lastname.Text}%");
                cmd.Parameters.AddWithValue("@pn", $"%{Phonesh.Text}%");
               
                cmd.Parameters.AddWithValue("@natid", $"%{nationalid.Text}%");

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ClientList.Add(new Client
                    {
                        ID = reader.GetInt32(0),
                        FirstName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        LastName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                        Phone = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        Adress = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                        NationalId = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                    });
                    
                }

                ClientsGrid.ItemsSource =ClientList;
            }
            catch (Exception ex)
            {
                // Log and bail out — prevents design-time exceptions or crash if DB/table missing
                Debug.WriteLine("LoadUsers error: " + ex.Message);
            }
        }

        public ClientMnager()
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

           LoadClients();

        }

        private void EnsureDatabaseAndTables()
        {
            using var con = new SqliteConnection("Data Source=DatabaseHotel.db");
            con.Open();

            using var cmd = con.CreateCommand();
            // Create Users table if it does not exist. Column names match usages throughout the control.
            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Reservations (
    ReservationID INTEGER PRIMARY KEY AUTOINCREMENT,
    customer_id INTEGER NOT NULL,
    RoomID INTEGER NOT NULL,
    CheckInDate TEXT NOT NULL,
    CheckOutDate TEXT NOT NULL,
    Status TEXT DEFAULT 'Booked',
    TotalPrice REAL,
    FOREIGN KEY (customer_id) REFERENCES Customers(customer_id),
    FOREIGN KEY (RoomID) REFERENCES Rooms(RoomID)
);
    ";
            cmd.ExecuteNonQuery();
        }



        public void AddUsersInforamtions()
        {
            try
            {
                using (var connection = new SqliteConnection("Data Source=DatabaseHotel.db"))
                {
                    connection.Open();

                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"INSERT INTO Customers (first_name,last_name,phone, address, national_id) VALUES (@fn, @ln, @pn, @add, @ni)";
                    cmd.Parameters.AddWithValue("@fn", firstnamecl.Text);
                    cmd.Parameters.AddWithValue("@ln", lastnamecl.Text);
                    cmd.Parameters.AddWithValue("@pn", phonecl.Text);
                    cmd.Parameters.AddWithValue("@add", adress.Text);
                    cmd.Parameters.AddWithValue("@ni", National_ID.Text);
                    

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
            if (firstnamecl.Text == string.Empty || lastnamecl.Text == string.Empty )
            {
                MessageBox.Show("Please Fill All The Fields", "Required Field", MessageBoxButton.OK, MessageBoxImage.Information);

            }
           
            else if (phonecl.Text.Trim().Length != 10)
            {
                MessageBox.Show("Invalid Phone Number.", "Invalid Phone Number", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
          

         
            else
            {
                AddUsersInforamtions();
                MessageBox.Show("Client Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                firstnamecl.Text = "";
                lastnamecl.Text = "";
                adress.Text = "";
               phonecl.Text = "";
                National_ID.Text = "";
               
            }

            LoadClients();
        }

        private void ClientGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit)
                return;

            Client Clinet = (Client)e.Row.Item;
            try
            {
                using var con = new SqliteConnection("Data Source=DatabaseHotel.db");
                con.Open();

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                    @"UPDATE Customers 
              SET first_name=@fn, last_name=@ln, phone=@pn ,address=@addr, national_id=@ni 
              WHERE customer_id=@id";

                    cmd.Parameters.AddWithValue("@fn", Clinet.FirstName);
                    cmd.Parameters.AddWithValue("@LN", Clinet.LastName);
                    cmd.Parameters.AddWithValue("@pn", Clinet.Phone);
                    cmd.Parameters.AddWithValue("@addr", Clinet.Adress);
                    cmd.Parameters.AddWithValue("@ni", Clinet.NationalId);
                    cmd.Parameters.AddWithValue("@id", Clinet.ID);
                    

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("UsersGrid_RowEditEnding error: " + ex.Message);
                MessageBox.Show("Failed to update user: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Phone_OnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]+$");
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            Client Client = (Client)((Button)sender).DataContext;

            if (MessageBox.Show($"Delete {Client.FirstName}?",
                "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            try
            {
                using (var connection = new SqliteConnection("Data Source = DatabaseHotel.db"))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "DELETE FROM Customers WHERE customer_id=@id";
                    cmd.Parameters.AddWithValue("@id", Client.ID);
                    cmd.ExecuteNonQuery();
                }

                ClientList.Remove(Client);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DeleteUser_Click error: " + ex.Message);
                MessageBox.Show("Failed to delete user: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Firstname_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadClients();

        }

        private void lastname_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadClients();
        }

        private void nationalid_TextChanged(object sender, TextChangedEventArgs e)
        {
                 LoadClients();
        }

        private void Phonesh_TextChanged(object sender, TextChangedEventArgs e)
        {
                  LoadClients();
        }

       
    }



}
