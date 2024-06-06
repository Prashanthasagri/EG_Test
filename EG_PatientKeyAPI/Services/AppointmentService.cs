using EG_PatientKeyAPI.Data;
using EG_PatientKeyAPI.Helpers;
using EG_PatientKeyAPI.Models.DTOs;
using EG_PatientKeyAPI.Models;

namespace EG_PatientKeyAPI.Services
{
    public class AppointmentService
    {
        private readonly AppDbContext _db = new AppDbContext();

        public List<TakenAppointmentDTO> GetTakenAppointments(DateTime start, DateTime end, List<string> calendarIds)
        {
            var doctorAppointments = _db.Apointments.Where(x => calendarIds.Contains(x.CalendarId))
                .Join(_db.Doctors, x => x.CalendarId, y => y.CalendarId, (appointment, doctor) => new TakenAppointmentDTO { Appointment = appointment, Doctor = doctor })
                .Where(x => x.Appointment.Start >= start && x.Appointment.End <= end).OrderBy(x => x.Doctor.CalendarId).ThenBy(x => x.Appointment.Start).ToList();

            doctorAppointments.Insert(0, new TakenAppointmentDTO
            {
                Appointment = new Appointment
                {
                    Start = start,
                    End = start,
                    OutOfOffice = true
                },
                Doctor = new Doctor()
            });

            doctorAppointments.Insert(doctorAppointments.Count, new TakenAppointmentDTO
            {
                Appointment = new Appointment
                {
                    Start = end,
                    End = end,
                    OutOfOffice = true
                }
            });

            TakenAppointmentDTO thisAppointment = new TakenAppointmentDTO { Appointment = new Appointment { Id = "1" } };
            TakenAppointmentDTO nextAppointment = new TakenAppointmentDTO();

            int count = 0;
            int index = 0;

            while (thisAppointment != nextAppointment && index + 1 < doctorAppointments.Count)
            {
                thisAppointment = doctorAppointments[index];
                nextAppointment = doctorAppointments[index + 1];


                if (thisAppointment.Appointment.End.Date < nextAppointment.Appointment.Start.Date)
                {
                    doctorAppointments.Insert(index, new TakenAppointmentDTO
                    {
                        Appointment = new Appointment
                        {
                            Start = DateTime.Parse($"{doctorAppointments[index].Appointment.End.Year}-{doctorAppointments[index].Appointment.End.Month}" +
                                                       $"-{doctorAppointments[index].Appointment.End.Day}T20:00:00"),

                            End = start.ChangeTime(8, 0).AddDays(1),
                            OutOfOffice = true
                        },
                        Doctor = new Doctor()

                    });

                    index++;
                }
                index++;
            }
            return doctorAppointments;
        }
        public List<TimeSlot> GetTimeslotsInTimePeriod(DateTime fromDate, DateTime toDate, List<string> calendarIds, int duration)
        {
            TimeSpan durationSpan = new TimeSpan(0, duration, 0);

            List<TimeSlot> timeSlots = _db.TimeSlots.Where(x => x.Start >= fromDate && x.End <= toDate)
                .Where(x => calendarIds.Contains(x.CalendarId))
                .OrderBy(x => x.CalendarId).ThenBy(x => x.Start).ToList();

            return timeSlots;
        }

        public List<AvailableAppointmentDTO> GetAvailableAppointments(DateTime start, DateTime end, List<TakenAppointmentDTO> takenAppointments, List<TimeSlot> timeSlots, int durration)
        {

            List<AvailableAppointmentDTO> availableAppointmentsDTO = new List<AvailableAppointmentDTO>();

            TimeSpan requestedMeetingDuration = new TimeSpan(0, durration, 0);

            DateTime lastAppointmentTime = new DateTime();


            for (int i = 0; i < takenAppointments.Count; i++)
            {
                if (lastAppointmentTime.Date < takenAppointments[i].Appointment.End.Date)
                {
                    lastAppointmentTime = takenAppointments[i].Appointment.End;
                }
                else if (lastAppointmentTime.Date == takenAppointments[i].Appointment.Start.Date)
                {
                    if (takenAppointments[i].Appointment.Start - lastAppointmentTime >= requestedMeetingDuration)
                    {
                        availableAppointmentsDTO.Add(
                            CreateAvailableAppointment(
                                lastAppointmentTime,
                                takenAppointments[i].Appointment.Start,
                                requestedMeetingDuration,
                                takenAppointments[i].Doctor,
                                timeSlots.Where(x => x.CalendarId == takenAppointments[i].Appointment.CalendarId)
                                .Where(x => x.Start >= lastAppointmentTime && x.End <= takenAppointments[i].Appointment.Start).ToList()
                                ));
                    }
                }
                lastAppointmentTime = takenAppointments[i].Appointment.End;
            }
            return availableAppointmentsDTO;
        }

        private AvailableAppointmentDTO CreateAvailableAppointment(DateTime fromTime, DateTime toTime, TimeSpan meetingDuration, Doctor doctor, List<TimeSlot> slots)
        {

            AvailableAppointmentDTO availableAppointment = new AvailableAppointmentDTO
            {
                Doctor = doctor,
                AvailableDateTime = new AvailableTimeModelDTO
                {
                    AvailableFromTime = fromTime,
                    AvailableToTime = toTime - meetingDuration,
                    DoctorUnavailableFrom = toTime,
                    RequestedTimeWithDoctor = (int)meetingDuration.TotalMinutes
                },
                TimeSlots = slots
            };

            return availableAppointment;
        }

    }
}

