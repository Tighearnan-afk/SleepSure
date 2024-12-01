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
    public class DeviceLocationsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public DeviceLocationsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/DeviceLocations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceLocation>>> GetLocations()
        {
            return await _context.Locations.ToListAsync();
        }

        // GET: api/DeviceLocations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceLocation>> GetDeviceLocation(int id)
        {
            var deviceLocation = await _context.Locations.FindAsync(id);

            if (deviceLocation == null)
            {
                return NotFound();
            }

            return deviceLocation;
        }

        // PUT: api/DeviceLocations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeviceLocation(int id, DeviceLocation deviceLocation)
        {
            if (id != deviceLocation.Id)
            {
                return BadRequest();
            }

            _context.Entry(deviceLocation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceLocationExists(id))
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

        // POST: api/DeviceLocations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DeviceLocation>> PostDeviceLocation(DeviceLocation deviceLocation)
        {
            _context.Locations.Add(deviceLocation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDeviceLocation", new { id = deviceLocation.Id }, deviceLocation);
        }

        // DELETE: api/DeviceLocations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeviceLocation(int id)
        {
            var deviceLocation = await _context.Locations.FindAsync(id);
            if (deviceLocation == null)
            {
                return NotFound();
            }

            _context.Locations.Remove(deviceLocation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeviceLocationExists(int id)
        {
            return _context.Locations.Any(e => e.Id == id);
        }
    }
}
