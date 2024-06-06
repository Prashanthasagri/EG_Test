using System.ComponentModel.DataAnnotations;

namespace EG_PatientKeyAPI.Models.DTOs
{
    public class AppointmentCheckDTO
    {

        [Required]
        public List<string> CalendarId { get; set; }
        [Required]
        [Range(15, 90, ErrorMessage = "The duration must be in range from 15 to 90 minutes")]
        public int Duration { get; set; }
        [Required]
     
        public DateTime Start { get; set; }
        [Required]
   
        public DateTime End { get; set; }
    }
}
