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
    public class DoorSensorsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public DoorSensorsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/DoorSensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoorSensor>>> GetDoorSensors()
        {
            return await _context.DoorSensors.ToListAsync();
        }

        // GET: api/DoorSensors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DoorSensor>> GetDoorSensor(int id)
        {
            var doorSensor = await _context.DoorSensors.FindAsync(id);

            if (doorSensor == null)
            {
                return NotFound();
            }

            return doorSensor;
        }

        // PUT: api/DoorSensors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoorSensor(int id, DoorSensor doorSensor)
        {
            if (id != doorSensor.Id)
            {
                return BadRequest();
            }

            _context.Entry(doorSensor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoorSensorExists(id))
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

        // POST: api/DoorSensors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DoorSensor>> PostDoorSensor(DoorSensor doorSensor)
        {
            _context.DoorSensors.Add(doorSensor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDoorSensor", new { id = doorSensor.Id }, doorSensor);
        }

        // DELETE: api/DoorSensors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoorSensor(int id)
        {
            var doorSensor = await _context.DoorSensors.FindAsync(id);
            if (doorSensor == null)
            {
                return NotFound();
            }

            _context.DoorSensors.Remove(doorSensor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DoorSensorExists(int id)
        {
            return _context.DoorSensors.Any(e => e.Id == id);
        }
    }
}
