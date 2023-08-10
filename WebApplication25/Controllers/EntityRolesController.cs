using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication25.Models.Data;
using WebApplication25.Models.ModelView;

namespace WebApplication25.Controllers
{
    [Authorize(Roles ="IT")]
    public class EntityRolesController : Controller
    {
        private readonly Model _context;

        public EntityRolesController(Model context)
        {
            _context = context;
        }

        // GET: EntityRoles
        public async Task<IActionResult> Index()
        {

            var role=await _context.Role.ToListAsync();
             var roleforms=new List<RoleForm>();
             foreach(var item in role)
             {
                var site=await _context.RoleSite.Include(x=>x.Site)
                        .Where(x=>x.RoleID==item.ID&& x.Site.ID==x.SiteID).Select(x=>x.Site.Name).ToListAsync();
                var roleform = new RoleForm
                {
                    ID = item.ID,
                    Name = item.Name,
                    SiteForm = site
                };
                roleforms.Add(roleform);

             }
              return roleforms != null ? 
                          View(roleforms) :
                          Problem("Entity set 'Model.Role'  is null.");
        }

        

        [HttpGet]
        public IActionResult Create()
        {
            var model = new EntityRole();
             var sites = _context.Site.ToList();
            ViewBag.Sites = sites;
            return View(model); 
        }


        // POST: EntityRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] EntityRole entityRole, int[] selectedSites)
        {
            if (ModelState.IsValid)
            {
                if (selectedSites != null)
                {
                    foreach (var siteId in selectedSites)
                    {
                        var siteEntity = await _context.Site.FindAsync(siteId);
                        if (siteEntity != null)
                        {
                            var entityRoleSiteEntity = new RoleSiteEntity
                            {
                                Role = entityRole,
                                Site = siteEntity
                            };

                            _context.RoleSite.Add(entityRoleSiteEntity);
                        }
                    }
                }

                _context.Add(entityRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(entityRole);
        }
        private bool IsSelect(int? rolid, Int64 sitid)
        {
            bool select = false;

            var selectedSites = _context.RoleSite
        .Where(rs => rs.RoleID == rolid && rs.SiteID == sitid)
        .Select(rs => rs.SiteID).ToList();
            foreach (var selectedSite in selectedSites)
            {
                int v = selectedSite;
                string x = selectedSite.ToString();
                if (x != null)
                {
                    select = true;
                }
            }
            return select;
        }
        // GET: EntityRoles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Role == null)
            {
                return NotFound();
            }
            var entityRole = await _context.Role.FindAsync(id);
            if (entityRole == null)
            {
                return NotFound();
            }
            var sites = await _context.Site.ToListAsync();
            var selectedSites = _context.RoleSite
                .Where(rs => rs.RoleID == id)
                .Select(rs => rs.SiteID)
                .ToList();
            var ViewModel = new RoleUserView
            {
                ID = entityRole.ID,
                Name = entityRole.Name,
                sites =sites.Select(x=> new SiteView
                {
                    Id=x.Site_ID,
                    Name=x.Name,
                    IsSelected= IsSelect(id ,x.ID)
                }).ToList()
            };
  
            return View(ViewModel);
        }
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleUserView Model)
        {
           
            if (ModelState.IsValid)
            {
                try
                {
                    var existingRole = await _context.Role
                        .Include(r => r.Site)
                        .FirstOrDefaultAsync(r => r.ID == Model.ID);

                    if (existingRole == null)
                    {
                        return NotFound();
                    }
                  var selectedSites = _context.RoleSite
                   .Where(rs => rs.RoleID == Model.ID)
                   .Select(rs => rs.SiteID)
                   .ToList();
                  //  var sites = await _context.Site.ToListAsync();

                    foreach (var item in Model.sites)
                        {
                             var  sitid=_context.Site.Where(z=>z.Name== item.Name).Select(z=>z.ID);
                                   int x=0;
                        foreach(var n in sitid) { x = n; }
                            if(selectedSites.Any(a=>a==x)&& !item.IsSelected)
                            {
                                
                                var entityRoleSiteEntity = new RoleSiteEntity
                                {
                                    RoleID = Model.ID,
                                    SiteID = x
                                };
                                _context.RoleSite.Remove(entityRoleSiteEntity);
                            }
                            if (!selectedSites.Any(a => a == x) && item.IsSelected)
                            {
                                var entityRoleSiteEntity = new RoleSiteEntity
                                {
                                    RoleID = Model.ID,
                                    SiteID = x
                                };
                                _context.RoleSite.Add(entityRoleSiteEntity);
                            }
                    }
                    existingRole.Name = Model.Name;

                    _context.Update(existingRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!EntityRoleExists(Model.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Content(ex.Message);
                    }
                }
                return RedirectToAction(nameof(Index));
            }

              return View( Model);
        }

      
        //=======================================================
      

        // GET: EntityRoles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Role == null)
            {
                return NotFound();
            }

            var entityRole = await _context.Role
                .FirstOrDefaultAsync(m => m.ID == id);
            if (entityRole == null)
            {
                return NotFound();
            }

            return View(entityRole);
        }

        // POST: EntityRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Role == null)
            {
                return Problem("Entity set 'Model.Role'  is null.");
            }
            var entityRole = await _context.Role.FindAsync(id);
            var sites=await _context.RoleSite.Where(x=>x.RoleID == id).Select(x=>x.SiteID).ToListAsync();  
            foreach(var site in sites)
            {
                var entityRoleSiteEntity = new RoleSiteEntity
                {
                    RoleID = id,
                    SiteID = site
                };
                _context.RoleSite.Remove(entityRoleSiteEntity);
            }

            if (entityRole != null)
            {
                _context.Role.Remove(entityRole);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntityRoleExists(int id)
        {
          return (_context.Role?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
