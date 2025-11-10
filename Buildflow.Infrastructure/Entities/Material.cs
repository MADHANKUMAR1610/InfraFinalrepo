using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Material
{
    public int Materialid { get; set; }

    public int? VendorId { get; set; }

    public string? Material1 { get; set; }

    public string? Unit { get; set; }

    public decimal? Currentprice { get; set; }

    public decimal? Price { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public string? Createdby { get; set; }

    public string? Updatedby { get; set; }

    public int? Materialcategoryid { get; set; }

    public virtual Materialcategory? Materialcategory { get; set; }

    public virtual Vendor? Vendor { get; set; }
}
