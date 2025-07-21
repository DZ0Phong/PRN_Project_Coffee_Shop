using System;
using System.Collections.Generic;

namespace PRN_Project_Coffee_Shop.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string? CustomerName { get; set; }

    public string? Email { get; set; }

    public int? Points { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
