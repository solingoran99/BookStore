using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int? OrderId { get; set; }

    public string? Isbn { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual Book? IsbnNavigation { get; set; }

    public virtual Order? Order { get; set; }
}
