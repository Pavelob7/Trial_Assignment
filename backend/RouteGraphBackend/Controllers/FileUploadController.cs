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

        // POST метод для загрузки файла
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // Проверка наличия и корректности файла
            if (file == null || file.Length <= 0)
            {
                return BadRequest("File is not selected or empty.");
            }

            try
            {
                // Начало транзакции для атомарного сохранения данных
                using var transaction = await _context.Database.BeginTransactionAsync();

                // Чтение данных из Excel файла
                var (points, tracks) = await ReadPointsAndTracksFromExcel(file);

                // Проверка на корректность данных из Excel
                if (points == null || tracks == null)
                {
                    return BadRequest("Invalid data format in Excel file.");
                }

                // Создаем новую запись в таблице Uploads
                var upload = new Upload
                {
                    UploadTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() // или другое значение времени загрузки
                };
                _context.Uploads.Add(upload);
                await _context.SaveChangesAsync(); // Сохраняем изменения, чтобы получить UploadId

                // Привязываем точки и треки к созданной загрузке
                foreach (var point in points)
                {
                    point.UploadId = upload.UploadId;
                    _context.Points.Add(point);
                }

                foreach (var track in tracks)
                {
                    track.UploadId = upload.UploadId;
                    _context.Tracks.Add(track);
                }

                await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
                await transaction.CommitAsync(); // Фиксируем транзакцию

                // Создаем JSON-ответ
                var jsonResponse = await CreateJsonResponse(file, upload.UploadId);

                return Ok(jsonResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to process file upload: {ex.Message}");
            }
        }

        // Создание JSON-ответа на основе загруженного файла
        private async Task<string> CreateJsonResponse(IFormFile file, int uploadId)
        {
            var (points, tracks) = await ReadPointsAndTracksFromExcel(file);

            if (points == null || tracks == null)
            {
                return null;
            }

            // Формируем объект JSON
            var responseObject = new Dictionary<long, object>
            {
                {
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds(), new
                    {
                        points = points.Select(p => new
                        {
                            id = p.PointId,
                            name = p.Name,
                            height = p.Height,
                            uploadId = uploadId
                        }).ToList<object>(),
                        tracks = tracks.Select(t => new
                        {
                            firstId = t.FirstId,
                            secondId = t.SecondId,
                            distance = t.Distance,
                            surface = t.Surface.ToString().ToUpper(),
                            maxSpeed = t.MaxSpeed.ToString().ToUpper(),
                            uploadId = uploadId
                        }).ToList<object>()
                    }
                }
            };

            // Преобразуем в JSON-строку
            string jsonResponse = JsonSerializer.Serialize(responseObject);

            return jsonResponse;
        }

        // Чтение данных (точек и треков) из Excel-файла
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

                        // Читаем данные из ячеек
                        string pointIdText = worksheet.Cells[row, 1].Text;
                        string name = worksheet.Cells[row, 2].Text;
                        string heightText = worksheet.Cells[row, 3].Text;

                        // Проверка на пустые значения
                        if (string.IsNullOrWhiteSpace(pointIdText) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(heightText))
                        {
                            // Логика обработки пустых значений, например, пропуск строки или возврат ошибки
                            continue; // или return BadRequest("Invalid data format in Excel file.");
                        }

                        // Парсим значения
                        int pointId = int.Parse(pointIdText);
                        int height = int.Parse(heightText);

                        // Создаем новую точку
                        var point = new Point
                        {
                            PointId = pointId, // Используем pointId как Id точки
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
