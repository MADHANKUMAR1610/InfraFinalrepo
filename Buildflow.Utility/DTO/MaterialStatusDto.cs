using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public class MaterialStatusDto
    {
        public string ItemName { get; set; } = string.Empty;
        public string InStockDisplay { get; set; } = string.Empty;
        public string RequiredDisplay { get; set; } = string.Empty;
    }
}
