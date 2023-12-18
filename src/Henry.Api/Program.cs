using Microsoft.EntityFrameworkCore;
using Henry.Api.Data;
using Henry.Api;
using Henry.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var dbPath = Path.Join(Directory.GetCurrentDirectory(), "Databases");
if (!Directory.Exists(dbPath))
    Directory.CreateDirectory(dbPath);
builder.Services.AddDbContext<HenryDbContenxt>(options => 
    options.UseSqlite($"Data Source={Path.Join(Directory.GetCurrentDirectory(), "Databases", "Henry.db")}"));

builder.Services.AddTransient<AppointmentService>();

var app = builder.Build();

// In a production app we likely wouldn't want to run migrations on startup becuase of the potential for data loss.
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetService<HenryDbContenxt>();
db?.Database.MigrateAsync();

// Map v0 endpoints
app.MapGroup("/api/v0/appointments").MapAppointmentsEndpointsV0();

app.Run();
