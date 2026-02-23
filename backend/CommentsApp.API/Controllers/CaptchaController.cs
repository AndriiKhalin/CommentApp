using CommentsApp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CommentsApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        private readonly CaptchaService _captchaService;

        public CaptchaController(CaptchaService captchaService)
        {
            _captchaService = captchaService;
        }

        [HttpGet]
        public IActionResult GetCaptcha()
        {
            var (sessionId, code) = _captchaService.GenerateCaptcha();
            var imageBytes = GenerateCaptchaImage(code);

            Response.Headers["X-Captcha-Session"] = sessionId;
            return File(imageBytes, "image/png");
        }

        private static byte[] GenerateCaptchaImage(string code)
        {
            using var image = new Image<Rgba32>(200, 60);
            var rnd = new Random();

            image.Mutate(ctx =>
            {
                ctx.BackgroundColor(Color.White);

                // Noise lines
                for (int i = 0; i < 8; i++)
                {
                    var color = Color.FromRgb(
                        (byte)rnd.Next(150, 220),
                        (byte)rnd.Next(150, 220),
                        (byte)rnd.Next(150, 220));
                    ctx.DrawLine(color, 1f,
                        new PointF(rnd.Next(0, 200), rnd.Next(0, 60)),
                        new PointF(rnd.Next(0, 200), rnd.Next(0, 60)));
                }

                // Text
                var font = SystemFonts.CreateFont("Arial", 30, FontStyle.Bold);
                ctx.DrawText(code, font, Color.DarkBlue, new PointF(20, 15));
            });

            using var ms = new MemoryStream();
            image.SaveAsPng(ms);
            return ms.ToArray();
        }
    }
}
