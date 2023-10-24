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
                // Hvis ikke vi bruger Mapster, så skal vi manuelt lave/have en 
                // metode, der konverterer om fra vores City klasse til vores 
                // CityDTO klasse. I tilfældet her bruger vi metoden CityToCityDTO til
                // at stå for denne konvertering. Denne metode er i bunden af vores fil her.
                // Hvis vi vælger ikke at bruge Mapster til vores data konvertering, skal vi
                // have 2 metoder i alle vores controllers, der i princippet gør det samme.
                // Nemlig at:
                // 1) Konvertere fra vores model klasse til vores DTO klasse
                // 2) Konvertere fra vores DTO klasse til vores model klasse
                // I tilfældet her med vores CitiesController har vi de 2 metoder:
                // CityToCityDTO og CityDTOToCity
                CityDTOList = CityList.Select(c => CityToCityDTO(c, IncludeRelations)).ToList();
            }
            else
            {
                // Hvis vi vælger at bruge Mapster til at konvertere data mellem vores
                // model klasse og vores DTO klasse, så bliver det hele meget meget lettere.
                // Så kan vi altid bruge Mapster til at konvertere:
                // 1) Data fra vores model klasse til vores DTO klasse
                // 2) Data fra vores DTO klasse til vores model klasse
                // Syntaksen for at bruge Mapster er (i forhold til eksemplet lige herunder):
                // Vi starter med vores liste af objekter af vores City model klasse. I eksemplet
                // herunder er CityList en liste af objekter af vores City klasse. CityDTO angiver
                // den (DTO) klasse, som vores City objekter (CityList) skal konverteres til.
                // Så i praksis sker der dette i linjen herunder. Ved brug af Mapster bliver
                // vores liste af City objekter (CityList) konverteret til et array af CityDTO
                // objekter. Efterfølgende bliver dette array af CityDTO objekter konverteret til en
                // liste af CityDTO objekter ved brug af kommandoen .ToList()
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
        public async Task<ActionResult<CityDTO>> GetCity(int CityId)
        {
          if (_context.Cities == null)
          {
              return NotFound();
          }
            City City_Object = await _context.Cities.Include(c => c.Country).FirstOrDefaultAsync(c => c.CityId == CityId);
            
            if (City_Object == null)
            {
                return NotFound();
            }

            // Hvis vi vælger at bruge Mapster til at konvertere data mellem vores
            // model klasse og vores DTO klasse, så bliver det hele meget meget lettere.
            // Så kan vi altid bruge Mapster til at konvertere:
            // 1) Data fra vores model klasse til vores DTO klasse
            // 2) Data fra vores DTO klasse til vores model klasse
            // Syntaksen for at bruge Mapster er (i forhold til eksemplet lige herunder):
            // Vi starter med vores objekt af vores City model klasse. I eksemplet
            // herunder er City_Object et objekt af vores City klasse. CityDTO angiver
            // den (DTO) klasse, som vores City objekt (City_Object) skal konverteres til.
            // Så i praksis sker der dette i linjen herunder. Ved brug af Mapster bliver
            // vores City objekt (City_Object) konverteret til et objekt af CityDTO
            // klassen. 
            CityDTO CityDTO_Object = City_Object.Adapt<CityDTO>();

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
                City_Object = CityForUpdateDTO_Object.Adapt<City>();
            }
            else
            {
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
                City_Object = CityForSaveDTO_Object.Adapt<City>();
            }
            else
            {
                City_Object = CityForSaveDTOToCity(CityForSaveDTO_Object);
            }

            _context.Cities.Add(City_Object);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCity", new { id = City_Object.CityId }, City_Object);
        }

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

        private static City CityForUpdateDTOToCity(CityForUpdateDTO CityForUpdateDTO_Object)
        {
            City City_Object = new City();

            City_Object.CityId = CityForUpdateDTO_Object.CityId;
            City_Object.CityName = CityForUpdateDTO_Object.CityName;
            City_Object.CityDescription = CityForUpdateDTO_Object.CityDescription;
            City_Object.CountryId = CityForUpdateDTO_Object.CountryId;

            return City_Object;
        }

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
