using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Vendormaterial
{
    public int Vendormaterialid { get; set; }

    public int VendorId { get; set; }

    public string? Material { get; set; }

    public string? Unit { get; set; }

    public decimal? Currentprice { get; set; }

    public decimal? Price { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public string? Createdby { get; set; }

    public string? Updatedby { get; set; }

    public virtual Vendor Vendor { get; set; } = null!;
}
