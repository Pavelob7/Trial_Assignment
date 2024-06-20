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
    public class PointsController : ControllerBase
    {
        private readonly Data.RouteContext _context; // Контекст базы данных для работы с точками

        public PointsController(Data.RouteContext context) // Конструктор контроллера
        {
            _context = context;
        }

        // GET: api/Points
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Point>>> GetPoints()
        {
            // Получение всех точек из базы данных
            return await _context.Points.ToListAsync();
        }

        // GET: api/Points/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Point>> GetPoint(int id)
        {
            // Поиск точки по Id
            var point = await _context.Points.FindAsync(id);

            if (point == null)
            {
                return NotFound(); // Возвращаем 404 Not Found, если точка не найдена
            }

            return point; // Возвращаем найденную точку
        }

        // PUT: api/Points/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPoint(int id, Point point)
        {
            if (id != point.Id)
            {
                return BadRequest(); // Возвращаем ошибку BadRequest, если Id в URL не совпадает с Id точки
            }

            _context.Entry(point).State = EntityState.Modified; // Помечаем точку как измененную

            try
            {
                await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PointExists(id))
                {
                    return NotFound(); // Возвращаем 404 Not Found, если точка не найдена
                }
                else
                {
                    throw; // В случае других ошибок выбрасываем исключение
                }
            }

            return NoContent(); // Возвращаем успешный ответ без содержимого (204 No Content)
        }

        // POST: api/Points
        [HttpPost]
        public async Task<ActionResult<Point>> PostPoint(Point point)
        {
            _context.Points.Add(point); // Добавляем новую точку в контекст базы данных
            await _context.SaveChangesAsync(); // Сохраняем изменения

            // Возвращаем успешный ответ с кодом 201 Created и информацией о новой точке
            return CreatedAtAction(nameof(GetPoint), new { id = point.Id }, point);
        }

        // DELETE: api/Points/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePoint(int id)
        {
            var point = await _context.Points.FindAsync(id); // Находим точку по Id
            if (point == null)
            {
                return NotFound(); // Возвращаем 404 Not Found, если точка не найдена
            }

            _context.Points.Remove(point); // Удаляем точку из контекста базы данных
            await _context.SaveChangesAsync(); // Сохраняем изменения

            return NoContent(); // Возвращаем успешный ответ без содержимого (204 No Content)
        }

        private bool PointExists(int id)
        {
            return _context.Points.Any(e => e.Id == id); // Проверяем существование точки по Id
        }
    }
}
