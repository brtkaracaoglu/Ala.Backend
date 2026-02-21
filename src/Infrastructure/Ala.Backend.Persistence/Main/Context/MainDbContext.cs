using Ala.Backend.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Ala.Backend.Persistence.Main.Context
{
    public class MainDbContext
     : IdentityDbContext<
         User,
         Role,
         int,
         UserClaim,
         UserRole,
         UserLogin,
         RoleClaim,
         UserToken>
    {
        public MainDbContext(DbContextOptions<MainDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(
                typeof(MainDbContext).Assembly);
        }
    }
}
