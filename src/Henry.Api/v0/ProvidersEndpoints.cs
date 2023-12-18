using Henry.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Henry.Api.v0
{
    public static class ProvidersEndpoints
    {
        /// <summary>
        /// Maps the endpoints for the providers API
        /// </summary>
        /// <param name="routeGroupBuilder">The route group to map endpoints to</param>
        /// <returns>A route group builder with mapped endpoints for the providers api</returns>
        public static RouteGroupBuilder MapProviderV0Endpoints(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder.MapGet("/", async (HenryDbContenxt db) => await db.Providers.ToListAsync())
                .WithOpenApi();

            routeGroupBuilder.MapGet("/{id}", async (HenryDbContenxt db, int id) => await db.Providers.FindAsync(id))
                .WithOpenApi();

            routeGroupBuilder.MapPost("/", async (HenryDbContenxt db, Provider provider) =>
            {
                db.Providers.Update(provider);
                await db.SaveChangesAsync();
                return provider;
            }).WithOpenApi();

            return routeGroupBuilder;
        }
    }
}
