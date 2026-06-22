using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VittaTest.Models;

namespace VittaTest.Data
{
    // TODO Redo according to the new thing with DI
    public class OrderAccountingContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<CashInflow> CashInflows { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .Property(o => o.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<CashInflow>()
                .Property(ci => ci.RowVersion)
                .IsRowVersion();

            // Игнорируем Expected*Version при маппинге (они нужны только для передачи в триггер)
            modelBuilder.Entity<Payment>()
                .Property(p => p.ExpectedOrderVersion)
                .HasColumnName("ExpectedOrderVersion");

            modelBuilder.Entity<Payment>()
                .Property(p => p.ExpectedInflowVersion)
                .HasColumnName("ExpectedInflowVersion");
        }
    }
}
