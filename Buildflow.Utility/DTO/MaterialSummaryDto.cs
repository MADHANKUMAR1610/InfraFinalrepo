using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public class MaterialSummaryDto
    {
        public string ItemName { get; set; }
        public decimal Required { get; set; }
        public decimal InStock { get; set; }
    }

}
