# Co je to ORM?
* Object-relational mapping
* Nakreslit obrázek (tabulku), popsat Model (Tabulka) a Data (Řádek).
* Pomáhá nám řešit problémy s ručním psaní SQL příkazů.
* V dnešní době jsou ORM téměř stejně rychlé, jako ručně psané SQL.

```
C#                      SQL
Class       Model       Table
Object      Data        Row
Function                S-I-U-D
```

# Co je to Entity framework?
* Implementace ORM
* Dnes už se udržuje pouze EF Core
* Rozdíl mezi EF a EF Core je zaměření na použití (desktop aplikace vs. WEB aplikace), mají rozdílné defaultní nastavení

```
3.5     4.0     4.5     ...     4.8     .NET Framework
------------------------------------------------------>
EF       4          5         6

                   ---------------------------------------------------------->
                   .NET Core             3.1       5.0      6.0     7.0     8.0     ...
                    EF Core 1            3.1       5.0      6.0     7.0     8.0
```

# DBContext
* Jedná se o základní rozhraní mezi třídou a databází.
* Otevíra konexe ne databázi.
* Musí znát tvar tříd, tvar tabulek v DB a mapu mezi těmito entitami.
* Mapa často není potřeba, EF je chytrý a spoustu věcí si odvodí sám.

# Stáhneme si EFCore SQL server package
* Vytvoříme si DB Context pro filmy (MoviesContext).
* Přidat DBContext do Service Collection.
* Zkusit spustit a vysvětlit error.
* Nakonfigurovat MoviesContext.
* Zkusit spustit a vysvětlit error.
* Nakonfigurovat SCOPE, kde deletujeme/vytváříme db tabulku.
* Zpustit a ukázat funkčnost.

# Implementovat metody na controlleru
* Celkem přímočaré
* popsat rozdíl mezi First, Single a Find
* SaveChanges

# Ukázat logování dotazů na DB
* optionsBuilder.LogTo(Console.WriteLine);
* Zmínit, že defaultně se nezobrazují hodnoty

# Projecting data to optimize queries
* Jedná se o způsob, jak zlepšit performance.
* Načítáme jen sloupce, které jsou potřeba.
* Ukázat v logu, že se nedotazujeme na vše, jako před tím.

```
    var filteredTitles = await _context.Movies
       .Where(movie => movie.AgeRating <= ageRating)
       .Select(movie => new MovieTitle { Id = movie.Identifier, Title = movie.Title})
       .ToListAsync();
```

# DBSchema
* Ukázat v Azure Data Studio jak EF vytváří DB
* Ukázat DataAnotace pro EF
* NEZAPOMEŇ UKAZOVAT ZMĚNY!

```
    [Table("Pictures)]
    public class Movie
    {
        [Key]
        public int Id { get; set; } // Zmínit název ID vs Identifier se kterým bude mít EF problém!
        [MaxLength(128)]
        [Required]
        public string? Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        [Column("Plot", TypeName = "varchar(max)")]
        public string? Synopsis { get; set; }
    }
```
## Fluent způsob mapování
```
    // Movies context
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new GenreMapping());
        modelBuilder.ApplyConfiguration(new MovieMapping());
    }
```
```
    // Samotný soubor 
    public class MovieMapping : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder
                .ToTable("Pictures")
                .HasQueryFilter(movie => movie.ReleaseDate > new DateTime(1990,1,1))
                .HasKey(movie => movie.Identifier);

            builder.Property(movie => movie.Title)
                .HasColumnType("varchar")
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(movie => movie.ReleaseDate)
                .HasColumnType("char(8)")
                .HasConversion(new DateTimeToChar8Converter());

            builder.Property(movie => movie.Synopsis)
                .HasColumnType("varchar(max)")
                .HasColumnName("Plot");

            builder
                .HasOne(movie => movie.Genre)
                .WithMany(genre => genre.Movies)
                .HasPrincipalKey(genre => genre.Id)
                .HasForeignKey(movie => movie.MainGenreId);

            builder
                .OwnsOne(movie => movie.Director)
                .ToTable("Pictures_Directors"); 
            
            builder
                .OwnsMany(movie => movie.Actors);
        }
    }
```

# One-to-many relationship
* Přidáme property Genre na Movie třídu.
* Přidáme na controller GET pro film Include metodu.
* Můžeme přidat zpětnou property Movies na Genre třídu.
* Ukázat vznik primary/foreign klíčů v azure data studio.

```
    var movie = await _context.Movies
        .Include(movie => movie.Genre)
        .SingleOrDefaultAsync(m => m.Identifier == id);
```
```
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [JsonIgnore] // Jinak se nám zacyklí serializer
        public ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();
    }
```

# Converting Datatypes
```
    builder.Property(movie => movie.ReleaseDate)
       .HasColumnType("char(23)")
       .HasConversion<string>();
```
**NEBO PŘESNĚJI**
```
    builder.Property(movie => movie.ReleaseDate)
       .HasColumnType("char(8)")
       .HasConversion(new DateTimeToChar8Converter());
```
```
public class DateTimeToChar8Converter : ValueConverter<DateTime, string>
{
    public DateTimeToChar8Converter() : base(
        dateTime => dateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
        stringValue => DateTime.ParseExact(stringValue, "yyyyMMdd", CultureInfo.InvariantCulture)
        )
    { }
}
```
