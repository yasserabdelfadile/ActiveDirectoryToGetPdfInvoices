using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication25.Models;
using WebApplication25.Models.Data;
using WebApplication25.Models.ModelView;
using static Azure.Core.HttpHeader;

namespace WebApplication25.Controllers
{
    [Authorize(Roles = "IT")]
    public class EntityUserIDsController : Controller
    {
        private readonly Model _context;
        private readonly IHttpContextAccessor _accessor;
        private IConfiguration config;
        private string? connectionString;
        private string domainName = "Alshahin.local ";
 
        public EntityUserIDsController(Model context , IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
 
            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            connectionString = config.GetConnectionString("SecondConection");
        }

        // GET: EntityUserIDs
        public async Task<IActionResult> Index()
        {
            var role = await _context.UserID.OrderBy(x=>x.Name).ToListAsync();
            var roleforms = new List<UserForm>();
            foreach (var item in role)
            {
                var site = await _context.RoleUserID.Include(x => x.Role)
                        .Where(x => x.EntityUser == item.ID && x.Role.ID == x.EntityRoleID).Select(x => x.Role.Name).ToListAsync();
                var roleform = new UserForm
                {
                    ID = item.ID,
                    Name = item.Name,
                    SamAccountName=item.SamAccountName,
                    RoleForm = site
                };
                roleforms.Add(roleform);

            }
            return roleforms != null ? 
                          View(roleforms) :
                          Problem("Entity set 'Model.UserID'  is null.");
        }


        private bool IsSelect(int? userid, int roleid)
        {
            bool select = false;

            var selectedSites = _context.RoleUserID
        .Where(rs => rs.EntityUser == userid && rs.EntityRoleID == roleid)
        .Select(rs => rs.EntityRoleID).ToList();
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
        //===========================================================
        // GET: EntityUserIDs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserID == null)
            {
                return NotFound();
            }
            var entityUserID = await _context.UserID.FindAsync(id);
            if (entityUserID == null)
            {
                return NotFound();
            }
            var roles = await _context.Role.ToListAsync();
            var selectedRole = await _context.RoleUserID.Where(x => x.EntityUser == id).Select(x => x.EntityRoleID).ToListAsync();
            var userroleview = new UserRoleForm
            {
                ID=entityUserID.ID,
                Name=entityUserID.Name,
                roleUs=roles.Select(x=>new RoleUForm
                {
                    ID= x.ID,
                    Name=x.Name,
                    IsSelected=IsSelect(id,x.ID)
                }).ToList()
            };
            return View(userroleview);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserRoleForm model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRole = await _context.UserID
                        .Include(r => r.Role)
                        .FirstOrDefaultAsync(r => r.ID == model.ID);

                    if (existingRole == null)
                    {
                        return NotFound();
                    }
                    var selectedSites = _context.RoleUserID
                     .Where(rs => rs.EntityUser == model.ID)
                     .Select(rs => rs.EntityRoleID)
                     .ToList();
                    foreach (var item in model.roleUs)
                    {
                        var sitid = _context.Role.Where(z => z.Name == item.Name).Select(z => z.ID);
                        int x = 0;
                        foreach (var n in sitid) { x = n; }
                        if (selectedSites.Any(a => a == x) && !item.IsSelected)
                        {

                            var entityRoleSiteEntity = new RoleUserIDEntity
                            {
                                 EntityUser = model.ID,
                                EntityRoleID = x
                            };
                            _context.RoleUserID.Remove(entityRoleSiteEntity);
                        }
                        if (!selectedSites.Any(a => a == x) && item.IsSelected)
                        {
                            var entityRoleSiteEntity = new RoleUserIDEntity
                            {
                                EntityUser = model.ID,
                                EntityRoleID = x
                            };
                            _context.RoleUserID.Add(entityRoleSiteEntity);
                        }
                    }
 
                    _context.Update(existingRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!EntityUserIDExists(model.ID))
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
            return View(model);
        }
        //=============================================================
        private bool EntityUserIDExists(int id)
        {
          return (_context.UserID?.Any(e => e.ID == id)).GetValueOrDefault();
        }
        //==================================================================
        //store all sid and varefiy if it store 
        public async Task<IActionResult> storeuser()
        {
            try
            {
                //create database table to link user with id and link user with role    
                // and by this filter data through this
                // Specify the domain and container for your Active Directory
                // e.g., mydomain.local
                string[] details = { " " };                                    //string containerName = "CONTAINER"; // e.g., UserID
                List<string> list = new List<string>();
                // Create a PrincipalContext using the domain and container
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domainName))
                {
                    // Create a UserPrincipal object to perform a user search
                    using (UserPrincipal userPrincipal = new UserPrincipal(context))
                    {
                        // Create a PrincipalSearcher to search for users
                        using (PrincipalSearcher searcher = new PrincipalSearcher(userPrincipal))
                        {
                            // Get the search results (all users matching the search criteria)
                            PrincipalSearchResult<Principal> searchResults = searcher.FindAll();

                            // StringBuilder to store the users' data
                            StringBuilder userData = new StringBuilder();

                            // Loop through each user and append their data to the StringBuilder
                            foreach (UserPrincipal user in searchResults)
                            {
                                var Name = user.Name;
                                var sadnam = user.SamAccountName;
                                var SID = user.Sid.ToString();
                                string verfiy = curentuser(sadnam);
                                if (verfiy == "NULL")
                                {
                                    using (SqlConnection connection = new SqlConnection(connectionString))
                                    {

                                        try
                                        {
                                            connection.Open();
                                            using (SqlCommand command = new SqlCommand("insert into UserID (Name,SamAccountName,SID) values (@Name,@SamAccountName,@Sid );", connection))
                                            {
                                                command.CommandTimeout = 120;
                                                command.Parameters.AddWithValue("@Name", Name);
                                                command.Parameters.AddWithValue("@SamAccountName", sadnam);
                                                command.Parameters.AddWithValue("@Sid", SID);
                                                int rowsAffected = await command.ExecuteNonQueryAsync();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            return Content(ex.Message);
                                        }
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }


        //================================
        public string curentuser(string userSamAccountName)
        {
            try
            {
                // Get the authenticated user's SamAccountName
                // string userSamAccountName = _accessor.HttpContext.User.Identity.Name;

                // Specify the domain and container for your Active Directory

                // Create a PrincipalContext using the domain and container
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domainName))
                {
                    // Find the user by their SamAccountName
                    UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userSamAccountName);
                    string Result = "NULL";
                    if (userPrincipal != null)
                    {
                        // Collect user information
                        string SAName = userPrincipal.SamAccountName;
                        string Sid = userPrincipal.Sid.ToString();
                        //========================================================
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            // open the connection
                            connection.Open();
                            // create a SQL command object
                            SqlCommand command = new SqlCommand("select Name from UserID where SID=@SID and SamAccountName=@Name ", connection);
                            command.CommandTimeout = 120;
                            command.Parameters.AddWithValue("@SID", Sid);
                            command.Parameters.AddWithValue("@Name", SAName);
                            List<string> list = new List<string>();
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                List<string> idList = new List<string>();
                                while (reader.Read())
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        Result = reader.GetString(reader.GetOrdinal("Name"));
                                    }
                                }
                            }
                        }

                        return Result;

                    }
                    else
                    {
                        // Handle the case when the user is not found
                        return "User not found in Active Directory.";
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the process
                return "Error: " + ex.Message;
            }

        }
        //==================================================================
        public class ActiveDirectoryHelper
        {
            public List<string> GetGroupsForUser(string userSamAccountName, string domainName, string containerName)
            {
                List<string> userGroups = new List<string>();

                try
                {
                    using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domainName, containerName))
                    {
                        // Find the user by their SamAccountName
                        UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, userSamAccountName);

                        if (userPrincipal != null)
                        {
                            // Get the user's principal entry
                            PrincipalSearchResult<Principal>? groups = userPrincipal?.GetAuthorizationGroups();

                            foreach (Principal group in groups)
                            {
                                // Check if the principal is a GroupPrincipal (i.e., a group)
                                if (group is GroupPrincipal)
                                {
                                    // Add the group name to the list
                                    userGroups.Add(group.Name);
                                }
                            }
                        }
                        else
                        {
                            // Handle the case when the user is not found
                            Console.WriteLine("User not found in Active Directory.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that may occur during the process
                    Console.WriteLine("Error: " + ex.Message);
                }
                return userGroups;
            }
        }

        //==================================================================   delete UserID
        public async Task<string> Delete()
        {
            int rowsAffected;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(" delete UserID ", connection))
                    {
                        command.CommandTimeout = 120;
                             rowsAffected = await command.ExecuteNonQueryAsync();
                    }
                    return "Ok number of row affected : "+ rowsAffected;    
                }
                catch (Exception ex)
                {
                    return (ex.Message);
                }
            }


        }
        //==================================================================



    }
}
