using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PRN_Project_Coffee_Shop.Models;

public partial class OrderDetail : INotifyPropertyChanged
{
    private int _quantity;

    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity != value)
            {
                _quantity = value;
                OnPropertyChanged(); // Notify that Quantity has changed
                OnPropertyChanged(nameof(TotalPrice)); // Notify that TotalPrice has also changed
            }
        }
    }

    public decimal Price { get; set; }

    public int SugarPercent { get; set; }

    public int IcePercent { get; set; }

    public decimal TotalPrice => (Price + Toppings.Sum(t => t.Price)) * Quantity;

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<Product> Toppings { get; set; } = new List<Product>();

    public string Options
    {
        get
        {
            var options = new List<string>();
            if (Product?.Category?.CategoryName == "Cà Phê" || Product?.Category?.CategoryName == "Trà")
            {
                options.Add($"{SugarPercent}% Sugar");
                options.Add($"{IcePercent}% Ice");
            }
            if (Toppings.Any())
            {
                options.Add("Toppings: " + string.Join(", ", Toppings.Select(t => t.ProductName)));
            }
            return string.Join(", ", options);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void RefreshTotalPrice()
    {
        OnPropertyChanged(nameof(TotalPrice));
        OnPropertyChanged(nameof(Options));
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
