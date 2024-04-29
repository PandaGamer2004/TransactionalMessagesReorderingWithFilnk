using Microsoft.EntityFrameworkCore;
using RocketGateway.DbContext.Models;

namespace RocketGateway.DbContext;

public class RocketsDbContext: Microsoft.EntityFrameworkCore.DbContext
{

    public RocketsDbContext(DbContextOptions<RocketsDbContext> options) : base(options)
    {
    }

    public RocketsDbContext()
    {
        
    }
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseNpgsql("Host=localhost;Database=rocketsdb;Username=root;Password=12345");
    // }

    public DbSet<RocketDAO> Rockets { get; set; }
}