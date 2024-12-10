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
    public class WaterLeakSensorsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public WaterLeakSensorsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/WaterLeakSensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WaterLeakSensor>>> GetWaterLeakSensors()
        {
            return await _context.WaterLeakSensors.ToListAsync();
        }

        // GET: api/WaterLeakSensors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WaterLeakSensor>> GetWaterLeakSensor(int id)
        {
            var waterLeakSensor = await _context.WaterLeakSensors.FindAsync(id);

            if (waterLeakSensor == null)
            {
                return NotFound();
            }

            return waterLeakSensor;
        }

        // PUT: api/WaterLeakSensors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWaterLeakSensor(int id, WaterLeakSensor waterLeakSensor)
        {
            if (id != waterLeakSensor.Id)
            {
                return BadRequest();
            }

            _context.Entry(waterLeakSensor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WaterLeakSensorExists(id))
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

        // POST: api/WaterLeakSensors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<WaterLeakSensor>> PostWaterLeakSensor(WaterLeakSensor waterLeakSensor)
        {
            _context.WaterLeakSensors.Add(waterLeakSensor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWaterLeakSensor", new { id = waterLeakSensor.Id }, waterLeakSensor);
        }

        // DELETE: api/WaterLeakSensors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWaterLeakSensor(int id)
        {
            var waterLeakSensor = await _context.WaterLeakSensors.FindAsync(id);
            if (waterLeakSensor == null)
            {
                return NotFound();
            }

            _context.WaterLeakSensors.Remove(waterLeakSensor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WaterLeakSensorExists(int id)
        {
            return _context.WaterLeakSensors.Any(e => e.Id == id);
        }
    }
}
