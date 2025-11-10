using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Infrastructure.Models
{
    using System.Text.Json.Serialization;

    //[JsonPropertyName("filename")]
    //public string? FileName { get; set; }
    //[JsonPropertyName("filepath")]
    //public string? FilePath { get; set; }
    public class ReportData
    {
        [JsonPropertyName("dailyprogresssummary")]
        public List<DailyProgressItem> DailyProgressSummary { get; set; }

        [JsonPropertyName("materialusagereport")]
        public List<MaterialUsageItem> MaterialUsageReport { get; set; }

        [JsonPropertyName("safetycompliancereport")]
        public List<SafetyComplianceItem> SafetyComplianceReport { get; set; }

        [JsonPropertyName("issueriskreport")]
        public List<IssueRiskItem> IssueRiskReport { get; set; }
    }

    public class DailyProgressItem
    {
        [JsonPropertyName("serialno")]
        public int SerialNo { get; set; }

        [JsonPropertyName("workactivity")]
        public string WorkActivity { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        //[JsonPropertyName("action")]
        //public string Action { get; set; }


        //[JsonPropertyName("filepath")]

        //public string? FilePath { get; set; } // Store the uploaded file path


    }

    public class MaterialUsageItem
    {
        [JsonPropertyName("serialno")]
        public int SerialNo { get; set; }

        [JsonPropertyName("material")]
        public string Material { get; set; }

        [JsonPropertyName("stock")]
        public string Stock { get; set; }

        [JsonPropertyName("level")]
        public string Level { get; set; } // e.g., "Low Stock", "Sufficient"
    }

    public class SafetyComplianceItem
    {
        [JsonPropertyName("serialno")]
        public int SerialNo { get; set; }

        [JsonPropertyName("item")]
        public string Item { get; set; } // e.g., PPE Compliance, Safety Incident

        [JsonPropertyName("report")]
        public string Report { get; set; }
    }

    public class IssueRiskItem
    {
        [JsonPropertyName("serialno")]
        public int SerialNo { get; set; }

        [JsonPropertyName("issue")]
        public string Issue { get; set; }

        [JsonPropertyName("impact")]
        public string Impact { get; set; }
    }

    public class UpsertReportRequest
    {
        public Buildflow.Infrastructure.Entities.Report Report { get; set; }
        public List<int>? AssigneeIds { get; set; }
    }
}
