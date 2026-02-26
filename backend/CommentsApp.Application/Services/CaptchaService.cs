using CommentsApp.Application.Interfaces;

namespace CommentsApp.Application.Services;

public class CaptchaService(ICacheService cache)
{
    private static readonly Random _rnd = new();

    public async Task<(string SessionId, string Code)> GenerateCaptchaAsync()
    {
        var code = GenerateCode();
        var sessionId = Guid.NewGuid().ToString();
        await cache.SetAsync($"captcha:{sessionId}", code, TimeSpan.FromMinutes(5));
        return (sessionId, code);
    }

    public async Task<bool> ValidateCaptchaAsync(string sessionId, string userInput)
    {
        var key = $"captcha:{sessionId}";
        var stored = await cache.GetAsync<string>(key);
        if (stored is null) return false;

        await cache.RemoveAsync(key); // one-time use
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