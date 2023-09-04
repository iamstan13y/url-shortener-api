using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Entities;
using UrlShortener.API.Extensions;
using UrlShortener.API.Models;
using UrlShortener.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

builder.Services.AddDbContext<AppDbContext>(o =>
o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.MapPost("api/shorten", async (
    ShortenUrlRequest request,
    UrlShorteningService urlShorteningService,
    AppDbContext context,
    HttpContext httpContext) =>
{
    if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("The specified URL is invalid.");
    }

    var code = await urlShorteningService.GenerateUniqueCode();

    var shortenedUrl = new ShortenedUrl
    {
        Id = Guid.NewGuid(),
        Code = code,
        LongUrl = request.Url,
        ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{code}"
    };

    context.ShortenedUrls!.Add(shortenedUrl);
    await context.SaveChangesAsync();

    return Results.Ok(shortenedUrl.ShortUrl);
});

app.Run();