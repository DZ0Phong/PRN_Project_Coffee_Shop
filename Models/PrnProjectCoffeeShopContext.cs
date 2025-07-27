using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PRN_Project_Coffee_Shop.Models;

public partial class PrnProjectCoffeeShopContext : DbContext
{
    public PrnProjectCoffeeShopContext()
    {
    }

    public PrnProjectCoffeeShopContext(DbContextOptions<PrnProjectCoffeeShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductIngredient> ProductIngredients { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // Added Pooling=False as a diagnostic step to address the identity seed jumping issue.
        // This is not a permanent solution and may impact performance, but it can help determine
        // if connection state is related to the problem.
        => optionsBuilder.UseSqlServer("server=localhost;database=PRN_Project_Coffee_Shop;uid=sa;pwd=sa123456;TrustServerCertificate=True;Pooling=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B88A158C8");

            entity.HasIndex(e => e.CategoryName, "UQ__Categori__8517B2E0A2C5BA2E").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B802D874C7");

            entity.HasIndex(e => e.Email, "UQ__Customer__A9D10534DC1A3AA5").IsUnique();

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Points).HasDefaultValue(0);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1172B5779");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.Salary).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Employees)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Employees__UserI__619B8048");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PK__Ingredie__BEAEB27A9C92AFA1");

            entity.HasIndex(e => e.IngredientName, "UQ__Ingredie__A1B2F1CC5C319EA2").IsUnique();

            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.IngredientName).HasMaxLength(100);
            entity.Property(e => e.QuantityInStock).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Unit).HasMaxLength(20);
            entity.Property(e => e.WarningThreshold).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAFD0A9A569");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PromotionCode).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TableNumber).HasMaxLength(10);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Orders__Customer__5165187F");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__UserID__5070F446");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C227AE148");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.SugarPercent).HasDefaultValue(100);
            entity.Property(e => e.IcePercent).HasDefaultValue(100);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__5629CD9C");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Produ__571DF1D5");

            entity.HasMany(d => d.Toppings).WithMany(p => p.OrderDetailsNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "OrderDetailTopping",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("ToppingId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__OrderDeta__Toppi__5AEE82B9"),
                    l => l.HasOne<OrderDetail>().WithMany()
                        .HasForeignKey("OrderDetailId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__OrderDeta__Order__59FA5E80"),
                    j =>
                    {
                        j.HasKey("OrderDetailId", "ToppingId").HasName("PK__OrderDet__9D59FFC20D35239C");
                        j.ToTable("OrderDetailToppings");
                        j.IndexerProperty<int>("OrderDetailId").HasColumnName("OrderDetailID");
                        j.IndexerProperty<int>("ToppingId").HasColumnName("ToppingID");
                    });
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6ED7C569036");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(100);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Products__Catego__44FF419A");
        });

        modelBuilder.Entity<ProductIngredient>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.IngredientId }).HasName("PK__ProductI__0FE62DCAAB7160FE");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.IngredientId).HasColumnName("IngredientID");
            entity.Property(e => e.QuantityRequired).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.ProductIngredients)
                .HasForeignKey(d => d.IngredientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductIn__Ingre__48CFD27E");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductIngredients)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductIn__Produ__47DBAE45");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PK__Promotio__52C42F2F08EB5289");

            entity.HasIndex(e => e.PromotionCode, "UQ__Promotio__A617E4B69F22F761").IsUnique();

            entity.Property(e => e.PromotionId).HasColumnName("PromotionID");
            entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PromotionCode).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A0CA0EFAD");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B61608F3FBAEC").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC21113AB2");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053489A78EF9").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleID__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
