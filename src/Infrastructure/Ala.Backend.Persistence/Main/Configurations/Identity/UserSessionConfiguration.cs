using Ala.Backend.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ala.Backend.Persistence.Main.Configurations.Identity
{
    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.ToTable("UserSessions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.FamilyId)
                .IsRequired();

            builder.Property(x => x.CreatedByIp)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.CreatedByUserAgent)
                .HasMaxLength(512);

            builder.Property(x => x.CreatedOnUtc)
                .IsRequired();

            builder.Property(x => x.LastActivityOnUtc)
                .IsRequired();

            builder.Property(x => x.RevokedByIp)
                .HasMaxLength(64);

            builder.Property(x => x.ReasonRevoked)
                .HasMaxLength(256);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.FamilyId);
            builder.HasIndex(x => new { x.UserId, x.FamilyId })
                .IsUnique();

            builder.HasOne(x => x.User)
                .WithMany(x => x.Sessions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}