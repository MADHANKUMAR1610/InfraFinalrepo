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
            { "cement (50kg)", 2000 },
            { "steel rods (50mm)", 500 },
            { "pvc pipes", 500 },
            { "wire (4mm)", 500 },
            { "sand", 20 },
            { "bricks", 500 }
        };
    }
}

