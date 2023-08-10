using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.AccountManagement;
using WebApplication25.Models;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using NuGet.Packaging;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using WebApplication25.Models.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.IO.Compression;
using OfficeOpenXml;

namespace WebApplication25.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly DB _db;
        private readonly IMemoryCache _cache;
        private readonly Model _context;
        private IConfiguration config;
        private string? connectionString;
        private string domainName = "Alshahin.local ";
         public HomeController(ILogger<HomeController> logger,IHttpContextAccessor accessor, DB db, IMemoryCache cache, Model context)
        {
            _logger = logger;
            _accessor = accessor;
            _db = db;
            _context = context;
            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            connectionString = config.GetConnectionString("SecondConection");
            _cache = cache;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {

            try
            {

             //  var  userSamAccountName = _accessor?.HttpContext?.User?.Identity?.Name;
                //userSamAccountName = userSamAccountName.Replace("ALSHAHIN\\yasseres", "yasseres");
                    //  List<GetResponseForInvoicingEntity> query = new List<GetResponseForInvoicingEntity>();
                string name = curentuser();
                var user=await _context.UserID.Include(x=> x.RoleUserID)
                              .Where(x=>x.SamAccountName== name)
                         .Select(x=>x.RoleUserID).ToListAsync();
                int roleid=0;
                var ID =new List<int>();
                foreach(var nvcv in user)
                {
                     for(int i=0; i<nvcv.Count; i++)
                     {
                        if (nvcv.Count>0)
                        {
                            var c = nvcv[i];
                            roleid = c.EntityRoleID;
                            ID.Add(roleid);
                        } 
                     }
                 }
                var role = new List<long>();
                role=await _context.RoleSite.Include(x=>x.Site).Where(x=>ID.Contains(x.RoleID) ).Select(x=>x.Site.Site_ID).ToListAsync();

                // Check if data is already cached
                if (!_cache.TryGetValue("ResponseForInvoicingData", out List<EntityGetResponseForInvoicing>? query))
                {
                    var CachInM = new MemoryCacheEntryOptions()
                                    //.SetSlidingExpiration(TimeSpan.FromSeconds(30))
                                    .SetAbsoluteExpiration(TimeSpan.FromHours(1))
                                    .SetPriority(CacheItemPriority.Normal);
                    query = await _db.GetResponseForInvoicing.Where(x => x.Binary_File != null).ToListAsync();
                    _cache.Set("ResponseForInvoicingData", query, CachInM); 
                }
                query= query?.Where(x=>role.Contains((long)x.Site_ID)).OrderByDescending(x => x.UploadDate).ToList();
                if (startDate != null && endDate != null)
                {
                    query = query?.Where(x => x.UploadDate >= startDate && x.UploadDate <= endDate).OrderByDescending(x => x.UploadDate).ToList();
                }
                // var data = query;
                 return View(query);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }

        //===============================================

        public IActionResult DownloadPDF(int id)
        {
            try
            {
                var data = _db.GetResponseForInvoicing.FirstOrDefault(x => x.ID == id && x.Binary_File != null);
                if (data != null)
                {
                    // Generate the file name as InternalID.pdf
                    string fileName = $"{data.internalID}.pdf";

                    // Return the file as bytes
                    return File(data.Binary_File, "application/pdf", fileName);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {

                return Content(ex.Message);
            }


        }

        public IActionResult PreviewPDF(int id)
        {

            try
            {
                var data = _db.GetResponseForInvoicing.FirstOrDefault(x => x.ID == id && x.Binary_File != null);
                if (data != null)
                {
                    // Generate the file name as InternalID.pdf
                    string fileName = $"{data.internalID}.pdf";

                    // Return the file as bytes with "inline" content disposition
                    var fileContentResult = new FileContentResult(data.Binary_File, "application/pdf")
                    {
                        FileDownloadName = fileName,
                    };
                    fileContentResult.FileDownloadName = null;
                    Response.Headers["Content-Disposition"] = $"inline; filename={fileName}";

                    return fileContentResult;
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {

                return Content(ex.Message);
            }

        }


        public IActionResult DownloadSelected(string ids)
        {

            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    var selectedIds = ids.Split(',').Select(int.Parse).ToList();
                    var files = _db.GetResponseForInvoicing.Where(x => selectedIds.Contains(x.ID) && x.Binary_File != null).ToList();

                    if (files.Count > 0)
                    {
                        var zipStream = new MemoryStream();
                        using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                        {
                            foreach (var file in files)
                            {
                                var entry = zipArchive.CreateEntry($"{file.internalID}.pdf", System.IO.Compression.CompressionLevel.Optimal);
                                using (var entryStream = entry.Open())
                                {
                                    entryStream.Write(file.Binary_File, 0, file.Binary_File.Length);
                                }
                            }
                        }

                        zipStream.Position = 0;
                        return File(zipStream.ToArray(), "application/zip", "SelectedFiles.zip");
                    }
                }

                return NotFound();
            }
            catch (Exception ex)
            {

                return Content(ex.Message);
            }


        }


        public IActionResult DownloadAll()
        {
            try
            {
                var files = _db.GetResponseForInvoicing.Where(x => x.Binary_File != null).ToList();

                if (files.Count > 0)
                {
                    var zipStream = new MemoryStream();
                    using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var file in files)
                        {
                            var entry = zipArchive.CreateEntry($"{file.internalID}.pdf", System.IO.Compression.CompressionLevel.Optimal);
                            using (var entryStream = entry.Open())
                            {
                                entryStream.Write(file.Binary_File, 0, file.Binary_File.Length);
                            }
                        }
                    }

                    zipStream.Position = 0;
                    return File(zipStream.ToArray(), "application/zip", "AllFiles.zip");
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }




 
        public async Task <IActionResult> DownloadGridViewAsExcel()
        {


 
            List<EntityGetResponseForInvoicing> query = new List<EntityGetResponseForInvoicing>();
            var siteIDs = new List<long?>();

            string name = curentuser();
            var user = await _context.UserID.Include(x => x.RoleUserID)
                          .Where(x => x.SamAccountName == name)
                     .Select(x => x.RoleUserID).ToListAsync();
            int roleid = 0;
            var ID = new List<int>();
            foreach (var nvcv in user)
            {
                for (int i = 0; i < nvcv.Count; i++)
                {
                    if (nvcv.Count > 0)
                    {
                        var c = nvcv[i];
                        roleid = c.EntityRoleID;
                        ID.Add(roleid);
                    }
                }
            }
            var role = new List<long>();
            role = await _context.RoleSite.Include(x => x.Site).Where(x => ID.Contains(x.RoleID)).Select(x => x.Site.Site_ID).ToListAsync();


            query = await _db.GetResponseForInvoicing.Where(x => x.Binary_File != null).ToListAsync();

            query = query.Where(x => role.Contains((long)x.Site_ID)).OrderByDescending(x => x.UploadDate).ToList();


            var data = query; // Retrieve the data from the database or any other source
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo("MyWorkbook.xlsx")))
            {
                var worksheet = package.Workbook.Worksheets.Add("Grid View");

                // Set the headers
                var headers = new List<string> { "#", "InternalID", "Receiver Name", "Upload Date", "Sales Order Reference", "Registration Number" };
                for (var i = 0; i < headers.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Populate the data rows
                for (var i = 0; i < data.Count; i++)
                {
                    var row = i + 2; // Start from the second row
                    worksheet.Cells[row, 1].Value = i + 1;
                    worksheet.Cells[row, 2].Value = data[i].internalID;
                    worksheet.Cells[row, 3].Value = data[i].ReciverName;

                    worksheet.Cells[row, 4].Value = data[i].UploadDate.ToString();
                    worksheet.Cells[row, 5].Value = data[i].salesOrderReference;
                    worksheet.Cells[row, 6].Value = data[i].Registration_Number;
                }

                // Auto fit the columns
                worksheet.Cells.AutoFitColumns();

                // Generate the byte array of the Excel file
                var fileBytes = package.GetAsByteArray();

                // Set the content type and file name for the response
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var date = DateTime.Now;
                var fileName = "GridView-Report-" + date + ".xlsx";

                // Return the Excel file as a downloadable attachment
                return  File(fileBytes, contentType, fileName);
            }
        }


        //===============================================
        public IActionResult Privacy()
        {
            return View();
        }
        //======================================================
        //======================================================
        public string curentuser()
        {
            try
            {
                // Get the authenticated user's SamAccountName
                string userSamAccountName = _accessor.HttpContext.User.Identity.Name;

                // Specify the domain and container for your Active Directory

                // Create a PrincipalContext using the domain and container
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domainName))
                {
                    // Find the user by their SamAccountName
                    UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userSamAccountName);
                   
                        // Collect user information
                        string SamAccountName = userPrincipal.SamAccountName;
                        string Name= userPrincipal.Name;
                        string Sid = userPrincipal.Sid.ToString();
                    return SamAccountName;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the process
                return "Error: " + ex.Message;
            }

        }

        //======================================================
        public IActionResult ClearCach()
        {
            try
            {
                _cache.Remove("ResponseForInvoicingData");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                return Content(ex.Message);
            }

        }
        //======================================================

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}