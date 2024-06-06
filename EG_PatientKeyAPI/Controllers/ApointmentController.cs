﻿using EG_PatientKeyAPI.Helpers;
using EG_PatientKeyAPI.Models.DTOs;
using EG_PatientKeyAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace EG_PatientKeyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApointmentController : ControllerBase
    {

        private readonly AppointmentService _apointmentService;

        public ApointmentController(AppointmentService apointmentService)
        {
            _apointmentService = apointmentService;
        }

        [HttpPost]
        [Route("Available")]
        public IActionResult AvailableDoctors(AppointmentCheckDTO appointmentCheckDTO)
        {

            if (appointmentCheckDTO.Start.Hour < 8)
                appointmentCheckDTO.Start = appointmentCheckDTO.Start.ChangeTime(8, 0);


            if (appointmentCheckDTO.End.Hour > 20)
                appointmentCheckDTO.End = appointmentCheckDTO.Start.ChangeTime(20, 0);


            if (appointmentCheckDTO.Start >= appointmentCheckDTO.End)
                return BadRequest("Start time must be before End time");

            if (appointmentCheckDTO.Duration <= 14)
                return BadRequest("Minimal duration time with doctor is 15 minutes");

            var takenAppoitments = _apointmentService.GetTakenAppointments(appointmentCheckDTO.Start, appointmentCheckDTO.End, appointmentCheckDTO.CalendarId);

            var timeSlots = _apointmentService.GetTimeslotsInTimePeriod(appointmentCheckDTO.Start, appointmentCheckDTO.End, appointmentCheckDTO.CalendarId, appointmentCheckDTO.Duration);

            var freeAppointments = _apointmentService.GetAvailableAppointments(appointmentCheckDTO.Start, appointmentCheckDTO.End, takenAppoitments, timeSlots, appointmentCheckDTO.Duration);

            return Ok(freeAppointments);
        }

    }
}
