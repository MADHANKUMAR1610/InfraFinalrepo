using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class DailyStock
{
    public int Id { get; set; }

    public string ItemName { get; set; } = null!;

    public decimal DefaultQty { get; set; }

    public decimal RemainingQty { get; set; }

    public DateTime Date { get; set; }

    public int? ProjectId { get; set; }

    public virtual Project? Project { get; set; }
}
