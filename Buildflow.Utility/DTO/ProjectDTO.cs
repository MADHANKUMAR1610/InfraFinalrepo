using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Buildflow.Utility.DTO
{
    class ProjectDTO
    {
    }

    public class ProjectApprovalDto
    {
        public int ApprovedBy { get; set; }
        public int TicketId { get; set; }
        public string ApprovalType {  get; set; }   


    }
    //projectdto



    public class BoqItemsDto
    {
        [Column("boq_id")]
        public int? BoqId { get; set; }

        [Column("boq_items_id")]
        public int? BoqItemsId { get; set; }

        [Column("item_name")]
        public string? ItemName { get; set; }

        [Column("unit")]
        public string? Unit { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("quantity")]
        public int? Quantity { get; set; }

        [Column("total")]
        public double? Total { get; set; }
        
        [Column("approval_status")]
        public string? ApprovalStatus { get; set; }
    }

public class BoqDetailsFullDto
    {

        [Column("ticket_id")]
        public int? TicketId { get; set; }
        [Column("boq_id")]
        public int? BoqId { get; set; }

        [Column("boq_name")]
        public string? BoqName { get; set; }

        [Column("boq_description")]
        public string? BoqCode { get; set; }

        [Column("project_id")]
        public int? ProjectId { get; set; }

        [Column("project_name")]
        public string? ProjectName { get; set; }

        [Column("created_by")]
        public int? CreatedBy { get; set; }

        [Column("vendor_id")]
        public int? VendorId { get; set; }

        [Column("vendor_name")]
        public string? VendorName { get; set; }

        // Nested List of Items
        public List<BoqItemDetail> BoqItems { get; set; }
        public List<ApproverDto> Approvers { get; set; } = new();
    }

    public class BoqItemDetail
    {
        [Column("boq_items_id")]
        public int? BoqItemsId { get; set; }

        [Column("item_name")]
        public string? ItemName { get; set; }

        [Column("quantity")]
        public int? Quantity { get; set; }

        [Column("unit")]
        public string? Unit { get; set; }

        [Column("price")]
        public decimal? Price { get; set; }

        [Column("total")]
        public double? Total { get; set; }
    }
    public class UpsertBoqRequestDto
    {
        public int EmpId { get; set; }
        public int? BoqId { get; set; }  // null for insert
        public string BoqName { get; set; } = string.Empty;
        public string BoqCode { get; set; } = string.Empty;
        public List<BoqItemDto> BoqItems { get; set; } = new();
        public List<int> AssignTo { get; set; } = new();
        public string TicketType { get; set; } = string.Empty;
        public int VendorId { get; set; }
    }
    public class BoqItemDto
    {
        public int? BoqItemsId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
    public class GetProjectDetailsByIdDto
    {
        public List<ProjectInput> Project { get; set; }
        public List<ProjectBudgetInput> ProjectBudget { get; set; }
        public List<ProjectTeamUpsertDto> ProjectTeam { get; set; }
        public List<ProjectPermissionFinanceApprovalInput> ProjectPermissionFinanceApproval { get; set; }
        public List<ProjectMilestoneInput> ProjectMilestone { get; set; }
        public List<RiskManagementInput> RiskManagement { get; set; }
    }
    public class ProjectTeamInputDto
    {
        public int ProjectId { get; set; }
        public List<ProjectTeamDto> TeamList { get; set; } = new();
    }

    public class ProjectTeamUpsertDto
    {
        public int ProjectId { get; set; }
        //public int ProjectTeamId { get; set; }
        public List<int> PmId { get; set; }
        public List<int> ApmId { get; set; }
        public List<int> LeadEnggId { get; set; }
        public List<int> SiteSupervisorId { get; set; }
        public List<int> QsId { get; set; }
        public List<int> AqsId { get; set; }
        public List<int> SiteEnggId { get; set; }
        public List<int> EnggId { get; set; }
        public List<int> DesignerId { get; set; }
        public List<int> VendorId { get; set; }
        public List<int> SubcontractorId { get; set; }
    }


    public class ProjectTeamDto
    {
        public int? ProjectTeamId { get; set; }
        public List<int>? PmId { get; set; }
        public List<int>? ApmId { get; set; }
        public List<int>? LeadEnggId { get; set; }
        public List<int>? SiteSupervisorId { get; set; }
        public List<int>? QsId { get; set; }
        public List<int>? AqsId { get; set; }
        public List<int>? SiteEnggId { get; set; }
        public List<int>? EnggId { get; set; }
        public List<int>? DesignerId { get; set; }
        public List<int>? VendorId { get; set; }
        public List<int>? SubContractorId { get; set; }
        public DateTime AssignmentStartDate { get; set; }
        public DateTime AssignmentEndDate { get; set; }
    }


    public class ProjectTeam
    {
        public int ProjectId { get; set; }
        public List<int> PM { get; set; }
        public List<int> AM { get; set; }

    }

    public class ProjectInput
    {
        public int ProjectId { get; set; }  
        public string ProjectName { get; set; } = null!;

        public string? ProjectLocation { get; set; }

        //public string Location { get; set; } = null!; 

        //public string ProjectCode { get; set; }
        public int ProjectTypeId { get; set; }

        public int ProjectSectorId { get; set; }

        public DateOnly ProjectStartDate { get; set; }

        public DateOnly? ExpectedCompletionDate { get; set; }

        public string? Description { get; set; }
    }


    public class ProjectTypeDTO
    {
        public int Id { get; set; }
        public string ProjectTypeName { get; set; }
    }

    public class ProjectSectorDTO
    {
        public int Id { get; set; }
        public string ProjectSectorName { get; set; }
    }

    public class ProjectTypeAndSector
    {
        public List<ProjectTypeDTO> ProjectTypes { get; set; }
        public List<ProjectSectorDTO> ProjectSectors { get; set; }
    }

    public class ProjectBudgetInput
    {
        public int ProjectBudgetId { get; set; }
        public string ProjectExpenseCategory { get; set; } = null!;
        public decimal EstimatedCost { get; set; }
        public decimal? ApprovedBudget { get; set; }
       
        // public int ProjectBudgetMasterId { get; set; }
    }


    public class ProjectBudgetOutputDto
    {
        public int ProjectBudgetId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectExpenseCategory { get; set; }
        public decimal EstimatedCost { get; set; }
        public decimal ApprovedBudget { get; set; }
    }

    public class ProjectBudgetInputDto
    {
        public int ProjectId { get; set; }
        public decimal? TotalCost { get; set; }
        public IEnumerable<ProjectBudgetInput> ProjectBudgetList { get; set; } = new List<ProjectBudgetInput>();
    }


    public class ProjectMilestoneInput
    {
        //public int ProjectId { get; set; }
        public int MilestoneId { get; set; }

        public string? MilestoneName { get; set; } = null!;

        public string? MilestoneDescription { get; set; }
        
        public DateOnly? MilestoneStartDate { get; set; }

        public DateOnly? MilestoneEndDate { get; set; }

        public string? Status { get; set; } = null!;
        public string? Remarks { get; set; } = null!;
    }

    public class ProjectMilestoneInputDto
    {
        public int ProjectId{get; set;}
        //public ICollection<ProjectMilestoneInput> ProjectMilestoneList { get; set; } = new List<ProjectMilestoneInput>();
        public ICollection<ProjectMilestoneInput> MilestoneList { get; set; } = new List<ProjectMilestoneInput>();
    }

    public class ProjectPermissionFinanceApprovalInput
    {
        public int? EmpId { get; set; }
        public double? Amount { get; set; }
    }
    public class ProjectPermissionFinanceApprovalInputDto
    {
        public int ProjectId { get; set; }
        public IEnumerable<ProjectPermissionFinanceApprovalInput> ProjectPermissionFinanceApprovalList { get; set; } = new List<ProjectPermissionFinanceApprovalInput>();
    }

    public class RiskManagementInput
    {
        public int RiskId { get; set; }
       
        
        public string? CategoryName { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }
       
        public int ProjectId { get; set; }
       
        public IFormFile? UploadedFile { get; set; }
    }

    public class RiskManagementBulkInput
    {
        public List<RiskManagementInput> Risks { get; set; }
    }



    public enum ProjectStatus
    {
        Planned,
        InProgress,
        Completed,
        OnHhold,
        Cancelled,
        Approved,
        NotApproved

    }

    public class ProjectBudgetData
    {
        public int ProjectBudgetId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectExpenseCategory { get; set; }
        public decimal EstimatedCost { get; set; }
        public decimal ApprovedBudget { get; set; }
    }

    public class ProjectTeamData
    {
        public List<string>? Pm { get; set; }
        public List<string>? Apm { get; set; }
        public List<string>? LeadEngg { get; set; }
        public List<string>? SiteSupervisor { get; set; }
        public List<string>? Qs { get; set; }
        public List<string>? Aqs { get; set; }
        public List<string>? SiteEngg { get; set; }
        public List<string>? Engg { get; set; }
        public List<string>? Designer { get; set; }
        public List<string>? Vendors { get; set; }
        public List<string>? Subcontractors { get; set; }
    }

    public class FinanceApprovalData
    {
        public int PermissionFinanceApprovalId { get; set; }
        public int ProjectId { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public decimal Amount { get; set; }
    }

    public class ProjectMilestoneData
    {
        public int MilestoneId { get; set; }
        public int ProjectId { get; set; }
        public string MilestoneName { get; set; }
        public string MilestoneDescription { get; set; }
        public DateTime MilestoneStartDate { get; set; }
        public DateTime MilestoneEndDate { get; set; }
        public string MilestoneStatus { get; set; }
    }
    public class ProjectData
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public int ProjectTypeId { get; set; }
        public string ProjectTypeName { get; set; }
        public int ProjectSectorId { get; set; }
        public string ProjectSectorName { get; set; }
        public string ProjectStatus { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
        public decimal ProjectTotalBudget { get; set; }
        public string? ProjectLocation { get; set; }
        }

    public class RiskManagementData
    {
        public int RiskId { get; set; }
        public int ProjectId { get; set; }
        public string CategoryName { get; set; }
        public string RiskDescription { get; set; }
        public string RiskStatus { get; set; }
        public string ImageUrl { get; set; }
    }

    public class ProjectDetailsResultDto
    {
        public ProjectData Project { get; set; }
        public List<ProjectBudgetData> BudgetDetails { get; set; }
        public ProjectTeamData TeamDetails { get; set; }
        public List<FinanceApprovalData> FinanceApprovalData { get; set; }
        public List<ProjectMilestoneData> MilestoneDetails { get; set; }
        public List<RiskManagementData> RiskManagementData { get; set; }

    }

    public class ProjectDto
    {
        public int project_id { get; set; }
        public string project_name { get; set; }
        public string project_description { get; set; }
        public int project_type_id { get; set; }

        public string project_type_name { get; set; }
        public int project_sector_id { get; set; }
        public string project_sector_name { get; set; }
        public string project_status { get; set; }
        public DateTime project_start_date { get; set; }
        public DateTime project_end_date { get; set; }
        public decimal project_total_budget { get; set; }
        public decimal? project_actual_cost { get; set; }
        public string client_name { get; set; }
        public string project_location { get; set; }

        // These are returned as strings from PostgreSQL and will be parsed manually.
        public string vendor_names { get; set; }
        public string subcontractor_names { get; set; }
        public string project_manager_names { get;set; }

        public int ticket_count { get; set; }

        
     
    }





}
