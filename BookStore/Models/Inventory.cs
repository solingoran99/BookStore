using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Inventory
{
    public int StoreId { get; set; }

    public string Isbn { get; set; } = null!;

    public int Quantity { get; set; }

    public virtual Book IsbnNavigation { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}
