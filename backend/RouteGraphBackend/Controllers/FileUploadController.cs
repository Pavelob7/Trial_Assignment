
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using RouteGraphBackend.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
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
            if (file == null || file.Length <= 0)
            {
                return BadRequest("File is not selected or empty.");
            }

            try
            {
                string jsonResponse = await CreateJsonResponse(file);

                if (jsonResponse == null)
                {
                    return BadRequest("Invalid data format in Excel file.");
                }

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to process file upload: {ex.Message}");
            }
        }

        private async Task<string> CreateJsonResponse(IFormFile file)
        {
            var (points, tracks) = await ReadPointsAndTracksFromExcel(file);

            if (points == null || tracks == null)
            {
                return null;
            }

            // Генерация уникального времени для UploadId
            long uploadTimeUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Формирование объекта JSON
            var responseObject = new Dictionary<long, object>
            {
                { uploadTimeUnix, new {
                    points = points.Select(p => new {
                        id = p.Id,
                        name = p.Name,
                        height = p.Height
                    }).ToList<object>(),
                    tracks = tracks.Select(t => new {
                        firstId = t.FirstId,
                        secondId = t.SecondId,
                        distance = t.Distance,
                        surface = t.Surface.ToString().ToUpper(),
                        maxSpeed = t.MaxSpeed.ToString().ToUpper()
                    }).ToList<object>()
                }}
            };

            // Преобразование объекта в JSON-строку
            string jsonResponse = JsonSerializer.Serialize(responseObject);

            return jsonResponse;
        }

        private async Task<(List<Point>, List<Track>)> ReadPointsAndTracksFromExcel(IFormFile file)
        {
            var points = new List<Point>();
            var tracks = new List<Track>();

            // Установка контекста лицензии EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // начинаем с 2, если первая строка - заголовок
                    {
                        // Пропускаем пустые строки
                        if (worksheet.Cells[row, 1].Value == null)
                        {
                            continue;
                        }

                        string idText = worksheet.Cells[row, 1].Text;
                        string name = worksheet.Cells[row, 2].Text;
                        string heightText = worksheet.Cells[row, 3].Text;

                        // Проверка на пустые значения
                        if (string.IsNullOrWhiteSpace(idText) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(heightText))
                        {
                            // Логика обработки пустых значений, например, пропуск строки или возврат ошибки
                            continue; // или return BadRequest("Invalid data format in Excel file.");
                        }

                        int id = int.Parse(idText);
                        int height = int.Parse(heightText);

                        // Создаем новую точку
                        var point = new Point
                        {
                            Id = id,
                            Name = name,
                            Height = height
                        };

                        points.Add(point);
                    }

                    // Чтение треков
                    for (int row = 2; row <= rowCount; row++) // начинаем с 2, если первая строка - заголовок
                    {
                        // Пропускаем пустые строки
                        if (worksheet.Cells[row, 4].Value == null)
                        {
                            continue;
                        }

                        string firstIdText = worksheet.Cells[row, 4].Text;
                        string secondIdText = worksheet.Cells[row, 5].Text;
                        string distanceText = worksheet.Cells[row, 6].Text;
                        string surfaceText = worksheet.Cells[row, 7].Text;
                        string maxSpeedText = worksheet.Cells[row, 8].Text;

                        // Проверка на пустые значения
                        if (string.IsNullOrWhiteSpace(firstIdText) || string.IsNullOrWhiteSpace(secondIdText) ||
                            string.IsNullOrWhiteSpace(distanceText) || string.IsNullOrWhiteSpace(surfaceText) ||
                            string.IsNullOrWhiteSpace(maxSpeedText))
                        {
                            // Логика обработки пустых значений, например, пропуск строки или возврат ошибки
                            continue; // или return BadRequest("Invalid data format in Excel file.");
                        }

                        int firstId = int.Parse(firstIdText);
                        int secondId = int.Parse(secondIdText);
                        int distance = int.Parse(distanceText);
                        Surface surface = Enum.Parse<Surface>(surfaceText, true);
                        MaxSpeed maxSpeed = Enum.Parse<MaxSpeed>(maxSpeedText, true);

                        // Создаем новый трек
                        var track = new Track
                        {
                            FirstId = firstId,
                            SecondId = secondId,
                            Distance = distance,
                            Surface = surface,
                            MaxSpeed = maxSpeed
                        };

                        tracks.Add(track);
                    }
                }
            }

            return (points, tracks);
        }
    }
}
