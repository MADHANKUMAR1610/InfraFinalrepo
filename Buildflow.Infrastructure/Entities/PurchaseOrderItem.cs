using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class PurchaseOrderItem
{
    public int PurchaseOrderItemsId { get; set; }

    public int? PurchaseOrderId { get; set; }

    public string? ItemName { get; set; }

    public string? Unit { get; set; }

    public decimal? Price { get; set; }

    public int? Quantity { get; set; }

    public double? Total { get; set; }

    public virtual PurchaseOrder? PurchaseOrder { get; set; }
}
