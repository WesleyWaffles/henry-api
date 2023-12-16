namespace Henry.Api.Data
{
    public class Appointments
    {
        public int Id { get; set; }
        public Provider Provider { get; set; }
        public Client Client { get; set; }
        public DateOnly AppointmentOn { get; set; }
        public TimeOnly AppointmentFrom { get; set; }
        public TimeOnly AppointmentTo { get; set; }
    }
}
