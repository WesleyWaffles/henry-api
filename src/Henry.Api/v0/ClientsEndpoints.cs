using Henry.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Henry.Api.v0
{
    public static class ClientsEndpoints
    {
        /// <summary>
        /// Maps the endpoints for the clients API
        /// </summary>
        /// <param name="routeGroupBuilder">The route group to map endpoints to</param>
        /// <returns>A route group builder with mapped endpoints for the clients api</returns>
        public static RouteGroupBuilder MapClientV0Endpoints(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder.MapGet("/", async (HenryDbContenxt db) => await db.Clients.ToListAsync())
                .WithOpenApi();

            routeGroupBuilder.MapGet("/{id}", async (HenryDbContenxt db, int id) => await db.Clients.FindAsync(id))
                .WithOpenApi();

            routeGroupBuilder.MapPost("/", async (HenryDbContenxt db, Client client) =>
            {
                db.Clients.Update(client);
                await db.SaveChangesAsync();
                return client;
            }).WithOpenApi();

            return routeGroupBuilder;
        }
    }
}
