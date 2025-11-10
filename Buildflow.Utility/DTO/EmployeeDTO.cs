using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{

    public class LoginEmployeeFullDetailDto
    {
        [Column("emp_id")]
        public int EmpId { get; set; }

        [Column("employee_code")]
        public string EmployeeCode { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("middle_name")]
        public string MiddleName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("role_id")]
        public int? RoleId { get; set; }

        [Column("role_name")]
        public string RoleName { get; set; }


        [Column("rolecode")]
        public string RoleCode { get; set; }

        [Column("dept_id")]
        public int? DeptId { get; set; }

        [Column("dept_name")]
        public string DeptName { get; set; }

        public BoardData Board { get; set; }
        public List<BoardLabelData> BoardLabels { get; set; } = new();

        public List<TicketDetail> Tickets { get; set; }
        public List<UserNotifications> Notifications { get; set; }
        public List<ProjectDetails> Projects { get; set; }

    }

    public class BoardData
    {
        public int BoardId { get; set; }
        public string BoardName { get; set; }
        public string BoardDescription { get; set; }
        public List<BoardLabelData> Labels { get; set; } = new();
    }
    public class BoardLabelData
    {
        public int? BoardId { get; set; }
        public int LabelId { get; set; }
        public string LabelName { get; set; }

        public List<TicketDetail> Tickets { get; set; } = new();
    }
    public class ProjectDetails
    {
        public int? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public int? ProjectTypeId { get; set; }
        public string? ProjectTypeName { get; set; }
        public int? ProjectSectorId { get; set; }
        public string? ProjectSectorName { get; set; }
        public string? ProjectStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ClientName { get; set; }
        public string? VendorName { get; set; }
        public string? SubcontractorName { get; set; }
    }
    public class UserNotifications
    {
        public int notificationId { get; set; }

        public int emp_id { get; set; }

        public bool is_read { get; set; }

        public string message {  get; set; }
    }
    public class TicketDetail
    {
        public int TicketId { get; set; }
        public string TicketNo { get; set; }
        public string TicketName { get; set; }
        
        public string TicketDescription { get; set; }
        public DateTime TicketCreatedDate { get; set; }
        public string TicketType { get; set; }
        public int BoardId { get; set; }
        public string BoardName { get; set; }
        public string BoardDescription { get; set; }
        
    }

    public class EmployeeData
    {
        public int EmpId { get; set; }
        public int RoleId { get; set; }



        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }

        public string? Role { get; set; }
        public string? Rolecode { get; set; }
        public bool? IsAllocated { get; set; }

    }

    public class EmployeeDataList
    {
        public Dictionary<string, List<EmployeeData>> EmployeesByRole { get; set; } = new();

    }

    public class VendorDTO
    {
        public int Id { get; set; }
        public string VendorName { get; set; }
    }

    public class SubcontractorDTO
    {
        public int Id { get; set; }
        public string SubcontractorName { get; set; }
    }

    public class VendorAndSubcontractor
    {
        public List<VendorDTO> Vendors { get; set; }
        public List<SubcontractorDTO> Subcontractors { get; set; }
    }

    public class EmployeeByRoleDto
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public int EmpId { get; set; }
    }

    public class EmployeeByDeptDto
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string DeptName { get; set; } = string.Empty;
        public int EmpId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class EmployeeDTO
    {
        public int EmpId { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Department { get; set; }
        public int Designation { get; set; }
        public int Project { get; set; }
        public string EmployeeCode { get; set; }
        public string Gender { get; set; }
    }
  
    public class GetEmployeeDto
    {
        public int EmpId { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string Phone { get; set; } = null!;
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string Email { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public bool IsAllocated { get; set; }
        public string RoleCode {  get; set; }




    }

}
