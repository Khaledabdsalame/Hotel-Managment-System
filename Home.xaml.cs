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


namespace Hotel_Managment_System
{
    

    public partial class Home : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        
        public Home(String Username)
        {
            InitializeComponent();
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

      

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Visible;
        }

      
    }
}
