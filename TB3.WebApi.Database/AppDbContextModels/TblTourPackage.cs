using System;
using System.Collections.Generic;

namespace TB3.WebApi.Database.AppDbContextModels;

public partial class TblTourPackage
{
    public int PackageId { get; set; }

    public string PackageName { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public int DurationDays { get; set; }

    public decimal PricePerPerson { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool? DeleteFlag { get; set; }

    public DateTime CreateDateTime { get; set; }

    public DateTime? ModifiedDateTime { get; set; }
}
