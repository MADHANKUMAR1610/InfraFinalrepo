using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public  class MaterialDto
    {
        public int SNo { get; set; }
        public string MaterialList { get; set; }
        public string InStockQuantity { get; set; }
        public string RequiredQuantity { get; set; }
        public string Level { get; set; }
        public string RequestStatus { get; set; }
    }
}
