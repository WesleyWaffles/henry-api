using Microsoft.EntityFrameworkCore;

namespace Henry.Api.Data
{
    public class HenryDbContenxt : DbContext
    {
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ProviderAvailability> ProviderAvailabilities { get; set; }
        public DbSet<Appointments> Appointments { get; set; }
     
        public HenryDbContenxt(DbContextOptions options) : base(options)
        {
        }
    }
}
