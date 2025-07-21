using System;
using System.Collections.Generic;

namespace PRN_Project_Coffee_Shop.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int CategoryId { get; set; }

    public decimal Price { get; set; }

    public string? Size { get; set; }

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    public bool IsOutOfStock { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();
}
