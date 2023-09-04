using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Services;

namespace UrlShortener.API.Entities;

public class AppDbContext : DbContext
{
    protected AppDbContext(DbContextOptions options) 
        : base(options)
    {
    }

    public DbSet<ShortenedUrl>? ShortenedUrls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortenedUrl>(builder =>
        {
            builder.Property(s => s.Code).HasMaxLength(UrlShorteningService.NumberOfCharsInShortLink);
            builder.HasIndex(s => s.Code).IsUnique();
        });
    }
}