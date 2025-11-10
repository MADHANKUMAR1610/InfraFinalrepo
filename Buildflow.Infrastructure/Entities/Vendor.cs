using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Vendor
{
    public int VendorId { get; set; }

    public string? VendorCode { get; set; }

    public string VendorName { get; set; } = null!;

    public string? OrganizationName { get; set; }

    public string Mobile { get; set; } = null!;

    public string? AlternativeMobile { get; set; }

    public string Email { get; set; } = null!;

    public string? Website { get; set; }

    public string? Street { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? PostalCode { get; set; }

    public string? TaxId { get; set; }

    public string? PaymentTerms { get; set; }

    public string? PreferredPaymentMethod { get; set; }

    public string? DeliveryTerms { get; set; }

    public decimal? CreditLimit { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<Boq> Boqs { get; set; } = new List<Boq>();

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

    public virtual ICollection<VendorDepartment> VendorDepartments { get; set; } = new List<VendorDepartment>();

    public virtual ICollection<Vendorrole> Vendorroles { get; set; } = new List<Vendorrole>();
}
