using Microsoft.EntityFrameworkCore;
using WebApplication25.Models.Data;

namespace WebApplication25.Models
{
    public class DB:DbContext
    {
        public DB(DbContextOptions<DB> options) : base(options)
        {
        }
         public DbSet<EntityGetResponseForInvoicing> GetResponseForInvoicing { get; set; }
    }

   
}
