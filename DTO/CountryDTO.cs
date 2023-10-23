using CityWebApiNotDecoupled.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CityWebApiNotDecoupled.DTO
{
    public class CountryForSaveDTO
    {
        public string CountryName { get; set; }
    }

    public class CountryForUpdateDTO : CountryForSaveDTO
    {
        public int CountryId { get; set; }
    }

    public class CountryDTO : CountryForUpdateDTO
    {
        public ICollection<CityDTOMinusRelations> Cities { get; set; }
    }

    public class CountryDTOMinusRelations : CountryForUpdateDTO
    {
       
    }
}
