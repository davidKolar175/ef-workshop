using Dometrain.EFCore.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dometrain.EFCore.API.Data;

public class MoviesContext : DbContext
{
    public DbSet<Movie> Movies => Set<Movie>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("""
            Data Source=localhost\SQLExpress;
            Initial Catalog=MoviesDB;
            Integrated Security=True;
            Persist Security Info=True;
            Pooling=False;
            Application Name=sqlops-connection-string;
            Trusted_Connection=True;
            TrustServerCertificate=True;
            """);

        optionsBuilder.LogTo(Console.WriteLine);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new MovieMapping());
    }

}