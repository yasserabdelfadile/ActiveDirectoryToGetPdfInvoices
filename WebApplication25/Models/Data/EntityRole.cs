using System.ComponentModel.DataAnnotations;

namespace WebApplication25.Models.Data
{
    public class EntityRole
    {
        public int ID { get; set; }
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
          public List<EntityUserID>? UserID { get; set; }
        public List<SiteEntity>? Site { get; set; }
        public List<RoleSiteEntity>? RoleSite { get; set; }
        public List<RoleUserIDEntity>? RoleUserID { get; set; }


    }
}
