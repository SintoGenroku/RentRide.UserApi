using Microsoft.EntityFrameworkCore;
using Users.Common;

namespace Users.Data;

public class UsersDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public UsersDbContext() : base() { }

    public UsersDbContext(DbContextOptions<UsersDbContext> options) 
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.Property(u => u.Fullname).IsRequired();
        });


    }
}