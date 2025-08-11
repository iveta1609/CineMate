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
            // 1) Конфигуриране на IConfiguration, за да прочетем appsettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())      // working dir на проекта
                .AddJsonFile("appsettings.json", optional: false)  // ще хвърли, ако го няма
                .Build();

            // 2) Вземаме DefaultConnection от конфигурацията
            var connStr = config.GetConnectionString("DefaultConnection");

            // 3) Създаваме Опции за нашия DbContext
            var optionsBuilder = new DbContextOptionsBuilder<CineMateDbContext>();
            optionsBuilder.UseSqlServer(connStr);

            // 4) Връщаме новия контекст с тези опции
            return new CineMateDbContext(optionsBuilder.Options);
        }
    }
}
