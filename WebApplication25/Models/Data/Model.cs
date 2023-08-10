using Microsoft.EntityFrameworkCore;

namespace WebApplication25.Models.Data
{
    public class Model : DbContext
    {
        public Model(DbContextOptions<Model> options) : base(options)
        {
        }
        public DbSet<EntityRole> Role { get; set; }
        public DbSet<EntityUserID> UserID { get; set; }
        public DbSet<SiteEntity> Site { get; set; }
        public DbSet<RoleUserIDEntity> RoleUserID { get; set; }
        public DbSet<RoleSiteEntity> RoleSite{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RoleSiteEntity>()
                .HasKey(e => new { e.RoleID, e.SiteID });

            modelBuilder.Entity<RoleSiteEntity>()
                .HasOne(e => e.Role)
                .WithMany(e => e.RoleSite)
                .HasForeignKey(e => e.RoleID);

            modelBuilder.Entity<RoleSiteEntity>()
                .HasOne(e => e.Site)
                .WithMany()
                .HasForeignKey(e => e.SiteID);

            modelBuilder.Entity<RoleUserIDEntity>()

                .HasKey(e => new { e.EntityRoleID, e.EntityUser });
            modelBuilder.Entity<RoleUserIDEntity>()
               .HasOne(e => e.Role)
               .WithMany(e => e.RoleUserID)
               .HasForeignKey(e => e.EntityRoleID);
            modelBuilder.Entity<RoleUserIDEntity>()
             .HasOne(e => e.UserID)
             .WithMany(e => e.RoleUserID)
             .HasForeignKey(e => e.EntityUser);
        }
    }
}
