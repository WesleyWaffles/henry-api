using Microsoft.EntityFrameworkCore;
using Henry.Api.Data;
using Henry.Api;
using Henry.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<HenryDbContenxt>(options => 
    options.UseSqlite($"Data Source={Path.Join(Directory.GetCurrentDirectory(), "Databases", "Henry.db")}"));

builder.Services.AddTransient<AppointmentService>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetService<HenryDbContenxt>();
db?.Database.MigrateAsync();

// Map v0 endpoints
app.MapGroup("/api/v0/appointments").MapAppointmentsEndpointsV0();

app.Run();
