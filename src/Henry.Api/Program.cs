using Microsoft.EntityFrameworkCore;
using Henry.Api.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HenryDbContenxt>(options => 
    options.UseSqlite($"Data Source={Path.Join(Directory.GetCurrentDirectory(), "Databases", "Henry.db")}"));

var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetService<HenryDbContenxt>();
db?.Database.MigrateAsync();

app.MapGet("/", () => "Hello World!");

app.Run();
