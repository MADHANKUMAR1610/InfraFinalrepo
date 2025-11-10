using Buildflow.Library.UOW;
using Buildflow.Service.Service.Master;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildflow.Api.Controllers.Master
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {

        private readonly RoleService _service;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        public RoleController(RoleService service, IConfiguration config, IUnitOfWork unitOfWork)
        {
            _service = service;
            _config = config;
            _unitOfWork = unitOfWork;
        }


        [HttpGet("getRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var response = await _service.GetRoles();
            return Ok(response);
        }

       



    }
}
