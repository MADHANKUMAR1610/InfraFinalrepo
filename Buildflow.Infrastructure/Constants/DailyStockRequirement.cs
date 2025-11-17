using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Buildflow.Infrastructure.Constants
{
    public static class DailyStockRequirement
    {
        public static readonly Dictionary<string, decimal> RequiredStock = new()
        {
            //HardCored Value for Daily Stock
            { "Cement (50kg)", 2000 },
            { "Steel Rods (50mm)", 500 },
            { "PVC Pipes", 500 },
            { "Wire (4mm)", 500 },
            { "Sand", 20 },
            { "Bricks", 500 }
        };
    }
}

