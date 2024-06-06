using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EG_PatientKeyAPI.Models;
using Newtonsoft.Json;
using System.Numerics;

namespace EG_PatientKeyAPI.Data
{
 
        public class AppDbContext
        {
            public List<Appointment> Apointments { get; set; }
            public List<TimeSlot> TimeSlots { get; set; }
            public List<Doctor> Doctors { get; set; }

            public AppDbContext()
            {

                string jsonApoitmentsFile = File.ReadAllText(Directory.GetCurrentDirectory() + "\\PatientsSkyData\\Appoitments.json");

                Apointments = JsonConvert.DeserializeObject<List<Appointment>>(jsonApoitmentsFile);

                string jsonTimeSlotsFile = File.ReadAllText(Directory.GetCurrentDirectory() + "\\PatientsSkyData\\TimeSlots.json");
                TimeSlots = JsonConvert.DeserializeObject<List<TimeSlot>>(jsonTimeSlotsFile);

                Doctors = new List<Doctor>();
                Doctors.Add(new Doctor { CalendarId = "48644c7a-975e-11e5-a090-c8e0eb18c1e9", FirstName = "Joanna", LastName = "Hef" });
                Doctors.Add(new Doctor { CalendarId = "48cadf26-975e-11e5-b9c2-c8e0eb18c1e9", FirstName = "Danny", LastName = "Boy" });
                Doctors.Add(new Doctor { CalendarId = "452dccfc-975e-11e5-bfa5-c8e0eb18c1e9", FirstName = "Emma", LastName = "Win" });

            }
        }
}
