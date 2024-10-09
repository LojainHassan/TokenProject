using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TokenProject.Model
{
    public class DbContext:IdentityDbContext<ApplicationUser>
    {
        public DbContext(DbContextOptions<DbContext> options):base(options)
        {

        }
    }
}
