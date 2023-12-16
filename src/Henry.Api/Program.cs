using Microsoft.EntityFrameworkCore;
using Henry.Api.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HenryDbContenxt>(options => 
    options.UseSqlite($"Data Source={Path.Join(Directory.GetCurrentDirectory(), "Databases", "Henry.db")}"));

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetService<HenryDbContenxt>();
db?.Database.MigrateAsync();

app.MapGet("/appointments", async (HenryDbContenxt db) => await db.Appointments.ToListAsync());

app.MapGet("/providers", async (HenryDbContenxt db) => await db.Providers.ToListAsync());

app.MapGet("/clients", async (HenryDbContenxt db) => await db.Clients.ToListAsync());

app.MapGet("/providers/{id}/availability", async (int Id, HenryDbContenxt db) =>
{
    var provider = await db.Providers.FindAsync(Id);
    if (provider is null)
        return Results.NotFound();

    var providerAvailability = await db.ProviderAvailabilities.Where(x => x.Provider.Id == Id).ToListAsync();
    return Results.Ok(providerAvailability);
});

app.MapPost("/appointments", async (HenryDbContenxt db, Appointment appointment) =>
{
    await db.Appointments.AddAsync(appointment);
    await db.SaveChangesAsync();
    return Results.Created($"/appointments/{appointment.Id}", appointment);
});

app.MapPost("/providers", async (HenryDbContenxt db, Provider provider) =>
{
    await db.Providers.AddAsync(provider);
    await db.SaveChangesAsync();
    return Results.Created($"/providers/{provider.Id}", provider);
});

app.MapPost("/clients", async (HenryDbContenxt db, Client client) =>
{
    await db.Clients.AddAsync(client);
    await db.SaveChangesAsync();
    return Results.Created($"/clients/{client.Id}", client);
});

app.MapPost("/providers/{id}/availability", async (int Id, HenryDbContenxt db, ProviderAvailability providerAvailability) =>
{
    var provider = await db.Providers.FindAsync(Id);
    if (provider is null)
        return Results.NotFound();

    providerAvailability.Provider = provider;
    await db.ProviderAvailabilities.AddAsync(providerAvailability);
    await db.SaveChangesAsync();
    return Results.Created($"/providers/{provider.Id}/availability/{providerAvailability.Id}", providerAvailability);
});

app.Run();
