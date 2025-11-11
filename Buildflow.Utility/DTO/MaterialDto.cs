using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public  class MaterialDto
    {
        public string? MaterialList { get; set; }
        public int InStockQuantity { get; set; }
        public int RequiredQuantity { get; set; }
        public string? Level { get; set; }
        public string? RequestStatus { get; set; }
    }
}
