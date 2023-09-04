using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Entities;

namespace UrlShortener.API.Services;

public class UrlShorteningService
{
    public const int NumberOfCharsInShortLink = 7;
    private const string Alphabet = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890";
    private readonly AppDbContext _context;

    private readonly Random _random = new();

    public UrlShorteningService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateUniqueCode()
    {
        var codeChars = new char[NumberOfCharsInShortLink];

        while (true)
        {
            for (int i = 0; i < NumberOfCharsInShortLink; i++)
            {
                var randomIndex = _random.Next(Alphabet.Length - 1);
                codeChars[i] = Alphabet[randomIndex];
            }

            var code = new string(codeChars);

            if (!await _context.ShortenedUrls!.AnyAsync(s => s.Code == code))
            {
                return code;
            }
        }
    }
}