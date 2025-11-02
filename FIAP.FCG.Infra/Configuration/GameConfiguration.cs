using FIAP.FCG.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIAP.FCG.Infra.Configuration
{
	public class GameConfiguration : IEntityTypeConfiguration<Game>
	{
		public void Configure(EntityTypeBuilder<Game> builder)
		{
			builder.ToTable("Game");
			builder.HasKey(p => p.Id);
			builder.Property(p => p.Id).HasColumnType("INT").UseIdentityColumn();
			builder.Property(p => p.Name).HasColumnType("VARCHAR(100)").IsRequired();
			builder.Property(p => p.Platform).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.PublisherName).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.Description).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(p => p.Price).HasColumnType("DECIMAL(12,2)").IsRequired();
		}
	}
}
