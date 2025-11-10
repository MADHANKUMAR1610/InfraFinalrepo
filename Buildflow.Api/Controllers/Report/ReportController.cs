using Buildflow.Api.Model;
using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Infrastructure.Models;
using Buildflow.Service.Service.Project;
using Buildflow.Service.Service.Report;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Buildflow.Api.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {


        private readonly ReportService _service;
        private readonly IConfiguration _config;
        private readonly BuildflowAppContext _context;





        public ReportController(ReportService service, IConfiguration config, BuildflowAppContext context)
        {
            _service = service;
            _config = config;
            _context = context;
            //_env = env;
            //_logger = logger;
        }

        //[HttpPost("upload")]
        //[Consumes("multipart/form-data")]
        //[AllowAnonymous]
        //public async Task<IActionResult> UploadReport([FromForm] UploadReportRequest request)
        //{
        //    if (string.IsNullOrWhiteSpace(request.ReportJson))
        //        return BadRequest("reportJson is required.");

        //    Buildflow.Infrastructure.Entities.Report report;
        //    try
        //    {
        //        report = JsonSerializer.Deserialize<Buildflow.Infrastructure.Entities.Report>(request.ReportJson, new JsonSerializerOptions
        //        {
        //            PropertyNameCaseInsensitive = true
        //        })!;
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Invalid JSON: {ex.Message}");
        //    }

        //    // Upload path
        //    string uploadPath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");

        //    if (!Directory.Exists(uploadPath))
        //        Directory.CreateDirectory(uploadPath);

        //    // Save files and update report paths
        //    foreach (var item in report.ReportDataJson.DailyProgressSummary)
        //    {
        //        var matchingFile = request.Files?.FirstOrDefault(f => f.FileName == item.FileName);
        //        if (matchingFile != null)
        //        {
        //            string savedFileName = $"{Guid.NewGuid()}_{matchingFile.FileName}";
        //            string fullPath = Path.Combine(uploadPath, savedFileName);

        //            using (var stream = new FileStream(fullPath, FileMode.Create))
        //            {
        //                await matchingFile.CopyToAsync(stream);
        //            }

        //            item.FilePath = $"/uploads/{savedFileName}";
        //        }
        //    }

        //    // Save to database
        //    await _service.UpsertReportAsync(report);

        //    return Ok(new { message = "Report saved successfully." });
        //}
        //[HttpPost("upsert-report")]
        //public async Task<IActionResult> UpsertReport([FromForm] string reportJson, [FromForm] List<IFormFile> files)
        //{
        //    // Deserialize the JSON string into the Report object
        //    var report = JsonSerializer.Deserialize<Buildflow.Infrastructure.Entities.Report>(reportJson, new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    });

        //    if (report == null)
        //        return BadRequest("Invalid report data.");

        //    // Match files to DailyProgressItems by index or name
        //    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", "DailyProgress");
        //    Directory.CreateDirectory(uploadFolder);

        //    foreach (var item in report.ReportDataJson.DailyProgressSummary)
        //    {
        //        // Assuming one file per item and files are ordered
        //        var file = files.FirstOrDefault(f => f.FileName == item.FileName);
        //        if (file != null)
        //        {
        //            var savedFileName = $"{Guid.NewGuid()}_{file.FileName}";
        //            var fullPath = Path.Combine(uploadFolder, savedFileName);
        //            using (var stream = new FileStream(fullPath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(stream);
        //            }

        //            item.FilePath = Path.Combine("UploadedFiles", "DailyProgress", savedFileName).Replace("\\", "/");
        //        }
        //    }

        //    await _service.UpsertReportAsync(report); // Call your existing method

        //    return Ok(new { message = "Report saved successfully" });
        //}



        [HttpPost("upsert-report")]
        //[AllowAnonymous]
        public async Task<IActionResult> UpsertReport(Buildflow.Infrastructure.Entities.Report report)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.UpsertReportAsync(report);
            return Ok(new
            {
                message = "Report saved successfully.",
                reportId = report.ReportId
            });
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}"; // Generate GUID-based name
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{uniqueFileName}"; // Return file path to be stored
        }

        [HttpPost("upload-daily-attachments")]
        public async Task<IActionResult> UploadDailyAttachments([FromForm] DailyAttachmentUploadDto dto)
        {
            try
            {
                if (dto == null || dto.SNo == null || dto.Files == null || dto.SNo.Count != dto.Files.Count)
                    return BadRequest("Mismatch between serial numbers and files.");

                var report = await _context.Reports.FirstOrDefaultAsync(r => r.ReportId == dto.ReportId);
                if (report == null)
                    return NotFound($"Report with ID {dto.ReportId} not found.");

                Dictionary<string, JsonElement> reportData;
                try
                {
                    reportData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(report.ReportData.ToString());
                }
                catch (Exception ex)
                {
                    return BadRequest($"Invalid JSON format in report_data: {ex.Message}");
                }

                if (!reportData.TryGetValue("dailyprogresssummary", out var dailyProgressJson))
                    return BadRequest("dailyprogresssummary section not found in report_data.");

                List<Dictionary<string, object>> progressList;
                try
                {
                    progressList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(dailyProgressJson.GetRawText());
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to parse dailyprogresssummary: {ex.Message}");
                }

                for (int i = 0; i < dto.SNo.Count; i++)
                {
                    int serialNo = dto.SNo[i];
                    var file = dto.Files[i];

                    if (file == null || file.Length == 0)
                        return BadRequest($"File at index {i} is null or empty.");

                    string filePath;
                    try
                    {
                        filePath = await SaveFileAsync(file);
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, $"Error saving file at index {i}: {ex.Message}");
                    }

                    //var item = progressList.FirstOrDefault(p => p.ContainsKey("serialno") && Convert.ToInt32(p["serialno"]) == serialNo);
                    var item = progressList.FirstOrDefault(p =>
    p.ContainsKey("serialno") &&
    p["serialno"] is JsonElement je &&
    je.ValueKind == JsonValueKind.Number &&
    je.TryGetInt32(out var value) &&
    value == serialNo
);
                    if (item == null)
                        return BadRequest($"No daily progress item found with serialno {serialNo}.");

                    item["filepath"] = filePath; // ✅ Add the new field here
                }

                var updatedData = new Dictionary<string, object>();
                foreach (var kvp in reportData)
                {
                    if (kvp.Key == "dailyprogresssummary")
                    {
                        var updatedJson = JsonSerializer.Serialize(progressList, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                        });

                        updatedData[kvp.Key] = JsonSerializer.Deserialize<JsonElement>(updatedJson);
                    }
                    else
                    {
                        updatedData[kvp.Key] = kvp.Value;
                    }
                }

                try
                {
                    report.ReportData = JsonSerializer.Serialize(updatedData);
                    report.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error saving updated report data: {ex.Message}");
                }

                return Ok(new { message = "Filepaths added successfully to dailyprogresssummary." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        //[HttpPost("upload-daily-attachments")]
        //public async Task<IActionResult> UploadDailyAttachments([FromForm] DailyAttachmentUploadDto dto)
        //{
        //    if (dto.SNo == null || dto.Files == null || dto.SNo.Count != dto.Files.Count)
        //        return BadRequest("Mismatch between serial numbers and files.");

        //    var savedFiles = new List<object>();

        //    for (int i = 0; i < dto.Files.Count; i++)
        //    {
        //        var file = dto.Files[i];
        //        var serialNo = dto.SNo[i];

        //        var savedFilePath = await SaveFileAsync(file);

        //        savedFiles.Add(new
        //        {
        //            SerialNo = serialNo,
        //            FilePath = savedFilePath
        //        });
        //    }

        //    return Ok(new { message = "Files uploaded successfully.", data = savedFiles });
        //}


        //[HttpPost("upsert-report")]
        //[AllowAnonymous]
        //public async Task<IActionResult> UpsertReport([FromForm] IFormFile file, [FromForm] string reportJson)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    // Deserialize the report JSON
        //    var report = JsonSerializer.Deserialize<Buildflow.Infrastructure.Entities.Report>(reportJson);
        //    if (report == null)
        //        return BadRequest("Invalid report data.");

        //    // Handle file upload if a file is provided
        //    string filePath = null;
        //    if (file != null && file.Length > 0)
        //    {
        //        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        //        Directory.CreateDirectory(uploadPath); // Ensure directory exists

        //        filePath = Path.Combine(uploadPath, file.FileName);
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }
        //    }

        //    // Assign file path to DailyProgressItem (assuming first item in list needs the file)
        //    if (report.ReportDataJson?.DailyProgressSummary != null && report.ReportDataJson.DailyProgressSummary.Any())
        //    {
        //        report.ReportDataJson.DailyProgressSummary.First().FilePath = filePath;
        //    }

        //    // Save or update the report
        //    await _service.UpsertReportAsync(report);

        //    return Ok(new
        //    {
        //        message = "Report saved successfully.",
        //        reportId = report.ReportId,
        //        uploadedFilePath = filePath
        //    });
        //}





        [HttpGet("getreport")]
        public async Task<IActionResult> GetReport()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reports = await _service.GetReportsAsync();

            return Ok(new
            {
                message = "Reports fetched successfully.",
                data = reports
            });
        }

        [HttpGet("getreportbyid")]
        public async Task<IActionResult> GetReportById(int reportid)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var report = await _service.GetReportByIdAsync(reportid);

            if (report == null)
                return NotFound(new { message = $"Report with ID {reportid} not found." });

            return Ok(new
            {
                message = "Report fetched successfully.",
                data = report
            });
        }

        [HttpGet("getreportattachmentbyid")]
        public async Task<IActionResult> GetReportAttachmentById(int reportid)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var report = await _service.GetReportAttachmentByIdAsync(reportid);

            if (report == null)
                return NotFound(new { message = $"Report with ID {reportid} not found." });

            return Ok(new
            {
                message = "Report Attachments fetched successfully.",
                data = report
            });

        }

        [HttpPost("upload-attachments")]
      
        public async Task<IActionResult> UploadAttachments(int reportId, List<IFormFile> files)
        {
            try
            {
                var report = await _context.Reports.FindAsync(reportId);
                if (report == null)
                    return NotFound("Report not found.");

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "report");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var uploadedFiles = new List<object>(); // To hold info about uploaded files

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName);
                        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                        var fullPath = Path.Combine(folderPath, uniqueFileName);
                        var relativePath = Path.Combine("uploads", "report", uniqueFileName);

                        using var stream = new FileStream(fullPath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        var attachment = new ReportAttachment
                        {
                            ReportId = reportId,
                            FileName = uniqueFileName,
                            FilePath = relativePath,
                            UploadedAt = DateTime.Now
                        };

                        await _context.ReportAttachments.AddAsync(attachment);

                        uploadedFiles.Add(new
                        {
                            fileName = uniqueFileName,
                            filePath = relativePath
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Attachments uploaded successfully.",
                    files = uploadedFiles
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "A database error occurred while uploading attachments.",
                    details = dbEx.Message
                });
            }
            catch (IOException ioEx)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while saving the file to the server.",
                    details = ioEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }


        [HttpGet("getreportbyreporttype")]
        public async Task<IActionResult> GetReportByReportType(int? typeId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var report = await _service.GetReportByReportType(typeId);

            if (report == null)
                return NotFound(new { message = $"Report with ID {typeId} not found." });

            return Ok(new
            {
                message = "Report Attachments fetched successfully.",
                data = report
            });

        }

        [HttpGet("getreportbyempid")]

        //[AllowAnonymous]

        public async Task<IActionResult> GetReportByEmpId(int? empId,int? typeId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var report = await _service.GetReportByEmpId(empId,typeId);

            if (report == null)
                return NotFound(new { message = $"Report with ID {empId} not found." });

            return Ok(new
            {
                message = "Report fetched successfully.",
                data = report
            });

        }


        [HttpGet("getreporttypes")]
        public async Task<IActionResult> GetReportTypes()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reports = await _service.GetReportTypes();

            return Ok(new
            {
                message = "Report types fetched successfully.",
                data = reports
            });
        }
        [HttpGet("Get-NewReportCode")]
        public async Task<ActionResult> GetNewReportCode()
        {
            var emp = await _context.Reports.OrderByDescending(e => e.ReportId).FirstOrDefaultAsync();
            //Random random = new Random();
            int NewReportCode = emp != null ? emp.ReportId + 1 : 0;

            return Ok("RP#" + NewReportCode);
        }

    }
}
