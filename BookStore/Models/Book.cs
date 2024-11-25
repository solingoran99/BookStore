using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Book
{
    public string Isbn13 { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Language { get; set; }

    public decimal Price { get; set; }

    public DateOnly? PublicationDate { get; set; }

    public int? AuthorId { get; set; }

    public int? PublisherId { get; set; }

    public virtual Author? Author { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Publisher? Publisher { get; set; }

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
