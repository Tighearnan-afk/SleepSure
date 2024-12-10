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
    public class VibrationSensorsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public VibrationSensorsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/VibrationSensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VibrationSensor>>> GetVibrationSensors()
        {
            return await _context.VibrationSensors.ToListAsync();
        }

        // GET: api/VibrationSensors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VibrationSensor>> GetVibrationSensor(int id)
        {
            var vibrationSensor = await _context.VibrationSensors.FindAsync(id);

            if (vibrationSensor == null)
            {
                return NotFound();
            }

            return vibrationSensor;
        }

        // PUT: api/VibrationSensors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVibrationSensor(int id, VibrationSensor vibrationSensor)
        {
            if (id != vibrationSensor.Id)
            {
                return BadRequest();
            }

            _context.Entry(vibrationSensor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VibrationSensorExists(id))
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

        // POST: api/VibrationSensors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<VibrationSensor>> PostVibrationSensor(VibrationSensor vibrationSensor)
        {
            _context.VibrationSensors.Add(vibrationSensor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVibrationSensor", new { id = vibrationSensor.Id }, vibrationSensor);
        }

        // DELETE: api/VibrationSensors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVibrationSensor(int id)
        {
            var vibrationSensor = await _context.VibrationSensors.FindAsync(id);
            if (vibrationSensor == null)
            {
                return NotFound();
            }

            _context.VibrationSensors.Remove(vibrationSensor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VibrationSensorExists(int id)
        {
            return _context.VibrationSensors.Any(e => e.Id == id);
        }
    }
}
