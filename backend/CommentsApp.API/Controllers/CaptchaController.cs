using CommentsApp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CommentsApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CaptchaController(CaptchaService captchaService, IWebHostEnvironment env) : ControllerBase
{
    private static FontFamily _cachedFontFamily;
    private static bool _fontLoaded;
    private static readonly object _fontLock = new();

    [HttpGet]
    public async Task<IActionResult> GetCaptcha()
    {
        var (sessionId, code) = await captchaService.GenerateCaptchaAsync();
        var imageBytes = GenerateCaptchaImage(code, env.ContentRootPath);

        Response.Headers["X-Captcha-Session"] = sessionId;
        Response.Headers["Access-Control-Expose-Headers"] = "X-Captcha-Session";
        return base.File(imageBytes, "image/png");
    }

    private static byte[] GenerateCaptchaImage(string code, string contentRoot)
    {
        using var image = new Image<Rgba32>(200, 60);
        var rnd = new Random();

        image.Mutate(ctx =>
        {
            ctx.BackgroundColor(Color.White);

            for (var i = 0; i < 8; i++)
            {
                var color = Color.FromRgb(
                    (byte)rnd.Next(150, 220),
                    (byte)rnd.Next(150, 220),
                    (byte)rnd.Next(150, 220));
                ctx.DrawLine(color, 1f,
                    new PointF(rnd.Next(0, 200), rnd.Next(0, 60)),
                    new PointF(rnd.Next(0, 200), rnd.Next(0, 60)));
            }

            var fontFamily = GetFontFamily(contentRoot);
            var font = fontFamily.CreateFont(30, FontStyle.Bold);

            ctx.DrawText(code, font, Color.DarkBlue, new PointF(20, 15));
        });

        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        return ms.ToArray();
    }

    private static FontFamily GetFontFamily(string contentRoot)
    {
        if (_fontLoaded) return _cachedFontFamily;

        lock (_fontLock)
        {
            if (_fontLoaded) return _cachedFontFamily;

            var fontPath = Path.Combine(contentRoot, "Fonts", "arial.ttf");

            if (!System.IO.File.Exists(fontPath))
            {
                if (SystemFonts.TryGet("Arial", out var systemFamily))
                {
                    _cachedFontFamily = systemFamily;
                    _fontLoaded = true;
                    return _cachedFontFamily;
                }

                if (SystemFonts.TryGet("DejaVu Sans", out var dejaVu))
                {
                    _cachedFontFamily = dejaVu;
                    _fontLoaded = true;
                    return _cachedFontFamily;
                }

                throw new FileNotFoundException(
                    $"Font file not found at {fontPath} and no system fallback available.");
            }

            var collection = new FontCollection();
            _cachedFontFamily = collection.Add(fontPath);
            _fontLoaded = true;
            return _cachedFontFamily;
        }
    }
}