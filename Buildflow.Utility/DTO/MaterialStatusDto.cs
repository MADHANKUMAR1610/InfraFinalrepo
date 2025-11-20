using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public class MaterialStatusDto
    {
        public string MaterialName { get; set; }
        public int InStock { get; set; }
        public int RequiredQty { get; set; }
    }
}
