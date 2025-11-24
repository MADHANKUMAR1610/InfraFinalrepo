using System;
using System.Collections.Generic;
using Buildflow.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Buildflow.Infrastructure.DatabaseContext;

public partial class BuildflowAppContext : DbContext
{
    public BuildflowAppContext()
    {
    }

    public BuildflowAppContext(DbContextOptions<BuildflowAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Board> Boards { get; set; }

    public virtual DbSet<Boardlabel> Boardlabels { get; set; }

    public virtual DbSet<Boq> Boqs { get; set; }

    public virtual DbSet<BoqApproval> BoqApprovals { get; set; }

    public virtual DbSet<BoqItem> BoqItems { get; set; }

    public virtual DbSet<DailyStock> DailyStocks { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DepartmentRoleMapping> DepartmentRoleMappings { get; set; }

    public virtual DbSet<EmployeeDepartment> EmployeeDepartments { get; set; }

    public virtual DbSet<EmployeeDetail> EmployeeDetails { get; set; }

    public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Materialcategory> Materialcategories { get; set; }

    public virtual DbSet<Milestonemaster> Milestonemasters { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectApproval> ProjectApprovals { get; set; }

    public virtual DbSet<ProjectBudget> ProjectBudgets { get; set; }

    public virtual DbSet<ProjectBudgetDetail> ProjectBudgetDetails { get; set; }

    public virtual DbSet<ProjectDetail> ProjectDetails { get; set; }

    public virtual DbSet<ProjectMilestone> ProjectMilestones { get; set; }

    public virtual DbSet<ProjectPermissionFinanceApproval> ProjectPermissionFinanceApprovals { get; set; }

    public virtual DbSet<ProjectRiskManagement> ProjectRiskManagements { get; set; }

    public virtual DbSet<ProjectSector> ProjectSectors { get; set; }

    public virtual DbSet<ProjectTask> ProjectTasks { get; set; }

    public virtual DbSet<ProjectTeam> ProjectTeams { get; set; }

    public virtual DbSet<ProjectType> ProjectTypes { get; set; }

    public virtual DbSet<Projectstatusmaster> Projectstatusmasters { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<PurchaseOrderApproval> PurchaseOrderApprovals { get; set; }

    public virtual DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

    public virtual DbSet<Refreshtoken> Refreshtokens { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<ReportAssignee> ReportAssignees { get; set; }

    public virtual DbSet<ReportAttachment> ReportAttachments { get; set; }

    public virtual DbSet<ReportTypeMaster> ReportTypeMasters { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StockInward> StockInwards { get; set; }

    public virtual DbSet<StockOutward> StockOutwards { get; set; }

    public virtual DbSet<Subcontractor> Subcontractors { get; set; }

    public virtual DbSet<Taskstatusmaster> Taskstatusmasters { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketAssignee> TicketAssignees { get; set; }

    public virtual DbSet<TicketAttachment> TicketAttachments { get; set; }

    public virtual DbSet<TicketComment> TicketComments { get; set; }

    public virtual DbSet<TicketMovement> TicketMovements { get; set; }

    public virtual DbSet<TicketParticipant> TicketParticipants { get; set; }

    public virtual DbSet<VBoardId> VBoardIds { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    public virtual DbSet<VendorDepartment> VendorDepartments { get; set; }

    public virtual DbSet<Vendorrole> Vendorroles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=103.91.186.169;Database=Infratracker_dev;Username=postgres;Password=Admin@123;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(e => e.BoardId).HasName("board_pkey");

            entity.ToTable("board", "board");

            entity.Property(e => e.BoardId).HasColumnName("board_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BoardCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("fk_board_created_by");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BoardUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("fk_board_updated_by");
        });

        modelBuilder.Entity<Boardlabel>(entity =>
        {
            entity.HasKey(e => e.Labelid).HasName("boardlabel_pkey");

            entity.ToTable("boardlabel", "board");

            entity.Property(e => e.Labelid).HasColumnName("labelid");
            entity.Property(e => e.BoardId).HasColumnName("board_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.Colorcode)
                .HasMaxLength(20)
                .HasColumnName("colorcode");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDefault)
                .HasDefaultValue(false)
                .HasColumnName("is_default");
            entity.Property(e => e.IsMoveState)
                .HasDefaultValue(false)
                .HasColumnName("is_move_state");
            entity.Property(e => e.Isexpand)
                .HasDefaultValue(false)
                .HasColumnName("isexpand");
            entity.Property(e => e.LastUpdatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_updated_date");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Position).HasColumnName("position");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.Wiplimit).HasColumnName("wiplimit");

            entity.HasOne(d => d.Board).WithMany(p => p.Boardlabels)
                .HasForeignKey(d => d.BoardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_boardlabel_board");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BoardlabelCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("fk_boardlabel_created_by");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BoardlabelUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("fk_boardlabel_updated_by");
        });

        modelBuilder.Entity<Boq>(entity =>
        {
            entity.HasKey(e => e.BoqId).HasName("boq_pkey");

            entity.ToTable("boq", "project");

            entity.Property(e => e.BoqId).HasColumnName("boq_id");
            entity.Property(e => e.BoqCode).HasColumnName("boq_code");
            entity.Property(e => e.BoqName)
                .HasMaxLength(255)
                .HasColumnName("boq_name");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.VendorId).HasColumnName("vendor_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Boqs)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("fk_boq_emp");

            entity.HasOne(d => d.Project).WithMany(p => p.Boqs)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("fk_project_boq");

            entity.HasOne(d => d.Vendor).WithMany(p => p.Boqs)
                .HasForeignKey(d => d.VendorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_vendor");
        });

        modelBuilder.Entity<BoqApproval>(entity =>
        {
            entity.HasKey(e => e.BoqApprovalId).HasName("boq_approval_pkey");

            entity.ToTable("boq_approval", "project");

            entity.Property(e => e.BoqApprovalId).HasColumnName("boq_approval_id");
            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(255)
                .HasColumnName("approval_status");
            entity.Property(e => e.ApprovedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("approved_at");
            entity.Property(e => e.BoqId).HasColumnName("boq_id");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Boq).WithMany(p => p.BoqApprovals)
                .HasForeignKey(d => d.BoqId)
                .HasConstraintName("fk_boq_approval_boq");

            entity.HasOne(d => d.Ticket).WithMany(p => p.BoqApprovals)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("fk_boq_ticket_id");
        });

        modelBuilder.Entity<BoqItem>(entity =>
        {
            entity.HasKey(e => e.BoqItemsId).HasName("boq_items_pkey");

            entity.ToTable("boq_items", "project");

            entity.Property(e => e.BoqItemsId).HasColumnName("boq_items_id");
            entity.Property(e => e.BoqId).HasColumnName("boq_id");
            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .HasColumnName("item_name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Total).HasColumnName("total");
            entity.Property(e => e.Unit)
                .HasMaxLength(255)
                .HasColumnName("unit");

            entity.HasOne(d => d.Boq).WithMany(p => p.BoqItems)
                .HasForeignKey(d => d.BoqId)
                .HasConstraintName("fk_boq_items_boq");
        });

        modelBuilder.Entity<DailyStock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("daily_stock_pkey");

            entity.ToTable("daily_stock", "inventory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("date");
            entity.Property(e => e.DefaultQty)
                .HasPrecision(18, 2)
                .HasColumnName("default_qty");
            entity.Property(e => e.InStock)
                .HasPrecision(18, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("inStock");
            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .HasColumnName("item_name");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.RemainingQty)
                .HasPrecision(18, 2)
                .HasColumnName("remaining_qty");

            entity.HasOne(d => d.Project).WithMany(p => p.DailyStocks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_daily_stock_project");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DeptId).HasName("department_pkey");

            entity.ToTable("department", "master");

            entity.HasIndex(e => e.DeptCode, "department_dept_code_key").IsUnique();

            entity.HasIndex(e => e.DeptName, "department_dept_name_key").IsUnique();

            entity.Property(e => e.DeptId).HasColumnName("dept_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeptCode)
                .HasMaxLength(20)
                .HasColumnName("dept_code");
            entity.Property(e => e.DeptDescription).HasColumnName("dept_description");
            entity.Property(e => e.DeptName)
                .HasMaxLength(100)
                .HasColumnName("dept_name");
            entity.Property(e => e.ParentDeptId).HasColumnName("parent_dept_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.ParentDept).WithMany(p => p.InverseParentDept)
                .HasForeignKey(d => d.ParentDeptId)
                .HasConstraintName("department_parent_dept_id_fkey");
        });

        modelBuilder.Entity<DepartmentRoleMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("department_role_mapping_pkey");

            entity.ToTable("department_role_mapping", "master");

            entity.HasIndex(e => new { e.DeptId, e.RoleId }, "department_role_mapping_dept_id_role_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DeptId).HasColumnName("dept_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Dept).WithMany(p => p.DepartmentRoleMappings)
                .HasForeignKey(d => d.DeptId)
                .HasConstraintName("department_role_mapping_dept_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.DepartmentRoleMappings)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("department_role_mapping_role_id_fkey");
        });

        modelBuilder.Entity<EmployeeDepartment>(entity =>
        {
            entity.HasKey(e => e.EmpDeptId).HasName("employee_department_pkey");

            entity.ToTable("employee_department", "employee");

            entity.HasIndex(e => new { e.EmpId, e.DeptId }, "unique_emp_dept").IsUnique();

            entity.Property(e => e.EmpDeptId).HasColumnName("emp_dept_id");
            entity.Property(e => e.AssignedDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("assigned_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeptId).HasColumnName("dept_id");
            entity.Property(e => e.EmpId).HasColumnName("emp_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IsPrimary)
                .HasDefaultValue(true)
                .HasColumnName("is_primary");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Dept).WithMany(p => p.EmployeeDepartments)
                .HasForeignKey(d => d.DeptId)
                .HasConstraintName("employee_department_dept_id_fkey");

            entity.HasOne(d => d.Emp).WithMany(p => p.EmployeeDepartments)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("employee_department_emp_id_fkey");
        });

        modelBuilder.Entity<EmployeeDetail>(entity =>
        {
            entity.HasKey(e => e.EmpId).HasName("employee_details_pkey");

            entity.ToTable("employee_details", "employee");

            entity.HasIndex(e => e.Email, "employee_details_email_key").IsUnique();

            entity.HasIndex(e => e.EmployeeCode, "employee_details_employee_code_key").IsUnique();

            entity.Property(e => e.EmpId).HasColumnName("emp_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.AlternativePhone)
                .HasMaxLength(20)
                .HasColumnName("alternative_phone");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(50)
                .HasColumnName("employee_code");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasColumnType("character varying")
                .HasColumnName("gender");
            entity.Property(e => e.HireDate).HasColumnName("hire_date");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsAllocated)
                .HasDefaultValue(false)
                .HasColumnName("is_allocated");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(100)
                .HasColumnName("middle_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("postal_code");
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .HasColumnName("state");
            entity.Property(e => e.TerminationDate).HasColumnName("termination_date");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<EmployeeRole>(entity =>
        {
            entity.HasKey(e => e.EmpRoleId).HasName("employee_role_pkey");

            entity.ToTable("employee_role", "employee");

            entity.HasIndex(e => new { e.EmpId, e.RoleId }, "unique_emp_role").IsUnique();

            entity.Property(e => e.EmpRoleId).HasColumnName("emp_role_id");
            entity.Property(e => e.AssignedDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("assigned_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.EmpId).HasColumnName("emp_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Emp).WithMany(p => p.EmployeeRoles)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("employee_role_emp_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.EmployeeRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("employee_role_role_id_fkey");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Materialid).HasName("material_pkey");

            entity.ToTable("material", "vendor");

            entity.Property(e => e.Materialid)
                .ValueGeneratedNever()
                .HasColumnName("materialid");
            entity.Property(e => e.Createdat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Currentprice).HasColumnName("currentprice");
            entity.Property(e => e.Material1).HasColumnName("material");
            entity.Property(e => e.Materialcategoryid).HasColumnName("materialcategoryid");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Unit).HasColumnName("unit");
            entity.Property(e => e.Updatedat)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updatedat");
            entity.Property(e => e.Updatedby).HasColumnName("updatedby");
            entity.Property(e => e.VendorId).HasColumnName("vendor_id");

            entity.HasOne(d => d.Materialcategory).WithMany(p => p.Materials)
                .HasForeignKey(d => d.Materialcategoryid)
                .HasConstraintName("material_materialcategoryid_fkey");

            entity.HasOne(d => d.Vendor).WithMany(p => p.Materials)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("material_vendor_id_fkey");
        });

        modelBuilder.Entity<Materialcategory>(entity =>
        {
            entity.HasKey(e => e.Materialcategoryid).HasName("materialcategory_pkey");

            entity.ToTable("materialcategory", "vendor");

            entity.Property(e => e.Materialcategoryid)
                .ValueGeneratedNever()
                .HasColumnName("materialcategoryid");
            entity.Property(e => e.Categoryname)
                .HasColumnType("character varying")
                .HasColumnName("categoryname");
        });

        modelBuilder.Entity<Milestonemaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("milestonemaster_pkey");

            entity.ToTable("milestonemaster", "project");

            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("notifications_pkey");

            entity.ToTable("notifications", "notification");

            entity.HasIndex(e => e.EmpId, "idx_notifications_emp_id");

            entity.HasIndex(e => new { e.EmpId, e.IsRead }, "idx_notifications_emp_id_is_read");

            entity.HasIndex(e => e.Timestamp, "idx_notifications_timestamp_desc").IsDescending();

            entity.HasIndex(e => e.NotificationType, "idx_notifications_type");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.EmpId).HasColumnName("emp_id");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.NotificationType)
                .HasMaxLength(255)
                .HasColumnName("notification_type");
            entity.Property(e => e.SourceEntityId).HasColumnName("source_entity_id");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.Emp).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("notifications_emp_id_fkey");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("project_pkey");

            entity.ToTable("project", "project");

            entity.HasIndex(e => e.ProjectCode, "project_project_code_key").IsUnique();

            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.ClientName)
                .HasMaxLength(200)
                .HasColumnName("client_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.ProjectActualCost)
                .HasPrecision(15, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("project_actual_cost");
            entity.Property(e => e.ProjectCode)
                .HasMaxLength(50)
                .HasColumnName("project_code");
            entity.Property(e => e.ProjectDescription).HasColumnName("project_description");
            entity.Property(e => e.ProjectEndDate).HasColumnName("project_end_date");
            entity.Property(e => e.ProjectLocation)
                .HasMaxLength(50)
                .HasColumnName("project_location");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(200)
                .HasColumnName("project_name");
            entity.Property(e => e.ProjectPriority)
                .HasMaxLength(20)
                .HasColumnName("project_priority");
            entity.Property(e => e.ProjectSectorId).HasColumnName("project_sector_id");
            entity.Property(e => e.ProjectStartDate).HasColumnName("project_start_date");
            entity.Property(e => e.ProjectStatus)
                .HasMaxLength(50)
                .HasColumnName("project_status");
            entity.Property(e => e.ProjectTotalBudget)
                .HasPrecision(15, 2)
                .HasColumnName("project_total_budget");
            entity.Property(e => e.ProjectTypeId).HasColumnName("project_type_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.ProjectSector).WithMany(p => p.Projects)
                .HasForeignKey(d => d.ProjectSectorId)
                .HasConstraintName("project_project_sector_id_fkey");

            entity.HasOne(d => d.ProjectType).WithMany(p => p.Projects)
                .HasForeignKey(d => d.ProjectTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("project_project_type_id_fkey");
        });

        modelBuilder.Entity<ProjectApproval>(entity =>
        {
            entity.HasKey(e => e.ApprovalId).HasName("project_approval_pkey");

            entity.ToTable("project_approval", "project");

            entity.Property(e => e.ApprovalId).HasColumnName("approval_id");
            entity.Property(e => e.ApprovalDate).HasColumnName("approval_date");
            entity.Property(e => e.ApprovalType)
                .HasMaxLength(50)
                .HasColumnName("approval_type");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DocumentUrl)
                .HasMaxLength(255)
                .HasColumnName("document_url");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.ProjectApprovals)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("project_approval_approved_by_fkey");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectApprovals)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("project_approval_project_id_fkey");

            entity.HasOne(d => d.Ticket).WithMany(p => p.ProjectApprovals)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("project_approval_ticket_id_fkey");
        });

        modelBuilder.Entity<ProjectBudget>(entity =>
        {
            entity.HasKey(e => e.ProjectBudgetId).HasName("project_budget_pkey");

            entity.ToTable("project_budget", "master");

            entity.Property(e => e.ProjectBudgetId).HasColumnName("project_budget_id");
            entity.Property(e => e.ProjectExpenseCategory)
                .HasMaxLength(100)
                .HasColumnName("project_expense_category");
        });

        modelBuilder.Entity<ProjectBudgetDetail>(entity =>
        {
            entity.HasKey(e => e.ProjectBudgetId).HasName("pk_project_budget_id");

            entity.ToTable("project_budget_details", "project");

            entity.Property(e => e.ProjectBudgetId).HasColumnName("project_budget_id");
            entity.Property(e => e.ApprovedBudget)
                .HasPrecision(12, 2)
                .HasColumnName("approved_budget");
            entity.Property(e => e.EstimatedCost)
                .HasPrecision(12, 2)
                .HasColumnName("estimated_cost");
            entity.Property(e => e.ProjectExpenseCategory)
                .HasMaxLength(100)
                .HasColumnName("project_expense_category");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectBudgetDetails)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("fk_project");
        });

        modelBuilder.Entity<ProjectDetail>(entity =>
        {
            entity.HasKey(e => e.ProjectDetailId).HasName("project_details_pkey");

            entity.ToTable("project_details", "project");

            entity.Property(e => e.ProjectDetailId).HasColumnName("project_detail_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.LandArea)
                .HasPrecision(15, 2)
                .HasColumnName("land_area");
            entity.Property(e => e.LandAreaUnit)
                .HasMaxLength(20)
                .HasDefaultValueSql("'sqft'::character varying")
                .HasColumnName("land_area_unit");
            entity.Property(e => e.Latitude)
                .HasPrecision(10, 8)
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasPrecision(11, 8)
                .HasColumnName("longitude");
            entity.Property(e => e.PlotNumber)
                .HasMaxLength(100)
                .HasColumnName("plot_number");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.ProjectLocation).HasColumnName("project_location");
            entity.Property(e => e.SiteAddress).HasColumnName("site_address");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.ZoningType)
                .HasMaxLength(100)
                .HasColumnName("zoning_type");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectDetails)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("project_details_project_id_fkey");
        });

        modelBuilder.Entity<ProjectMilestone>(entity =>
        {
            entity.HasKey(e => e.MilestoneId).HasName("project_milestone_pkey");

            entity.ToTable("project_milestone", "project");

            entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");
            entity.Property(e => e.ActualCost)
                .HasPrecision(15, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("actual_cost");
            entity.Property(e => e.CompletionPercentage)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("completion_percentage");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.MilestoneBudget)
                .HasPrecision(15, 2)
                .HasColumnName("milestone_budget");
            entity.Property(e => e.MilestoneDescription).HasColumnName("milestone_description");
            entity.Property(e => e.MilestoneEndDate).HasColumnName("milestone_end_date");
            entity.Property(e => e.MilestoneName)
                .HasMaxLength(200)
                .HasColumnName("milestone_name");
            entity.Property(e => e.MilestoneStartDate).HasColumnName("milestone_start_date");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectMilestones)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("project_milestone_project_id_fkey");
        });

        modelBuilder.Entity<ProjectPermissionFinanceApproval>(entity =>
        {
            entity.HasKey(e => e.PermissionFinanceApprovalId).HasName("project_permission_finance_approval_pkey");

            entity.ToTable("project_permission_finance_approval", "project");

            entity.Property(e => e.PermissionFinanceApprovalId)
                .HasDefaultValueSql("nextval('project.project_permission_finance_ap_permission_finance_approval_i_seq'::regclass)")
                .HasColumnName("permission_finance_approval_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.EmpId).HasColumnName("emp_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");

            entity.HasOne(d => d.Emp).WithMany(p => p.ProjectPermissionFinanceApprovals)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("project_permission_finance_approval_emp_id_fkey");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectPermissionFinanceApprovals)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("project_permission_finance_approval_project_id_fkey");
        });

        modelBuilder.Entity<ProjectRiskManagement>(entity =>
        {
            entity.HasKey(e => e.RiskId).HasName("project_risk_management_pkey");

            entity.ToTable("project_risk_management", "project");

            entity.Property(e => e.RiskId).HasColumnName("risk_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IdentifiedDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("identified_date");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.MitigationPlan).HasColumnName("mitigation_plan");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.ResolvedDate).HasColumnName("resolved_date");
            entity.Property(e => e.RiskDescription).HasColumnName("risk_description");
            entity.Property(e => e.RiskImpact)
                .HasMaxLength(50)
                .HasColumnName("risk_impact");
            entity.Property(e => e.RiskProbability)
                .HasPrecision(5, 2)
                .HasColumnName("risk_probability");
            entity.Property(e => e.RiskStatus)
                .HasMaxLength(50)
                .HasColumnName("risk_status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectRiskManagements)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("project_risk_management_project_id_fkey");
        });

        modelBuilder.Entity<ProjectSector>(entity =>
        {
            entity.HasKey(e => e.ProjectSectorId).HasName("project_sector_pkey");

            entity.ToTable("project_sector", "master");

            entity.HasIndex(e => e.ProjectSectorName, "project_sector_project_sector_name_key").IsUnique();

            entity.HasIndex(e => e.SectorCode, "project_sector_sector_code_key").IsUnique();

            entity.Property(e => e.ProjectSectorId).HasColumnName("project_sector_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ProjectSectorDescription).HasColumnName("project_sector_description");
            entity.Property(e => e.ProjectSectorName)
                .HasMaxLength(100)
                .HasColumnName("project_sector_name");
            entity.Property(e => e.SectorCode)
                .HasMaxLength(20)
                .HasColumnName("sector_code");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("project_task_pkey");

            entity.ToTable("project_task", "project");

            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DelayedDays).HasColumnName("delayed_days");
            entity.Property(e => e.DurationDays).HasColumnName("duration_days");
            entity.Property(e => e.FinishedDate).HasColumnName("finished_date");
            entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");
            entity.Property(e => e.PlannedEndDate).HasColumnName("planned_end_date");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TaskCode)
                .HasMaxLength(100)
                .HasColumnName("task_code");
            entity.Property(e => e.TaskName)
                .HasMaxLength(500)
                .HasColumnName("task_name");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.Milestone).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.MilestoneId)
                .HasConstraintName("fk_milestone");
        });

        modelBuilder.Entity<ProjectTeam>(entity =>
        {
            entity.HasKey(e => e.ProjectTeamId).HasName("project_team_pkey");

            entity.ToTable("project_team", "project");

            entity.Property(e => e.ProjectTeamId).HasColumnName("project_team_id");
            entity.Property(e => e.ApmId).HasColumnName("apm_id");
            entity.Property(e => e.AqsId).HasColumnName("aqs_id");
            entity.Property(e => e.AssignmentEndDate).HasColumnName("assignment_end_date");
            entity.Property(e => e.AssignmentStartDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("assignment_start_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DesignerId).HasColumnName("designer_id");
            entity.Property(e => e.EnggId).HasColumnName("engg_id");
            entity.Property(e => e.LeadEnggId).HasColumnName("lead_engg_id");
            entity.Property(e => e.PmId).HasColumnName("pm_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.QsId).HasColumnName("qs_id");
            entity.Property(e => e.SiteEnggId).HasColumnName("site_engg_id");
            entity.Property(e => e.SiteSupervisorId).HasColumnName("site_supervisor_id");
            entity.Property(e => e.SubcontractorId).HasColumnName("subcontractor_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.VendorId).HasColumnName("vendor_id");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectTeams)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("project_team_project_id_fkey");
        });

        modelBuilder.Entity<ProjectType>(entity =>
        {
            entity.HasKey(e => e.ProjectTypeId).HasName("project_types_pkey");

            entity.ToTable("project_types", "master");

            entity.HasIndex(e => e.ProjectTypeName, "project_types_project_type_name_key").IsUnique();

            entity.HasIndex(e => e.TypeCode, "project_types_type_code_key").IsUnique();

            entity.Property(e => e.ProjectTypeId).HasColumnName("project_type_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.ProjectTypeDescription).HasColumnName("project_type_description");
            entity.Property(e => e.ProjectTypeName)
                .HasMaxLength(100)
                .HasColumnName("project_type_name");
            entity.Property(e => e.TypeCode)
                .HasMaxLength(20)
                .HasColumnName("type_code");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<Projectstatusmaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("projectstatusmaster_pkey");

            entity.ToTable("projectstatusmaster", "project");

            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderId).HasName("purchase_order_pkey");

            entity.ToTable("purchase_order", "vendor");

            entity.Property(e => e.PurchaseOrderId).HasColumnName("purchase_order_id");
            entity.Property(e => e.BoqId).HasColumnName("boq_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeliveryStatus)
                .HasMaxLength(255)
                .HasColumnName("delivery_status");
            entity.Property(e => e.DeliveryStatusDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("delivery_status_date");
            entity.Property(e => e.PoId)
                .HasMaxLength(255)
                .HasColumnName("po_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.VendorId).HasColumnName("vendor_id");

            entity.HasOne(d => d.Boq).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.BoqId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_po_boq");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PurchaseOrders)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("fk_po_emp");
        });

        modelBuilder.Entity<PurchaseOrderApproval>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderApprovalId).HasName("purchase_order_approval_pkey");

            entity.ToTable("purchase_order_approval", "vendor");

            entity.Property(e => e.PurchaseOrderApprovalId).HasColumnName("purchase_order_approval_id");
            entity.Property(e => e.ApprovalStatus)
                .HasMaxLength(255)
                .HasColumnName("approval_status");
            entity.Property(e => e.PurchaseOrderId).HasColumnName("purchase_order_id");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseOrderApprovals)
                .HasForeignKey(d => d.PurchaseOrderId)
                .HasConstraintName("fk_po_approval_order");

            entity.HasOne(d => d.Ticket).WithMany(p => p.PurchaseOrderApprovals)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("fk_po_approval_ticket");
        });

        modelBuilder.Entity<PurchaseOrderItem>(entity =>
        {
            entity.HasKey(e => e.PurchaseOrderItemsId).HasName("purchase_order_items_pkey");

            entity.ToTable("purchase_order_items", "vendor");

            entity.Property(e => e.PurchaseOrderItemsId).HasColumnName("purchase_order_items_id");
            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .HasColumnName("item_name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.PurchaseOrderId).HasColumnName("purchase_order_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Total).HasColumnName("total");
            entity.Property(e => e.Unit)
                .HasMaxLength(255)
                .HasColumnName("unit");

            entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseOrderItems)
                .HasForeignKey(d => d.PurchaseOrderId)
                .HasConstraintName("fk_po_items_order");
        });

        modelBuilder.Entity<Refreshtoken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refreshtoken_pkey");

            entity.ToTable("refreshtoken", "employee");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Expirydate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expirydate");
            entity.Property(e => e.Isrevoked)
                .HasDefaultValue(false)
                .HasColumnName("isrevoked");
            entity.Property(e => e.Token).HasColumnName("token");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("reports_pkey");

            entity.ToTable("reports", "report");

            entity.Property(e => e.ReportId).HasColumnName("report_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.ReportCode)
                .HasColumnType("character varying")
                .HasColumnName("report_code");
            entity.Property(e => e.ReportData)
                .HasColumnType("jsonb")
                .HasColumnName("report_data");
            entity.Property(e => e.ReportDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("report_date");
            entity.Property(e => e.ReportType).HasColumnName("report_type");
            entity.Property(e => e.ReportedBy)
                .HasColumnType("character varying")
                .HasColumnName("reported_by");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ReportAssignee>(entity =>
        {
            entity.HasKey(e => e.ReportAssigneeId).HasName("report_assignee_pkey");

            entity.ToTable("report_assignee", "report");

            entity.HasIndex(e => new { e.ReportId, e.AssigneeId }, "uq_report_assignee").IsUnique();

            entity.Property(e => e.ReportAssigneeId).HasColumnName("report_assignee_id");
            entity.Property(e => e.AssigneeId).HasColumnName("assignee_id");
            entity.Property(e => e.ReportId).HasColumnName("report_id");
            entity.Property(e => e.SendBy).HasColumnName("send_by");

            entity.HasOne(d => d.Assignee).WithMany(p => p.ReportAssigneeAssignees)
                .HasForeignKey(d => d.AssigneeId)
                .HasConstraintName("fk_assignee");

            entity.HasOne(d => d.Report).WithMany(p => p.ReportAssignees)
                .HasForeignKey(d => d.ReportId)
                .HasConstraintName("fk_report");

            entity.HasOne(d => d.SendByNavigation).WithMany(p => p.ReportAssigneeSendByNavigations)
                .HasForeignKey(d => d.SendBy)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_send_by");
        });

        modelBuilder.Entity<ReportAttachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("report_attachment_pkey");

            entity.ToTable("report_attachment", "report");

            entity.Property(e => e.AttachmentId).HasColumnName("attachment_id");
            entity.Property(e => e.FileName).HasColumnName("file_name");
            entity.Property(e => e.FilePath).HasColumnName("file_path");
            entity.Property(e => e.ReportId).HasColumnName("report_id");
            entity.Property(e => e.UploadedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("uploaded_at");

            entity.HasOne(d => d.Report).WithMany(p => p.ReportAttachments)
                .HasForeignKey(d => d.ReportId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("attachment_report_id");
        });

        modelBuilder.Entity<ReportTypeMaster>(entity =>
        {
            entity.HasKey(e => e.ReportTypeId).HasName("report_type_master_pkey");

            entity.ToTable("report_type_master", "report");

            entity.Property(e => e.ReportTypeId).HasColumnName("report_type_id");
            entity.Property(e => e.ReportType)
                .HasMaxLength(255)
                .HasColumnName("report_type");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("role_pkey");

            entity.ToTable("role", "master");

            entity.HasIndex(e => e.RoleName, "role_role_name_key").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.IsSystemRole)
                .HasDefaultValue(false)
                .HasColumnName("is_system_role");
            entity.Property(e => e.RoleDescription).HasColumnName("role_description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .HasColumnName("role_name");
            entity.Property(e => e.Rolecode)
                .HasMaxLength(50)
                .HasColumnName("rolecode");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        });

        modelBuilder.Entity<StockInward>(entity =>
        {
            entity.HasKey(e => e.StockinwardId).HasName("stock_inward_pkey");

            entity.ToTable("stock_inward", "inventory");

            entity.Property(e => e.StockinwardId).HasColumnName("stockinward_id");
            entity.Property(e => e.DateReceived).HasColumnName("date_received");
            entity.Property(e => e.Grn)
                .HasMaxLength(100)
                .HasColumnName("grn");
            entity.Property(e => e.Itemname)
                .HasMaxLength(200)
                .HasColumnName("itemname");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.QuantityReceived)
                .HasPrecision(18, 2)
                .HasColumnName("quantity_received");
            entity.Property(e => e.ReceivedbyId).HasColumnName("receivedby_id");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");
            entity.Property(e => e.VendorId).HasColumnName("vendor_id");

            entity.HasOne(d => d.Project).WithMany(p => p.StockInwards)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("fk_stock_inward_project");

            entity.HasOne(d => d.Receivedby).WithMany(p => p.StockInwards)
                .HasForeignKey(d => d.ReceivedbyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_stock_inward_employee");

            entity.HasOne(d => d.Vendor).WithMany(p => p.StockInwards)
                .HasForeignKey(d => d.VendorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_stock_inward_vendor");
        });

        modelBuilder.Entity<StockOutward>(entity =>
        {
            entity.HasKey(e => e.StockOutwardId).HasName("stock_outward_pkey");

            entity.ToTable("stock_outward", "inventory");

            entity.Property(e => e.StockOutwardId).HasColumnName("stock_outward_id");
            entity.Property(e => e.DateIssued).HasColumnName("date_issued");
            entity.Property(e => e.IssueNo)
                .HasMaxLength(100)
                .HasColumnName("issue_no");
            entity.Property(e => e.IssuedQuantity)
                .HasPrecision(18, 2)
                .HasColumnName("issued_quantity");
            entity.Property(e => e.IssuedToId).HasColumnName("issued_to_id");
            entity.Property(e => e.ItemName)
                .HasMaxLength(200)
                .HasColumnName("item_name");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.RequestedById).HasColumnName("requested_by_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");

            entity.HasOne(d => d.IssuedTo).WithMany(p => p.StockOutwardIssuedTos)
                .HasForeignKey(d => d.IssuedToId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_stock_outward_issued_to");

            entity.HasOne(d => d.Project).WithMany(p => p.StockOutwards)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("fk_stock_outward_project");

            entity.HasOne(d => d.RequestedBy).WithMany(p => p.StockOutwardRequestedBies)
                .HasForeignKey(d => d.RequestedById)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_stock_outward_requested_by");
        });

        modelBuilder.Entity<Subcontractor>(entity =>
        {
            entity.HasKey(e => e.SubcontractorId).HasName("subcontractor_pkey");

            entity.ToTable("subcontractor", "subcontractor");

            entity.HasIndex(e => e.Email, "subcontractor_email_key").IsUnique();

            entity.HasIndex(e => e.SubcontractorCode, "subcontractor_subcontractor_code_key").IsUnique();

            entity.Property(e => e.SubcontractorId).HasColumnName("subcontractor_id");
            entity.Property(e => e.AlternativePhone)
                .HasMaxLength(20)
                .HasColumnName("alternative_phone");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.InsuranceDetails).HasColumnName("insurance_details");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("postal_code");
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .HasColumnName("state");
            entity.Property(e => e.Street).HasColumnName("street");
            entity.Property(e => e.SubcontractorCode)
                .HasMaxLength(50)
                .HasColumnName("subcontractor_code");
            entity.Property(e => e.SubcontractorName)
                .HasMaxLength(150)
                .HasColumnName("subcontractor_name");
            entity.Property(e => e.TaxId)
                .HasMaxLength(50)
                .HasColumnName("tax_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .HasColumnName("website");
        });

        modelBuilder.Entity<Taskstatusmaster>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("taskstatusmaster_pkey");

            entity.ToTable("taskstatusmaster", "project");

            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("tickets_pkey");

            entity.ToTable("tickets", "ticket");

            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.AssignBy).HasColumnName("assign_by");
            entity.Property(e => e.BoardId).HasColumnName("board_id");
            entity.Property(e => e.BoqId).HasColumnName("boq_id");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DueDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("due_date");
            entity.Property(e => e.Isapproved).HasColumnName("isapproved");
            entity.Property(e => e.LabelId).HasColumnName("label_id");
            entity.Property(e => e.MoveBy).HasColumnName("move_by");
            entity.Property(e => e.MoveTo).HasColumnName("move_to");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.TicketLabel)
                .HasMaxLength(50)
                .HasColumnName("ticket_label");
            entity.Property(e => e.TicketNo)
                .HasMaxLength(50)
                .HasColumnName("ticket_no");
            entity.Property(e => e.TicketType)
                .HasMaxLength(50)
                .HasColumnName("ticket_type");
            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.TicketApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("fk_ticket_approved_by");

            entity.HasOne(d => d.AssignByNavigation).WithMany(p => p.TicketAssignByNavigations)
                .HasForeignKey(d => d.AssignBy)
                .HasConstraintName("fk_ticket_assign_by");

            entity.HasOne(d => d.Board).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.BoardId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ticket_board");

            entity.HasOne(d => d.Boq).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.BoqId)
                .HasConstraintName("fk_boq_id");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TicketCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("fk_ticket_created_by");

            entity.HasOne(d => d.MoveByNavigation).WithMany(p => p.TicketMoveByNavigations)
                .HasForeignKey(d => d.MoveBy)
                .HasConstraintName("fk_ticket_move_by");

            entity.HasOne(d => d.MoveToNavigation).WithMany(p => p.TicketMoveToNavigations)
                .HasForeignKey(d => d.MoveTo)
                .HasConstraintName("fk_ticket_move_to");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.TicketUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("fk_ticket_updated_by");
        });

        modelBuilder.Entity<TicketAssignee>(entity =>
        {
            entity.HasKey(e => e.TicketAssigneeId).HasName("ticket_assignee_pkey");

            entity.ToTable("ticket_assignee", "ticket");

            entity.Property(e => e.TicketAssigneeId).HasColumnName("ticket_assignee_id");
            entity.Property(e => e.AssignBy).HasColumnName("assign_by");
            entity.Property(e => e.AssigneeId).HasColumnName("assignee_id");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketAssignees)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("fk_ticket_id");
        });

        modelBuilder.Entity<TicketAttachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("attachment_pkey");

            entity.ToTable("ticket_attachment", "ticket");

            entity.Property(e => e.AttachmentId)
                .HasDefaultValueSql("nextval('ticket.attachment_attachment_id_seq'::regclass)")
                .HasColumnName("attachment_id");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("file_path");
            entity.Property(e => e.TicketCommentId).HasColumnName("ticket_comment_id");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.TicketComment).WithMany(p => p.TicketAttachments)
                .HasForeignKey(d => d.TicketCommentId)
                .HasConstraintName("fk_comment");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketAttachments)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_attachment_ticket_id");
        });

        modelBuilder.Entity<TicketComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("ticket_comments_pkey");

            entity.ToTable("ticket_comments", "ticket");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedByType)
                .HasMaxLength(50)
                .HasColumnName("created_by_type");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_date");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketComments)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("fk_ticket_id");
        });

        modelBuilder.Entity<TicketMovement>(entity =>
        {
            entity.HasKey(e => e.MovementId).HasName("ticket_movements_pkey");

            entity.ToTable("ticket_movements", "ticket");

            entity.Property(e => e.MovementId).HasColumnName("movement_id");
            entity.Property(e => e.MoveBy).HasColumnName("move_by");
            entity.Property(e => e.MoveTo).HasColumnName("move_to");
            entity.Property(e => e.MovedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("moved_at");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketMovements)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("fk_ticket_id");
        });

        modelBuilder.Entity<TicketParticipant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ticket_participants_pkey");

            entity.ToTable("ticket_participants", "ticket");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("added_at");
            entity.Property(e => e.AddedBy).HasColumnName("added_by");
            entity.Property(e => e.ParticipantId).HasColumnName("participant_id");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");

            entity.HasOne(d => d.AddedByNavigation).WithMany(p => p.TicketParticipantAddedByNavigations)
                .HasForeignKey(d => d.AddedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ticket_participant_added_by");

            entity.HasOne(d => d.Participant).WithMany(p => p.TicketParticipantParticipants)
                .HasForeignKey(d => d.ParticipantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ticket_participant_employee");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketParticipants)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ticket_participant_ticket");
        });

        modelBuilder.Entity<VBoardId>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("v_board_id");

            entity.Property(e => e.BoardId).HasColumnName("board_id");
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.VendorId).HasName("vendor_pkey");

            entity.ToTable("vendor", "vendor");

            entity.HasIndex(e => e.Email, "vendor_email_key").IsUnique();

            entity.HasIndex(e => e.VendorCode, "vendor_vendor_code_key").IsUnique();

            entity.Property(e => e.VendorId).HasColumnName("vendor_id");
            entity.Property(e => e.AlternativeMobile)
                .HasMaxLength(20)
                .HasColumnName("alternative_mobile");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreditLimit)
                .HasPrecision(15, 2)
                .HasColumnName("credit_limit");
            entity.Property(e => e.DeliveryTerms).HasColumnName("delivery_terms");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.Mobile)
                .HasMaxLength(20)
                .HasColumnName("mobile");
            entity.Property(e => e.OrganizationName)
                .HasMaxLength(200)
                .HasColumnName("organization_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PaymentTerms).HasColumnName("payment_terms");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("postal_code");
            entity.Property(e => e.PreferredPaymentMethod)
                .HasMaxLength(100)
                .HasColumnName("preferred_payment_method");
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .HasColumnName("state");
            entity.Property(e => e.Street).HasColumnName("street");
            entity.Property(e => e.TaxId)
                .HasMaxLength(50)
                .HasColumnName("tax_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.VendorCode)
                .HasMaxLength(50)
                .HasColumnName("vendor_code");
            entity.Property(e => e.VendorName)
                .HasMaxLength(150)
                .HasColumnName("vendor_name");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .HasColumnName("website");
        });

        modelBuilder.Entity<VendorDepartment>(entity =>
        {
            entity.HasKey(e => e.VendorDeptId).HasName("department_pkey");

            entity.ToTable("vendor_department", "vendor");

            entity.Property(e => e.VendorDeptId)
                .HasDefaultValueSql("nextval('vendor.department_vendor_dept_id_seq'::regclass)")
                .HasColumnName("vendor_dept_id");
            entity.Property(e => e.DeptId).HasColumnName("dept_id");
            entity.Property(e => e.VendorId).HasColumnName("vendor_id");

            entity.HasOne(d => d.Dept).WithMany(p => p.VendorDepartments)
                .HasForeignKey(d => d.DeptId)
                .HasConstraintName("fk_vendor_dept");

            entity.HasOne(d => d.Vendor).WithMany(p => p.VendorDepartments)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("fk_vendor");
        });

        modelBuilder.Entity<Vendorrole>(entity =>
        {
            entity.HasKey(e => e.VendorRoleId).HasName("vendorrole_pkey");

            entity.ToTable("vendorrole", "vendor");

            entity.Property(e => e.VendorRoleId).HasColumnName("vendor_role_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.VendorId).HasColumnName("vendor_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Vendorroles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("vendorrole_role_id_fkey");

            entity.HasOne(d => d.Vendor).WithMany(p => p.Vendorroles)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("vendorrole_vendor_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
