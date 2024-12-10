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
    public class LightsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public LightsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Lights
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Light>>> GetLight()
        {
            return await _context.Light.ToListAsync();
        }

        // GET: api/Lights/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Light>> GetLight(int id)
        {
            var light = await _context.Light.FindAsync(id);

            if (light == null)
            {
                return NotFound();
            }

            return light;
        }

        // PUT: api/Lights/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLight(int id, Light light)
        {
            if (id != light.Id)
            {
                return BadRequest();
            }

            _context.Entry(light).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LightExists(id))
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

        // POST: api/Lights
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Light>> PostLight(Light light)
        {
            _context.Light.Add(light);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLight", new { id = light.Id }, light);
        }

        // DELETE: api/Lights/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLight(int id)
        {
            var light = await _context.Light.FindAsync(id);
            if (light == null)
            {
                return NotFound();
            }

            _context.Light.Remove(light);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LightExists(int id)
        {
            return _context.Light.Any(e => e.Id == id);
        }
    }
}
