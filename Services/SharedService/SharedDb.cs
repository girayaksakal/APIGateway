using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace SharedService;

public class SharedDbContext : DbContext {
    public SharedDbContext(DbContextOptions<SharedDbContext> options) : base(options) { 
        Users = Set<User>();
        Bookings = Set<Booking>();
        Listings = Set<Listing>();
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet <Listing> Listings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Listing>()
            .Property(l => l.Price)
            .HasColumnType("decimal(18, 2)");
    }
}

public class User {
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }
}

public class Booking {
    public int Id { get; set; }
    public int ListingId { get; set; }
    public int UserId { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
}

public class Listing {
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
}
