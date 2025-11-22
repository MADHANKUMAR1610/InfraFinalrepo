using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Projectstatusmaster
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;
}
