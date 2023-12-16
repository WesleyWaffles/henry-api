namespace Henry.Api.Data
{
    public class Appointment
    {
        public int Id { get; set; }
        public required Provider Provider { get; set; }
        public required Client Client { get; set; }
        public DateOnly AppointmentOn { get; set; }
        public TimeOnly AppointmentFrom { get; set; }
        public TimeOnly AppointmentTo { get; set; }
    }
}
