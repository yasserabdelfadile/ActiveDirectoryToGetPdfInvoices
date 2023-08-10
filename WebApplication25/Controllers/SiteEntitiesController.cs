using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication25.Models.Data;

namespace WebApplication25.Controllers
{
    [Authorize(Roles ="IT")]
    public class SiteEntitiesController : Controller
    {
        private readonly Model _context;
         private IConfiguration config;
        private string? connectionString;
        private string? connectionString1;
        public SiteEntitiesController(Model context)
        {
            _context = context;
            config = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                         .Build();
            connectionString = config.GetConnectionString("mSales");
            connectionString1 = config.GetConnectionString("SecondConection");
        }

        //=====================================================================================
        public List<string> InsertSit()
        {
            try
            {
                List<string> idList = new List<string>();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // open the connection
                    connection.Open();
                    // create a SQL command object
                    SqlCommand command = new SqlCommand("select NAME,SITE_ID from INV_SITE", connection);
                    command.CommandTimeout = 120;
                    List<string> list = new List<string>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            //for (int i = 0; i < reader.FieldCount; i++)
                            //{
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            string Sid = reader.GetInt64(reader.GetOrdinal("SITE_ID")).ToString();
                            string value = verfiy(reader.GetInt64(reader.GetOrdinal("SITE_ID")));
                            if (value != null && value=="NULL")
                            {
                                idList.Add(name);
                                idList.Add(Sid);
                            }
                            else
                            {
                                continue;
                            }


                            //}
                        }
                    }
                }
                return idList;
            }
            catch (Exception ex)
            {
                List<string> IdList = new List<string>();
                IdList.Add(ex.Message);
                return IdList;
            }
        }

        //=============================================
        public async Task<IActionResult> update()
        {
            try
            {
                var name = ""; Int64 sid = 0;
                List<string> site = InsertSit();
                for (int i = 0; i < (site.Count / 2); i++)
                {
                    if (i == 0)
                    {
                        name = site[i];
                        sid = Int64.Parse(site[i + 1]);
                    }
                    else
                    {
                        if (i % 2 == 1)
                        {
                            name = site[i + i];
                            sid = Int64.Parse(site[2 * i + 1]);
                        }
                        else
                        {
                            name = site[i * 2];
                            sid = Int64.Parse(site[2 * i + 1]);
                        }

                    }


                    using (SqlConnection connection = new SqlConnection(connectionString1))
                    {

                        try
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand("insert into Site (Name,Site_ID) VALUES(@Name,@siteid);", connection))
                            {
                                command.CommandTimeout = 120;
                                command.Parameters.AddWithValue("@Name", name);
                                command.Parameters.AddWithValue("@siteid", sid);
                                int rowsAffected = await command.ExecuteNonQueryAsync();
                            }
                        }

                        catch (Exception ex)
                        {
                            return Content(ex.Message);
                        }
                    }
                }
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {

                return Content(ex.Message);
            }
        }
        //================================================================
        public string verfiy(Int64 Siteid )
        {
            try
            {
                string resulte = "NULL";
                 using (SqlConnection connection = new SqlConnection(connectionString1))
                {
                    // open the connection
                    connection.Open();
                    // create a SQL command object
                    SqlCommand command = new SqlCommand("select Name from Site where Site_ID=@Site_id ", connection);
                    command.Parameters.AddWithValue("@Site_id",Siteid);
                    command.CommandTimeout = 120;
                    List<string> list = new List<string>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                              resulte = reader.GetString(reader.GetOrdinal("Name"));
                         
                        }
                    }
                }

                return  resulte;
            }
            catch (Exception)
            {

                throw;
            }

        }
        //================================================================
        //================================================================
        // GET: SiteEntities
        public async Task<IActionResult> Index()
        {
              return _context.Site != null ? 
                          View(await _context.Site.ToListAsync()) :
                          Problem("Entity set 'Model.Site'  is null.");
        }

        // GET: SiteEntities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Site == null)
            {
                return NotFound();
            }

            var siteEntity = await _context.Site
                .FirstOrDefaultAsync(m => m.ID == id);
            if (siteEntity == null)
            {
                return NotFound();
            }

            return View(siteEntity);
        }

        // GET: SiteEntities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SiteEntities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Site_ID,RoleID")] SiteEntity siteEntity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(siteEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(siteEntity);
        }

        // GET: SiteEntities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Site == null)
            {
                return NotFound();
            }

            var siteEntity = await _context.Site.FindAsync(id);
            if (siteEntity == null)
            {
                return NotFound();
            }
            return View(siteEntity);
        }

        // POST: SiteEntities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Site_ID,RoleID")] SiteEntity siteEntity)
        {
            if (id != siteEntity.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(siteEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SiteEntityExists(siteEntity.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(siteEntity);
        }

        // GET: SiteEntities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Site == null)
            {
                return NotFound();
            }

            var siteEntity = await _context.Site
                .FirstOrDefaultAsync(m => m.ID == id);
            if (siteEntity == null)
            {
                return NotFound();
            }

            return View(siteEntity);
        }

        // POST: SiteEntities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Site == null)
            {
                return Problem("Entity set 'Model.Site'  is null.");
            }
            var siteEntity = await _context.Site.FindAsync(id);
            if (siteEntity != null)
            {
                _context.Site.Remove(siteEntity);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SiteEntityExists(int id)
        {
          return (_context.Site?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
