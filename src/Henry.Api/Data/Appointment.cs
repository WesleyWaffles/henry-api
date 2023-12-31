﻿namespace Henry.Api.Data
{
    public class Appointment
    {
        public int Id { get; set; }
        public required Provider Provider { get; set; }
        public Client? Client { get; set; }
        public DateOnly AppointmentOn { get; set; }
        public TimeOnly AppointmentFrom { get; set; }
        public TimeOnly AppointmentTo { get; set; }
        public DateTime? ReservedOn { get; set; }
        public bool Confirmed { get; set; }
    }
}
