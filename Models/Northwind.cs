using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Packt.Shared;

public class NorthwindContext : DbContext
{
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string path = Path.Combine(
            Environment.CurrentDirectory, "Northwind.db"
        );
        optionsBuilder.UseSqlite($"Filename={path}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .Property(category => category.CategoryName)
            .IsRequired() // NOT NULL
            .HasMaxLength(15);
    }
}


public class Category
{
    public int CategoryID { get; set; }

    public string? CategoryName { get; set; }

    [Column(TypeName = "ntext")]
    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; }

    public Category()
    {
        Products = new List<Product>();
    }
}

public class Product
{
    public int ProductID { get; set; }

    [Required]
    [StringLength(40)]
    public string? ProductName { get; set; }

    [Column("UnitPrice", TypeName = "money")]
    public decimal? Cost { get; set; }

    [Column("UnitsInStock")]
    public short? Stock { get; set; }

    public bool Discontinued { get; set; }

    public int CategoryId { get; set; }

    public virtual Category? Category
    {
        get; set;
    }
}
