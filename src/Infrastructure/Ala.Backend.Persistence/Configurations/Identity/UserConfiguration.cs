using Ala.Backend.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ala.Backend.Persistence.Configurations.Identity
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.Property(x => x.FirstName).IsRequired();
            builder.Property(x => x.LastName).IsRequired();

            builder.Property(x => x.UserName).HasMaxLength(256);
            builder.Property(x => x.NormalizedUserName).HasMaxLength(256);

            builder.Property(x => x.Email).HasMaxLength(256);
            builder.Property(x => x.NormalizedEmail).HasMaxLength(256);
        }
    }
}