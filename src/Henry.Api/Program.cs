using Microsoft.EntityFrameworkCore;
using Henry.Api.Data;
using Henry.Api.Services;
using Henry.Api.v0;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbPath = Path.Join(Directory.GetCurrentDirectory(), "Databases");
if (!Directory.Exists(dbPath))
    Directory.CreateDirectory(dbPath);
builder.Services.AddDbContext<HenryDbContenxt>(options => 
    options.UseSqlite($"Data Source={Path.Join(Directory.GetCurrentDirectory(), "Databases", "Henry.db")}"));

builder.Services.AddTransient<AppointmentService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// In a production app we likely wouldn't want to run migrations on startup becuase of the potential for data loss.
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetService<HenryDbContenxt>();
db?.Database.MigrateAsync();

// Map v0 endpoints
app.MapGroup("/api/v0/appointments").MapAppointmentsV0Endpoints().WithTags("Appointments Endpoints");
app.MapGroup("/api/v0/providers").MapProviderV0Endpoints().WithTags("Providers Endpoints");
app.MapGroup("/api/v0/clients").MapClientV0Endpoints().WithTags("Clients Endpoints");

app.Run();
