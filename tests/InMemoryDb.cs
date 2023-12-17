using Henry.Api.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Henry.Api.UnitTests
{
    internal class InMemoryDb : IDbContextFactory<HenryDbContenxt>
    {
        public HenryDbContenxt CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<HenryDbContenxt>()
                .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc()}")
                .Options;

            return new HenryDbContenxt(options);
        }

    }
}
