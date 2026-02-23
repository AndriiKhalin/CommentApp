using CommentsApp.Application.DTOs.Comments;
using CommentsApp.Application.Interfaces;
using CommentsApp.Application.Services;
using CommentsApp.Application.Validators.Comments;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace CommentsApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _service;
        private readonly CaptchaService _captchaService;
        private readonly IWebHostEnvironment _env;
        private const int MaxImageWidth = 320;
        private const int MaxImageHeight = 240;
        private const long MaxTextFileSize = 100 * 1024; // 100KB

        public CommentsController(
            ICommentService service,
            CaptchaService captchaService,
            IWebHostEnvironment env)
        {
            _service = service;
            _captchaService = captchaService;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetComments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25,
            [FromQuery] string sortBy = "createdAt",
            [FromQuery] bool descending = true)
        {
            var result = await _service.GetCommentsAsync(page, pageSize, sortBy, descending);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(
            [FromForm] CreateCommentRequest request,
            [FromForm] string captchaSessionId,
            [FromForm] IFormFile? attachment)
        {
            // Validate captcha
            if (!_captchaService.ValidateCaptcha(captchaSessionId, request.Captcha))
                return BadRequest(new { error = "Invalid CAPTCHA" });

            // Validate request
            var validator = new CreateCommentValidator();
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            // Handle attachment
            string? attachmentPath = null;
            string? attachmentType = null;

            if (attachment != null)
            {
                var result = await ProcessAttachmentAsync(attachment);
                if (result.Error != null)
                    return BadRequest(new { error = result.Error });

                attachmentPath = result.Path;
                attachmentType = result.Type;
            }

            var comment = await _service.CreateCommentAsync(request, attachmentPath, attachmentType);
            return CreatedAtAction(nameof(GetComments), new { id = comment.Id }, comment);
        }

        private async Task<(string? Path, string? Type, string? Error)> ProcessAttachmentAsync(
            IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLower();
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);

            if (ext is ".jpg" or ".jpeg" or ".gif" or ".png")
            {
                // Resize image if needed
                using var image = await Image.LoadAsync(file.OpenReadStream());

                if (image.Width > MaxImageWidth || image.Height > MaxImageHeight)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(MaxImageWidth, MaxImageHeight)
                    }));
                }

                await image.SaveAsync(filePath);
                return ($"/uploads/{fileName}", "Image", null);
            }
            else if (ext == ".txt")
            {
                if (file.Length > MaxTextFileSize)
                    return (null, null, "Text file must be less than 100KB");

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                return ($"/uploads/{fileName}", "Text", null);
            }

            return (null, null, "Unsupported file type");
        }
    }
}
