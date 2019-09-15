using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EnergyApp.Data;

namespace EnergyApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Configuration> Configuration { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<Charger> Chargers { get; set; }
        public DbSet<Outlet> Outlets { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<CMPAssignment> CMPAssignments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CMPAssignment>()
                .HasKey(k => new {k.ChargerID, k.PartnerID,k.MeterID});

            builder.Entity<Configuration>(entity =>
            {
                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            base.OnModelCreating(builder);
        }        
    }
}
