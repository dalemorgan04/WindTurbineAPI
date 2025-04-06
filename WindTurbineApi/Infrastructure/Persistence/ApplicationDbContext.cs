using Microsoft.EntityFrameworkCore;
using WindTurbine.Domain.Entities;

namespace WindTurbineApi.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<SensorRecord> SensorRecords { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Sensor>()
                .HasIndex(s => s.Name)
                .IsUnique();

            modelBuilder.Entity<SensorRecord>()
                .HasOne(tr => tr.Sensor)
                .WithMany(ts => ts.SensorRecords)
                .HasForeignKey(tr => tr.SensorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SensorRecord>()
                .OwnsOne(sr => sr.Reading)
                    .Property(rv => rv.Unit)
                    .HasConversion<string>();
        }
    }
}
