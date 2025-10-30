using FIAP.FCG.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIAP.FCG.Infra.Configuration
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("User");
			builder.HasKey(p => p.Id);
			builder.Property(p => p.Id).HasColumnType("INT").UseIdentityColumn();
			builder.Property(p => p.Name).HasColumnType("VARCHAR(100)").IsRequired();
			builder.Property(p => p.Email).HasColumnType("VARCHAR(50)").IsRequired();
			builder.Property(p => p.Password).HasColumnType("VARCHAR(20)").IsRequired();
			builder.Property(p => p.IsAdmin).HasColumnType("BIT").IsRequired();
		}
	}
}
