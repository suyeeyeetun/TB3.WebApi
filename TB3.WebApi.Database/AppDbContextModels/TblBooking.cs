using System;
using System.Collections.Generic;

namespace TB3.WebApi.Database.AppDbContextModels;

public partial class TblBooking
{
    public int BookingId { get; set; }

    public int CustomerId { get; set; }

    public int PackageId { get; set; }

    public DateTime BookingDate { get; set; }

    public int NumberOfPeople { get; set; }

    public decimal TotalAmount { get; set; }

    public string BookingStatus { get; set; } = null!;

    public DateTime CreateDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }
}
