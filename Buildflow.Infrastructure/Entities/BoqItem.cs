using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class BoqItem
{
    public int BoqItemsId { get; set; }

    public int? BoqId { get; set; }

    public string? ItemName { get; set; }

    public string? Unit { get; set; }

    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    public double? Total { get; set; }

    public virtual Boq? Boq { get; set; }
}
