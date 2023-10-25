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
                // Hvis ikke vi vil have nogle relationer med ud til vores klient.
                // Det vil sige, vi ikke vil have nogle informationer ud omkring
                // Country, så bruger vi ikke nogen form form Include i vores
                // database forespørgsel.
                CityList = await _context.Cities.ToListAsync();
            }
            else
            {
                // Hvis vi vil have relationer med ud til vores klient.
                // Det vil sige, vi vil have informationer ud omkring
                // Country, så bruger en Include i vores
                // database forespørgsel.
                CityList = await _context.Cities.Include(c => c.Country).ToListAsync(); 
            }

            List<CityDTO> CityDTOList = new List<CityDTO>();
            
            if (false ==  UseMapster)
            {
                // Her bruger vi vores egen metode CityToCityDTO til at konvertere
                // en liste af City objekter om til en liste af CityDTO objekter.
                CityDTOList = CityList.Select(c => CityToCityDTO(c, IncludeRelations)).ToList();
            }
            else
            {
                // Her bruger vi Mapster til at konvertere en liste af City objekter
                // om til en liste af CityDTO objekter.
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
                // Her bruger vi vores egen metode CityToCityDTO til at konvertere
                // en liste af City objekter om til en liste af CityDTO objekter.
                CityDTOList = CityList.Select(c => CityToCityDTO(c, IncludeRelations)).ToList();
            }
            else
            {
                // Her bruger vi Mapster til at konvertere en liste af City objekter
                // om til en liste af CityDTO objekter.
                CityDTOList = CityList.Adapt<CityDTO[]>().ToList();
            }

            return Ok(CityDTOList);
        }

        // GET: api/Cities/5
        [HttpGet("{CityId}")]
        public async Task<ActionResult<CityDTO>> GetCity(int CityId,
                                                         bool UseMapster = true)
        {
          if (_context.Cities == null)
          {
              return NotFound();
          }
            City City_Object = await _context.Cities.Include(c => c.Country).FirstOrDefaultAsync(c => c.CityId == CityId);

            CityDTO CityDTO_Object = new CityDTO();

            if (City_Object == null)
            {
                return NotFound();
            }

            if (false == UseMapster)
            {
                // Her bruger vi vores egen metode CityToCityDTO til at konvertere
                // et City objekt om til et CityDTO objekt.
                CityDTO_Object = CityToCityDTO(City_Object);
            }
            else
            {
                // Her bruger vi Mapster til at konvertere et City objekt
                // om til et CityDTO objekt.
                CityDTO_Object = City_Object.Adapt<CityDTO>();
            }

            return CityDTO_Object;
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{CityId}")]
        public async Task<IActionResult> PutCity(int CityId, 
                                                 CityForUpdateDTO CityForUpdateDTO_Object,
                                                 bool UseMapster = true)
        {
            if (CityId != CityForUpdateDTO_Object.CityId)
            {
                return BadRequest();
            }

            City City_Object = new City();
            
            if (UseMapster)
            {
                // Vi bruger Mapster her til at konvertere et CityForUpdateDTO
                // Objekt om til til City Objekt
                City_Object = CityForUpdateDTO_Object.Adapt<City>();
            }
            else
            {
                // Vi bruger her vores egen metode CityForUpdateDTOToCity for
                // at konvertere et CityForUpdateDTO Objekt om til til City Objekt
                City_Object = CityForUpdateDTOToCity(CityForUpdateDTO_Object);
            }

            _context.Entry(City_Object).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(CityId))
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
        public async Task<ActionResult<CityForSaveDTO>> PostCity(CityForSaveDTO CityForSaveDTO_Object,
                                                                 bool UseMapster = true)
        {
          if (_context.Cities == null)
          {
              return Problem("Entity set 'DatabaseContext.Cities'  is null.");
          }

            City City_Object = new City();

            if (UseMapster)
            {
                // Vi bruger Mapster her til at konvertere et CityForSaveDTO Objekt
                // om til til City Objekt
                City_Object = CityForSaveDTO_Object.Adapt<City>();
            }
            else
            {
                // Vi bruger her vores egen metode CityForSaveDTOToCity for
                // at konvertere et CityForSaveDTO Objekt om til til City Objekt
                City_Object = CityForSaveDTOToCity(CityForSaveDTO_Object);
            }

            _context.Cities.Add(City_Object);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = City_Object.CityId }, City_Object);
        }

        // Når vi skal lave en Delete, behøver vi ikke bruge DTO klasser. 
        // For i tilfælde af Delete skal vi "bare" slette en record i 
        // vores Database med et givet ID
        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int CityId)
        {
            if (_context.Cities == null)
            {
                return NotFound();
            }
            City City_Object = await _context.Cities.FindAsync(CityId);
            if (City_Object == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(City_Object);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(int CityId)
        {
            return (_context.Cities?.Any(e => e.CityId == CityId)).GetValueOrDefault();
        }

        // Metoden herunder konverter data fra et City objekt
        // til et CityDTO objekt.
        private static CityDTO CityToCityDTO(City City_Object, bool IncludeRelations = true)
        {
            CityDTO CityDTO_Object = new CityDTO();

            CityDTO_Object.CityId = City_Object.CityId;
            CityDTO_Object.CityName = City_Object.CityName;
            CityDTO_Object.CityDescription = City_Object.CityDescription;
            CityDTO_Object.CountryId = City_Object.CountryId;

            if (IncludeRelations)
            {
                CityDTO_Object.Country = new CountryDTOMinusRelations();

                CityDTO_Object.Country.CountryName = City_Object.Country.CountryName;
                CityDTO_Object.Country.CountryId = City_Object.CountryId;
            }

            return CityDTO_Object;
        }

        // Metoden herunder konverter data fra et CityForUpdateDTO objekt
        // til et City objekt.
        private static City CityForUpdateDTOToCity(CityForUpdateDTO CityForUpdateDTO_Object)
        {
            City City_Object = new City();

            City_Object.CityId = CityForUpdateDTO_Object.CityId;
            City_Object.CityName = CityForUpdateDTO_Object.CityName;
            City_Object.CityDescription = CityForUpdateDTO_Object.CityDescription;
            City_Object.CountryId = CityForUpdateDTO_Object.CountryId;

            return City_Object;
        }

        // Metoden herunder konverter data fra et CityForSaveDTO objekt
        // til et City objekt.
        private static City CityForSaveDTOToCity(CityForSaveDTO CityForSaveDTO_Object)
        {
            City City_Object = new City();

            City_Object.CityName = CityForSaveDTO_Object.CityName;
            City_Object.CityDescription = CityForSaveDTO_Object.CityDescription;
            City_Object.CountryId = CityForSaveDTO_Object.CountryId;

            return City_Object;
        }
    }
}
