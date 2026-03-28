using domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<BillItem> BillItems => Set<BillItem>();
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<Payment> Payments => Set<Payment>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(c =>
        {
            c.HasKey(x => x.Id);
            c.Property(x => x.Name).IsRequired();
            c.Property(x => x.GstNo).IsRequired(false);
            c.Property(x => x.ShippingAddress).IsRequired(false);
            c.Property(x => x.BillingAddress).IsRequired(false);
            
        });

        modelBuilder.Entity<Item>(i =>
        {
            i.HasKey(x => x.Id);
            i.Property(x => x.Name).IsRequired();
            i.Property(x => x.Price).HasColumnType("REAL");
        });

        // ── Bill ─────────────────────────────────────────────────────
        modelBuilder.Entity<Bill>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.InvoiceNumber).IsRequired();
            b.Property(x => x.CgstPercentage).HasColumnType("REAL");
            b.Property(x => x.SgstPercentage).HasColumnType("REAL");

            // Computed properties — not stored, EF must ignore them
            b.Ignore(x => x.TotalTaxableAmount);
            b.Ignore(x => x.CgstAmount);
            b.Ignore(x => x.SgstAmount);
            b.Ignore(x => x.TotalAmount);

            // Bill → Customer (many bills can belong to one customer)
            b.HasOne(x => x.Customer)
             .WithMany()
             .HasForeignKey(x => x.CustomerId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(b => b.BillItems)
            .WithOne()
            .HasForeignKey(bi => bi.BillId)
            .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Payment>(i =>
        {
            i.HasKey(x => x.Id);
            i.Property(x => x.AmountPaid).IsRequired().HasColumnType("REAL");
            i.HasOne<Customer>()
             .WithMany()
             .HasForeignKey(x => x.CustomerId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BillItem>(bi =>
        {
            bi.HasKey(x => x.Id);
            bi.Property(x => x.ItemName).IsRequired();
            bi.Property(x => x.UnitPrice).HasColumnType("REAL");
            bi.Property(x => x.Quantity).HasColumnType("REAL");

            // Amount is computed — not stored
            bi.Ignore(x => x.Amount);

            // Soft reference back to Item catalog — no cascade
            // If Item is deleted, BillItem keeps its snapshot data intact
            bi.HasOne<Item>()
              .WithMany()
              .HasForeignKey(x => x.ItemId)
              .OnDelete(DeleteBehavior.SetNull)
              .IsRequired(false); // ItemId becomes nullable after a catalog delete
        });

    }

}
