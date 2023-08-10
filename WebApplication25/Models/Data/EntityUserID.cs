using System.ComponentModel.DataAnnotations;

namespace WebApplication25.Models.Data
{
    public class EntityUserID
    {
        public int ID { get; set; }
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        [MaxLength(255)]
        public string? SamAccountName { get; set; }
        [MaxLength(255)]
        [Required]
        public string SID { get; set; }
         public List<EntityRole>? Role { get; set; }
        public List<RoleUserIDEntity>? RoleUserID { get; set; }
    }
}
