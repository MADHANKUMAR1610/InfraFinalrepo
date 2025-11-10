using Buildflow.Library.UOW;
using Buildflow.Service.Service.Master;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildflow.Api.Controllers.Master
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentService _service;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentController(DepartmentService service, IConfiguration config, IUnitOfWork unitOfWork)
        {
            _service = service;
            _config = config;
            _unitOfWork = unitOfWork;
        }


        [HttpGet("get-department")]
        
        public async Task<IActionResult> GetDepartments()
        {
            var response = await _service.GetDepartments();
            return Ok(response);
        }


        [HttpGet("get-roles-by-deptId/{deptId}")]
        public async Task<IActionResult> GetDepartmentWithRoles(int deptId)
        {
            var response = await _service.GetDepartmentWithRoles(deptId);
            return Ok(response);
        }
    }
}
