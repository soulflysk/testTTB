using Microsoft.EntityFrameworkCore;
namespace DOTNETWEBAPI_DEV.Data
{
    public class DbContextClass : DbContext
{
    public DbContextClass(DbContextOptions<DbContextClass> options) : base(options) { }

        public DbSet<Claim> Claims { get; set; }
        }
    }
