using System.ComponentModel.DataAnnotations;

namespace WebApplication25.Models.Data
{
    public class SiteEntity
    {
        public int ID { get; set; }
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        public Int64 Site_ID { get; set; }
         public List<EntityRole>? Role { get; set; }
        public List<RoleSiteEntity>? RoleSite { get; set; }

    }
}
