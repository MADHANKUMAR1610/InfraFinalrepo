using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class ProjectDetail
{
    public int ProjectDetailId { get; set; }

    public int ProjectId { get; set; }

    public string ProjectLocation { get; set; } = null!;

    public string? PlotNumber { get; set; }

    public string? SiteAddress { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public decimal? LandArea { get; set; }

    public string? LandAreaUnit { get; set; }

    public string? ZoningType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Project Project { get; set; } = null!;
}
