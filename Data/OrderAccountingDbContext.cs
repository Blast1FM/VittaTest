using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using VittaTest.Models;

namespace VittaTest.Data;

public partial class OrderAccountingDbContext : DbContext
{
    public OrderAccountingDbContext()
    {
    }

    public OrderAccountingDbContext(DbContextOptions<OrderAccountingDbContext> options)
        : base(options)
    {

    }

    public virtual DbSet<CashInflow> CashInflows { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CashInflow>(entity =>
        {
            entity.HasKey(e => e.InflowNumber).HasName("PK__CashInfl__FA667380B8F28713");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.InflowDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Remaining).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderNumber).HasName("PK__Orders__CAC5E742A5C6EA6F");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A58CAE77324");

            entity.ToTable(tb => tb.HasTrigger("TR_Payments_ApplyPayment"));

            entity.Property(e => e.PaymentId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("PaymentID");
            entity.Property(e => e.ExpectedInflowVersion).HasMaxLength(8);
            entity.Property(e => e.ExpectedOrderVersion).HasMaxLength(8);
            entity.Property(e => e.PaymentAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.InflowNumberNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.InflowNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_CashInflows");

            entity.HasOne(d => d.OrderNumberNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Orders");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
