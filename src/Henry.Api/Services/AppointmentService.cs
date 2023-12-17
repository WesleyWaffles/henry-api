using Henry.Api.Data;
using System.ComponentModel.DataAnnotations;

namespace Henry.Api.Services
{
    public sealed class AppointmentService
    {
        public AppointmentService()
        {
            
        }

        public Appointment? AddAppointment(Appointment appointment)
        {
            if (!AppointmentIs15MinuteBlock(appointment))
                throw new ValidationException("Appointments must be in 15 minute blocks");
            return default;
        }

        // Probably a custom validatation attribute would be a better approach here but this will do for time constraints
        private bool AppointmentIs15MinuteBlock(Appointment appointment)
        {
            var duration = appointment.AppointmentTo - appointment.AppointmentFrom;
            if (duration != TimeSpan.FromMinutes(15))
                return false;
            return true;
        }
    }
}
