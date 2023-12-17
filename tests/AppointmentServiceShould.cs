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
            _globalClient = new Client { Id = 1, Name = "Han Solo", Email = "Han.Solo@FalconMail.com" };
            _globalDefaultDateTime = new DateTime(1, 1, 1, 0, 0, 0);
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

            Assert.ThrowsAsync<ValidationException>(async () => await sut.AddAppointment(tooLongAppointment));
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

            await sut.AddAppointment(validAppointment);
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

            await sut.AddAppointment(validAppointment);
            await sut.ReserveAppointment(validAppointment);
            Assert.That(validAppointment.ReservedOn, Is.GreaterThan(_globalDefaultDateTime));
        }

        [Test]
        public async Task FailToConfirmReservationsOlderThan15Minutes()
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
                ReservedOn = DateTime.Now.AddMinutes(-16)
            };

            await sut.AddAppointment(staleAppointment);
            Assert.ThrowsAsync<ValidationException>(async () => await sut.ConfirmAppointment(staleAppointment));
        }

        [Test]
        public async Task SuccessfullyConfirmReservationsNewerThan15Minutes()
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
                ReservedOn = DateTime.Now.AddMinutes(-14)
            };

            await sut.AddAppointment(freshAppointment);
            await sut.ConfirmAppointment(freshAppointment);
            Assert.That(freshAppointment.Confirmed, Is.True);
        }
    }
}

