using System.Collections.Generic;
using System.Threading.Tasks;
using CineMate.Data;
using CineMate.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CineMate.Data.Seed
{
    public static class CineMateSeeder
    {
 
        public static async Task SeedCitiesAndCinemasAsync(CineMateDbContext db)
        {
            static string Clean(string s) => (s ?? string.Empty).Trim();

            var plan = new Dictionary<string, string[]>
            {
                ["Sofia"] = new[]
                {
                    "CineMate Mall of Sofia",
                    "CineMate Paradise Center",
                    "CineMate Serdika Center",
                    "CineMate The Mall",
                    "CineMate Bulgaria Mall"
                },
                ["Plovdiv"] = new[]
                {
                    "CineMate Plovdiv Mall",
                    "CineMate Markovo Tepe"
                },
                ["Varna"] = new[]
                {
                    "CineMate Grand Mall",
                    "CineMate Varna Mall"
                }
            };

            foreach (var kvp in plan)
            {
                var cityName = Clean(kvp.Key);

                var city = await db.Cities
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == cityName.ToLower());

                if (city == null)
                {
                    city = new City { Name = cityName };
                    db.Cities.Add(city);
                    await db.SaveChangesAsync();
                }

                foreach (var rawCinemaName in kvp.Value)
                {
                    var cinemaName = Clean(rawCinemaName);

                    var exists = await db.Cinemas.AnyAsync(x =>
                        x.CityId == city.Id &&
                        x.Name.ToLower() == cinemaName.ToLower());

                    if (!exists)
                    {
                        db.Cinemas.Add(new Cinema
                        {
                            Name = cinemaName,
                            CityId = city.Id
                        });
                    }
                }
            }

            await db.SaveChangesAsync();
        }
    }
}
