using MeterManagement.Domain.Enums;
using MeterManagement.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MeterManagement.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Meter>(entity =>
            {
                entity.Property(m => m.Status)
                      .HasConversion<string>()
                      .IsRequired();

                entity.HasIndex(x => x.SerialNumber)
                      .IsUnique();

                entity.Property(m => m.Status)
                       .HasDefaultValue(MeterStatus.OnStock);
            });
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "Agent",
                    NormalizedName = "AGENT"
                }
            );
        }

        public DbSet<Meter> Meters { get; set; }

    }
}
