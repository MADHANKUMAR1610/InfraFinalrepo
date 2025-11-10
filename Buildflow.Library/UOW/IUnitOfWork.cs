using Buildflow.Library.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        IReportRepository reportRepository { get; }
        IVendorRepository Vendors { get; }
        INotificationRepository NotificationRepository { get; }
        IProjectRepository ProjectTeam { get; }
        IRegisterRepository Employees { get; }
        IRegisterRepository LoginEmployee { get; }
        IRegisterRepository EmployeeRoles { get; }
        IProjectRepository Boq { get; }
        IRegisterRepository RegisterUser { get; }
        IRegisterRepository VendorDetails { get; }
        IRegisterRepository SubcontractorDetails { get; }
        IProjectRepository ProjectPermissionFinanceApprovals { get; }

        IRoleRepository Roles { get; }
        IDepartmentRepository DepartmentRepository { get; }
        IProjectRepository Projects { get; }
        IProjectRepository ProjectTypes { get; }
        IProjectRepository ProjectSectors { get; }
        IProjectRepository ProjectBudgets { get; }
        IProjectRepository ProjectMilestone { get; }
        IProjectRepository ProjectMilestones { get; }
        ITicketRepository TicketRepository { get; }

        IEmployeeRepository EmployeeRepository { get; }
        Task<int> CompleteAsync();
    }
}
