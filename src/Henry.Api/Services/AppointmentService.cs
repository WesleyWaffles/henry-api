using Henry.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
        public async Task Add(Appointment appointment)
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
        public async Task Reserve(Appointment appointment)
        {
            if (!AppointmentIsReservable(appointment))
                throw new ValidationException("Appointments must be reserved at least 24 hours in advance");

            appointment.ReservedOn = DateTime.Now;
            _db.Appointments.Update(appointment);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Confirms an appointment
        /// </summary>
        /// <param name="appointment">The appointment to confirm</param>
        /// <exception cref="ValidationException"></exception>
        public async Task Confirm(Appointment appointment)
        {
            if (!AppointmentIsConfirmable(appointment))
                throw new ValidationException("Appointment reservation is more than 15 minutes old and can not be confirmed");
            
            appointment.Confirmed = true;
            _db.Appointments.Update(appointment);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all appointments
        /// </summary>
        /// <returns>A list of all appointments</returns>
        public async Task<List<Appointment>> Get() => await _db.Appointments.ToListAsync();

        /// <summary>
        /// Gets all available appointments including stale reservations
        /// </summary>
        /// <returns>A list of all available appointments</returns>
        public async Task<List<Appointment>> GetAvailable()
        { 
            return await _db.Appointments
                .Where(x => x.Confirmed == false
                    && (x.ReservedOn == null || x.ReservedOn > DateTime.Now.AddMinutes(-30)))
                .ToListAsync();
        }

        /// <summary>
        /// Gets an appointment by Id
        /// </summary>
        /// <param name="Id">The Id of the appointment</param>
        /// <returns>The appointment or null if not found</returns>
        public async Task<Appointment?> Get(int Id) => await _db.Appointments.FindAsync(Id);

        #region Validation
        // *********************************************************************** //
        // Probably custom validatation attributes would be a better approach here //
        // but this will do for time constraints                                   //
        // *********************************************************************** //

        /// <summary>
        /// Checks that an appointment is in an exact 15 minute block
        /// </summary>
        /// <param name="appointment">The appointment to check</param>
        /// <returns>Boolean indicating if the appoint time block is valid</returns>
        private bool AppointmentIs15MinuteBlock(Appointment appointment)
        {
            var duration = appointment.AppointmentTo - appointment.AppointmentFrom;
            if (duration != TimeSpan.FromMinutes(15))
                return false;
            return true;
        }

        /// <summary>
        /// Checks that an appointment reservation isn't older than 15 minutes
        /// </summary>
        /// <param name="appointment">The appointment to check</param>
        /// <returns>Boolean indicating if the appointment can be reserved</returns>
        private bool AppointmentIsConfirmable(Appointment appointment) => appointment.ReservedOn > DateTime.Now.AddMinutes(-30);

        /// <summary>
        /// Checks that an appointment is being reserved at least 24 hours in advance
        /// </summary>
        /// <param name="appointment">The appointment to check</param>
        /// <returns>Boolean indicating if the appointment is reservable</returns>
        private bool AppointmentIsReservable(Appointment appointment)
        {
            var now = DateTime.Now;
            var appointmentStartTime = new DateTime(
                appointment.AppointmentOn.Year, 
                appointment.AppointmentOn.Month, 
                appointment.AppointmentOn.Day, 
                appointment.AppointmentFrom.Hour, 
                appointment.AppointmentFrom.Minute, 
                0);
            return appointmentStartTime >= now.AddHours(24);
        }

        /// <summary>
        /// Checks if an appointment already exists
        /// </summary>
        /// <param name="appointment">The appointment to check</param>
        /// <returns>Boolean indicating if the appointment exists</returns>
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

        #endregion
    }
}
