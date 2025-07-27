# ☕ Coffee Shop Management System
![.NET CI](https://github.com/DZ0Phong/PRN_Project_Coffee_Shop/actions/workflows/dotnet.yml/badge.svg)

A professional desktop application for managing a coffee shop chain, built with .NET 8 WPF and following modern software architecture principles.

---

### 📋 Table of Contents
- [📖 Overview](#-overview)
- [🚀 Quick Start](#-quick-start)
- [✨ Features](#-features)
- [🏗️ Project Structure](#️-project-structure)
- [🎯 Architecture Principles](#-architecture-principles)
- [🗂️ Project Index](#️-project-index)
- [🛣️ Roadmap](#️-roadmap)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)
- [🙏 Acknowledgements](#-acknowledgements)

---

### 📖 Overview
The **Coffee Shop Management System** is a comprehensive desktop application designed to streamline all operational aspects of a coffee shop, from point-of-sale to inventory management. Built using .NET 8 WPF, it provides a secure, role-based environment for staff and administrators.

-   **Platform**: .NET 8 WPF (`net8.0-windows`)
-   **Language**: C#
-   **Architecture**: Service-Oriented with MVVM principles
-   **Database**: Entity Framework Core with SQL Server (Database-First)
-   **Testing**: MSTest

---

### 🚀 Quick Start

#### Prerequisites
-   .NET 8 SDK or later
-   SQL Server (LocalDB or a full instance)
-   Visual Studio 2022 (recommended)

#### 🔧 Installation
1.  **Clone the repository**
    ```bash
    git clone https://github.com/DZ0Phong/PRN_Project_Coffee_Shop.git
    cd PRN_Project_Coffee_Shop
    ```
2.  **Restore dependencies**
    ```bash
    dotnet restore
    ```
3.  **Build the project**
    ```bash
    dotnet build
    ```
4.  **Run the application**
    -   Open the solution in Visual Studio and run, or use the command:
    ```bash
    dotnet run --project PRN_Project_Coffee_Shop
    ```

#### 🗃️ Database Setup
The application uses a SQL Server database.
1.  Ensure your SQL Server instance is running.
2.  Open `final_database.sql` and execute the script in SQL Server Management Studio (SSMS) or a similar tool. This will create the database, schema, and seed data.
3.  Verify the connection string in `PRN_Project_Coffee_Shop/Models/PrnProjectCoffeeShopContext.cs`:
    ```csharp
    // Server=localhost;Database=PRN_Project_Coffee_Shop;uid=sa;pwd=sa123456;...
    ```

#### 🧪 Running Tests
```bash
dotnet test
```

---

### ✨ Features

#### 👨‍💼 Employee Features
-   **Point of Sale (POS):** Create and manage customer orders efficiently.
-   **Customer Management:** Look up customers, register new ones, and view their loyalty status.
-   **Order Status Tracking:** View the status of all pending and completed orders.
-   **Inventory Checks:** Receive real-time warnings for low-stock or expired ingredients.

#### 👑 Admin Features
-   **All Employee Features**
-   **Menu Management:** Add, edit, and manage products, categories, and their recipes.
-   **Employee Management:** Manage staff accounts and roles.
-   **Financial Dashboard:** View sales reports and financial summaries.

#### 🛡️ System Features
-   **Role-Based Access:** UI and features are dynamically adjusted based on user role (Admin/Employee).
-   **Atomic Transactions:** Database operations for orders are atomic, ensuring data integrity.
-   **Automated Inventory Control:** Product availability is automatically updated based on ingredient stock.
-   **Loyalty Program:** Automatically awards points and generates promotions for customers.

---

### 🏗️ Project Structure
```
PRN_Project_Coffee_Shop/
├── 📁 .github/workflows/              # CI/CD pipeline
│   └── 📄 dotnet.yml
├── 📁 CoffeeShop.Tests/                # Unit Test Project
│   ├── 📄 OrderServiceTests.cs
│   └── 📄 CoffeeShop.Tests.csproj
├── 📁 PRN_Project_Coffee_Shop/          # Main WPF Application
│   ├── 📁 Models/                     # EF Core Entities (Database-First)
│   │   ├── 📄 PrnProjectCoffeeShopContext.cs
│   │   ├── 📄 User.cs
│   │   ├── 📄 Order.cs
│   │   └── 📄 ... (other entities)
│   ├── 📁 Services/                   # Business Logic Services
│   │   └── 📄 OrderService.cs
│   ├── 📁 Views/                      # WPF XAML Views
│   │   ├── 📁 Pages/                  # Management pages
│   │   └── 📄 LoginWindow.xaml
│   │   └── 📄 MainWindow.xaml
│   ├── 📄 App.xaml.cs
│   └── 📄 PRN_Project_Coffee_Shop.csproj
├── 📄 .gitignore
├── 📄 final_database.sql             # Database creation script
├── 📄 PRN_Project_Coffee_Shop.sln
└── 📄 README.md
```

---

### 🎯 Architecture Principles
This project follows a **Service-Oriented Architecture** that incorporates principles from **MVVM (Model-View-ViewModel)**.

-   **Models**: Plain C# objects (POCOs) representing database entities, generated using EF Core's database-first approach.
-   **Views**: XAML files with minimal code-behind. The UI is driven by data binding and commands.
-   **Services**: Encapsulate all business logic and data access (e.g., `OrderService`). This is the core of the application's logic.
-   **Code-Behind as Controller**: The `.xaml.cs` files act as controllers, instantiating services and handling navigation, but do not contain business logic.
-   **Dependency Injection (Conceptual)**: Services are instantiated in the view's code-behind. A full DI container could be added in the future for better decoupling.

---

### 🗂️ Project Index

#### 🔧 Core Components
| Component          | Description                                      | Location                               |
| ------------------ | ------------------------------------------------ | -------------------------------------- |
| **Authentication** | User login/role validation                       | `Views/LoginWindow.xaml.cs`            |
| **Order Engine**   | Core logic for orders, inventory, and loyalty    | `Services/OrderService.cs`             |
| **DB Context**     | EF Core configuration and entity mappings        | `Models/PrnProjectCoffeeShopContext.cs`|
| **Main Shell**     | Main window with role-based navigation           | `Views/MainWindow.xaml.cs`             |

#### 💾 Database Entities
| Entity       | Purpose                               | Key Relationships                  |
|--------------|---------------------------------------|------------------------------------|
| **User**     | Admin and Employee accounts           | → Orders, Employees                |
| **Order**    | Customer sales transactions           | → User, Customer, OrderDetails     |
| **Product**  | Menu items (coffee, cakes, etc.)      | → Category, OrderDetails, Ingredients|
| **Ingredient**| Raw materials for products            | → Products                         |
| **Customer** | Loyalty program members               | → Orders, Promotions               |

---

### 🛣️ Roadmap

-   [✅] **Phase 1: Core Functionality**
    -   [✅] User Authentication & Role-Based UI
    -   [✅] Order & Customer Management
    -   [✅] Inventory & Menu Management
    -   [✅] Basic Unit Tests & CI/CD Pipeline
-   [🚧] **Phase 2: Advanced Features**
    -   [ ] Real-time order status updates (e.g., using SignalR)
    -   [ ] Advanced financial reporting dashboard with charts
    -   [ ] UI/UX Polish and Refinements
-   [🔮] **Phase 3: Enterprise Features**
    -   [ ] API for a potential mobile/web client
    -   [ ] Multi-language support
    -   [ ] Caching for performance optimization

---

### 🤝 Contributing
Contributions are welcome! Please follow these steps:
1.  Fork the repository.
2.  Create a feature branch (`git checkout -b feature/YourAmazingFeature`).
3.  Commit your changes (`git commit -m 'Add some AmazingFeature'`).
4.  Push to the branch (`git push origin feature/YourAmazingFeature`).
5.  Open a Pull Request.

---

### 📄 License
This project is licensed under the MIT License - see the `LICENSE` file for details.

---

### 🙏 Acknowledgements
-   **Microsoft .NET Team** for the powerful .NET framework and documentation.
-   **WPF & MVVM Community** for design patterns and best practices.
-   **Your Friend** for the excellent README inspiration.

---
*Made with ❤️ for better coffee.*

*© 2025 Coffee Shop Management System. All rights reserved.*
