using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Boardlabel
{
    public int Labelid { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int BoardId { get; set; }

    public int? Position { get; set; }

    public int? Wiplimit { get; set; }

    public string? Colorcode { get; set; }

    public bool? Isexpand { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? LastUpdatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Code { get; set; }

    public bool? IsDefault { get; set; }

    public bool? IsMoveState { get; set; }

    public virtual Board Board { get; set; } = null!;

    public virtual EmployeeDetail? CreatedByNavigation { get; set; }

    public virtual EmployeeDetail? UpdatedByNavigation { get; set; }
}
