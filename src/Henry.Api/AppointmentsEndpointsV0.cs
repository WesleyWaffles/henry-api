using Henry.Api.Data;
using Henry.Api.Dto;
using Henry.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Henry.Api
{
    public static class AppointmentsEndpointsV0
    {
        private static string AppointmentVersionUri = "/api/v0/appointments";

        /// <summary>
        /// Maps the endpoints for the appointments API
        /// </summary>
        /// <param name="routeGroupBuilder">The route group to map endpoints to</param>
        /// <returns>A route group builder with mapped endpoints for the appointments api</returns>
        public static RouteGroupBuilder MapAppointmentsEndpointsV0(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder.MapGet("/available", GetAvailableAppointments);
            
            routeGroupBuilder.MapGet("/{id}", GetAppointment);
            
            routeGroupBuilder.MapPost("/", CreateAppointment);

            routeGroupBuilder.MapPut("/{id}/reserve", ReserveAppointment);

            routeGroupBuilder.MapPut("/{id}/confirm", ConfirmAppointment);

            return routeGroupBuilder;
        }

        /// <summary>
        /// Gets an appointment by Id
        /// </summary>
        /// <param name="appointments">The appointment service to use</param>
        /// <param name="Id">The id of the appointment to get</param>
        /// <returns>An Appointment if found</returns>
        public static async Task<Results<Ok<AppointmentDto>, NotFound>> GetAppointment(AppointmentService appointments, int Id)
        {
            var appointment = await appointments.Get(Id);
            if (appointment == null)
                return TypedResults.NotFound();
            return TypedResults.Ok(GetAppointmentDto(appointment));
        }

        /// <summary>
        /// Gets all available appointments
        /// </summary>
        /// <param name="appointments">The appointment service to use</param>
        /// <returns>A list of all available appointments</returns>
        public static async Task<Ok<List<AppointmentDto>>> GetAvailableAppointments(AppointmentService appointments)
        {
            var availableAppointments = await appointments.GetAvailable();
            var appointmentDtos = availableAppointments.Select(x => GetAppointmentDto(x)).ToList();
            return TypedResults.Ok(appointmentDtos);
        }

        /// <summary>
        /// Creates an appointment
        /// </summary>
        /// <param name="appointments">The appointment service to use</param>
        /// <param name="appointment">The appointment to create</param>
        /// <returns>A created appointment</returns>
        public static async Task<Created<AppointmentDto>> CreateAppointment(AppointmentService appointments, Appointment appointment)
        {
            await appointments.Add(appointment);
            var appointmentDto = GetAppointmentDto(appointment);
            return TypedResults.Created(appointmentDto.GetAppointment, appointmentDto);
        }

        /// <summary>
        /// Reserves an appointment
        /// </summary>
        /// <param name="appointments">The appointment service to use</param>
        /// <param name="Id">The id of the appointment to reserve</param>
        /// <returns>A reserved appointment</returns>
        public static async Task<Results<Ok<AppointmentDto>, NotFound>> ReserveAppointment(AppointmentService appointments, int Id)
        {
            var appointment = await appointments.Get(Id);
            if (appointment == null)
                return TypedResults.NotFound();

            await appointments.Reserve(appointment);
            return TypedResults.Ok(GetAppointmentDto(appointment));
        }

        /// <summary>
        /// Confirms a reserved appointment
        /// </summary>
        /// <param name="appointments">The appointment service to use</param>
        /// <param name="Id">The id of the appointment to confirm a reservation for</param>
        /// <returns>A confirmed reserved appointment</returns>
        public static async Task<Results<Ok<AppointmentDto>, NotFound>> ConfirmAppointment(AppointmentService appointments, int Id)
        {
            var appointment = await appointments.Get(Id);
            if (appointment == null)
                return TypedResults.NotFound();

            await appointments.Confirm(appointment);
            return TypedResults.Ok(GetAppointmentDto(appointment));
        }

        /// <summary>
        /// A helper method to format the appointment dto
        /// </summary>
        /// <param name="appointment">The appointment entity to use</param>
        /// <returns>An AppointmentDto</returns>
        private static AppointmentDto GetAppointmentDto(Appointment appointment)
        {
            return new AppointmentDto(appointment,
                new Uri($"{AppointmentVersionUri}/{appointment.Id}", UriKind.Relative),
                new Uri($"{AppointmentVersionUri}/{appointment.Id}/reserve", UriKind.Relative),
                new Uri($"/api/v0/appointments/{appointment.Id}/confirm", UriKind.Relative));
        }
    }
}
