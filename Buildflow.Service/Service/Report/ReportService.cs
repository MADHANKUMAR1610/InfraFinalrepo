using Buildflow.Infrastructure.Entities;
using Buildflow.Infrastructure.Models;
using Buildflow.Library.UOW;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql.Internal.Postgres;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.Report
{
    public class ReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task UpsertReportAsync(Buildflow.Infrastructure.Entities.Report report)
        {
            await _unitOfWork.reportRepository.UpsertReportAsync(report);
        }

        public async Task<List<ReportDetails>> GetReportsAsync()
        {
            return await _unitOfWork.reportRepository.GetReportsAsync();
        }
        public async Task<List<Buildflow.Infrastructure.Entities.ReportTypeMaster>> GetReportTypes()
        {
            return await _unitOfWork.reportRepository.GetReportTypes();
        }
        public async Task<List<ReportDetails>> GetReportByReportType(int? typeId)
        {
            return await _unitOfWork.reportRepository.GetReportByReportType(typeId);
        }
        
        public async Task<List<ReportDetails>> GetReportByEmpId(int? empId,int? typeId)
        {
            return await _unitOfWork.reportRepository.GetReportByEmpId(empId, typeId);
        }
        //public async Task<List<Buildflow.Infrastructure.Entities.Report>> GetReportByReportType(int? typeId)
        //{
        //    return await _unitOfWork.reportRepository.GetReportByReportType(typeId);
        //}

        public async Task<ReportDetails> GetReportByIdAsync(int reportid)
        {
            return await _unitOfWork.reportRepository.GetReportByIdAsync(reportid);
        }
        public async Task<List<ReportAttachment>> GetReportAttachmentByIdAsync(int reportid)
        {
            return await _unitOfWork.reportRepository.GetReportAttachmentByIdAsync(reportid);
        }
        
    }
}
