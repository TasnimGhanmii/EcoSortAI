using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sevrer.Api.ML;
using sevrer.Api.Models;
using sevrer.Api.Enums;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace sevrer.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/classifications")]
    public class ClassificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly WastePredictionService _wastePrediction;
        private readonly ILogger<ClassificationsController> _logger;

        public ClassificationsController(
            ApplicationDbContext context,
            WastePredictionService wastePrediction,
            ILogger<ClassificationsController> logger)
        {
            _context = context;
            _wastePrediction = wastePrediction;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userIdGuid))
                return Unauthorized("User not authenticated.");

            var classifications = await _context.Classifications
                .Where(c => c.UserId == userIdGuid)
                .ToListAsync();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            return Ok(classifications.Select(c => new
            {
                id = c.Id,
                icon = GetIconForCategory(c.Category),
                image = $"{baseUrl}{c.ImageUrl}", 
                category = c.Category.ToString(),
                title = "Waste Item",
                accuracy = (int)Math.Round((c.PredictedConfidence ?? 0f) * 100),
                date = c.FormattedDate,
                time = c.FormattedTime
            }));
        }

        [HttpPost]
        public async Task<IActionResult> Classify([FromForm] IFormFile image)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userIdGuid))
                return Unauthorized("User not authenticated.");

            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Only .jpg, .jpeg, .png files are allowed.");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);
            var filePath = Path.Combine(uploadsFolder, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                var (category, confidence) = _wastePrediction.ClassifyImage(filePath);

                var classification = new Classification
                {
                    UserId = userIdGuid,
                    Category = category,
                    ImageUrl = $"/uploads/{fileName}",
                    PredictedConfidence = confidence,
                    ClassifiedAt = DateTime.UtcNow
                };

                _context.Classifications.Add(classification);
                await _context.SaveChangesAsync();

                var baseUrl = $"{Request.Scheme}://{Request.Host}";

                return Ok(new
                {
                    id = classification.Id,
                    icon = GetIconForCategory(category),
                    image = $"{baseUrl}{classification.ImageUrl}", 
                    category = category.ToString(),
                    title = "Uploaded Item",
                    accuracy = (int)Math.Round(confidence * 100),
                    date = classification.FormattedDate,
                    time = classification.FormattedTime
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Classification failed for user {UserId}", userIdGuid);
                return StatusCode(500, "Classification failed.");
            }
        }

        [HttpGet("progress")]
        public async Task<IActionResult> GetProgress()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userIdGuid))
                return Unauthorized("User not authenticated.");

            var classifications = await _context.Classifications
                .Where(c => c.UserId == userIdGuid)
                .ToListAsync();

            var total = classifications.Count;
            var byCategory = classifications
                .GroupBy(c => c.Category)
                .ToDictionary(g => g.Key, g => g.Count());

            var recycableCount = byCategory.GetValueOrDefault(WasteCategory.Recyclable, 0);
            var compostableCount = byCategory.GetValueOrDefault(WasteCategory.Compostable, 0);
            var hazardousCount = byCategory.GetValueOrDefault(WasteCategory.Hazardous, 0);
            var generalCount = byCategory.GetValueOrDefault(WasteCategory.General, 0);

            var progress = new
            {
                TotalItemsSorted = total,
                RecyclableItems = recycableCount,
                CompostableItems = compostableCount,
                HazardousItems = hazardousCount,
                GeneralItems = generalCount,
                RecyclablePercentage = total > 0 ? (int)Math.Round(recycableCount / (double)total * 100) : 0,
                CompostablePercentage = total > 0 ? (int)Math.Round(compostableCount / (double)total * 100) : 0,
                HazardousPercentage = total > 0 ? (int)Math.Round(hazardousCount / (double)total * 100) : 0,
                GeneralPercentage = total > 0 ? (int)Math.Round(generalCount / (double)total * 100) : 0,
                EcoScore = total > 0 ? (int)Math.Round((recycableCount + compostableCount) / (double)total * 100) : 0,
                LastClassifiedAt = classifications.Count > 0
                    ? classifications.Max(c => c.ClassifiedAt)
                    : (DateTime?)null
            };

            return Ok(progress);
        }

        private string GetIconForCategory(WasteCategory category)
        {
            return category switch
            {
                WasteCategory.Recyclable => "mdi:recycleVariant",
                WasteCategory.Compostable => "mdi:leaf",
                WasteCategory.Hazardous => "mdi:biohazard",
                WasteCategory.General => "mdi:deleteVariant",
                _ => "mdi:trashCanOutline"
            };
        }
    }
}
