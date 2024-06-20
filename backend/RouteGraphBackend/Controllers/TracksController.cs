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
    public class TracksController : ControllerBase
    {
        private readonly Data.RouteContext _context; // Контекст базы данных для работы с данными треков

        public TracksController(Data.RouteContext context) // Конструктор контроллера треков
        {
            _context = context;
        }

        // GET: api/Tracks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Track>>> GetTracks()
        {
            // Получаем все треки из базы данных и возвращаем их
            return await _context.Tracks.ToListAsync();
        }

        // GET: api/Tracks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Track>> GetTrack(int id)
        {
            // Находим трек по указанному идентификатору
            var track = await _context.Tracks.FindAsync(id);

            if (track == null)
            {
                return NotFound(); // Если трек не найден, возвращаем ошибку 404
            }

            return track; // Возвращаем найденный трек
        }

        // PUT: api/Tracks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrack(int id, Track track)
        {
            if (id != track.TrackId)
            {
                return BadRequest(); // Если идентификаторы не совпадают, возвращаем ошибку 400
            }

            _context.Entry(track).State = EntityState.Modified; // Устанавливаем состояние изменений для трека

            try
            {
                await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrackExists(id))
                {
                    return NotFound(); // Если трек не существует, возвращаем ошибку 404
                }
                else
                {
                    throw; // В случае других ошибок выбрасываем исключение
                }
            }

            return NoContent(); // Возвращаем успешный ответ без содержимого
        }

        // POST: api/Tracks
        [HttpPost]
        public async Task<ActionResult<Track>> PostTrack(Track track)
        {
            _context.Tracks.Add(track); // Добавляем новый трек в контекст
            await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных

            return CreatedAtAction(nameof(GetTrack), new { id = track.TrackId }, track); // Возвращаем успешный ответ с созданным треком
        }

        // DELETE: api/Tracks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrack(int id)
        {
            var track = await _context.Tracks.FindAsync(id); // Находим трек по идентификатору
            if (track == null)
            {
                return NotFound(); // Если трек не найден, возвращаем ошибку 404
            }

            _context.Tracks.Remove(track); // Удаляем трек из контекста
            await _context.SaveChangesAsync(); // Сохраняем изменения в базе данных

            return NoContent(); // Возвращаем успешный ответ без содержимого
        }

        private bool TrackExists(int id)
        {
            return _context.Tracks.Any(e => e.TrackId == id); // Проверяем, существует ли трек с указанным идентификатором
        }
    }
}
