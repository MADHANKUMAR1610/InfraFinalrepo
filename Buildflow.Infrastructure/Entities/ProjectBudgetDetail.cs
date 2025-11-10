using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class ProjectBudgetDetail
{
    public int ProjectBudgetId { get; set; }

    public int ProjectId { get; set; }

    public string ProjectExpenseCategory { get; set; } = null!;

    public decimal EstimatedCost { get; set; }

    public decimal? ApprovedBudget { get; set; }

    public virtual Project Project { get; set; } = null!;
}
