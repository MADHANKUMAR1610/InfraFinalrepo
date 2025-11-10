using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Buildflow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addProjectidintickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "board");

            migrationBuilder.EnsureSchema(
                name: "master");

            migrationBuilder.EnsureSchema(
                name: "employee");

            migrationBuilder.EnsureSchema(
                name: "project");

            migrationBuilder.EnsureSchema(
                name: "subcontractor");

            migrationBuilder.EnsureSchema(
                name: "ticket");

            migrationBuilder.EnsureSchema(
                name: "vendor");

            migrationBuilder.CreateTable(
                name: "department",
                schema: "master",
                columns: table => new
                {
                    dept_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dept_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    dept_description = table.Column<string>(type: "text", nullable: true),
                    dept_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    parent_dept_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("department_pkey", x => x.dept_id);
                    table.ForeignKey(
                        name: "department_parent_dept_id_fkey",
                        column: x => x.parent_dept_id,
                        principalSchema: "master",
                        principalTable: "department",
                        principalColumn: "dept_id");
                });

            migrationBuilder.CreateTable(
                name: "employee_details",
                schema: "employee",
                columns: table => new
                {
                    emp_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    employee_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    alternative_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    hire_date = table.Column<DateOnly>(type: "date", nullable: true),
                    termination_date = table.Column<DateOnly>(type: "date", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    address = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    state = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    image_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_details_pkey", x => x.emp_id);
                });

            migrationBuilder.CreateTable(
                name: "project_budget",
                schema: "master",
                columns: table => new
                {
                    project_budget_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_expense_category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("project_budget_pkey", x => x.project_budget_id);
                });

            migrationBuilder.CreateTable(
                name: "project_sector",
                schema: "master",
                columns: table => new
                {
                    project_sector_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_sector_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    project_sector_description = table.Column<string>(type: "text", nullable: true),
                    sector_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("project_sector_pkey", x => x.project_sector_id);
                });

            migrationBuilder.CreateTable(
                name: "project_types",
                schema: "master",
                columns: table => new
                {
                    project_type_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_type_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    project_type_description = table.Column<string>(type: "text", nullable: true),
                    type_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("project_types_pkey", x => x.project_type_id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                schema: "master",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    role_description = table.Column<string>(type: "text", nullable: true),
                    is_system_role = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("role_pkey", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "subcontractor",
                schema: "subcontractor",
                columns: table => new
                {
                    subcontractor_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    subcontractor_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subcontractor_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    alternative_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    website = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    street = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    state = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    tax_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    insurance_details = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("subcontractor_pkey", x => x.subcontractor_id);
                });

            migrationBuilder.CreateTable(
                name: "vendor",
                schema: "vendor",
                columns: table => new
                {
                    vendor_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    vendor_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    vendor_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    organization_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    mobile = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    alternative_mobile = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    website = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    street = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    state = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    tax_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    payment_terms = table.Column<string>(type: "text", nullable: true),
                    preferred_payment_method = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    delivery_terms = table.Column<string>(type: "text", nullable: true),
                    credit_limit = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("vendor_pkey", x => x.vendor_id);
                });

            migrationBuilder.CreateTable(
                name: "board",
                schema: "board",
                columns: table => new
                {
                    board_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("board_pkey", x => x.board_id);
                    table.ForeignKey(
                        name: "fk_board_created_by",
                        column: x => x.created_by,
                        principalSchema: "employee",
                        principalTable: "employee_details",
                        principalColumn: "emp_id");
                    table.ForeignKey(
                        name: "fk_board_updated_by",
                        column: x => x.updated_by,
                        principalSchema: "employee",
                        principalTable: "employee_details",
                        principalColumn: "emp_id");
                });

            migrationBuilder.CreateTable(
                name: "employee_department",
                schema: "employee",
                columns: table => new
                {
                    emp_dept_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    emp_id = table.Column<int>(type: "integer", nullable: false),
                    dept_id = table.Column<int>(type: "integer", nullable: false),
                    assigned_date = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "CURRENT_DATE"),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    is_primary = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_department_pkey", x => x.emp_dept_id);
                    table.ForeignKey(
                        name: "employee_department_dept_id_fkey",
                        column: x => x.dept_id,
                        principalSchema: "master",
                        principalTable: "department",
                        principalColumn: "dept_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "employee_department_emp_id_fkey",
                        column: x => x.emp_id,
                        principalSchema: "employee",
                        principalTable: "employee_details",
                        principalColumn: "emp_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project",
                schema: "project",
                columns: table => new
                {
                    project_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    project_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    project_description = table.Column<string>(type: "text", nullable: true),
                    project_type_id = table.Column<int>(type: "integer", nullable: false),
                    project_sector_id = table.Column<int>(type: "integer", nullable: true),
                    project_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    project_start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    project_end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    project_total_budget = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    project_actual_cost = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: true, defaultValueSql: "0.00"),
                    client_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    project_priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    project_location = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("project_pkey", x => x.project_id);
                    table.ForeignKey(
                        name: "project_project_sector_id_fkey",
                        column: x => x.project_sector_id,
                        principalSchema: "master",
                        principalTable: "project_sector",
                        principalColumn: "project_sector_id");
                    table.ForeignKey(
                        name: "project_project_type_id_fkey",
                        column: x => x.project_type_id,
                        principalSchema: "master",
                        principalTable: "project_types",
                        principalColumn: "project_type_id");
                });

            migrationBuilder.CreateTable(
                name: "employee_role",
                schema: "employee",
                columns: table => new
                {
                    emp_role_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    emp_id = table.Column<int>(type: "integer", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false),
                    assigned_date = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "CURRENT_DATE"),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_role_pkey", x => x.emp_role_id);
                    table.ForeignKey(
                        name: "employee_role_emp_id_fkey",
                        column: x => x.emp_id,
                        principalSchema: "employee",
                        principalTable: "employee_details",
                        principalColumn: "emp_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "employee_role_role_id_fkey",
                        column: x => x.role_id,
                        principalSchema: "master",
                        principalTable: "role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "boardlabel",
                schema: "board",
                columns: table => new
                {
                    labelid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    board_id = table.Column<int>(type: "integer", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: true),
                    wiplimit = table.Column<int>(type: "integer", nullable: true),
                    colorcode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    isexpand = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    last_updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_default = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    is_move_state = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("boardlabel_pkey", x => x.labelid);
                    table.ForeignKey(
                        name: "fk_boardlabel_board",
                        column: x => x.board_id,
                        principalSchema: "board",
                        principalTable: "board",
                        principalColumn: "board_id");
                    table.ForeignKey(
                        name: "fk_boardlabel_created_by",
                        column: x => x.created_by,
                        principalSchema: "employee",
                        principalTable: "employee_details",
                        principalColumn: "emp_id");
                    table.ForeignKey(
                        name: "fk_boardlabel_updated_by",
                        column: x => x.updated_by,
                        principalSchema: "employee",
                        principalTable: "employee_details",
                        principalColumn: "emp_id");
                });

            migrationBuilder.CreateTable(
                name: "tickets",
                schema: "ticket",
                columns: table => new
                {
                    ticket_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    ticket_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    create_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    due_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    board_id = table.Column<int>(type: "integer", nullable: false),
                    label_id = table.Column<int>(type: "integer", nullable: true),
                    assign_to = table.Column<int>(type: "integer", nullable: true),
                    approved_by = table.Column<int>(type: "integer", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    assign_by = table.Column<int>(type: "integer", nullable: true),
                    move_to = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ticket_label = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ticket_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tickets_pkey", x => x.ticket_id);
                    table.ForeignKey(
                        name: "fk_ticket_approved_by",
                        column: x => x.approved_by,
                        principalSchema: "employee",
                        principalTable: "employee_details",
                        principalColumn: "emp_id");
                    table.ForeignKey(
                        name: "fk_ticket_assign_by",
                        column: x => x.assign_by,
                        principalSchema: "employee",
                        principalTable: "employee_details",
                        principalColumn: "emp_id");
                    table.ForeignKey(
                        name: "fk_ticket_assign_to",
                        column: x => x.assign_to,
                        principalSchema: "employee",
                        principalTable: "employee_details",
                        principalColumn: "emp_id");
                    table.ForeignKey(
                        name: "fk_ticket_board",
                        column: x => x.board_id,
                        principalSchema: "board",
                        principalTable: "board",
                        principalColumn: "board_id");
                    table.ForeignKey(
                        name: "fk_ticket_created_by",
                        column: x => x.created_by,
                        principalSchema: "employee",
                        principalTable: "employee_details",
                        principalColumn: "emp_id");
                    table.ForeignKey(
                        name: "fk_ticket_updated_by",
                        column: x => x.updated_by,
                        principalSchema: "employee",
                        principalTable: "employee_details",
                        principalColumn: "emp_id");
                });

            migrationBuilder.CreateTable(
                name: "project_approval",
                schema: "project",
                columns: table => new
                {
                    approval_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<int>(type: "integer", nullable: false),
                    approval_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    approved_by = table.Column<int>(type: "integer", nullable: true),
                    approval_date = table.Column<DateOnly>(type: "date", nullable: true),
                    comments = table.Column<string>(type: "text", nullable: true),
                    document_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("project_approval_pkey", x => x.approval_id);
                    table.ForeignKey(
                        name: "project_approval_approved_by_fkey",
                        column: x => x.approved_by,
                        principalSchema: "employee",
                        principalTable: "employee_details",
                        principalColumn: "emp_id");
                    table.ForeignKey(
                        name: "project_approval_project_id_fkey",
                        column: x => x.project_id,
                        principalSchema: "project",
                        principalTable: "project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_budget_details",
                schema: "project",
                columns: table => new
                {
                    project_budget_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<int>(type: "integer", nullable: false),
                    project_expense_category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    estimated_cost = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    approved_budget = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_project_budget_id", x => x.project_budget_id);
                    table.ForeignKey(
                        name: "fk_project",
                        column: x => x.project_id,
                        principalSchema: "project",
                        principalTable: "project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_details",
                schema: "project",
                columns: table => new
                {
                    project_detail_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<int>(type: "integer", nullable: false),
                    project_location = table.Column<string>(type: "text", nullable: false),
                    plot_number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    site_address = table.Column<string>(type: "text", nullable: true),
                    latitude = table.Column<decimal>(type: "numeric(10,8)", precision: 10, scale: 8, nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(11,8)", precision: 11, scale: 8, nullable: true),
                    land_area = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: true),
                    land_area_unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, defaultValueSql: "'sqft'::character varying"),
                    zoning_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("project_details_pkey", x => x.project_detail_id);
                    table.ForeignKey(
                        name: "project_details_project_id_fkey",
                        column: x => x.project_id,
                        principalSchema: "project",
                        principalTable: "project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_milestone",
                schema: "project",
                columns: table => new
                {
                    milestone_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<int>(type: "integer", nullable: false),
                    milestone_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    milestone_description = table.Column<string>(type: "text", nullable: true),
                    milestone_start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    milestone_end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    milestone_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    milestone_budget = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: true),
                    actual_cost = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: true, defaultValueSql: "0.00"),
                    completion_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true, defaultValueSql: "0.00"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("project_milestone_pkey", x => x.milestone_id);
                    table.ForeignKey(
                        name: "project_milestone_project_id_fkey",
                        column: x => x.project_id,
                        principalSchema: "project",
                        principalTable: "project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_permission_finance_approval",
                schema: "project",
                columns: table => new
                {
                    permission_finance_approval_id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('project.project_permission_finance_ap_permission_finance_approval_i_seq'::regclass)"),
                    project_id = table.Column<int>(type: "integer", nullable: true),
                    emp_id = table.Column<int>(type: "integer", nullable: true),
                    amount = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("project_permission_finance_approval_pkey", x => x.permission_finance_approval_id);
                    table.ForeignKey(
                        name: "project_permission_finance_approval_emp_id_fkey",
                        column: x => x.emp_id,
                        principalSchema: "employee",
                        principalTable: "employee_details",
                        principalColumn: "emp_id");
                    table.ForeignKey(
                        name: "project_permission_finance_approval_project_id_fkey",
                        column: x => x.project_id,
                        principalSchema: "project",
                        principalTable: "project",
                        principalColumn: "project_id");
                });

            migrationBuilder.CreateTable(
                name: "project_risk_management",
                schema: "project",
                columns: table => new
                {
                    risk_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<int>(type: "integer", nullable: false),
                    category_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    risk_description = table.Column<string>(type: "text", nullable: true),
                    risk_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    image_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    risk_impact = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    risk_probability = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    mitigation_plan = table.Column<string>(type: "text", nullable: true),
                    identified_date = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "CURRENT_DATE"),
                    resolved_date = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("project_risk_management_pkey", x => x.risk_id);
                    table.ForeignKey(
                        name: "project_risk_management_project_id_fkey",
                        column: x => x.project_id,
                        principalSchema: "project",
                        principalTable: "project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_team",
                schema: "project",
                columns: table => new
                {
                    project_team_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    project_id = table.Column<int>(type: "integer", nullable: false),
                    pm_id = table.Column<List<int>>(type: "integer[]", nullable: true),
                    apm_id = table.Column<List<int>>(type: "integer[]", nullable: true),
                    lead_engg_id = table.Column<List<int>>(type: "integer[]", nullable: true),
                    site_supervisor_id = table.Column<List<int>>(type: "integer[]", nullable: true),
                    qs_id = table.Column<List<int>>(type: "integer[]", nullable: true),
                    aqs_id = table.Column<List<int>>(type: "integer[]", nullable: true),
                    site_engg_id = table.Column<List<int>>(type: "integer[]", nullable: true),
                    engg_id = table.Column<List<int>>(type: "integer[]", nullable: true),
                    designer_id = table.Column<List<int>>(type: "integer[]", nullable: true),
                    vendor_id = table.Column<List<int>>(type: "integer[]", nullable: true),
                    subcontractor_id = table.Column<List<int>>(type: "integer[]", nullable: true),
                    assignment_start_date = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "CURRENT_DATE"),
                    assignment_end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("project_team_pkey", x => x.project_team_id);
                    table.ForeignKey(
                        name: "project_team_project_id_fkey",
                        column: x => x.project_id,
                        principalSchema: "project",
                        principalTable: "project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_board_created_by",
                schema: "board",
                table: "board",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_board_updated_by",
                schema: "board",
                table: "board",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "IX_boardlabel_board_id",
                schema: "board",
                table: "boardlabel",
                column: "board_id");

            migrationBuilder.CreateIndex(
                name: "IX_boardlabel_created_by",
                schema: "board",
                table: "boardlabel",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_boardlabel_updated_by",
                schema: "board",
                table: "boardlabel",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "department_dept_code_key",
                schema: "master",
                table: "department",
                column: "dept_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "department_dept_name_key",
                schema: "master",
                table: "department",
                column: "dept_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_department_parent_dept_id",
                schema: "master",
                table: "department",
                column: "parent_dept_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_department_dept_id",
                schema: "employee",
                table: "employee_department",
                column: "dept_id");

            migrationBuilder.CreateIndex(
                name: "unique_emp_dept",
                schema: "employee",
                table: "employee_department",
                columns: new[] { "emp_id", "dept_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "employee_details_email_key",
                schema: "employee",
                table: "employee_details",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "employee_details_employee_code_key",
                schema: "employee",
                table: "employee_details",
                column: "employee_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employee_role_role_id",
                schema: "employee",
                table: "employee_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "unique_emp_role",
                schema: "employee",
                table: "employee_role",
                columns: new[] { "emp_id", "role_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_project_sector_id",
                schema: "project",
                table: "project",
                column: "project_sector_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_project_type_id",
                schema: "project",
                table: "project",
                column: "project_type_id");

            migrationBuilder.CreateIndex(
                name: "project_project_code_key",
                schema: "project",
                table: "project",
                column: "project_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_approval_approved_by",
                schema: "project",
                table: "project_approval",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "IX_project_approval_project_id",
                schema: "project",
                table: "project_approval",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_budget_details_project_id",
                schema: "project",
                table: "project_budget_details",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_details_project_id",
                schema: "project",
                table: "project_details",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_milestone_project_id",
                schema: "project",
                table: "project_milestone",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_permission_finance_approval_emp_id",
                schema: "project",
                table: "project_permission_finance_approval",
                column: "emp_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_permission_finance_approval_project_id",
                schema: "project",
                table: "project_permission_finance_approval",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_risk_management_project_id",
                schema: "project",
                table: "project_risk_management",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "project_sector_project_sector_name_key",
                schema: "master",
                table: "project_sector",
                column: "project_sector_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "project_sector_sector_code_key",
                schema: "master",
                table: "project_sector",
                column: "sector_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_team_project_id",
                schema: "project",
                table: "project_team",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "project_types_project_type_name_key",
                schema: "master",
                table: "project_types",
                column: "project_type_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "project_types_type_code_key",
                schema: "master",
                table: "project_types",
                column: "type_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "role_role_name_key",
                schema: "master",
                table: "role",
                column: "role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "subcontractor_email_key",
                schema: "subcontractor",
                table: "subcontractor",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "subcontractor_subcontractor_code_key",
                schema: "subcontractor",
                table: "subcontractor",
                column: "subcontractor_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tickets_approved_by",
                schema: "ticket",
                table: "tickets",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_assign_by",
                schema: "ticket",
                table: "tickets",
                column: "assign_by");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_assign_to",
                schema: "ticket",
                table: "tickets",
                column: "assign_to");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_board_id",
                schema: "ticket",
                table: "tickets",
                column: "board_id");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_created_by",
                schema: "ticket",
                table: "tickets",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_tickets_updated_by",
                schema: "ticket",
                table: "tickets",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "vendor_email_key",
                schema: "vendor",
                table: "vendor",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "vendor_vendor_code_key",
                schema: "vendor",
                table: "vendor",
                column: "vendor_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "boardlabel",
                schema: "board");

            migrationBuilder.DropTable(
                name: "employee_department",
                schema: "employee");

            migrationBuilder.DropTable(
                name: "employee_role",
                schema: "employee");

            migrationBuilder.DropTable(
                name: "project_approval",
                schema: "project");

            migrationBuilder.DropTable(
                name: "project_budget",
                schema: "master");

            migrationBuilder.DropTable(
                name: "project_budget_details",
                schema: "project");

            migrationBuilder.DropTable(
                name: "project_details",
                schema: "project");

            migrationBuilder.DropTable(
                name: "project_milestone",
                schema: "project");

            migrationBuilder.DropTable(
                name: "project_permission_finance_approval",
                schema: "project");

            migrationBuilder.DropTable(
                name: "project_risk_management",
                schema: "project");

            migrationBuilder.DropTable(
                name: "project_team",
                schema: "project");

            migrationBuilder.DropTable(
                name: "subcontractor",
                schema: "subcontractor");

            migrationBuilder.DropTable(
                name: "tickets",
                schema: "ticket");

            migrationBuilder.DropTable(
                name: "vendor",
                schema: "vendor");

            migrationBuilder.DropTable(
                name: "department",
                schema: "master");

            migrationBuilder.DropTable(
                name: "role",
                schema: "master");

            migrationBuilder.DropTable(
                name: "project",
                schema: "project");

            migrationBuilder.DropTable(
                name: "board",
                schema: "board");

            migrationBuilder.DropTable(
                name: "project_sector",
                schema: "master");

            migrationBuilder.DropTable(
                name: "project_types",
                schema: "master");

            migrationBuilder.DropTable(
                name: "employee_details",
                schema: "employee");
        }
    }
}
