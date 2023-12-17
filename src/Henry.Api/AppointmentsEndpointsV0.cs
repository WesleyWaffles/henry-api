using Henry.Api.Data;
using Henry.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Henry.Api
{
    public static class AppointmentsEndpointsV0
    {
        /// <summary>
        /// Maps the endpoints for the appointments API
        /// </summary>
        /// <param name="routeGroupBuilder">The route group to map endpoints to</param>
        /// <returns>A route group builder with mapped endpoints for the appointments api</returns>
        public static RouteGroupBuilder MapAppointmentsEndpointsV0(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder.MapGet("/", GetAvailableAppointments);
            
            routeGroupBuilder.MapGet("/{id}", GetAppointment);
            
            routeGroupBuilder.MapPost("/", CreateAppointment);

            routeGroupBuilder.MapPut("/{id}/reserve", ReserveAppointment);

            return routeGroupBuilder;
        }

        /// <summary>
        /// Gets an appointment by Id
        /// </summary>
        /// <param name="appointments">The appointment service to use</param>
        /// <param name="Id">The id of the appointment to get</param>
        /// <returns>An Appointment if found</returns>
        public static async Task<Results<Ok<Appointment>, NotFound>> GetAppointment(AppointmentService appointments, int Id)
        {
            var appointment = await appointments.Get(Id);
            if (appointment == null)
                return TypedResults.NotFound();
            return TypedResults.Ok(appointment);
        }

        /// <summary>
        /// Gets all available appointments
        /// </summary>
        /// <param name="appointments">The appointment service to use</param>
        /// <returns>A list of all available appointments</returns>
        public static async Task<Ok<List<Appointment>>> GetAvailableAppointments(AppointmentService appointments)
        {
            var availableAppointments = await appointments.Get();
            return TypedResults.Ok(availableAppointments);
        }

        /// <summary>
        /// Creates an appointment
        /// </summary>
        /// <param name="appointments">The appointment service to use</param>
        /// <param name="appointment">The appointment to create</param>
        /// <returns>A created appointment</returns>
        public static async Task<Created<Appointment>> CreateAppointment(AppointmentService appointments, Appointment appointment)
        {
            await appointments.Add(appointment);
            return TypedResults.Created($"/appointments/{appointment.Id}", appointment);
        }

        /// <summary>
        /// Reserves an appointment
        /// </summary>
        /// <param name="appointments">The appointment service to use</param>
        /// <param name="Id">The id of the appointment to reserve</param>
        /// <returns>A reserved appointment</returns>
        public static async Task<Results<Ok<Appointment>, NotFound>> ReserveAppointment(AppointmentService appointments, int Id)
        {
            var appointment = await appointments.Get(Id);
            if (appointment == null)
                return TypedResults.NotFound();
            await appointments.Reserve(appointment);
            return TypedResults.Ok(appointment);
        }
    }
}
