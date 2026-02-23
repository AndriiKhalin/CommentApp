using Microsoft.Extensions.Caching.Memory;

namespace CommentsApp.Application.Services;

public class CaptchaService(IMemoryCache cache)
{
    private static readonly Random _rnd = new();

    public (string SessionId, string Code) GenerateCaptcha()
    {
        var code = GenerateCode();
        var sessionId = Guid.NewGuid().ToString();
        cache.Set($"captcha_{sessionId}", code, TimeSpan.FromMinutes(5));
        return (sessionId, code);
    }

    public bool ValidateCaptcha(string sessionId, string userInput)
    {
        if (!cache.TryGetValue($"captcha_{sessionId}", out string? stored))
            return false;

        cache.Remove($"captcha_{sessionId}"); // one-time use
        return string.Equals(stored, userInput, StringComparison.OrdinalIgnoreCase);
    }

    private static string GenerateCode(int length = 6)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[_rnd.Next(chars.Length)])
            .ToArray());
    }
}