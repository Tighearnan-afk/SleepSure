﻿using System;
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
    public class TemperatureSensorsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public TemperatureSensorsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/TemperatureSensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TemperatureSensor>>> GetTemperatureSensors()
        {
            return await _context.TemperatureSensors.ToListAsync();
        }

        // GET: api/TemperatureSensors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TemperatureSensor>> GetTemperatureSensor(int id)
        {
            var temperatureSensor = await _context.TemperatureSensors.FindAsync(id);

            if (temperatureSensor == null)
            {
                return NotFound();
            }

            return temperatureSensor;
        }

        // PUT: api/TemperatureSensors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTemperatureSensor(int id, TemperatureSensor temperatureSensor)
        {
            if (id != temperatureSensor.Id)
            {
                return BadRequest();
            }

            _context.Entry(temperatureSensor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TemperatureSensorExists(id))
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

        // POST: api/TemperatureSensors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TemperatureSensor>> PostTemperatureSensor(TemperatureSensor temperatureSensor)
        {
            _context.TemperatureSensors.Add(temperatureSensor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTemperatureSensor", new { id = temperatureSensor.Id }, temperatureSensor);
        }

        // DELETE: api/TemperatureSensors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemperatureSensor(int id)
        {
            var temperatureSensor = await _context.TemperatureSensors.FindAsync(id);
            if (temperatureSensor == null)
            {
                return NotFound();
            }

            _context.TemperatureSensors.Remove(temperatureSensor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TemperatureSensorExists(int id)
        {
            return _context.TemperatureSensors.Any(e => e.Id == id);
        }
    }
}
