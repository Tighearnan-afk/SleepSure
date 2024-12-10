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
    public class HumiditySensorsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public HumiditySensorsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/HumiditySensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HumiditySensor>>> GetHumiditySensors()
        {
            return await _context.HumiditySensors.ToListAsync();
        }

        // GET: api/HumiditySensors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HumiditySensor>> GetHumiditySensor(int id)
        {
            var humiditySensor = await _context.HumiditySensors.FindAsync(id);

            if (humiditySensor == null)
            {
                return NotFound();
            }

            return humiditySensor;
        }

        // PUT: api/HumiditySensors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHumiditySensor(int id, HumiditySensor humiditySensor)
        {
            if (id != humiditySensor.Id)
            {
                return BadRequest();
            }

            _context.Entry(humiditySensor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HumiditySensorExists(id))
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

        // POST: api/HumiditySensors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HumiditySensor>> PostHumiditySensor(HumiditySensor humiditySensor)
        {
            _context.HumiditySensors.Add(humiditySensor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHumiditySensor", new { id = humiditySensor.Id }, humiditySensor);
        }

        // DELETE: api/HumiditySensors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHumiditySensor(int id)
        {
            var humiditySensor = await _context.HumiditySensors.FindAsync(id);
            if (humiditySensor == null)
            {
                return NotFound();
            }

            _context.HumiditySensors.Remove(humiditySensor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HumiditySensorExists(int id)
        {
            return _context.HumiditySensors.Any(e => e.Id == id);
        }
    }
}
