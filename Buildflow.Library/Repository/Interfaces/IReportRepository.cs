using Buildflow.Infrastructure.Entities;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository.Interfaces
{
    public interface IReportRepository
    {

        Task UpsertReportAsync(Buildflow.Infrastructure.Entities.Report report);
        Task<List<ReportDetails>>  GetReportsAsync();
        //Task<List<Buildflow.Infrastructure.Entities.Report>>  GetReportsAsync();
        Task<List<Buildflow.Infrastructure.Entities.ReportTypeMaster>> GetReportTypes();
        //Task<List<Buildflow.Infrastructure.Entities.Report>> GetReportByReportType(int? type);
        Task<List<ReportDetails>> GetReportByReportType(int? typeId);
        Task<List<ReportDetails>> GetReportByEmpId(int? empId,int? typeId);
        Task<ReportDetails>  GetReportByIdAsync(int reportid);
        //Task<Buildflow.Infrastructure.Entities.Report>  GetReportByIdAsync(int reportid);
        Task<List<ReportAttachment>> GetReportAttachmentByIdAsync(int reportId);
    }
}
