using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Subcontractor
{
    public int SubcontractorId { get; set; }

    public string? SubcontractorCode { get; set; }

    public string SubcontractorName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? AlternativePhone { get; set; }

    public string? Website { get; set; }

    public string? Street { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? PostalCode { get; set; }

    public string? TaxId { get; set; }

    public string? InsuranceDetails { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
