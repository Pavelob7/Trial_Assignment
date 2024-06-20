using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RouteGraphBackend.Data;
using RouteGraphBackend.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RouteGraphBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly Data.RouteContext _context;

        public FileUploadController(Data.RouteContext context)
        {
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File not selected");

            var points = new List<Point>();
            var tracks = new List<Track>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                        {
                            string idText = worksheet.Cells[row, 1].Text;
                            string name = worksheet.Cells[row, 2].Text;
                            string heightText = worksheet.Cells[row, 3].Text;

                            if (string.IsNullOrWhiteSpace(idText) ||
                                string.IsNullOrWhiteSpace(name) ||
                                string.IsNullOrWhiteSpace(heightText))
                            {
                                continue; // Пропускаем строки с пустыми значениями
                            }

                            int id = int.Parse(idText);
                            int height = int.Parse(heightText);

                            points.Add(new Point
                            {
                                Name = name,
                                Height = height,
                                RouteId = routeId // Присваиваем RouteId текущего маршрута
                            });

                            string firstIdText = worksheet.Cells[row, 4].Text;
                            string secondIdText = worksheet.Cells[row, 5].Text;
                            string distanceText = worksheet.Cells[row, 6].Text;
                            string surfaceText = worksheet.Cells[row, 7].Text;
                            string maxSpeedText = worksheet.Cells[row, 8].Text;

                            if (!string.IsNullOrEmpty(firstIdText) &&
                                !string.IsNullOrEmpty(secondIdText) &&
                                !string.IsNullOrEmpty(distanceText) &&
                                !string.IsNullOrEmpty(surfaceText) &&
                                !string.IsNullOrEmpty(maxSpeedText))
                            {
                                int firstId = int.Parse(firstIdText);
                                int secondId = int.Parse(secondIdText);
                                int distance = int.Parse(distanceText);
                                Surface surface = Enum.Parse<Surface>(surfaceText, true); // true для игнорирования регистра
                                MaxSpeed maxSpeed = Enum.Parse<MaxSpeed>(maxSpeedText, true); // true для игнорирования регистра

                                tracks.Add(new Track
                                {
                                    FirstId = firstId,
                                    SecondId = secondId,
                                    Distance = distance,
                                    Surface = surface,
                                    MaxSpeed = maxSpeed,
                                    RouteId = routeId // Присваиваем RouteId текущего маршрута
                                });
                            }
                        }
                    }
                }

                // Добавление точек и треков в контекст данных
                _context.Points.AddRange(points);
                _context.Tracks.AddRange(tracks);

                // Сохранение изменений в базе данных
                await _context.SaveChangesAsync();

                // Генерация структуры JSON для ответа
                var routeId = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var response = new Dictionary<long, object>
                {
                    { routeId, new { points, tracks } }
                };

                return Ok(response);
            }
            catch (DbUpdateException ex)
            {
                // Если произошла ошибка сохранения в базу данных
                var errorMessage = "Database update error";
                if (ex.InnerException != null)
                    errorMessage += ": " + ex.InnerException.Message;

                return StatusCode(500, errorMessage);
            }
            catch (Exception ex)
            {
                // В случае других внутренних ошибок сервера
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
