using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class VendorDepartment
{
    public int VendorDeptId { get; set; }

    public int? VendorId { get; set; }

    public int? DeptId { get; set; }

    public virtual Department? Dept { get; set; }

    public virtual Vendor? Vendor { get; set; }
}
