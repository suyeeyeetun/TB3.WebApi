using System;
using System.Collections.Generic;

namespace TB3.WebApi.Database.AppDbContextModels;

public partial class TblBorrow
{
    public int BorrowId { get; set; }

    public int BookId { get; set; }

    public string BorrowerName { get; set; } = null!;

    public DateTime IssueDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public string Status { get; set; } = null!;
}
