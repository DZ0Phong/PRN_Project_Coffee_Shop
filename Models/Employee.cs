using System;
using System.Collections.Generic;

namespace PRN_Project_Coffee_Shop.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public decimal Salary { get; set; }

    public virtual User User { get; set; } = null!;
}
