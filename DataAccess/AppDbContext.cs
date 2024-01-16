using System.Reflection;

namespace pdfsharp_online_backend.DataAccess
{
    public class AppDbContext: DbContext
    {
        public DbSet<ImageItem> ImageItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles{ get; set; }
        public DbSet<EditView> EditViews { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
