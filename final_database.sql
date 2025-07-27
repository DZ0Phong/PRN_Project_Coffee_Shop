-- Drop the database if it exists to ensure a clean start
IF DB_ID('PRN_Project_Coffee_Shop') IS NOT NULL
BEGIN
    ALTER DATABASE PRN_Project_Coffee_Shop SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PRN_Project_Coffee_Shop;
END
GO

-- Create Database
CREATE DATABASE PRN_Project_Coffee_Shop;
GO

USE PRN_Project_Coffee_Shop;
GO

-- Table for Users (Admin and Staff)
-- RoleID: 1 = Admin, 2 = Employee
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    RoleID INT NOT NULL,
    IsLocked BIT NOT NULL DEFAULT 0
);
GO

-- Table for Product Categories (Coffee, Tea, Cake)
CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- Table for Ingredients
CREATE TABLE Ingredients (
    IngredientID INT PRIMARY KEY IDENTITY(1,1),
    IngredientName NVARCHAR(100) NOT NULL UNIQUE,
    QuantityInStock DECIMAL(10, 2) NOT NULL,
    Unit NVARCHAR(20) NOT NULL,
    ExpiryDate DATE,
    WarningThreshold DECIMAL(10, 2) NOT NULL
);
GO

-- Table for Products (Menu Items)
CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(100) NOT NULL,
    CategoryID INT NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    IsOutOfStock BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID)
);
GO

-- Junction table for Product and Ingredients (Many-to-Many)
CREATE TABLE ProductIngredients (
    ProductID INT NOT NULL,
    IngredientID INT NOT NULL,
    QuantityRequired DECIMAL(10, 2) NOT NULL,
    PRIMARY KEY (ProductID, IngredientID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    FOREIGN KEY (IngredientID) REFERENCES Ingredients(IngredientID)
);
GO

-- Table for Customers (Members)
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(100) NOT NULL UNIQUE,
    CustomerName NVARCHAR(100),
    Points INT DEFAULT 0
);
GO

-- Table for Orders
CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    CustomerID INT, -- Nullable for non-member customers
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(10, 2) NOT NULL,
    Status NVARCHAR(50) NOT NULL, -- e.g., Pending, Preparing, Completed
    TableNumber NVARCHAR(10), -- For dine-in orders
    IsDelivery BIT NOT NULL,
    DeliveryNotes NVARCHAR(MAX), -- For delivery orders
    PromotionCode NVARCHAR(50),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID)
);
GO

-- Table for Order Details (Items in an order)
CREATE TABLE OrderDetails (
    OrderDetailID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    SugarPercent INT NOT NULL DEFAULT 100,
    IcePercent INT NOT NULL DEFAULT 100,
    Notes NVARCHAR(MAX), -- Customer notes for the item
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);
GO

-- Junction table for OrderDetail and Toppings (Many-to-Many)
CREATE TABLE OrderDetailToppings (
    OrderDetailID INT NOT NULL,
    ToppingID INT NOT NULL, -- This is a ProductID from the Products table
    PRIMARY KEY (OrderDetailID, ToppingID),
    FOREIGN KEY (OrderDetailID) REFERENCES OrderDetails(OrderDetailID),
    FOREIGN KEY (ToppingID) REFERENCES Products(ProductID)
);
GO

-- Table for Promotions
CREATE TABLE Promotions (
    PromotionID INT PRIMARY KEY IDENTITY(1,1),
    PromotionCode NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(MAX),
    DiscountPercentage DECIMAL(5, 2) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CustomerId INT,
    IsUsed BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerID)
);
GO

-- Table for Employees
CREATE TABLE Employees (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Position NVARCHAR(50) NOT NULL, -- e.g., Barista, Cashier, Cleaner
    Salary DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
GO

-- === INSERT INITIAL DATA ===

-- Users
INSERT INTO Users (Email, PasswordHash, RoleID) VALUES 
(N'admin@coffeeshop.com', N'123', 1), -- In a real app, HASH this password
(N'staff@coffeeshop.com', N'123', 2);
GO

-- Categories
INSERT INTO Categories (CategoryName) VALUES
(N'Cà Phê'),
(N'Trà'),
(N'Bánh'),
(N'Topping');
GO

-- Ingredients
INSERT INTO Ingredients (IngredientName, QuantityInStock, Unit, WarningThreshold) VALUES
(N'Hồng trà', 1000, N'g', 100),
(N'Bột matcha', 500, N'g', 50),
(N'Trà nhài', 1000, N'g', 100),
(N'Trà ô long', 1000, N'g', 100),
(N'Sữa tươi', 20000, N'ml', 2000),
(N'Kem béo', 5000, N'ml', 500),
(N'Đường', 10000, N'g', 1000),
(N'Hạt cà phê Robusta', 5000, N'g', 500),
(N'Sữa đặc', 10000, N'g', 1000),
(N'Nước cốt dừa', 2000, N'ml', 200),
(N'Trân châu đen', 2000, N'g', 200),
(N'Trân châu trắng', 2000, N'g', 200),
(N'Kem mặn', 1000, N'g', 100),
(N'Kem phô mai', 1000, N'g', 100),
(N'Bột mì', 5000, N'g', 500),
(N'Trứng', 100, N'quả', 10),
(N'Bơ', 2000, N'g', 200),
(N'Táo', 50, N'quả', 5);
GO

-- Products
DECLARE @catCaPhe INT = (SELECT CategoryID FROM Categories WHERE CategoryName = N'Cà Phê');
DECLARE @catTra INT = (SELECT CategoryID FROM Categories WHERE CategoryName = N'Trà');
DECLARE @catBanh INT = (SELECT CategoryID FROM Categories WHERE CategoryName = N'Bánh');
DECLARE @catTopping INT = (SELECT CategoryID FROM Categories WHERE CategoryName = N'Topping');

INSERT INTO Products (ProductName, CategoryID, Price) VALUES
-- Cà Phê
(N'Đen đá', @catCaPhe, 20000),
(N'Nâu đá', @catCaPhe, 25000),
(N'Bạc xỉu', @catCaPhe, 30000),
(N'Cà phê cốt dừa', @catCaPhe, 35000),
-- Trà
(N'Hồng trà sữa', @catTra, 35000),
(N'Matcha Latte', @catTra, 45000),
(N'Trà sữa Nhài', @catTra, 35000),
(N'Ô long sữa', @catTra, 40000),
-- Bánh
(N'Bánh táo', @catBanh, 30000),
(N'Bánh ngàn lớp', @catBanh, 35000),
(N'Bánh bông lan trứng muối', @catBanh, 40000),
-- Topping
(N'Trân châu đen', @catTopping, 5000),
(N'Trân châu trắng', @catTopping, 5000),
(N'Kem mặn', @catTopping, 7000),
(N'Kem phô mai', @catTopping, 7000);
GO

-- Product Ingredients (Recipes)
-- Declare ingredient IDs
DECLARE @ingHongTra INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Hồng trà');
DECLARE @ingMatcha INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Bột matcha');
DECLARE @ingTraNhai INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Trà nhài');
DECLARE @ingOLong INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Trà ô long');
DECLARE @ingSuaTuoi INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Sữa tươi');
DECLARE @ingKemBeo INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Kem béo');
DECLARE @ingDuong INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Đường');
DECLARE @ingCaPhe INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Hạt cà phê Robusta');
DECLARE @ingSuaDac INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Sữa đặc');
DECLARE @ingCotDua INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Nước cốt dừa');
DECLARE @ingBotMi INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Bột mì');
DECLARE @ingTrung INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Trứng');
DECLARE @ingBo INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Bơ');
DECLARE @ingTao INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Táo');

-- Declare product IDs
DECLARE @prodDenDa INT = (SELECT ProductID FROM Products WHERE ProductName = N'Đen đá');
DECLARE @prodNauDa INT = (SELECT ProductID FROM Products WHERE ProductName = N'Nâu đá');
DECLARE @prodBacXiu INT = (SELECT ProductID FROM Products WHERE ProductName = N'Bạc xỉu');
DECLARE @prodCaPheCotDua INT = (SELECT ProductID FROM Products WHERE ProductName = N'Cà phê cốt dừa');
DECLARE @prodHongTraSua INT = (SELECT ProductID FROM Products WHERE ProductName = N'Hồng trà sữa');
DECLARE @prodMatchaLatte INT = (SELECT ProductID FROM Products WHERE ProductName = N'Matcha Latte');
DECLARE @prodTraSuaNhai INT = (SELECT ProductID FROM Products WHERE ProductName = N'Trà sữa Nhài');
DECLARE @prodOLongSua INT = (SELECT ProductID FROM Products WHERE ProductName = N'Ô long sữa');
DECLARE @prodBanhTao INT = (SELECT ProductID FROM Products WHERE ProductName = N'Bánh táo');

INSERT INTO ProductIngredients (ProductID, IngredientID, QuantityRequired) VALUES
-- Đen đá: Cà phê, Đường
(@prodDenDa, @ingCaPhe, 25),
(@prodDenDa, @ingDuong, 10),
-- Nâu đá: Cà phê, Sữa đặc
(@prodNauDa, @ingCaPhe, 25),
(@prodNauDa, @ingSuaDac, 20),
-- Bạc xỉu: Cà phê, Sữa đặc, Sữa tươi
(@prodBacXiu, @ingCaPhe, 20),
(@prodBacXiu, @ingSuaDac, 15),
(@prodBacXiu, @ingSuaTuoi, 30),
-- Cà phê cốt dừa: Cà phê, Nước cốt dừa, Sữa đặc
(@prodCaPheCotDua, @ingCaPhe, 25),
(@prodCaPheCotDua, @ingCotDua, 40),
(@prodCaPheCotDua, @ingSuaDac, 10),
-- Hồng trà sữa: Hồng trà, Sữa tươi, Đường
(@prodHongTraSua, @ingHongTra, 10),
(@prodHongTraSua, @ingSuaTuoi, 50),
(@prodHongTraSua, @ingDuong, 15),
-- Matcha Latte: Bột matcha, Sữa tươi, Kem béo
(@prodMatchaLatte, @ingMatcha, 15),
(@prodMatchaLatte, @ingSuaTuoi, 60),
(@prodMatchaLatte, @ingKemBeo, 20),
-- Bánh táo: Bột mì, Trứng, Bơ, Táo, Đường
(@prodBanhTao, @ingBotMi, 100),
(@prodBanhTao, @ingTrung, 1),
(@prodBanhTao, @ingBo, 50),
(@prodBanhTao, @ingTao, 1),
(@prodBanhTao, @ingDuong, 30);
GO

-- Additional Recipes based on feedback
-- Declare more product IDs
DECLARE @prodBanhBongLan INT = (SELECT ProductID FROM Products WHERE ProductName = N'Bánh bông lan trứng muối');
DECLARE @prodTCDen INT = (SELECT ProductID FROM Products WHERE ProductName = N'Trân châu đen');
DECLARE @prodTCTrang INT = (SELECT ProductID FROM Products WHERE ProductName = N'Trân châu trắng');
DECLARE @prodKemMan INT = (SELECT ProductID FROM Products WHERE ProductName = N'Kem mặn');
DECLARE @prodKemPhoMai INT = (SELECT ProductID FROM Products WHERE ProductName = N'Kem phô mai');

-- Declare more ingredient IDs
DECLARE @ingTCDen INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Trân châu đen');
DECLARE @ingTCTrang INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Trân châu trắng');
DECLARE @ingKemMan INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Kem mặn');
DECLARE @ingKemPhoMai INT = (SELECT IngredientID FROM Ingredients WHERE IngredientName = N'Kem phô mai');


INSERT INTO ProductIngredients (ProductID, IngredientID, QuantityRequired) VALUES
-- Trà sữa Nhài: Trà nhài, Sữa tươi, Đường
(@prodTraSuaNhai, @ingTraNhai, 10),
(@prodTraSuaNhai, @ingSuaTuoi, 50),
(@prodTraSuaNhai, @ingDuong, 15),
-- Ô long sữa: Trà ô long, Sữa tươi, Đường
(@prodOLongSua, @ingOLong, 10),
(@prodOLongSua, @ingSuaTuoi, 50),
(@prodOLongSua, @ingDuong, 15),
-- Bánh bông lan trứng muối: Bột mì, Trứng, Bơ, Đường
(@prodBanhBongLan, @ingBotMi, 80),
(@prodBanhBongLan, @ingTrung, 2),
(@prodBanhBongLan, @ingBo, 40),
(@prodBanhBongLan, @ingDuong, 25),
-- Toppings
(@prodTCDen, @ingTCDen, 30),
(@prodTCTrang, @ingTCTrang, 30),
(@prodKemMan, @ingKemMan, 20),
(@prodKemPhoMai, @ingKemPhoMai, 20);
GO


-- Customers
INSERT INTO Customers (Email, CustomerName, Points) VALUES
(N'le.c@email.com', N'Lê C', 15),
(N'pham.d@email.com', N'Phạm D', 8);
GO

PRINT 'Database created and initial data inserted successfully.';
GO
