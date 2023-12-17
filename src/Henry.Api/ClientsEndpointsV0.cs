using Henry.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Henry.Api
{
    public static class ClientsEndpointsV0
    {
        public static RouteGroupBuilder MapClientsEndpointsV0(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder.MapGet("/", async (HenryDbContenxt db) => await db.Clients.ToListAsync());

            routeGroupBuilder.MapPost("/", async (HenryDbContenxt db, Client client) =>
            {
                await db.Clients.AddAsync(client);
                await db.SaveChangesAsync();
                return Results.Created($"/clients/{client.Id}", client);
            });

            return routeGroupBuilder;
        }

    }
}
