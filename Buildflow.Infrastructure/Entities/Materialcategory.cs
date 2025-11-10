using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Materialcategory
{
    public int Materialcategoryid { get; set; }

    public string? Categoryname { get; set; }

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
