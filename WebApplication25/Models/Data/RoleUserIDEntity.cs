using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication25.Models.Data
{
     public class RoleUserIDEntity
    {
        [ForeignKey("Role")]
        public int EntityRoleID { get; set; }
        public EntityRole Role { get; set; }
        [ForeignKey("UserID")]
        public int EntityUser { get; set; }
        public EntityUserID UserID { get; set; }

    }
}
