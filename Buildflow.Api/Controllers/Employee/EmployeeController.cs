using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Library.UOW;
using Buildflow.Service.Service.Employee;
using Buildflow.Service.Service.Notification;
using Buildflow.Service.Service.Ticket;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buildflow.Api.Controllers.Employee
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private readonly EmployeeService _service;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly BuildflowAppContext _context;


        public EmployeeController(EmployeeService service, IConfiguration config, IUnitOfWork unitOfWork,BuildflowAppContext context)
        {
            _service = service;
            _config = config;
            _unitOfWork = unitOfWork;
            _context = context;
        }
        [HttpPost("create-employee")]
        
        public async Task<ActionResult<List<EmployeeDTO>>> CreateEmployee([FromBody] EmployeeDTO input)
        {
            var result = await _service.CreateEmployeeAsync(input);
            return Ok(result);
        }

        [HttpGet("Get-Employee")]
  
        public async Task<ActionResult<List<GetEmployeeDto>>> GetEmployees()
        {
            var result = await _service.GetEmployees();
            return Ok(result);
        }


        [HttpGet("Get-NewEmpId")]
    

        public async Task<ActionResult> GetNewEmployeeId()
        {
            var emp = await _context.EmployeeDetails.OrderByDescending(e => e.EmpId).FirstOrDefaultAsync();
            Random random = new Random();
            int NewEmpId = emp != null ? emp.EmpId + 1 :0; 

            return Ok("EMP#" + NewEmpId);
        }

        [HttpDelete("Delete-Employee")]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteEmployee(int empId)
        {
            await _service.DeleteEmployee(empId);

            return Ok(new { message = "Employee deleted successfully." });
        }

        [HttpPost("createOrupdate-employee")]
        [AllowAnonymous]
        public async Task<ActionResult<List<EmployeeDTO>>> CreateOrUpdateEmployeeAsync([FromBody] EmployeeDTO input)
        {
            var result = await _service.CreateOrUpdateEmployeeAsync(input);
            return Ok(result);
        }
    }

}
