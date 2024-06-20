using IPInformationRetrieval.Models;
using Microsoft.EntityFrameworkCore;

namespace IPInformationRetrieval.Database.DatabaseContext
{
    public class IpInformationDbContext : DbContext
    {
        public IpInformationDbContext(DbContextOptions<IpInformationDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
        public DbSet<Country> Countries { get; set; }
        public DbSet<IpAddress> IpAddresses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>().HasKey(x => x.Id);
            modelBuilder.Entity<Country>().ToTable("Countries");

            modelBuilder.Entity<IpAddress>().HasKey(x => x.Id);
            modelBuilder.Entity<IpAddress>().ToTable("IpAddresses");

            base.OnModelCreating(modelBuilder);
        }
    }
}