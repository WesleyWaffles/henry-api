using Microsoft.EntityFrameworkCore;

namespace Henry.Api.Data
{
    public class HenryDbContenxt : DbContext
    {
        public DbSet<Appointment> Appointments { get; set; }
     
        public HenryDbContenxt(DbContextOptions options) : base(options)
        {
        }
    }
}
