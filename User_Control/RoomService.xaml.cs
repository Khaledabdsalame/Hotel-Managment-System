using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
       
     public enum RoomStatus
      {
            Available,
            Occupied,
            Maintenance

      }
public partial class RoomService : UserControl
    {
        ObservableCollection<Room> RoomList = new();


        public class Room
        {
            public int RoomID { get; set; }
            public string RoomNumber { get; set; }
            public string RoomType { get; set; }
            public double PricePerNight { get; set; }
            public RoomStatus Status { get; set; }
        }

        public bool CanUpdateRoomStatus(int roomId, RoomStatus newStatus)
        {
            using var con = new SqliteConnection("Data Source=DatabaseHotel.db");
            con.Open();

            var cmd = con.CreateCommand();

            // تحقق إذا كانت الغرفة محجوزة حاليا
            cmd.CommandText = @"
        SELECT COUNT(*) 
        FROM Reservations
        WHERE RoomID = @roomId
          AND date(CheckInDate) <= date('now')
          AND date(CheckOutDate) > date('now');";

            cmd.Parameters.AddWithValue("@roomId", roomId);
            int activeReservations = Convert.ToInt32(cmd.ExecuteScalar());

            if (activeReservations > 0)
            {
                // لا يمكن وضع الغرفة Available أو Maintenance إذا هي مشغولة
                if (newStatus == RoomStatus.Available || newStatus == RoomStatus.Maintenance)
                    return false;
            }

            // تحقق إذا كانت الغرفة محجوزة مستقبلًا
            cmd.CommandText = @"
        SELECT COUNT(*) 
        FROM Reservations
        WHERE RoomID = @roomId
          AND date(CheckInDate) > date('now');";

            int futureReservations = Convert.ToInt32(cmd.ExecuteScalar());

            if (futureReservations > 0)
            {
                // لا يمكن وضع الغرفة Occupied إذا لديها حجز مستقبلي
                if (newStatus == RoomStatus.Occupied)
                    return false;
            }

            return true; // تحديث مسموح
        }

        public RoomService()
        {
            InitializeComponent();
            LoadRooms();
            Avalaibilty.ItemsSource = Enum.GetValues(typeof(RoomStatus));
            AvalaibiltySh.ItemsSource = Enum.GetValues(typeof(RoomStatus));
        }

        public bool IsRoomExists(string roomNumber)
        {
            using (var connection = new SqliteConnection("Data Source=DatabaseHotel.db"))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM Rooms WHERE RoomNumber = @num";
                cmd.Parameters.AddWithValue("@num", roomNumber);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
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
                @"
    SELECT * FROM Rooms
    WHERE RoomNumber LIKE @num
      AND RoomType LIKE @type
      AND Status LIKE @Stat
    ";
                cmd.Parameters.AddWithValue("@num", $"%{RoomNumbersh.Text}%");
                cmd.Parameters.AddWithValue("@type", $"%{RoomTypeSH.Text}%");
                var selectedStatus = AvalaibiltySh.SelectedItem is RoomStatus rs
     ? rs.ToString()
     : "";

                cmd.Parameters.AddWithValue("@Stat", $"%{selectedStatus}%");


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
                            Status = Enum.TryParse<RoomStatus>(
                             reader.GetString(4),
                             out var parsedStatus)
                             ? parsedStatus
                             : RoomStatus.Available
                        });
                    }
                }
            }
            RoomGrid.ItemsSource = RoomList;
        }

        public void AddRoom()
        {
            using (var connection = new SqliteConnection("Data Source=DatabaseHotel.db"))
            {
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText =
                @"INSERT INTO Rooms (RoomNumber, RoomType, PricePerNight,Status)
          VALUES (@num, @type, @price , @stat)";

                cmd.Parameters.AddWithValue("@num", Room_Number.Text);
                cmd.Parameters.AddWithValue("@type", Room_Type.Text);
                cmd.Parameters.AddWithValue("@price", Price.Text);
                cmd.Parameters.AddWithValue("@stat", ((RoomStatus)Avalaibilty.SelectedItem).ToString());

                cmd.ExecuteNonQuery();


            }
        }

        private void OnlyNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9]+$");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Room_Number.Text) || string.IsNullOrWhiteSpace(Room_Type.Text) || string.IsNullOrWhiteSpace(Price.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (IsRoomExists(Room_Number.Text))
            {
                MessageBox.Show("Room Is already exists.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                try
                {
                    AddRoom();
                    LoadRooms();
                    MessageBox.Show("Room added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Room_Number.Clear();
                    Room_Type.Clear();
                    Price.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error adding room: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }


            }
        }

        private void firstnamesh_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadRooms();

        }

        private void lastnamesh_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadRooms();
        }

        private void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RoomGrid.SelectedItem is Room selectedRoom)
                {
                    var result = MessageBox.Show($"Are you sure you want to delete Room {selectedRoom.RoomNumber}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        using (var connection = new SqliteConnection("Data Source=DatabaseHotel.db"))
                        {
                            connection.Open();
                            var cmd = connection.CreateCommand();
                            cmd.CommandText = "DELETE FROM Rooms WHERE RoomID = @id";
                            cmd.Parameters.AddWithValue("@id", selectedRoom.RoomID);
                            cmd.ExecuteNonQuery();
                        }
                        LoadRooms();
                        MessageBox.Show("Room deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a room to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }catch(Exception ex)
            {

                MessageBox.Show("You Cant Delete This Room", "Room Reserved Before", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
        }


        public void RoomGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
           if (e.EditAction == DataGridEditAction.Commit && e.Row.Item is Room editedRoom)
    {
        if (!CanUpdateRoomStatus(editedRoom.RoomID, editedRoom.Status))
        {
            MessageBox.Show("Cannot change room status due to existing reservations.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            LoadRooms(); // إعادة تحميل الغرف لإلغاء التغيير غير المسموح
            return;
        }

        // تحديث الحالة في قاعدة البيانات
        using var con = new SqliteConnection("Data Source=DatabaseHotel.db");
        con.Open();
        var cmd = con.CreateCommand();
        cmd.CommandText = @"UPDATE Rooms SET RoomNumber=@num, RoomType=@type, PricePerNight=@price, Status=@status WHERE RoomID=@id";
        cmd.Parameters.AddWithValue("@num", editedRoom.RoomNumber);
        cmd.Parameters.AddWithValue("@type", editedRoom.RoomType);
        cmd.Parameters.AddWithValue("@price", editedRoom.PricePerNight);
        cmd.Parameters.AddWithValue("@status", editedRoom.Status.ToString());
        cmd.Parameters.AddWithValue("@id", editedRoom.RoomID);
        cmd.ExecuteNonQuery();
    }
        }

        private void AvalaibiltySh_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRooms();
        }

       
    }
}


        
    
   
