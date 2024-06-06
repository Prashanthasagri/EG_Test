using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EG_PatientKeyAPI.Models.DTOs
{
    public class TakenAppointmentDTO
    {
        public Doctor Doctor { get; set; }
        public Appointment Appointment { get; set; }
    }
}
