using Ala.Backend.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ala.Backend.Persistence.Main.Configurations.Identity
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            // Primary Key tanımı
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            // 1. Role tablosu ile olan ilişkiyi KESİNLEŞTİRİYORUZ (Shadow property oluşmasını engeller)
            builder.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles) // Eğer Role sınıfında UserRoles koleksiyonu yoksa sadece .WithMany() yazabilirsin.
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // 2. User tablosu ile olan ilişkiyi KESİNLEŞTİRİYORUZ
            builder.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles) // Eğer User sınıfında UserRoles koleksiyonu yoksa sadece .WithMany() yazabilirsin.
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        }
    }
}