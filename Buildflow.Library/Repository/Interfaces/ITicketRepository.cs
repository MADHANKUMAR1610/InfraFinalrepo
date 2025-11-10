using Buildflow.Infrastructure.Entities;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository.Interfaces
{
    public interface ITicketRepository
    {
        Task<BaseResponse> ExecuteCreateTicketSp(CreateTicketDto dto);
        Task<BaseResponse> CreateCustomTicketAsync(CreateCustomTicketRequestDto dto);
        Task<(int TicketId, string Message, string Status)> UpdateTicketById(UpdateTicketDto input);
        Task<TicketData> GetTicketById(int ticketId);

        Task<TicketCommentAttachmentResponseDto> AddCommentAndAttachmentAsync(TicketCommentAttachmentInputDto inputDto);
    }
}
