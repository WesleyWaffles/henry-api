using Henry.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Henry.Api.Services
{
    /// <summary>
    /// Service for managing appointments
    /// </summary>
    public sealed class AppointmentService
    {
        private readonly HenryDbContenxt _db;

        /// <summary>
        /// Instantiates a new instance of the AppointmentService
        /// </summary>
        /// <param name="db">The database context to use for this instance</param>
        public AppointmentService(HenryDbContenxt db) => _db = db;

        /// <summary>
        /// Adds an appointment to the database
        /// </summary>
        /// <param name="appointment">The appointment to add</param>
        /// <exception cref="ValidationException"></exception>
        public async Task AddAppointment(Appointment appointment)
        {
            if (!AppointmentIs15MinuteBlock(appointment))
                throw new ValidationException("Appointments must be in 15 minute blocks");
            if (await AppointmentExists(appointment))
                throw new ValidationException("Appointment already exists");

            await _db.Appointments.AddAsync(appointment);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Places a Reservation on an appointment
        /// </summary>
        /// <param name="appointment">The appointment to reserve</param>
        public async Task ReserveAppointment(Appointment appointment)
        {
            appointment.ReservedOn = DateTime.Now;
            _db.Appointments.Update(appointment);
            await _db.SaveChangesAsync();
        }

        // Probably a custom validatation attribute would be a better approach here but this will do for time constraints
        /// <summary>
        /// Checks that an appointment is in an exact 15 minute block
        /// </summary>
        /// <param name="appointment">The appointment to check</param>
        /// <returns>True or False indicating if the appoint time block is valid</returns>
        private bool AppointmentIs15MinuteBlock(Appointment appointment)
        {
            var duration = appointment.AppointmentTo - appointment.AppointmentFrom;
            if (duration != TimeSpan.FromMinutes(15))
                return false;
            return true;
        }

        /// <summary>
        /// Checks if an appointment already exists
        /// </summary>
        /// <param name="appointment">The appointment to check</param>
        /// <returns>True or False indicating if the appointment exists</returns>
        private async Task<bool> AppointmentExists(Appointment appointment)
        {
            var appointmentExistsQuery = _db.Appointments.Where(x => 
                x.Provider == appointment.Provider
                && x.Client == appointment.Client
                && x.AppointmentOn == appointment.AppointmentOn 
                && x.AppointmentFrom == appointment.AppointmentFrom
                && x.AppointmentTo == appointment.AppointmentTo);

            var existingAppointment = await appointmentExistsQuery.FirstOrDefaultAsync();
            return existingAppointment is not null;
        }
    }
}
