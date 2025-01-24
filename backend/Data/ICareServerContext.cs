using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    //Mário
    public class ICareServerContext : IdentityDbContext<User>
    {
        public ICareServerContext(DbContextOptions<ICareServerContext> options)
            : base(options)
        {
        }
        public DbSet<backend.Models.User> User { get; set; } = default!;
        public DbSet<backend.Models.UserLog> UserLogs { get; set; } = default!;

    }
}
