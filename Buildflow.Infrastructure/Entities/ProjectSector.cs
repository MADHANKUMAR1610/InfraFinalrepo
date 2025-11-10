using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class ProjectSector
{
    public int ProjectSectorId { get; set; }

    public string ProjectSectorName { get; set; } = null!;

    public string? ProjectSectorDescription { get; set; }

    public string? SectorCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
