
using System.Collections.ObjectModel;
using System.Diagnostics;

using System.Windows;
using System.Windows.Controls;

using Microsoft.Data.Sqlite;


namespace Hotel_Managment_System.User_Control
{
    /// <summary>
    /// Interaction logic for ReservationSystem.xaml
    /// </summary>
    public partial class ReservationSystem : UserControl
    {
        
        ObservableCollection<Client> ClientList = new();
        ObservableCollection<Room> RoomList = new();
        ObservableCollection<Reservation> ReservationList = new();

        public ReservationSystem()
        {
            InitializeComponent();
            Datapicker.SelectedDate = DateTime.Today.AddDays(1);
            Datapicker.DisplayDateStart = DateTime.Today;

            UpdateRoomStatus();
            LoadClients();
            LoadRooms();
            LoadReservation();
        }


        
        

            
        

        public void UpdateRoomStatus()
        {
            using var con = new SqliteConnection("Data Source=DatabaseHotel.db");
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = @"
        UPDATE Rooms
SET Status = CASE
    WHEN Status IN ('Maintenance') THEN Status
    WHEN EXISTS (
        SELECT 1 FROM Reservations r
        WHERE r.RoomID = Rooms.RoomID
          AND date(@today) BETWEEN date(r.CheckInDate) AND date(r.CheckOutDate)
    )
    THEN 'Occupied'
    ELSE 'Available'
END;
    ";

            cmd.Parameters.AddWithValue("@today", DateTime.Now.ToString("yyyy-MM-dd"));
            cmd.ExecuteNonQuery();
        }

        public class Room
        {
            public int RoomID { get; set; }
            public string RoomNumber { get; set; }
            public string RoomType { get; set; }
            public double PricePerNight { get; set; }
            public string Status { get; set; }
        }
        public class Client
        {
            public int ID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }
            public string Adress { get; set; }
            public string NationalId { get; set; }

        }

        public class Reservation
        {
            public int RESID { get; set; }

            public int RID { get; set; }
            public string FirstNameR { get; set; }
            public string LastNameR { get; set; }
            public string CheckINR { get; set; }
            public string CheckoutR { get; set; }
            public string NationalIdR { get; set; }
            public string phoneR { get; set; }
            public string statusR { get; set; }
            public string totalpriceR { get; set; }

        }

        public static void AddReservation(
     int customerId,
     int roomId,
     DateTime checkIn,
     DateTime checkOut,
     string firstName,
     string lastName,
     double pricePerNight,
     string national_id,
     string  phone)
        {
            if (checkOut <= checkIn)
                throw new Exception("Check-out date must be after check-in date.");

            int nights = (checkOut - checkIn).Days;
            double total = nights * pricePerNight;

            using var con = new SqliteConnection("Data Source=DatabaseHotel.db");
            con.Open();

            using var transaction = con.BeginTransaction();

            try
            {
                var cmd = con.CreateCommand();
                cmd.Transaction = transaction;

                cmd.CommandText = @"
        INSERT INTO Reservations
        (customer_id, RoomID, FirstName, LastName, CheckInDate, CheckOutDate,national_id,phone,TotalPrice)
        VALUES (@cId, @rId, @fn, @ln, @in, @out,@nat,@pn, @total);

        UPDATE Rooms 
        SET Status = 'Occupied'
        WHERE RoomID = @rId;";

                cmd.Parameters.AddWithValue("@cId", customerId);
                cmd.Parameters.AddWithValue("@rId", roomId);
                cmd.Parameters.AddWithValue("@fn", firstName);
                cmd.Parameters.AddWithValue("@ln", lastName);
                cmd.Parameters.AddWithValue("@in", checkIn.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@out", checkOut.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@nat", national_id);
                cmd.Parameters.AddWithValue("@pn", phone);
                cmd.Parameters.AddWithValue("@total", total);

                cmd.ExecuteNonQuery();

                transaction.Commit();
                
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            
        }

        public void LoadRooms()
        {
            RoomList.Clear();
            using (var connection = new SqliteConnection("Data Source=DatabaseHotel.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText =
            cmd.CommandText = @"
        SELECT * FROM Rooms
        WHERE (RoomNumber LIKE @num OR RoomType LIKE @type)
          AND Status = 'Available'
        ";
                cmd.Parameters.AddWithValue("@num", $"%{Romres.Text}%");
                cmd.Parameters.AddWithValue("@type", $"%{Romres.Text}%");
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RoomList.Add(new Room
                        {
                            RoomID = reader.GetInt32(0),
                            RoomNumber = reader.GetString(1),
                            RoomType = reader.GetString(2),
                            PricePerNight = reader.GetDouble(3),
                            Status = reader.GetString(4)
                        });
                    }
                }
            }
            RoomGridres.ItemsSource = RoomList;
        }

        public void LoadReservation()
        {

           ReservationList.Clear();

            try
            {
                using var con = new SqliteConnection("Data Source=DatabaseHotel.db");
                con.Open();

                var cmd = con.CreateCommand();
                cmd.CommandText = @"
    SELECT * FROM Reservations
    WHERE FirstName LIKE @fn
      OR LastName LIKE @ln
      OR Phone LIKE @pn
      OR National_id LIKE @natid
    ";

                cmd.Parameters.AddWithValue("@fn", $"%{shfirstname.Text}%");
                cmd.Parameters.AddWithValue("@ln", $"%{shlastname.Text}%");
                cmd.Parameters.AddWithValue("@pn", $"%{phonesh.Text}%");
                cmd.Parameters.AddWithValue("@natid", $"%{nationalid.Text}%");

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                   ReservationList.Add(new Reservation
                    {
                        RESID = reader.GetInt32(0),
                       RID = reader.GetInt32(2),
                       FirstNameR = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        LastNameR = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                        CheckINR = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                        CheckoutR = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                        NationalIdR = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                       phoneR = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                       statusR = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                       totalpriceR = reader.IsDBNull(10) ? string.Empty : reader.GetString(10)

                   });

                }

               ReservationGrid.ItemsSource = ReservationList;
            }
            catch (Exception ex)
            {
                // Log and bail out — prevents design-time exceptions or crash if DB/table missing
                Debug.WriteLine("LoadUsers error: " + ex.Message);
            }
        }



        

        public void LoadClients()
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
      OR last_name LIKE @ln
      OR phone LIKE @pn
      OR national_id LIKE @natid
    ";

                cmd.Parameters.AddWithValue("@fn", $"%{Clientres.Text}%");
                cmd.Parameters.AddWithValue("@ln", $"%{Clientres.Text}%");
                cmd.Parameters.AddWithValue("@pn", $"%{Clientres.Text}%");

                cmd.Parameters.AddWithValue("@natid", $"%{Clientres.Text}%");

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

                ClientsGridres.ItemsSource = ClientList;
            }
            catch (Exception ex)
            {
                // Log and bail out — prevents design-time exceptions or crash if DB/table missing
                Debug.WriteLine("LoadUsers error: " + ex.Message);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (ClientsGridres.SelectedItem is not Client selectedClient)
            {
                MessageBox.Show("Please select a client.");
                return;
            }

            if (RoomGridres.SelectedItem is not Room selectedRoom)
            {
                MessageBox.Show("Please select a room.");
                return;
            }

            if (!Datapicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select check-out date.");
                return;
            }

            try
            {
                AddReservation(
                    selectedClient.ID,
                    selectedRoom.RoomID,
                    DateTime.Today,
                    Datapicker.SelectedDate.Value,
                    selectedClient.FirstName,
                    selectedClient.LastName,
                    selectedRoom.PricePerNight,
                    selectedClient.NationalId,
                    selectedClient.Phone);

                MessageBox.Show("Reservation added successfully.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                LoadRooms();
                LoadReservation();
                UpdateRoomStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Clientres_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadClients();
        }

        private void Romres_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadRooms();

        }

        private void Deletres(object sender, RoutedEventArgs e)
        {

            try
            {
                if (ReservationGrid.SelectedItem is Reservation selectedRes)
                {
                    var result = MessageBox.Show($"Are you sure you want to delete This Reservation ?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (var connection = new SqliteConnection("Data Source=DatabaseHotel.db"))
                        {
                            connection.Open();
                            var cmd = connection.CreateCommand();
                            cmd.CommandText = @"DELETE FROM Reservations WHERE ReservationID = @id;

                             UPDATE Rooms 
                             SET Status = 'Available'
                             WHERE RoomID = @rId;";
                            cmd.Parameters.AddWithValue("@id", selectedRes.RESID);
                            cmd.Parameters.AddWithValue("@rId", selectedRes.RID);
                            cmd.ExecuteNonQuery();
                        }
                        LoadReservation();
                        MessageBox.Show("Resrvation deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a Reservation to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("You Cant Delete This Room", "Room Reserved Before", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void shfirstname_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadReservation();
        }

        private void shlastname_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadReservation();
        }

        private void phonesh_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadReservation();
        }

        private void nationalid_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadReservation();
        }
    }
}
