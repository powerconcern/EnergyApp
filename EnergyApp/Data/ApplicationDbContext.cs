using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EnergyApp.Data;

namespace EnergyApp.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public DbSet<Configuration> Configuration { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public DbSet<Meter> Meters { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public DbSet<Charger> Chargers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public DbSet<Outlet> Outlets { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public DbSet<Partner> Partners { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public DbSet<CMPAssignment> CMPAssignments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public DbSet<ChargeSession> ChargeSession { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
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
