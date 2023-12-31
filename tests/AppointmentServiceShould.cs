using Henry.Api.Data;
using Henry.Api.Services;
using System.ComponentModel.DataAnnotations;

namespace Henry.Api.UnitTests
{
    public class AppointmentServiceShould
    {
        private Provider _globalProvider;
        private Client _globalClient;
        private DateTime _globalDefaultDateTime;

        [SetUp]
        public void Setup()
        {
            _globalProvider = new Provider { Id = 1, Name = "Lando Calrisian" };
            _globalClient = new Client { Id = 1, Name = "Han Solo" };
        }

        [Test]
        public async Task RequireAppointmentsIn15MinuteBlocks()
        {
            await using var db = new InMemoryDb().CreateDbContext();
            var sut = new AppointmentService(db);

            var tooLongAppointment = new Appointment
            {
                Client = _globalClient,
                Provider = _globalProvider,
                AppointmentOn = new DateOnly(2024, 01, 05),
                AppointmentFrom = new TimeOnly(13, 0),
                AppointmentTo = new TimeOnly(14, 0)
            };

            Assert.ThrowsAsync<ValidationException>(async () => await sut.Add(tooLongAppointment));
        }

        [Test]
        public async Task AddAppointmentAndReturnWithPrimaryKey()
        {
            await using var db = new InMemoryDb().CreateDbContext();
            var sut = new AppointmentService(db);

            var validAppointment = new Appointment
            {
                Client = _globalClient,
                Provider = _globalProvider,
                AppointmentOn = new DateOnly(2024, 01, 05),
                AppointmentFrom = new TimeOnly(13, 0),
                AppointmentTo = new TimeOnly(13, 15)
            };

            await sut.Add(validAppointment);
            Assert.That(validAppointment.Id, Is.GreaterThan(0));
        }

        [Test]
        public async Task ReturnReservedAppointment()
        {
            await using var db = new InMemoryDb().CreateDbContext();
            var sut = new AppointmentService(db);

            var validAppointment = new Appointment
            {
                Client = _globalClient,
                Provider = _globalProvider,
                AppointmentOn = new DateOnly(2024, 01, 05),
                AppointmentFrom = new TimeOnly(13, 0),
                AppointmentTo = new TimeOnly(13, 15)
            };

            await sut.Add(validAppointment);
            await sut.Reserve(validAppointment);
            Assert.That(validAppointment.ReservedOn, Is.GreaterThan(_globalDefaultDateTime));
        }

        [Test]
        public async Task FailToConfirmReservationsOlderThan30Minutes()
        {
            await using var db = new InMemoryDb().CreateDbContext();
            var sut = new AppointmentService(db);

            var staleAppointment = new Appointment
            {
                Client = _globalClient,
                Provider = _globalProvider,
                AppointmentOn = new DateOnly(2024, 01, 05),
                AppointmentFrom = new TimeOnly(13, 0),
                AppointmentTo = new TimeOnly(13, 15),
                ReservedOn = DateTime.Now.AddMinutes(-31)
            };

            await sut.Add(staleAppointment);
            Assert.ThrowsAsync<ValidationException>(async () => await sut.Confirm(staleAppointment));
        }

        [Test]
        public async Task SuccessfullyConfirmReservationsNewerThan30Minutes()
        {
            await using var db = new InMemoryDb().CreateDbContext();
            var sut = new AppointmentService(db);

            var freshAppointment = new Appointment
            {
                Client = _globalClient,
                Provider = _globalProvider,
                AppointmentOn = new DateOnly(2024, 01, 05),
                AppointmentFrom = new TimeOnly(13, 0),
                AppointmentTo = new TimeOnly(13, 15),
                ReservedOn = DateTime.Now.AddMinutes(-21)
            };

            await sut.Add(freshAppointment);
            await sut.Confirm(freshAppointment);
            Assert.That(freshAppointment.Confirmed, Is.True);
        }

        [Test]
        public async Task ReturnAppointmentById()
        {
            await using var db = new InMemoryDb().CreateDbContext();
            var sut = new AppointmentService(db);

            var appointment = new Appointment
            {
                Client = _globalClient,
                Provider = _globalProvider,
                AppointmentOn = new DateOnly(2024, 01, 05),
                AppointmentFrom = new TimeOnly(13, 0),
                AppointmentTo = new TimeOnly(13, 15),
                ReservedOn = DateTime.Now.AddMinutes(-14)
            };
            await sut.Add(appointment);
            var returnedAppointment = await sut.Get(appointment.Id);
            Assert.That(returnedAppointment?.Id, Is.EqualTo(appointment.Id));
        }

        [Test]
        public async Task Require24HourNoticeForReservation()
        {
            await using var db = new InMemoryDb().CreateDbContext();
            var sut = new AppointmentService(db);

            var now = DateTime.Now;
            var appointment = new Appointment
            {
                Client = _globalClient,
                Provider = _globalProvider,
                AppointmentOn = new DateOnly(now.Year, now.Month, now.Day),
                AppointmentFrom = new TimeOnly(13, 0),
                AppointmentTo = new TimeOnly(13, 15),
                ReservedOn = DateTime.Now.AddMinutes(-14)
            };

            await sut.Add(appointment);
            Assert.ThrowsAsync<ValidationException>(async () => await sut.Reserve(appointment));
        }

        [Test]
        public async Task ReturnAllAvailableAppointmentsAndExcludeUnavailableAppointments()
        {
            await using var db = new InMemoryDb().CreateDbContext();
            var sut = new AppointmentService(db);

            var appointment1 = new Appointment
            {
                Client = _globalClient,
                Provider = _globalProvider,
                AppointmentOn = new DateOnly(2024, 01, 05),
                AppointmentFrom = new TimeOnly(13, 0),
                AppointmentTo = new TimeOnly(13, 15),
                ReservedOn = DateTime.Now.AddMinutes(-14)
            };

            var appointment2 = new Appointment
            {
                Client = _globalClient,
                Provider = _globalProvider,
                AppointmentOn = new DateOnly(2024, 01, 05),
                AppointmentFrom = new TimeOnly(13, 15),
                AppointmentTo = new TimeOnly(13, 30),
                ReservedOn = DateTime.Now.AddMinutes(-20)
            };

            var appointment3 = new Appointment
            {
                Client = _globalClient,
                Provider = _globalProvider,
                AppointmentOn = new DateOnly(2024, 01, 05),
                AppointmentFrom = new TimeOnly(13, 30),
                AppointmentTo = new TimeOnly(13, 45),
                ReservedOn = DateTime.Now.AddMinutes(-14),
                Confirmed = true
            };

            var appointment4 = new Appointment
            {
                Client = _globalClient,
                Provider = _globalProvider,
                AppointmentOn = new DateOnly(2024, 01, 05),
                AppointmentFrom = new TimeOnly(13, 45),
                AppointmentTo = new TimeOnly(14, 0),
                ReservedOn = DateTime.Now.AddMinutes(-31),
            };

            await sut.Add(appointment1);
            await sut.Add(appointment2);
            await sut.Add(appointment3);
            await sut.Add(appointment4);
            var appointments = await sut.GetAvailable();
            Assert.That(appointments.Count, Is.EqualTo(2));
        }
    }
}