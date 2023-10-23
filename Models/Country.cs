using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CityWebApiNotDecoupled.Models
{
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CountryId { get; set; }
        [Required]
        public string CountryName { get; set; }
        public ICollection<City> Cities { get; set; }
    }
}
