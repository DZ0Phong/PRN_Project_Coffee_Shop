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

    public string? Notes { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    // Calculated property for total price
    public decimal TotalPrice => Price * Quantity;
}
