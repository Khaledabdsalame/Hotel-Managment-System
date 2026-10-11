# Hotel Management System

A Windows desktop application for managing hotel operations: rooms, clients and reservations, with a live dashboard. Built with **C#**, **WPF** and **.NET 8**, using an embedded **SQLite** database that is created automatically on first launch, so there is nothing to install or configure.

![Platform](https://img.shields.io/badge/platform-Windows-0078D6?logo=windows&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![SQLite](https://img.shields.io/badge/database-SQLite-003B57?logo=sqlite&logoColor=white)

## ✨ Features

- **Login** with show/hide password and input validation
- **Dashboard** with live statistics: customers, rooms, reservations and revenue (DA)
- **Room management**: number, type, price per night and status
- **Client management**: name, phone, address and national ID
- **Reservations**: check-in and check-out dates, with total price per booking
- **Automatic room status updates** based on reservations
- **Safe log out** with confirmation
- **Zero-setup database**: `DatabaseHotel.db` and all tables are created on first run

## 🛠 Tech Stack

| Layer    | Technology                       |
| -------- | -------------------------------- |
| Language | C#                               |
| UI       | WPF (XAML) with user controls    |
| Platform | .NET 8 (`net8.0-windows`)        |
| Database | SQLite (`Microsoft.Data.Sqlite`) |

## 🚀 Getting Started

**Requirements:** Windows 10 or 11 and the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
git clone https://github.com/Khaledabdsalame/Hotel-Managment-System.git
cd Hotel-Managment-System
dotnet run
```

Or open `Hotel Managment System.sln` in Visual Studio 2022 and press **F5**.

### Default Login

| Username | Password   |
| -------- | ---------- |
| `admin`  | `admin123` |

> ⚠️ Change this password before using the app in a real environment.

## 📂 Project Structure

```
├── User_Control/         # Screens: rooms, clients, reservations, settings
├── MainWindow.xaml(.cs)  # Login window and database creation
├── Home.xaml(.cs)        # Main window, navigation and dashboard
├── App.xaml(.cs)         # Application entry point
└── SQLquery.sql          # Legacy SQL Server script (not used by the app)
```

## 🗄 Database

SQLite file `DatabaseHotel.db` with four tables:

| Table          | Main fields                                                  |
| -------------- | ------------------------------------------------------------ |
| `Users`        | username, name, job, email, phone, password                  |
| `Customers`    | first name, last name, phone, address, national ID           |
| `Rooms`        | room number, type, price per night, status                   |
| `Reservations` | customer, room, check-in, check-out, status, total price     |

Each reservation links one customer to one room.

## 🤝 Contributing

Contributions are welcome. Fork the repository, create a feature branch, commit your changes and open a Pull Request.

## 📄 License

No license has been specified yet. Consider adding one, such as the [MIT License](https://choosealicense.com/licenses/mit/).

## 👤 Author

**Khaled Abdsalame** · [GitHub](https://github.com/Khaledabdsalame)

## Preview

https://github.com/user-attachments/assets/0e81349f-db0e-4d4e-b2d5-30b642993920


