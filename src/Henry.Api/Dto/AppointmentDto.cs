using Henry.Api.Data;

namespace Henry.Api.Dto
{
    public sealed record AppointmentDto(Appointment Appointment, Uri GetAppointment, Uri ReserveAppointment, Uri ConfirmReservation);
}
