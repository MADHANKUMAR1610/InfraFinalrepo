using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public class ApprovedBoqListDto
    {
        public int BoqId { get; set; }
        public string BoqName { get; set; }
        public string BoqCode { get; set; }
        public string Description { get; set; }
        public string VendorName { get; set; }
        public int ApprovedBy { get; set; }
        public DateTime ApprovedAt { get; set; }
    }
   
}
