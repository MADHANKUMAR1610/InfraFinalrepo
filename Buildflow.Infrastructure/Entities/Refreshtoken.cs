using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Refreshtoken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime Expirydate { get; set; }

    public bool? Isrevoked { get; set; }
}
