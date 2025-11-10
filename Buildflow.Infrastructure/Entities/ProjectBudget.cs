using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class ProjectBudget
{
    public int ProjectBudgetId { get; set; }

    public string ProjectExpenseCategory { get; set; } = null!;
}
