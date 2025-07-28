using Microsoft.EntityFrameworkCore;
using Moq;
using PRN_Project_Coffee_Shop.Models;
using PRN_Project_Coffee_Shop.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CoffeeShop.Tests
{
    public class OrderServiceTests
    {
        private readonly DbContextOptions<PrnProjectCoffeeShopContext> _options;

        public OrderServiceTests()
        {
            _options = new DbContextOptionsBuilder<PrnProjectCoffeeShopContext>()
                .UseInMemoryDatabase(databaseName: "CoffeeShopTestDb")
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
        }

        [Fact]
        public void CreateOrder_WithEmptyOrder_ShouldFail()
        {
            // Arrange
            using (var context = new PrnProjectCoffeeShopContext(_options))
            {
                var service = new OrderService(context);
                var currentUser = new User { UserId = 1 };
                var orderItems = new List<OrderDetail>();

                // Act
                var result = service.CreateOrder(currentUser, orderItems, "Table 1", false, "", "", null);

                // Assert
                Assert.False(result.Success);
                Assert.Equal("Cannot confirm an empty order.", result.Message);
            }
        }

        [Fact]
        public void CreateOrder_WithValidItems_ShouldSucceed()
        {
            // Arrange
            using (var context = new PrnProjectCoffeeShopContext(_options))
            
            {
                // Seed dataaa
                var coffeeCategory = new Category { CategoryId = 1, CategoryName = "Cà Phê" };
                var ingredient = new Ingredient { IngredientId = 1, IngredientName = "Coffee Beans", QuantityInStock = 100, Unit = "g", ExpiryDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30)) };
                var product = new Product { ProductId = 1, ProductName = "Espresso", Price = 30000, CategoryId = 1, Category = coffeeCategory, ProductIngredients = new List<ProductIngredient> { new ProductIngredient { IngredientId = 1, QuantityRequired = 10, Ingredient = ingredient } } };
                context.Categories.Add(coffeeCategory);
                context.Ingredients.Add(ingredient);
                context.Products.Add(product);
                context.SaveChanges();

                var service = new OrderService(context);
                var currentUser = new User { UserId = 1 };
                var orderItems = new List<OrderDetail>
                {
                    new OrderDetail { ProductId = 1, Product = product, Quantity = 2, Price = 30000, Toppings = new List<Product>() }
                };

                // Actttt
                var result = service.CreateOrder(currentUser, orderItems, "Table 2", false, "No sugar", "", null);

                // Asserttt
                Assert.True(result.Success);
                Assert.Equal("Order successfully created!", result.Message);
                Assert.NotNull(result.NewOrder);
                Assert.Equal(1, context.Orders.Count());
                var createdOrder = context.Orders.Include(o => o.OrderDetails).First();
                Assert.Equal(60000, createdOrder.TotalAmount);
                Assert.Equal("Pending", createdOrder.Status);
                Assert.Equal(1, createdOrder.OrderDetails.Count);
            }
        }
    }
}
