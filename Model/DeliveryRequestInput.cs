using System.ComponentModel.DataAnnotations;

namespace PHPAPI.Model
{
    public class DeliveryRequestInput
    {
        [Required]
        public Ware[] Wares { get; set; }

        [Required]
        public Location Location { get; set; }

        [Required]
        public string TimeOfRequest { get; set; }
    }
}
