using System;
using System.Collections.Generic;

namespace TB3.WebApi.Database.AppDbContextModels;

public partial class TblBookManagement
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string Category { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime CreateDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }

    public bool DeleteFlag { get; set; }
}
