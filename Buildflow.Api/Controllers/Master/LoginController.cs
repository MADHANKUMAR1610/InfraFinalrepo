using AutoMapper;
using Buildflow.Api.Model;
using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;

using Buildflow.Library.Repository.Interfaces;
using Buildflow.Library.UOW;
using Buildflow.Service.Service.Master;
using Buildflow.Service.Service.Project;
using Buildflow.Service.Service.Vendor;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;



using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Buildflow.Api.Controllers.Master
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly RegisterService _service;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;


        public LoginController(RegisterService service, IConfiguration config, IUnitOfWork unitOfWork)
        {
            _service = service;
            _config = config;
            _unitOfWork = unitOfWork;
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _service.VerifyLogin(request.Email, request.Password, request.type);
            if (user == null)
                return Unauthorized("Invalid credentials");

            string token = string.Empty;
            string refreshToken = GenerateRefreshToken();

            if (user is EmployeeDetail employee)
            {
                token = GenerateJwtTokenForEmployee(employee);
                await _service.SaveRefreshToken(employee.Email, refreshToken); // Implement this

            }
            else if (user is Buildflow.Infrastructure.Entities.Vendor vendor)
            {
                token = GenerateJwtTokenForVendor(vendor);
                await _service.SaveRefreshToken(vendor.Email, refreshToken); // Implement this

            }
            else
            {
                return Unauthorized("Invalid user type");
            }

            // Save refreshToken to DB

            return Ok(new
            {
                Token = token,
                RefreshToken = refreshToken
            });
        }


        private string GenerateJwtTokenForEmployee(EmployeeDetail user)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("EmpId", user.EmpId.ToString()),
        new Claim("UserType", "Employee")
    };

            return CreateJwt(claims);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }


        private string GenerateJwtTokenForVendor(Buildflow.Infrastructure.Entities.Vendor user)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("VendorId", user.VendorId.ToString()),
        new Claim("UserType", "Vendor")
    };

            return CreateJwt(claims);
        }

        private string CreateJwt(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
        {
            var existingToken = await _service.GetRefreshToken(request.RefreshToken);

            if (existingToken == null || existingToken.Isrevoked == true || existingToken.Expirydate <= DateTime.Now)
                return Unauthorized("Invalid refresh token");

            
            var user = await _service.GetUserByEmail(existingToken.Email);
            if (user == null)
                return Unauthorized();

            string newAccessToken = user is EmployeeDetail emp
                ? GenerateJwtTokenForEmployee(emp)
                : GenerateJwtTokenForVendor((Buildflow.Infrastructure.Entities.Vendor)user);

            // Optional: generate a new refresh token and revoke the old one
            existingToken.Isrevoked = true;
            string newRefreshToken = GenerateRefreshToken();
            await _service.SaveRefreshToken(existingToken.Email, newRefreshToken);

            return Ok(new { Token = newAccessToken, RefreshToken = newRefreshToken });
        }


        [HttpGet("LoginUserDetail")]

        public async Task<IActionResult> LoginUserDetail()
        {
            var userType = User.FindFirst("UserType")?.Value;

            if (userType == "Employee")
            {
                var empIdClaim = User.FindFirst("EmpId")?.Value;
                if (!int.TryParse(empIdClaim, out var empId))
                    return BadRequest("Invalid EmpId.");

                var result = await _service.GetLoginEmployeeFullDetailAsync(empId);
                if (result == null)
                    return NotFound("Employee not found");

                return Ok(result);
            }
            else if (userType == "Vendor")
            {
                var vendorIdClaim = User.FindFirst("VendorId")?.Value;
                if (!int.TryParse(vendorIdClaim, out var vendorId))
                    return BadRequest("Invalid VendorId.");

                var result = await _service.GetLoginVendorFullDetailAsync(vendorId);
                if (result == null)
                    return NotFound("Vendor not found");

                return Ok(result);
            }

            return Unauthorized("User type not recognized.");
        }

        [Authorize]
        [HttpGet("LoginUser")]
        public async Task<IActionResult> GetCurrentUser()

        {
            var empIdClaim = User.FindFirst("EmpId")?.Value;

            if (string.IsNullOrEmpty(empIdClaim))
                return Unauthorized("No EmpId found in token.");

            // Convert from string to int
            if (!int.TryParse(empIdClaim, out var empId))
                return BadRequest("Invalid EmpId format.");

            var employee = await _unitOfWork.RegisterUser.GetById(empId);

            if (employee == null)
                return NotFound("Employee not found.");

            return Ok(employee);
        }


        [HttpGet("board-details/{empId}")]
        public async Task<IActionResult> GetBoardDetails(int empId, int? roleId = null)
        {
            var result = await _service.GetEmployeeBoardsAsync(empId, roleId);
            if (result == null || result.Count == 0)
                return NotFound("No board data found for employee.");

            return Ok(result);
        }
        [HttpGet("getEmployeesByRoles")]

        public async Task<IActionResult> GetEmployeesByRoles()
        {
            var response = await _service.GetEmployeesByRoles();
            return Ok(response);
        }

        [HttpGet("getEmployeesByRole/{roleId}")]
        public async Task<IActionResult> GetEmployeesByRole(int roleId)
        {
            var result = await _service.GetEmployeesByRole(roleId);
            return Ok(result);
        }


        [HttpGet("getEmployeesByDepartment/{deptId}")]
        public async Task<IActionResult> GetEmployeesByDept(int deptId)
        {
            var result = await _service.GetEmployeesByDept(deptId);
            return Ok(result);
        }




        [HttpGet("getVendorsAndSubcontractors")]
        public async Task<IActionResult> GetVendorsAndSubcontractors()
        {
            var response = await _service.GetVendorsAndSubcontractors();
            return Ok(response);
        }

        [HttpGet("labels-with-tickets/{employeeId}")]

        public async Task<IActionResult> GetLabelsWithTickets(int employeeId)
        {
            var result = await _service.GetLabelsWithTicketsByEmployeeIdAsync(employeeId);

            if (result == null || !result.Any())
            {
                return NotFound(new BaseResponse
                {
                    Success = false,
                    Message = "No labels or tickets found.",
                    Data = null
                });
            }

            return Ok(new BaseResponse
            {
                Success = true,
                Message = "Labels with tickets fetched successfully.",
                Data = result
            });
        }





    }







}

