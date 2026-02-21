using Ala.Backend.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ala.Backend.Persistence.Main.Configurations.Identity
{
    public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
    {
        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTable("UserLogins");
            builder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });
        }
    }
}