using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Vendorrole
{
    public int VendorRoleId { get; set; }

    public int? VendorId { get; set; }

    public int? RoleId { get; set; }

    public virtual Role? Role { get; set; }

    public virtual Vendor? Vendor { get; set; }
}
