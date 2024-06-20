using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RouteGraphBackend.Data;
using RouteGraphBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RouteGraphBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteDataController : ControllerBase
    {
        private readonly Data.RouteContext _context; // Контекст базы данных для работы с данными

        public RouteDataController(Data.RouteContext context) // Конструктор контроллера
        {
            _context = context;
        }

        // GET: api/RouteData/PointsAndTracks
        [HttpGet("PointsAndTracks")]
        public async Task<IActionResult> GetPointsAndTracks()
        {
            // Получаем все загрузки данных с точками и треками из базы данных
            var uploads = await _context.Uploads
                .Include(u => u.Points) // Включаем точки связанные с каждой загрузкой
                .Include(u => u.Tracks) // Включаем треки связанные с каждой загрузкой
                .ToListAsync();

            var response = new Dictionary<long, object>(); // Инициализируем словарь для ответа

            // Обрабатываем каждую загрузку данных
            foreach (var upload in uploads)
            {
                // Формируем список точек для текущей загрузки
                var points = upload.Points.Select(p => new
                {
                    id = p.PointId,
                    name = p.Name,
                    height = p.Height
                }).ToList();

                // Формируем список треков для текущей загрузки
                var tracks = upload.Tracks.Select(t => new
                {
                    firstId = t.FirstId,
                    secondId = t.SecondId,
                    distance = t.Distance,
                    surface = t.Surface.ToString().ToUpper(),
                    maxSpeed = t.MaxSpeed.ToString().ToUpper()
                }).ToList();

                // Добавляем текущую загрузку в ответ с использованием времени загрузки как ключа
                response[upload.UploadTime] = new { points, tracks };
            }

            return Ok(response); // Возвращаем успешный ответ с сформированным JSON-ответом
        }
    }
}
