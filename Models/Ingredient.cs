using System;
using System.Collections.Generic;

namespace PRN_Project_Coffee_Shop.Models;

public partial class Ingredient
{
    public int IngredientId { get; set; }

    public string IngredientName { get; set; } = null!;

    public decimal QuantityInStock { get; set; }

    public string Unit { get; set; } = null!;

    public DateOnly? ExpiryDate { get; set; }

    public decimal WarningThreshold { get; set; }

    public virtual ICollection<ProductIngredient> ProductIngredients { get; set; } = new List<ProductIngredient>();
}
