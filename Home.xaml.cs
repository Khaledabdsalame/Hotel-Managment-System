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
using System.Windows.Shapes;
using System.Windows.Threading;

using Microsoft.Data.Sqlite;


namespace Hotel_Managment_System
{
    

    public partial class Home : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        
        public Home(String Username)
        {
            InitializeComponent();
            LoadDashboardStats();
            ReservationSystem.UpdateRoomStatus();
            UserName.Content = Username;
            
            
            timer.Tick += Timer_Tick;
            timer.Start();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Date.Text = DateTime.Now.ToString("dd/MM/yyyy  HH:mm:ss");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
    "Are you sure you want to Log Out?",
    "LogOut",
    MessageBoxButton.OKCancel,
    MessageBoxImage.Question
);
            if (result == MessageBoxResult.OK)
            {
                UserName.Content = "";
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
               
            }
            
        }

        public void LoadDashboardStats()
        {
            using var con = new SqliteConnection("Data Source=DatabaseHotel.db");
            con.Open();

            var cmd = con.CreateCommand();
            cmd.CommandText = @"
        SELECT 
            (SELECT COUNT(*) FROM Customers) AS TotalCustomers,
            (SELECT COUNT(*) FROM Rooms) AS TotalRooms,
            (SELECT COUNT(*) FROM Reservations) AS TotalReservations,
            (SELECT IFNULL(SUM(TotalPrice),0) FROM Reservations) AS TotalRevenue
    ";

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                TotalCustomersText.Content = reader.GetInt32(0).ToString();
                TotalRoomsText.Content = reader.GetInt32(1).ToString();
                TotalReservationsText.Content = reader.GetInt32(2).ToString();
                TotalRevenueText.Content = reader.GetDouble(3).ToString("0.00") + " DA";
            }
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Visible;
            ClientsStteings.Visibility = Visibility.Hidden;
            RoomManager.Visibility = Visibility.Hidden;
            ReservationSystem.Visibility = Visibility.Hidden;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ClientsStteings.Visibility = Visibility.Visible;
            Dashbord.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Hidden;
            RoomManager.Visibility = Visibility.Hidden;
            ReservationSystem.Visibility = Visibility.Hidden;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            RoomManager.Visibility = Visibility.Visible;
            RoomManager.LoadRooms();
            ClientsStteings.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Hidden;
            ReservationSystem.Visibility = Visibility.Hidden;
            Dashbord.Visibility = Visibility.Hidden;

        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        { 
            ReservationSystem.UpdateRoomStatus();
            ReservationSystem.Visibility = Visibility.Visible;
            RoomManager.Visibility= Visibility.Hidden;
            ClientsStteings.Visibility  = Visibility.Hidden;
            Settings.Visibility= Visibility.Hidden;
            Dashbord.Visibility = Visibility.Hidden;
            ReservationSystem.LoadRooms();
            ReservationSystem.LoadClients();
            ReservationSystem.LoadReservation();
           
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            LoadDashboardStats();
            Dashbord.Visibility = Visibility.Visible;
            ReservationSystem.Visibility = Visibility.Hidden;
            RoomManager.Visibility = Visibility.Hidden;
            ClientsStteings.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Hidden;
        }
    }
}
