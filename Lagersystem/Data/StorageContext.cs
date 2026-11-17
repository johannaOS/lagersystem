using Microsoft.EntityFrameworkCore;

public class StorageContext(DbContextOptions<StorageContext> options) : DbContext(options)
{
    public DbSet<Lagersystem.Models.Products> Products { get; set; } = default!;
}
