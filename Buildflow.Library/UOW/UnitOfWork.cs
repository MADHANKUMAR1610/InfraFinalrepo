using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository;
using Buildflow.Library.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Buildflow.Library.UOW
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly BuildflowAppContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private readonly IConfiguration _configuration;

        // Existing Repositories
        public IProjectRepository Boq { get; private set; }
        public IReportRepository ReportRepository { get; private set; }
        public INotificationRepository NotificationRepository { get; private set; }
        public IEmployeeRepository EmployeeRepository { get; private set; }
        public IProjectRepository ProjectTeam { get; private set; }
        public IRegisterRepository Employees { get; private set; }
        public IRegisterRepository LoginEmployee { get; private set; }
        public IRegisterRepository EmployeeRoles { get; private set; }
        public IRegisterRepository RegisterUser { get; private set; }
        public IRegisterRepository VendorDetails { get; private set; }
        public IRegisterRepository SubcontractorDetails { get; private set; }
        public IRoleRepository Roles { get; private set; }
        public IDepartmentRepository DepartmentRepository { get; private set; }
        public IVendorRepository Vendors { get; private set; }
        public IProjectRepository Projects { get; private set; }
        public IProjectRepository ProjectTypes { get; private set; }
        public IProjectRepository ProjectSectors { get; private set; }
        public IProjectRepository ProjectBudgets { get; private set; }
        public IProjectRepository ProjectMilestone { get; private set; }
        public IProjectRepository ProjectMilestones { get; private set; }
        public IProjectRepository ProjectPermissionFinanceApprovals { get; private set; }
        public ITicketRepository TicketRepository { get; private set; }
        public IInventoryRepository InventoryRepository { get; private set; }
        public IMaterialRepository MaterialRepository { get; private set; }

        // ✅ New ones
        public IMaterialStockAlertRepository MaterialStockAlertRepository { get; private set; }

        public IReportRepository reportRepository => throw new NotImplementedException();

        public IMaterialStatusRepository MaterialStatusRepository => throw new NotImplementedException();

        IMaterialStockAlertRepository IUnitOfWork.MaterialStockAlertRepository => throw new NotImplementedException();

        public UnitOfWork(
            BuildflowAppContext context,
            IConfiguration configuration,
            ILogger<UnitOfWork> logger,
            ILogger<RegisterRepository> registerLogger,
            ILogger<ProjectRepository> projectLogger,
            ILogger<GenericRepository<Notification>> notificationLogger,
            ILogger<GenericRepository<EmployeeDetail>> employeeLogger,
            ILogger<GenericRepository<Report>> reportLogger,
            ILogger<GenericRepository<Ticket>> ticketLogger,
            ILogger<GenericRepository<Vendor>> vendorLogger,
            ILogger<GenericRepository<StockInward>> inventoryLogger,
            IRoleRepository roles,
            IDepartmentRepository departments
        )
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;

            // --- Existing Repository Initializations ---
            TicketRepository = new TicketRepository(configuration, context, ticketLogger);
            EmployeeRepository = new EmployeeRepository(configuration, context, employeeLogger);
            ReportRepository = new ReportRepository(configuration, context, reportLogger);
            Boq = new ProjectRepository(configuration, context, projectLogger);
            Roles = roles;
            Vendors = new VendorRepository(configuration, context, vendorLogger);
            DepartmentRepository = departments;

            Employees = new RegisterRepository(configuration, context, registerLogger);
            RegisterUser = new RegisterRepository(configuration, context, registerLogger);
            EmployeeRoles = new RegisterRepository(configuration, context, registerLogger);
            LoginEmployee = new RegisterRepository(configuration, context, registerLogger);
            VendorDetails = new RegisterRepository(configuration, context, registerLogger);
            SubcontractorDetails = new RegisterRepository(configuration, context, registerLogger);

            Projects = new ProjectRepository(configuration, context, projectLogger);
            ProjectTypes = new ProjectRepository(configuration, context, projectLogger);
            ProjectSectors = new ProjectRepository(configuration, context, projectLogger);
            ProjectBudgets = new ProjectRepository(configuration, context, projectLogger);
            ProjectTeam = new ProjectRepository(configuration, context, projectLogger);
            ProjectPermissionFinanceApprovals = new ProjectRepository(configuration, context, projectLogger);
            ProjectMilestone = new ProjectRepository(configuration, context, projectLogger);
            ProjectMilestones = new ProjectRepository(configuration, context, projectLogger);

            NotificationRepository = new NotificationRepository(configuration, context, notificationLogger);
            InventoryRepository = new InventoryRepository(configuration, context, inventoryLogger);
            MaterialRepository = new MaterialRepository(configuration, context, new LoggerFactory().CreateLogger<MaterialRepository>());

            // ✅ Register new MaterialStockAlert repository
            MaterialStockAlertRepository = new MaterialStockAlertRepository(
                configuration,
                context,
                new LoggerFactory().CreateLogger<MaterialStockAlertRepository>()
            );
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
