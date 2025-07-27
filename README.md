# Coffee Shop Chain Management System

This project is a comprehensive management system for a coffee shop chain, developed as a course assignment for PRN212. It is a C# Windows Presentation Foundation (WPF) application built on .NET 8, designed to streamline daily operations from order management to inventory control.

## 1. Overview

The system provides a role-based interface for internal staff (`Employee` and `Admin`) to manage all aspects of the coffee shop's operations. It connects to a Microsoft SQL Server database to handle all data persistence. Customers do not interact with this system directly; all actions are performed by employees on their behalf.

## 2. Core Features

-   **Secure Authentication:** A login system to authenticate users based on their roles.
-   **Role-Based Access Control:**
    -   **Employee:** Can manage orders, customers, and inventory.
    -   **Admin:** Has full access, including employee management, menu configuration, and financial reporting.
-   **Order Management:**
    -   Create new orders for dine-in or delivery.
    -   Add products and toppings to an order.
    -   Apply promotional discounts.
    -   Track order status (e.g., Pending, Preparing, Completed).
-   **Customer Management:**
    -   Register new customers and manage existing ones.
    -   A loyalty program that automatically awards points for purchases.
    -   Automatically generate discount promotions when a customer reaches a points threshold.
-   **Inventory Management:**
    -   Real-time ingredient availability checks before confirming an order.
    -   Automated warnings for low-stock and expiring ingredients.
    -   Products are automatically marked as "Out of Stock" if required ingredients are unavailable.
-   **Menu Management (Admin):**
    -   Add, edit, or remove products and categories from the menu.
    -   Define product recipes by linking them to ingredients.
-   **Financial Reporting (Admin):**
    -   View sales trends and financial summaries.

## 3. Technologies Used

-   **Backend:** C# 12, .NET 8
-   **UI Framework:** Windows Presentation Foundation (WPF)
-   **Database:** Microsoft SQL Server
-   **Data Access:** Entity Framework Core 8
-   **IDE:** Visual Studio 2022
-   **Version Control:** Git & GitHub
-   **Testing:** MSTest
-   **CI/CD:** GitHub Actions

## 4. Getting Started

### Prerequisites

-   Visual Studio 2022 (with .NET desktop development workload)
-   .NET 8 SDK
-   Microsoft SQL Server (2019 or later)

### Setup and Installation

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/DZ0Phong/PRN_Project_Coffee_Shop.git
    cd PRN_Project_Coffee_Shop
    ```

2.  **Set up the database:**
    -   Open SQL Server Management Studio (SSMS).
    -   Open the `final_database.sql` file from the project root.
    -   Execute the script. This will create the `PRN_Project_Coffee_Shop` database, all necessary tables, and insert initial seed data.

3.  **Configure the connection string:**
    -   Open the `PRN_Project_Coffee_Shop/Models/PrnProjectCoffeeShopContext.cs` file.
    -   Locate the `OnConfiguring` method.
    -   Update the connection string with your SQL Server credentials if they are different from the default (`uid=sa;pwd=sa123456`).
    ```csharp
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("server=localhost;database=PRN_Project_Coffee_Shop;uid=sa;pwd=sa123456;TrustServerCertificate=True;Pooling=False;");
        }
    }
    ```

4.  **Build and run the application:**
    -   Open the `PRN_Project_Coffee_Shop.sln` file in Visual Studio.
    -   Set `PRN_Project_Coffee_Shop` as the startup project.
    -   Press `F5` or click the "Start" button to build and run the application.

## 5. Login Credentials

You can use the following default accounts to log in:

-   **Admin:**
    -   **Email:** `admin@coffeeshop.com`
    -   **Password:** `123`
-   **Employee:**
    -   **Email:** `staff@coffeeshop.com`
    -   **Password:** `123`

*(Note: Passwords are not hashed in this version for simplicity. In a production environment, a secure hashing algorithm like BCrypt should be used.)*

## 6. CI/CD Pipeline

This project is configured with a GitHub Actions workflow to automate the build and test process. The pipeline is triggered on every push or pull request to the `main` branch, ensuring code quality and integration. You can view the workflow configuration in `.github/workflows/dotnet.yml`.
