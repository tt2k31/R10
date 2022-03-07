using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
// R08.models.MyBlogContext
namespace R10.models
{
    public class MyBlogContext : IdentityDbContext<AppUser>
    {
        public MyBlogContext(DbContextOptions<MyBlogContext> options) : base(options)
        {
            //thiết lập thêm khi tạo ra

        }

        public DbSet<Article> articles {set; get;}


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var e in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = e.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    e.SetTableName(tableName.Substring(6));
                }
            }
        }
    }
}