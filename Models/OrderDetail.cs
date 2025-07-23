using System;
using System.Collections.Generic;

namespace PRN_Project_Coffee_Shop.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public int SugarPercent { get; set; }

    public int IcePercent { get; set; }

    public string? Notes { get; set; }

    public decimal TotalPrice => Price * Quantity;

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<Product> Toppings { get; set; } = new List<Product>();
}
