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
    public class MotionSensorsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public MotionSensorsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/MotionSensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MotionSensor>>> GetMotionSensors()
        {
            return await _context.MotionSensors.ToListAsync();
        }

        // GET: api/MotionSensors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MotionSensor>> GetMotionSensor(int id)
        {
            var motionSensor = await _context.MotionSensors.FindAsync(id);

            if (motionSensor == null)
            {
                return NotFound();
            }

            return motionSensor;
        }

        // PUT: api/MotionSensors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMotionSensor(int id, MotionSensor motionSensor)
        {
            if (id != motionSensor.Id)
            {
                return BadRequest();
            }

            _context.Entry(motionSensor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MotionSensorExists(id))
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

        // POST: api/MotionSensors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MotionSensor>> PostMotionSensor(MotionSensor motionSensor)
        {
            _context.MotionSensors.Add(motionSensor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMotionSensor", new { id = motionSensor.Id }, motionSensor);
        }

        // DELETE: api/MotionSensors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMotionSensor(int id)
        {
            var motionSensor = await _context.MotionSensors.FindAsync(id);
            if (motionSensor == null)
            {
                return NotFound();
            }

            _context.MotionSensors.Remove(motionSensor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MotionSensorExists(int id)
        {
            return _context.MotionSensors.Any(e => e.Id == id);
        }
    }
}
