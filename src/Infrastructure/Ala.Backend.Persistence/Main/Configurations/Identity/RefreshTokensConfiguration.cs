using Ala.Backend.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ala.Backend.Persistence.Main.Configurations.Identity
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.UserId)
                .IsRequired();

            builder.Property(rt => rt.TokenHash)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(rt => rt.FamilyId)
                .IsRequired();

            builder.Property(rt => rt.ParentRefreshTokenId);

            builder.Property(rt => rt.ReplacedByTokenHash)
                .HasMaxLength(128);

            builder.Property(rt => rt.JwtId)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(rt => rt.ExpiresAtUtc)
                .IsRequired();

            builder.Property(rt => rt.CreatedOnUtc)
                .IsRequired();

            builder.Property(rt => rt.CreatedByIp)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(rt => rt.CreatedByUserAgent)
                .HasMaxLength(512);

            builder.Property(rt => rt.RevokedByIp)
                .HasMaxLength(64);

            builder.Property(rt => rt.ReasonRevoked)
                .HasMaxLength(256);

            builder.Property(rt => rt.RevokedAtUtc);

            builder.Property(rt => rt.IsUsed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(rt => rt.UsedAtUtc);

            builder.HasIndex(rt => rt.TokenHash)
                .IsUnique();

            builder.HasIndex(rt => rt.UserId);
            builder.HasIndex(rt => rt.FamilyId);
            builder.HasIndex(rt => new { rt.UserId, rt.FamilyId });

            builder
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}