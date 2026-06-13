# Gym Management System

A comprehensive web application for managing gym operations including members, trainers, packages, classes, and payments.

## Features

### Admin Functions
- Account Management (Users, Members, Trainers)
- Member Management
- Trainer Management
- Package/Plan Management
- Class Schedule Management
- Payment Management
- Reports & Statistics

### Cashier Functions
- Collect Payments
- Register Package for Members
- Renew Packages
- Print Invoices
- View Payment History

### Member Functions
- Login/Register Account
- View Class Schedule
- View Member List
- Check-in to Classes
- View Available Packages
- Register for Packages
- View Training Schedule
- Book Training Sessions

### Trainer Functions
- Mark Member Attendance
- View Assigned Classes
- Manage Training Sessions

## Technology Stack

- **Backend:** ASP.NET Core MVC
- **Database:** SQL Server
- **Frontend:** HTML, CSS, Bootstrap, Vanilla JavaScript
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Core Identity

## Project Structure

```
GymManager/
├── Models/
│   ├── User.cs
│   ├── Member.cs
│   ├── Trainer.cs
│   ├── Package.cs
│   ├── Class.cs
│   ├── Payment.cs
│   └── Attendance.cs
├── Controllers/
│   ├── AdminController.cs
│   ├── MemberController.cs
│   ├── CashierController.cs
│   ├── TrainerController.cs
│   └── AuthController.cs
├── Views/
│   ├── Admin/
│   ├── Member/
│   ├── Cashier/
│   ├── Trainer/
│   └── Shared/
├── Services/
├── Data/
│   └── GymDbContext.cs
├── appsettings.json
├── Program.cs
└── GymManager.csproj
```

## Setup Instructions

### Prerequisites
- Visual Studio 2022 or later
- .NET 6.0 or later
- SQL Server 2019 or later

### Installation

1. Clone the repository
```bash
git clone https://github.com/Nguyenquoctrung1111/GymManager.git
cd GymManager
```

2. Update Connection String in `appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=GymManager;Trusted_Connection=true;"
}
```

3. Create Database
```bash
dotnet ef database update
```

4. Run the Application
```bash
dotnet run
```

5. Open in browser: `https://localhost:5001`

## Default Accounts

- **Admin:** admin@gym.com / Admin@123
- **Cashier:** cashier@gym.com / Cashier@123
- **Member:** member@gym.com / Member@123
- **Trainer:** trainer@gym.com / Trainer@123

## License

MIT License

## Author

Nguyenquoctrung1111
