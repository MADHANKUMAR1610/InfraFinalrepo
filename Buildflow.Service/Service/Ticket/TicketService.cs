using Buildflow.Library.UOW;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.Ticket
{
    public class TicketService
    {

        private readonly IUnitOfWork _unitOfWork;
        public TicketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse> CreateTicketAsync(CreateTicketDto dto)
        {
            return await _unitOfWork.TicketRepository.ExecuteCreateTicketSp(dto);
        } 
        public async Task<BaseResponse> CreateCustomTicketAsync(CreateCustomTicketRequestDto dto)
        {
            return await _unitOfWork.TicketRepository.CreateCustomTicketAsync(dto);
        }

        public async Task<TicketData> GetTicketById(int ticketId)
        {
            return await _unitOfWork.TicketRepository.GetTicketById(ticketId);
        }

        public async Task <(int TicketId, string Message, string Status)> UpdateTicketById(UpdateTicketDto input)
        {
            return await _unitOfWork.TicketRepository.UpdateTicketById(input);
        } 
        
        public async Task <TicketCommentAttachmentResponseDto> AddCommentAndAttachmentAsync(TicketCommentAttachmentInputDto inputDto)
        {
            return await _unitOfWork.TicketRepository.AddCommentAndAttachmentAsync(inputDto);
        }
    }
}
