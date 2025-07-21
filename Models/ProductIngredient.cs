using System;
using System.Collections.Generic;

namespace PRN_Project_Coffee_Shop.Models;

public partial class ProductIngredient
{
    public int ProductId { get; set; }

    public int IngredientId { get; set; }

    public decimal QuantityRequired { get; set; }

    public virtual Ingredient Ingredient { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
