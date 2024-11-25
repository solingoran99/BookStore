using System;
using System.Collections.Generic;

namespace BookStore.Models;

public partial class Genre
{
    public int GenreId { get; set; }

    public string GenreName { get; set; } = null!;

    public virtual ICollection<Book> Isbns { get; set; } = new List<Book>();
}
