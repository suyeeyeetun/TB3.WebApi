using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TB3.WebApi.Database.AppDbContextModels;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<TblBookManagement> TblBookManagements { get; set; }

    public virtual DbSet<TblBooking> TblBookings { get; set; }

    public virtual DbSet<TblBorrow> TblBorrows { get; set; }

    public virtual DbSet<TblCustomer> TblCustomers { get; set; }

    public virtual DbSet<TblProduct> TblProducts { get; set; }

    public virtual DbSet<TblProductCategory> TblProductCategories { get; set; }

    public virtual DbSet<TblSale> TblSales { get; set; }

    public virtual DbSet<TblTourPackage> TblTourPackages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=Batch3MiniPOS;User ID=sa;Password=sasa@123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK_Tbl_Product");

            entity.ToTable("Product");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ProductName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblBookManagement>(entity =>
        {
            entity.HasKey(e => e.BookId);

            entity.ToTable("Tbl_BookManagement");

            entity.Property(e => e.Author)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreateDateTime).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblBooking>(entity =>
        {
            entity.HasKey(e => e.BookingId);

            entity.ToTable("Tbl_Booking");

            entity.Property(e => e.BookingDate).HasColumnType("datetime");
            entity.Property(e => e.BookingStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreateDateTime).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<TblBorrow>(entity =>
        {
            entity.HasKey(e => e.BorrowId);

            entity.ToTable("Tbl_Borrow");

            entity.Property(e => e.BorrowerName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.IssueDate).HasColumnType("datetime");
            entity.Property(e => e.ReturnDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblCustomer>(entity =>
        {
            entity.HasKey(e => e.CustomerId);

            entity.ToTable("Tbl_Customer");

            entity.Property(e => e.CreateDateTime).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<TblProduct>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK_Tbl_Product_1");

            entity.ToTable("Tbl_Product");

            entity.Property(e => e.CreateDateTime).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ProductName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblProductCategory>(entity =>
        {
            entity.HasKey(e => e.ProductCategoryId);

            entity.ToTable("Tbl_ProductCategory");

            entity.Property(e => e.ProductCategoryCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProductCategoryName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblSale>(entity =>
        {
            entity.HasKey(e => e.SaleId);

            entity.ToTable("Tbl_Sale");

            entity.Property(e => e.CreateDateTime).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<TblTourPackage>(entity =>
        {
            entity.HasKey(e => e.PackageId);

            entity.ToTable("Tbl_TourPackage");

            entity.Property(e => e.CreateDateTime).HasColumnType("datetime");
            entity.Property(e => e.Destination)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.PackageName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PricePerPerson).HasColumnType("decimal(18, 0)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
