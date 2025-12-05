using System;
using System.Collections.Generic;

namespace TB3.WebApi.Database.AppDbContextModels;

public partial class TblProduct
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool DeleteFlag { get; set; }

    public DateTime CreateDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }
}
