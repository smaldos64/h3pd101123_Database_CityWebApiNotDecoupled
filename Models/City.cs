using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityWebApiNotDecoupled.Models
{
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CityId { get; set; }
        [Required]
        public string CityName { get; set; }
        [Required]
        public string CityDescription { get; set; }

        [Required]
        [ForeignKey("CountryId")]
        public int CountryId { get; set; }
        public Country? Country { get; set; }
    }
}
