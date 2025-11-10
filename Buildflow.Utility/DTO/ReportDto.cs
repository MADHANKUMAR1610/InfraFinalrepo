using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public class ReportDto
    {
    }

    public class DailyAttachmentUploadDto
    {
        [FromForm]
        public int ReportId { get; set; }


        [FromForm]
        public List<int> SNo { get; set; }

        [FromForm]
        public List<IFormFile> Files { get; set; }
    }

    public class UploadReportRequest
    {
        [Required]
        //[FromForm(Name = "reportJson")]
        public string ReportJson { get; set; }

        [FromForm(Name = "files")]
        public List<IFormFile> Files { get; set; }
    }

    public class ReportDetails
    {
        
            public int ReportId { get; set; }
            public string? ReportCode { get; set; }
            public int ReportType { get; set; }

            public string ReportTypeName { get; set; }
            public int ProjectId { get; set; }
            public string ProjectName { get; set; }
            public DateTime ReportDate { get; set; }
            public string ReportedBy { get; set; }

        //public ReportData ReportData { get; set; }
        public ReportDataDto ReportData { get; set; } = new();
        public DateTime? CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
    public class ReportDataDto
    {
        public List<IssueRiskReport>? IssueRiskReport { get; set; }
        public List<MaterialUsageReport>? MaterialUsageReport { get; set; }
        public List<DailyProgressSummary>? DailyProgressSummary { get; set; }
        public List<SafetyComplianceReport>? SafetyComplianceReport { get; set; }
    }

    public class IssueRiskReport
    {
        public string? Issue { get; set; }
        public string? Impact { get; set; }
        public int SerialNo { get; set; }
    }

    public class MaterialUsageReport
    {
        public string? Level { get; set; }
        public string? Stock { get; set; }
        public string? Material { get; set; }
        public int SerialNo { get; set; }
    }

    public class DailyProgressSummary
    {
        public string? FilePath { get; set; }
        public string? Status { get; set; }
        public int SerialNo { get; set; }
        public string? WorkActivity { get; set; }
    }

    public class SafetyComplianceReport
    {
        public string? Item { get; set; }
        public string? Report { get; set; }
        public int SerialNo { get; set; }
    }

 


}
