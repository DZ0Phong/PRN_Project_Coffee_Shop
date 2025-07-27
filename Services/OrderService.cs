using Microsoft.EntityFrameworkCore;
using PRN_Project_Coffee_Shop.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PRN_Project_Coffee_Shop.Services
{
    public class OrderService
    {
        private readonly PrnProjectCoffeeShopContext _context;

        public OrderService(PrnProjectCoffeeShopContext context)
        {
            _context = context;
        }

        public (bool Success, string Message, Order NewOrder) CreateOrder(User currentUser, ICollection<OrderDetail> currentOrderItems, string tableNumber, bool isDelivery, string deliveryNotes, string customerEmail, Promotion appliedPromotion)
        {
            if (!currentOrderItems.Any())
            {
                return (false, "Cannot confirm an empty order.", null);
            }

            var reservedStock = GetReservedStock();
            var allItemsInOrder = new List<Product>();
            currentOrderItems.ToList().ForEach(item => {
                allItemsInOrder.Add(item.Product);
                allItemsInOrder.AddRange(item.Toppings);
            });

            var distinctItems = allItemsInOrder.Distinct();

            foreach (var product in distinctItems)
            {
                int quantity = allItemsInOrder.Count(p => p.ProductId == product.ProductId);
                string availabilityError = CheckIngredientAvailability(product, quantity, reservedStock);
                if (availabilityError != null)
                {
                    return (false, availabilityError, null);
                }
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    decimal originalTotal = currentOrderItems.Sum(item => (item.Price + item.Toppings.Sum(t => t.Price)) * item.Quantity);
                    decimal finalTotal = originalTotal;

                    if (appliedPromotion != null)
                    {
                        finalTotal = originalTotal - (originalTotal * (appliedPromotion.DiscountPercentage / 100));
                    }

                    var newOrder = new Order
                    {
                        UserId = currentUser.UserId,
                        OrderDate = DateTime.Now,
                        TotalAmount = finalTotal,
                        Status = "Pending",
                        TableNumber = tableNumber,
                        IsDelivery = isDelivery,
                        DeliveryNotes = deliveryNotes,
                        PromotionCode = appliedPromotion?.PromotionCode,
                        OrderDetails = new List<OrderDetail>()
                    };

                    foreach (var itemVM in currentOrderItems)
                    {
                        var newDetail = new OrderDetail
                        {
                            ProductId = itemVM.ProductId,
                            Quantity = itemVM.Quantity,
                            Price = itemVM.Price,
                            SugarPercent = itemVM.SugarPercent,
                            IcePercent = itemVM.IcePercent,
                            Toppings = new List<Product>()
                        };

                        var toppingIds = itemVM.Toppings.Select(t => t.ProductId).ToList();
                        var trackedToppings = _context.Products.Where(p => toppingIds.Contains(p.ProductId)).ToList();
                        foreach (var topping in trackedToppings)
                        {
                            newDetail.Toppings.Add(topping);
                        }
                        newOrder.OrderDetails.Add(newDetail);
                    }

                    if (!string.IsNullOrWhiteSpace(customerEmail))
                    {
                        var customer = _context.Customers.FirstOrDefault(c => c.Email == customerEmail);
                        bool isNewCustomer = customer == null;

                        if (isNewCustomer)
                        {
                            customer = new Customer { Email = customerEmail, CustomerName = "New Customer", Points = 0 };
                            _context.Customers.Add(customer);
                        }
                        
                        newOrder.Customer = customer;

                        int pointsToAdd = 0;
                        foreach (var item in newOrder.OrderDetails)
                        {
                            var product = _context.Products.Include(p => p.Category).First(p => p.ProductId == item.ProductId);
                            if (product.Category.CategoryName == "Cà Phê" || product.Category.CategoryName == "Trà")
                            {
                                pointsToAdd += 5 * item.Quantity;
                            }
                            else if (product.Category.CategoryName == "Bánh")
                            {
                                pointsToAdd += 8 * item.Quantity;
                            }
                        }
                        customer.Points = (customer.Points ?? 0) + pointsToAdd;
                        _context.Customers.Update(customer);
                        
                        _context.SaveChanges(); 

                        if (customer.Points >= 100)
                        {
                            int promotionsToCreate = customer.Points.Value / 100;
                            for (int i = 0; i < promotionsToCreate; i++)
                            {
                                customer.Points -= 100;
                                // This part needs to be handled carefully, as it sends an email.
                                // For now, we'll just create the promotion.
                                CreatePromotion(customer);
                            }
                            _context.Customers.Update(customer);
                        }
                    }

                    if (appliedPromotion != null)
                    {
                        var promotionToUpdate = _context.Promotions.First(p => p.PromotionId == appliedPromotion.PromotionId);
                        promotionToUpdate.IsUsed = true;
                        promotionToUpdate.IsActive = false;
                        _context.Promotions.Update(promotionToUpdate);
                    }

                    _context.Orders.Add(newOrder);
                    _context.SaveChanges();

                    transaction.Commit();
                    
                    CheckAndUpdateAllProductAvailability();

                    return (true, "Order successfully created!", newOrder);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return (false, $"An error occurred: {ex.InnerException?.Message ?? ex.Message}", null);
                }
            }
        }

        public string CheckIngredientAvailability(Product product, int quantity, Dictionary<int, decimal> reservedStock)
        {
            var productIngredients = _context.ProductIngredients
                                             .Include(pi => pi.Ingredient)
                                             .Where(pi => pi.ProductId == product.ProductId)
                                             .ToList();
            var today = DateOnly.FromDateTime(DateTime.Today);

            foreach (var pi in productIngredients)
            {
                decimal reserved = reservedStock.GetValueOrDefault(pi.IngredientId, 0);
                decimal effectiveStock = pi.Ingredient.QuantityInStock - reserved;

                if (effectiveStock < (pi.QuantityRequired * quantity))
                {
                    return $"Không đủ '{pi.Ingredient.IngredientName}' để làm '{product.ProductName}'.";
                }

                if (pi.Ingredient.ExpiryDate.HasValue && pi.Ingredient.ExpiryDate.Value < today)
                {
                    return $"Nguyên liệu '{pi.Ingredient.IngredientName}' cho món '{product.ProductName}' đã hết hạn.";
                }
            }
            return null;
        }

        public Dictionary<int, decimal> GetReservedStock()
        {
            var reservedStock = new Dictionary<int, decimal>();
            var pendingOrders = _context.Orders
                                        .Include(o => o.OrderDetails)
                                            .ThenInclude(od => od.Product)
                                                .ThenInclude(p => p.ProductIngredients)
                                        .Include(o => o.OrderDetails)
                                            .ThenInclude(od => od.Toppings)
                                                .ThenInclude(t => t.ProductIngredients)
                                        .Where(o => o.Status == "Pending")
                                        .ToList();

            foreach (var order in pendingOrders)
            {
                foreach (var detail in order.OrderDetails)
                {
                    foreach (var pi in detail.Product.ProductIngredients)
                    {
                        if (reservedStock.ContainsKey(pi.IngredientId))
                            reservedStock[pi.IngredientId] += pi.QuantityRequired * detail.Quantity;
                        else
                            reservedStock[pi.IngredientId] = pi.QuantityRequired * detail.Quantity;
                    }

                    foreach (var topping in detail.Toppings)
                    {
                        foreach (var pi in topping.ProductIngredients)
                        {
                            if (reservedStock.ContainsKey(pi.IngredientId))
                                reservedStock[pi.IngredientId] += pi.QuantityRequired * detail.Quantity;
                            else
                                reservedStock[pi.IngredientId] = pi.QuantityRequired * detail.Quantity;
                        }
                    }
                }
            }
            return reservedStock;
        }
        
        public void CheckAndUpdateAllProductAvailability()
        {
            var allProducts = _context.Products.Include(p => p.ProductIngredients).ThenInclude(pi => pi.Ingredient).ToList();
            var reservedStock = GetReservedStock();
            foreach (var product in allProducts)
            {
                string availabilityError = CheckIngredientAvailability(product, 1, reservedStock);
                product.IsOutOfStock = (availabilityError != null);
                _context.Products.Update(product);
            }
            _context.SaveChanges();
        }

        private void CreatePromotion(Customer customer)
        {
            if (customer.CustomerId == 0)
            {
                 // Cannot create promotion for a customer that hasn't been saved to the database yet.
                 return;
            }

            var newPromotion = new Promotion
            {
                PromotionCode = $"DISCOUNT20_{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                Description = $"Discount 20% for customer {customer.Email}",
                DiscountPercentage = 20,
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(6)),
                IsActive = true,
                IsUsed = false,
                CustomerId = customer.CustomerId
            };
            _context.Promotions.Add(newPromotion);
        }
    }
}
