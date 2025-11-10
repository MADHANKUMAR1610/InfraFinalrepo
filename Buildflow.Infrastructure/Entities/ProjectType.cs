using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class ProjectType
{
    public int ProjectTypeId { get; set; }

    public string ProjectTypeName { get; set; } = null!;

    public string? ProjectTypeDescription { get; set; }

    public string? TypeCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
