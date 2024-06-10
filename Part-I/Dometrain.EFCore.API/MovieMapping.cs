using Dometrain.EFCore.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dometrain.EFCore.API;

public class MovieMapping : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder
               .ToTable("Pictures")
               .HasKey(movie => movie.Kod);

        builder.Property(movie => movie.Title)
            .IsRequired()
            .HasMaxLength(256)
            .HasColumnName("Nazev");

        builder.HasData(new Movie
        {
            Kod = 1,
            Title = "Fight Club",
            ReleaseDate = DateTime.Now,
            Synopsis = "Hehe",
        });
    }
}
