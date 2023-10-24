using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CityWebApiNotDecoupled.Models;
using CityWebApiNotDecoupled.DTO;

using Mapster;

namespace CityWebApiNotDecoupled.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public CitiesController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityDTO>>> GetCities(bool IncludeRelations = true,
                                                                        bool UseMapster = true)
        {
            List<City> CityList = new List<City>();

            if (_context.Cities == null)
            {
               return NotFound();
            }

            if (false == IncludeRelations)
            {
                CityList = await _context.Cities.ToListAsync();
            }
            else
            {
                CityList = await _context.Cities.Include(c => c.Country).ToListAsync(); 
            }

            List<CityDTO> CityDTOList = new List<CityDTO>();
            
            if (false ==  UseMapster)
            {
                CityDTOList = CityList.Select(c => CityToCityDTO(c, IncludeRelations)).ToList();
            }
            else
            {
                CityDTOList = CityList.Adapt<CityDTO[]>().ToList();
            }
            
            return Ok(CityDTOList);
        }

        // GET: api/Cities
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IEnumerable<CityDTO>>> GetCitiesWithinCountry(int CountryId,
                                                                                     bool IncludeRelations = true,
                                                                                     bool UseMapster = true)
        {
            List<City> CityList = new List<City>();

            if (_context.Cities == null)
            {
                return NotFound();
            }

            if (false == IncludeRelations)
            {
                CityList = await _context.Cities.Where(c => c.CountryId == CountryId).ToListAsync();
            }
            else
            {
                CityList = await _context.Cities.Include(c => c.Country).Where(c => c.CountryId == CountryId).ToListAsync();
            }

            List<CityDTO> CityDTOList = new List<CityDTO>();
           
            if (false == UseMapster)
            {
                CityDTOList = CityList.Select(c => CityToCityDTO(c, IncludeRelations)).ToList();
            }
            else
            {
                CityDTOList = CityList.Adapt<CityDTO[]>().ToList();
            }

            return Ok(CityDTOList);
        }

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CityDTO>> GetCity(int id)
        {
          if (_context.Cities == null)
          {
              return NotFound();
          }
            //var city = await _context.Cities.FindAsync(id);
            var city = await _context.Cities.Include(c => c.Country).FirstOrDefaultAsync(c => c.CityId == id);
            
            if (city == null)
            {
                return NotFound();
            }

            CityDTO CityDTO_Object = city.Adapt<CityDTO>();

            //return city;
            return CityDTO_Object;
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, City city)
        {
            if (id != city.CityId)
            {
                return BadRequest();
            }

            _context.Entry(city).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
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

        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CityForSaveDTO>> PostCity(CityForSaveDTO city)
        {
          if (_context.Cities == null)
          {
              return Problem("Entity set 'DatabaseContext.Cities'  is null.");
          }

            City City_Object = new City();
            City_Object = city.Adapt<City>();
            _context.Cities.Add(City_Object);

            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetCity", new { id = city.CityId }, city);
            return CreatedAtAction("GetCity", new { id = City_Object.CityId }, city);
        }

        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            if (_context.Cities == null)
            {
                return NotFound();
            }
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(int id)
        {
            return (_context.Cities?.Any(e => e.CityId == id)).GetValueOrDefault();
        }

        private static CityDTO CityToCityDTO(City city, bool IncludeRelations = true)
        {
            CityDTO CityDTO_Object = new CityDTO();

            CityDTO_Object.CityId = city.CityId;
            CityDTO_Object.CityName = city.CityName;
            CityDTO_Object.CityDescription = city.CityDescription;
            CityDTO_Object.CountryId = city.CountryId;

            if (IncludeRelations)
            {
                CityDTO_Object.Country = new CountryDTOMinusRelations();

                CityDTO_Object.Country.CountryName = city.Country.CountryName;
                CityDTO_Object.Country.CountryId = city.CountryId;
            }

            return CityDTO_Object;

        }
    }
}
