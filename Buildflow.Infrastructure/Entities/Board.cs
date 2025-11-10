using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Board
{
    public int BoardId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Boardlabel> Boardlabels { get; set; } = new List<Boardlabel>();

    public virtual EmployeeDetail? CreatedByNavigation { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual EmployeeDetail? UpdatedByNavigation { get; set; }
}
