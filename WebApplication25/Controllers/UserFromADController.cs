using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices.AccountManagement;
using System.Text;
using WebApplication25.Models.Data;

namespace WebApplication25.Controllers
{
    public class UserFromADController : Controller
    {

        private readonly IHttpContextAccessor _accessor;
        private IConfiguration config;
        private string? connectionString;
         private string domainName = "Alshahin.local ";
        private readonly Model _db;
        public UserFromADController(IHttpContextAccessor accessor, Model db)
        {
             _accessor = accessor;
            _db = db;

            config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            connectionString = config.GetConnectionString("SecondConection");
        }

        //store all sid and varefiy if it store 
        public async Task<string> storeuser()
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
                                //userData.AppendLine($"User Name: {user.Name}");
                                //userData.AppendLine($"User SamAccountName: {user.SamAccountName}");
                                //userData.AppendLine($"User Distinguished Name: {user.DistinguishedName}");
                                //userData.AppendLine($"User Email Address: {user.EmailAddress}");
                                //userData.AppendLine($"User Enabled: {user.Enabled}");
                                //userData.AppendLine("----------");
                                var Name = user.Name;
                                var sadnam = user.SamAccountName;
                                var SID = user.Sid.ToString();
                                string verfiy = curentuser(sadnam);
                                if(verfiy=="NULL")
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
                                            return ex.Message;
                                        }



                                    }

                                }
                                else
                                {
                                    continue;
                                }



                            }

                            return userData.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
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

        //==================================================
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

 
        public async Task<IActionResult> Index()
        {
            string name=await storeuser();
             //return all users
            return View(await _db.UserID.ToListAsync());
        }

        // edit or assign role to user 
        public async Task<IActionResult>ManageUser(string sid)
        {
            var update = await _db.UserID.FirstOrDefaultAsync(x=>x.SID==sid);
            var role= await _db.Role.ToListAsync();
            if (update == null)
            {
                return NotFound();
            }

            if(role.Any()) 
            {
                return Content("at Least assign user with one role");
            }
             
             update.Role = (List<EntityRole>?)role.Select(x=>x.Name);
               _db.Update(update);

            return View();

        }



    }
}
