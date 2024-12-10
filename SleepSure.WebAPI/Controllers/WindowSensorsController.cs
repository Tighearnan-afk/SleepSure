using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SleepSure.WebAPI.DataAccess;
using SleepSure.WebAPI.Models;

namespace SleepSure.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WindowSensorsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public WindowSensorsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/WindowSensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WindowSensor>>> GetWindowSensors()
        {
            return await _context.WindowSensors.ToListAsync();
        }

        // GET: api/WindowSensors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WindowSensor>> GetWindowSensor(int id)
        {
            var windowSensor = await _context.WindowSensors.FindAsync(id);

            if (windowSensor == null)
            {
                return NotFound();
            }

            return windowSensor;
        }

        // PUT: api/WindowSensors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWindowSensor(int id, WindowSensor windowSensor)
        {
            if (id != windowSensor.Id)
            {
                return BadRequest();
            }

            _context.Entry(windowSensor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WindowSensorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/WindowSensors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<WindowSensor>> PostWindowSensor(WindowSensor windowSensor)
        {
            _context.WindowSensors.Add(windowSensor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWindowSensor", new { id = windowSensor.Id }, windowSensor);
        }

        // DELETE: api/WindowSensors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWindowSensor(int id)
        {
            var windowSensor = await _context.WindowSensors.FindAsync(id);
            if (windowSensor == null)
            {
                return NotFound();
            }

            _context.WindowSensors.Remove(windowSensor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WindowSensorExists(int id)
        {
            return _context.WindowSensors.Any(e => e.Id == id);
        }
    }
}
