using Buildflow.Library.UOW;

using Buildflow.Service.Service.Ticket;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildflow.Api.Controllers.Ticket
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {

        private readonly TicketService _service;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;


        public TicketController(TicketService service, IConfiguration config, IUnitOfWork unitOfWork)
        {
            _service = service;
            _config = config;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("createTicket")]
        [AllowAnonymous]
    
       
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
        {
            var response = await _service.CreateTicketAsync(dto);
            return Ok(response);
        }

        [HttpGet("get-ticket-by-id")]
        [AllowAnonymous]
       
       
        public async Task<IActionResult> GetTicketByTicketId(int ticketId)
        {
            try
            {
                var response = await _service.GetTicketById(ticketId);

                if (response == null)
                    return NotFound($"Ticket with ID {ticketId} not found.");

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost("create-custom-ticket")]
       
        public async Task<IActionResult> CreateCustomTicket([FromBody] CreateCustomTicketRequestDto dto)
        {
            var response = await _service.CreateCustomTicketAsync(dto);
            return Ok(response);
        }

        [HttpPut("update-ticket-by-id")]
        [AllowAnonymous]
    
        public async Task<IActionResult> UpdateTicket([FromBody] UpdateTicketDto input)
        {
            try
            {
                var (ticketId, message, status) = await _service.UpdateTicketById(input);

                var response = new BaseResponse
                {
                    Success = status.Equals("true", StringComparison.OrdinalIgnoreCase),
                    Message = message,
                    Data = new
                    {
                        TicketId = ticketId,
                        Status = status
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}",
                    Data = null
                });
            }
        }

        [HttpPost("add-comment-attachment")]       
        public async Task<IActionResult> AddCommentWithAttachment([FromForm] TicketCommentAttachmentInputDto inputDto)
        {
            try
            {
                var result = await _service.AddCommentAndAttachmentAsync(inputDto);

                var response = new BaseResponse
                {
                    Success = true,
                    Message = "Comment and attachment added successfully.",
                    Data = result
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new BaseResponse
                {
                    Success = false,
                    Message = $"Error occurred: {ex.Message}",
                    Data = null
                };

                return StatusCode(500, errorResponse);
            }
        }


    }
}
