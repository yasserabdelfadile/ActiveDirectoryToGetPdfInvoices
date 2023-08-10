using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication25.Models.Data
{

      public class RoleSiteEntity
    {

        [ForeignKey("Role")]
         public int RoleID { get; set; }
        public EntityRole Role { get; set; }

       [ForeignKey("Site")]
        public int SiteID { get; set; }
        
        public SiteEntity Site { get; set; }
    }

}

