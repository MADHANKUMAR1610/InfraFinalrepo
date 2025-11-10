using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public class CreateTicketDto
    {
        public int? ProjectId { get; set; }
        public int? BoqId { get; set; }
        public int? POId { get; set; }
        public string? TicketType { get; set; }
        public int[]? AssignTo { get; set; } = [];
        public int CreatedBy { get; set; }
        //public string? TicketName {  get; set; }
    }
    public class CreateCustomTicketRequestDto
    {
        public string TicketName { get; set; }
        public int CreatedBy { get; set; }
    }



    public class TicketData
    {
        [Column("ticket_id")]
        public int ticket_id { get; set; }

        [Column("ticket_no")]
        public string ticket_no { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("description")]
        public string description { get; set; }

        [Column("create_date")]
        public DateTime? create_date { get; set; }

        [Column("due_date")]
        public DateTime? due_date { get; set; }

        [Column("isapproved")]
        public int isapproved { get; set; }

        [Column("updated_at")]
        public DateTime? updated_at { get; set; }

        [Column("created_at")]
        public DateTime? created_at { get; set; }

        [Column("board_id")]
        public int? board_id { get; set; }

        [Column("label_id")]
        public int? label_id { get; set; }

        [Column("approved_by")]
        public int? approved_by { get; set; }

        [Column("created_by")]
        public int? created_by { get; set; }

        [Column("updated_by")]
        public int? updated_by { get; set; }

        [Column("assign_by")]
        public int? assign_by { get; set; }

        [Column("move_to")]
        public int? move_to { get; set; }

        [Column("ticket_label")]
        public string ticket_label { get; set; }

        [Column("ticket_type")]
        public string ticket_type { get; set; }

        [Column("transaction_id")]
        public int? transaction_id { get; set; }

        [Column("ticket_owner_name")]
        public string ticket_owner_name { get; set; }

        [Column("role_name")]
        public string role_name { get; set; }

        [Column("comments_with_attachments_json")]
        public List<TicketCommentWithAttachmentDto> CommentsAndAttachments { get; set; }

        [Column("approvals")]

        public Dictionary<string, List<TicketApprovalDto>> ApprovalsGrouped { get; set; }

        [Column("vendor_id")]
        public int? VendorId { get; set; }

        [Column("vendor_name")]
        public string? VendorName { get; set; }
    }

    public class TicketApprovalDto
    {
        [Column("approval_type")]
        public string approval_type { get; set; }

        [Column("approved_by_name")]
        public string approved_by_name { get; set; }

        [Column("approved_by_id")]
        public int? approved_by_id { get; set; }
    }

    
    public class TicketAttachmentDto
    {
        [JsonPropertyName("attachment_id")]
        public int AttachmentId { get; set; }

        [JsonPropertyName("ticket_id")]
        public int TicketId { get; set; }

        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        [JsonPropertyName("file_path")]
        public string FilePath { get; set; }

        [JsonPropertyName("created_by")]
        public int CreatedBy { get; set; }

        [JsonPropertyName("created_date")]
        public DateTime? CreatedDate { get; set; }
    }


    public class TicketCommentDto
    {
        [JsonPropertyName("comment_id")]
        public int CommentId { get; set; }

        [JsonPropertyName("ticket_id")]
        public int TicketId { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("created_by")]
        public int CreatedBy { get; set; }

        [JsonPropertyName("created_date")]
        public DateTime? CreatedDate { get; set; }
    }

    public class TicketCommentWithAttachmentDto
    {
        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("created_by")]
        public int CreatedBy { get; set; }

        [JsonPropertyName("created_by_type")]
        public string? CreatedByType { get; set; }

        [JsonPropertyName("created_date")]
        public DateTime? CreatedDate { get; set; }

        [JsonPropertyName("filename")]
        public string FileName { get; set; }  // nullable if no attachment

        [JsonPropertyName("file_path")]
        public string FilePath { get; set; }  // nullable if no attachment

        [JsonPropertyName("attachment_created_date")]
        public DateTime? AttachmentCreatedDate { get; set; }  // nullable if no attachment

        [JsonPropertyName("created_by_name")]
        public string? CreatedByName { get; set; }

        [JsonPropertyName("created_by_role")]
        public string? CreatedByRole { get; set; }

    }

    public class UpdateTicketDto
    {
        public int TicketId { get; set; }
        //public string? TicketNo { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? IsApproved { get; set; }
        public int? BoardId { get; set; }
        public int? LabelId { get; set; }
        //public int? ApprovedBy { get; set; }
        //public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int[]? AssignTo { get; set; }
        public int? AssignBy { get; set; }
        public int[]? MoveTo { get; set; }
        public int? MoveBy { get; set; }
        //public string? TicketLabel { get; set; }
        public string? TicketType { get; set; }
        //public int? ProjectId { get; set; }
    }
    public class TicketCommentAttachmentInputDto
    {
        public int TicketId { get; set; }
        public string? Comment { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByType { get; set; }
        public IFormFile? File { get; set; } // For uploading file
    }

    public class TicketCommentAttachmentResponseDto
    {
        public int TicketId { get; set; }
        public string? FilePath { get; set; }
    }


}
