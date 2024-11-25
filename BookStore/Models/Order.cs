using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? CustomerId { get; set; }

    public DateOnly OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string? OrderStatus { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
