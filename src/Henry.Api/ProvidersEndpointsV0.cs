using Henry.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Henry.Api
{
    public static class ProvidersEndpointsV0
    {
        public static RouteGroupBuilder MapProvidersEndpointsV0(this RouteGroupBuilder routeGroupBuilder)
        {
            routeGroupBuilder.MapGet("/", async (HenryDbContenxt db) => await db.Providers.ToListAsync());

            routeGroupBuilder.MapPost("/", async (HenryDbContenxt db, Provider provider) =>
            {
                await db.Providers.AddAsync(provider);
                await db.SaveChangesAsync();
                return Results.Created($"/providers/{provider.Id}", provider);
            });

            return routeGroupBuilder;
        }

    }
}
