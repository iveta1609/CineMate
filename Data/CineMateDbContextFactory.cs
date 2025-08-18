using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CineMate.Data
{
    public class CineMateDbContextFactory : IDesignTimeDbContextFactory<CineMateDbContext>
    {
        public CineMateDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())      
                .AddJsonFile("appsettings.json", optional: false)  
                .Build();

            var connStr = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<CineMateDbContext>();
            optionsBuilder.UseSqlServer(connStr);

            return new CineMateDbContext(optionsBuilder.Options);
        }
    }
}
