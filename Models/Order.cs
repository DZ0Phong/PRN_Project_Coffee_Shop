using System;
using System.Collections.Generic;

namespace PRN_Project_Coffee_Shop.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public string? TableNumber { get; set; }

    public bool IsDelivery { get; set; }

    public string? DeliveryNotes { get; set; }

    public string? PromotionCode { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User User { get; set; } = null!;
}
